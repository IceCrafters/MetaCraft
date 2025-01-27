// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive.References;
using MetaCraft.Common;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Localisation;
using Semver;

namespace MetaCraft.Core.Transactions;

public class UpdateReferrersTransaction : ArgumentedTransaction<UpdateReferrersTransaction.Parameters>
{
    public UpdateReferrersTransaction(IPackageScope target, Parameters argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        if (!Argument.IgnoreSerial && Target.Container.GetSerial() != -1 &&
            Target.Referrals.GetSerial() == Target.Container.GetSerial())
        {
            // Already up to date
            return;
        }
        
        var tree = new Dictionary<string, ReferralIndexDictionary>();

        agent.PrintInfo(Lc.L("Updating referrals database..."));
        
        FillReferrersInternal(agent, tree);
        UpdateSelectedInternal(agent, tree);
        SaveReferrersInternal(agent, tree);
    }

    private void SaveReferrersInternal(ITransactionAgent agent, Dictionary<string, ReferralIndexDictionary> tree)
    {
        agent.PrintInfo(Lc.L("Saving referrals database..."));
        foreach (var (id, data) in tree)
        {
            try
            {
                Target.Referrals.Store.WriteFile(id, data);
            }
            catch (Exception ex)
            {
                agent.PrintWarning(Lc.L("Failed to save referrals of '{0}'", id));
                agent.PrintDebug(ex, Lc.L("Exception details:"));
            }
        }
    }

    private void UpdateSelectedInternal(ITransactionAgent agent, Dictionary<string, ReferralIndexDictionary> tree)
    {
        agent.PrintInfo(Lc.L("Applying preferences..."));

        foreach (var (id, data) in tree)
        {
            foreach (var (version, index) in data)
            {
                var selected = Target.Referrals.PreferenceProvider.GetPreferredId(id, version.Value);
                
                // If null or not valid, order alphabetically
                if (selected == null || !index.Referrers.ContainsKey(selected))
                {
                    selected = index.Referrers.Keys.Order(StringComparer.Ordinal)
                        .FirstOrDefault();   
                }

                // If still null, use current value.
                if (selected == null)
                {
                    agent.PrintWarning(Lc.L("Referrer index for '{0}' ({1}) contains no valid packages",
                        id,
                        version));
                    continue;
                }

                index.Current = selected;
            }
        }
    }

    private void FillReferrersInternal(ITransactionAgent agent, Dictionary<string, ReferralIndexDictionary> tree)
    {
        foreach (var package in Target.Container.EnumeratePackages())
        {
            foreach (var version in Target.Container.EnumerateVersions(package))
            {
                var manifest = Target.Container.InspectLocal(package, version);
                if (manifest == null)
                {
                    agent.PrintWarning(Lc.L("The package directory contains an invalid package"));
                    continue;
                }

                // Each package provides itself as a provision - mandatory
                InsertReferrer(manifest.Id, manifest.Version, PackageReference.Of(manifest));

                // Go through all provisions it provides if any
                if (manifest.Provides == null || manifest.Provides.Count == 0)
                {
                    continue;
                }

                foreach (var provided in manifest.Provides)
                {
                    InsertReferrer(provided.Key, provided.Value, PackageReference.Of(manifest));
                }
            }
        }

        return;
        
        void InsertReferrer(string id, SemVersion version, PackageReference to)
        {
            // The ID used by MetaCraft is the referenced ID and version combined.
            var idToAdd = $"{to.Name}-{to.Version}";

            var data = tree.GetOrAdd(id,
                () => []);

            var referrers = data.GetOrAdd(new SemVersionKey(version),
                () => new PackageReferralIndex([], idToAdd));     

            referrers.Referrers.Add(idToAdd, to);
        }
    }

    public readonly record struct Parameters(bool IgnoreSerial);
}