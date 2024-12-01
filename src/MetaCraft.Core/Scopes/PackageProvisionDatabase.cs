// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using MetaCraft.Core.Archive;
using Semver;

namespace MetaCraft.Core.Scopes;

public class PackageProvisionDatabase : ISerialed
{
    private const string ProvisionFolder = "provision";

    private readonly PackageScope _scope;
    private readonly SerialFile _serial;

    public PackageProvisionDatabase(PackageScope scope)
    {
        _scope = scope;
        Root = Path.Combine(_scope.Root, ProvisionFolder);
        Directory.CreateDirectory(Root);

        _serial = new SerialFile(Path.Combine(Root, "serial"));
    }

    public string Root { get; }

    public void Clear()
    {
        if (!Directory.Exists(Root))
        {
            return;
        }

        Directory.Delete(Root, true);
        Directory.CreateDirectory(Root);
    }

    public bool CompareSerialWith(SerialFile serial)
    {
        return ((ISerialed)_serial).CompareSerialWith(serial);
    }

    public void CopySerial(SerialFile from)
    {
        ((ISerialed)_serial).CopySerial(from);
    }

    public void CopySerial(ISerialed from)
    {
        ((ISerialed)_serial).CopySerial(from);
    }

    public PackageReference? GetProvisionOrDefault(RangedPackageReference reference)
    {
        var provisionPath = Path.Combine(Root,
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

    public long GetSerial()
    {
        return ((ISerialed)_serial).GetSerial();
    }
}