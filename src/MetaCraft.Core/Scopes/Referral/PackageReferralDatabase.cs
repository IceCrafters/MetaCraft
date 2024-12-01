// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using Semver;

namespace MetaCraft.Core.Scopes.Referral;

public class PackageReferralDatabase : ISerialed
{
    private readonly SerialFile _serialFile;

    public PackageReferralDatabase(IReferralDatabaseStore store)
    {
        Store = store;

        _serialFile = Store.GetSerialFile();
    }
    
    internal IReferralDatabaseStore Store { get; }

    public PackageReference? Locate(RangedPackageReference reference)
    {
        // Get index dictionary data

        var data = Store.ReadFile(reference.Id);
        if (data == null)
        {
            return null;
        }

        // Use LINQ to select the latest version ranging that range
        var realKey = data.Keys
            .Where(x => reference.Version.Contains(x.Value))
            .OrderByDescending(x => x.Value, SemVersion.SortOrderComparer)
            .FirstOrDefault();
        if (realKey == null)
        {
            return null;
        }

        var index = data[realKey];
        return LocateInternal(index);
    }

    private static PackageReference? LocateInternal(PackageReferralIndex index)
    {
        if (index.Referrers.TryGetValue(index.Current, out var preferred))
        {
            return preferred;
        }

        var selected = index.Referrers.Keys
            .Order(StringComparer.InvariantCulture)
            .FirstOrDefault();
        if (selected == null)
        {
            return null;
        }

        return index.Referrers[selected];
    }

    public void Clear()
    {
        Store.Clear();
    }

    public bool CompareSerialWith(SerialFile serial)
    {
        return ((ISerialed)_serialFile).CompareSerialWith(serial);
    }

    public void CopySerial(ISerialed from)
    {
        ((ISerialed)_serialFile).CopySerial(from);
    }

    public long GetSerial()
    {
        return ((ISerialed)_serialFile).GetSerial();
    }
}
