// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using MetaCraft.Core.Archive;
using Semver;

namespace MetaCraft.Core.Scopes;

public class PackageProvisionDatabase
{
    private const string ProvisionFolder = "provision";

    private readonly PackageScope _scope;
    private readonly SerialFile _serial;
    private readonly string _root;

    public PackageProvisionDatabase(PackageScope scope)
    {
        _scope = scope;
        _root = Path.Combine(_scope.Root, ProvisionFolder);
        Directory.CreateDirectory(_root);

        _serial = new SerialFile(Path.Combine(_root, "serial"));
    }

    public PackageReference? GetProvisionOrDefault(ProvisionReference reference)
    {
        var provisionPath = Path.Combine(_root,
            $"{reference.Id}.json");

        if (!File.Exists(provisionPath))
        {
            return null;
        }

        Dictionary<string, PackageReference>? versions;
        using (var stream = File.OpenRead(provisionPath))
        {
            versions = JsonSerializer.Deserialize(stream, CoreJsonContext.Default.ProvisionMap);
        }
        
        if (versions == null)
        {
            return null;
        }

        // Do some constructive ordering
        // This will be slow. REAL SLOW.
        var realKey = versions.Keys.Select(x => SemVersion.Parse(x))
            .Where(x => reference.Version.Contains(x))
            .OrderDescending(SemVersion.SortOrderComparer)
            .FirstOrDefault();
        if (realKey == null)
        {
            return null;
        }

        return versions[realKey.ToString()];
    }
}