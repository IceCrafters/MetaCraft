// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Archive.Json;
using MetaCraft.Archive.References;
using MetaCraft.Common.Json;
using Semver;

namespace MetaCraft.Core.Archive;

[JsonConverter(typeof(RangedReferenceDictionaryConverter))]
public sealed class RangedPackageReferenceDictionary : Dictionary<string, SemVersionRange>
{
    public RangedPackageReferenceDictionary()
    {
    }

    public RangedPackageReferenceDictionary(IDictionary<string, SemVersionRange> dictionary) : base(dictionary)
    {
    }

    public RangedPackageReferenceDictionary(IEnumerable<KeyValuePair<string, SemVersionRange>> collection) : base(collection)
    {
    }

    public RangedPackageReferenceDictionary(int capacity) : base(capacity)
    {
    }
}
