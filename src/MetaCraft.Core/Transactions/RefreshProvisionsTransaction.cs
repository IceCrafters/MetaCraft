// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Locales;
using MetaCraft.Core.Scopes;
using Semver;

namespace MetaCraft.Core.Transactions;

public class RefreshProvisionsTransaction : ArgumentedTransaction<RefreshProvisionsTransaction.Parameters>
{
    public readonly record struct Parameters(bool IgnoreSerial);

    public RefreshProvisionsTransaction(PackageContainer target, Parameters argument) : base(target, argument)
    {
    }

    public override void Commit(ITransactionAgent agent)
    {
        if (!Argument.IgnoreSerial && Target.GetSerial() != -1 && Target.Parent.Provisions.GetSerial() == Target.GetSerial())
        {
            // Already up to date
            return;
        }

        agent.PrintInfo(Lc.L("Updating provisions database..."));

        var provisions = new Dictionary<string, Dictionary<string, PackageReference>>();

        void InsertProvision(string id, SemVersion version, PackageReference to)
        {
            var dict = provisions.GetOrAdd(id, () => []);
            dict.Add(version.ToString(), to);
        }

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
                InsertProvision(manifest.Id, manifest.Version, PackageReference.Of(manifest));

                // Go through all provisions it provides if any
                if (manifest.Provides == null || manifest.Provides.Count == 0)
                {
                    continue;
                }

                foreach (var provided in manifest.Provides)
                {
                    InsertProvision(provided.Key, provided.Value, PackageReference.Of(manifest));
                }
            }
        }

        ApplyProvisions(provisions, agent);

        Target.Parent.Provisions.CopySerial(Target);
    }

    private void ApplyProvisions(Dictionary<string, Dictionary<string, PackageReference>> provisions,
        ITransactionAgent agent)
    {
        Target.Parent.Provisions.Clear();

        foreach (var provision in provisions)
        {
            var filePath = Path.Combine(Target.Parent.Provisions.Root, $"{provision.Key}.json");

            try
            {
                using var stream = File.Create(filePath);
                JsonSerializer.Serialize(stream, provision.Value, CoreJsonContext.Default.ProvisionMap);
            }
            catch (Exception ex)
            {
                agent.PrintWarning(Lc.L("failed to update provision '{0}': {1}", provision.Key, ex.Message));
            }
        }
    }
}
