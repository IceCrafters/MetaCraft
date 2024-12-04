// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Core.Scopes.Referral;

public class PackageReferralDatabase : ISerialed
{
    private readonly SerialFile _serialFile;

    public PackageReferralDatabase(IReferralDatabaseStore store,
        IReferralPreferenceProvider preferenceProvider)
    {
        Store = store;
        PreferenceProvider = preferenceProvider;

        _serialFile = Store.GetSerialFile();
    }
    
    internal IReferralDatabaseStore Store { get; }
    
    internal IReferralPreferenceProvider PreferenceProvider { get; }

    public SemVersion? GetLatestClauseVersion(string clauseId)
    {
        var file = Store.ReadFile(clauseId);

        return file?.Keys.OrderByDescending(x => x.Value, SemVersion.SortOrderComparer)
            .Select(x => x.Value)
            .FirstOrDefault();
    }
    
    /// <summary>
    /// Determines whether the specified clause (or clause index) exists.
    /// </summary>
    /// <param name="clauseId">The identifier of the clause.</param>
    /// <param name="version">
    /// The version to locate. If <see langword="null"/>, returns true if the clause index exists 
    /// and is not empty.
    /// </param>
    /// <returns><see langword="true"/> if clause exists; otherwise, <see langword="false"/>.</returns>
    public bool ContainsClause(string clauseId, SemVersion? version)
    {
        var file = Store.ReadFile(clauseId);

        if (version == null)
        {
            return file is { Count: > 0 };
        }
        
        return file?.ContainsKey(new SemVersionKey(version)) ?? false;
    }

    public bool ContainsReferrer(string clauseId, SemVersion version, string referrerId)
    {
        var file = Store.ReadFile(clauseId);
        if (file == null)
        {
            return false;
        }

        if (!file.TryGetValue(new SemVersionKey(version), out var clause))
        {
            return false;
        }
        
        return clause.Referrers.ContainsKey(referrerId);
    }

    public IEnumerable<KeyValuePair<string, PackageReference>> EnumerateReferrers(string clauseId, SemVersion version)
    {
        var file = Store.ReadFile(clauseId);
        if (file == null)
        {
            yield break;
        }

        if (!file.TryGetValue(new SemVersionKey(version), out var referrers))
        {
            yield break;
        }

        foreach (var referrer in referrers.Referrers)
        {
            yield return referrer;
        }
    }
    
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

    public void SetPreferred(string clauseName, SemVersion clauseVersion, string referrerName)
    {
        PreferenceProvider.SetPreferredId(clauseName, clauseVersion, referrerName);
    }
}
