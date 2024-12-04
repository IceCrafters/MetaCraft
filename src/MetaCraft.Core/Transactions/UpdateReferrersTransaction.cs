// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Core.Transactions;

public class UpdateReferrersTransaction : ArgumentedTransaction<UpdateReferrersTransaction.Parameters>
{
    public UpdateReferrersTransaction(IPackageContainer target, Parameters argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        if (!Argument.IgnoreSerial && Target.GetSerial() != -1 &&
            Target.Parent.Referrals.GetSerial() == Target.GetSerial())
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
                Target.Parent.Referrals.Store.WriteFile(id, data);
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
                var selected = Target.Parent.Referrals.PreferenceProvider.GetPreferredId(id, version.Value);
                
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
        foreach (var package in Target.EnumeratePackages())
        {
            foreach (var version in Target.EnumerateVersions(package))
            {
                var manifest = Target.InspectLocal(package, version);
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
            var idToAdd = $"{id}-{version}";

            var data = tree.GetOrAdd(id,
                () => new ReferralIndexDictionary());

            var referrers = data.GetOrAdd(new SemVersionKey(version),
                () => new PackageReferralIndex([], idToAdd));
            referrers.Referrers.Add(idToAdd, to);
        }
    }

    public readonly record struct Parameters(bool IgnoreSerial);
}