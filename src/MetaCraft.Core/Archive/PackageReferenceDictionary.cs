// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Core.Archive;

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
}
