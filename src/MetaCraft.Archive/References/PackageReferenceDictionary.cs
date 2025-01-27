// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Archive.Json;
using Semver;

namespace MetaCraft.Archive.References;

[JsonConverter(typeof(PackageReferenceDictionaryConverter))]
public sealed class PackageReferenceDictionary : Dictionary<string, SemVersion>
{
    public PackageReferenceDictionary()
    {
    }

    public PackageReferenceDictionary(IDictionary<string, SemVersion> dictionary) : base(dictionary)
    {
    }

    public PackageReferenceDictionary(IEnumerable<KeyValuePair<string, SemVersion>> collection) : base(collection)
    {
    }

    public PackageReferenceDictionary(int capacity) : base(capacity)
    {
    }

    public void Add(PackageReference reference)
    {
        Add(reference.Name, reference.Version);
    }

    public void AddRange(IEnumerable<PackageReference> references)
    {
        foreach (var reference in references)
        {
            Add(reference.Name, reference.Version);
        }
    }
}
