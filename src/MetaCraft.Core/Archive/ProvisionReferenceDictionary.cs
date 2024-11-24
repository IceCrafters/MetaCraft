// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json.Serialization;
using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Core.Archive;

[JsonConverter(typeof(ProvisionReferenceDictionaryConverter))]
public sealed class ProvisionReferenceDictionary : Dictionary<string, SemVersionRange>
{
    public ProvisionReferenceDictionary()
    {
    }

    public ProvisionReferenceDictionary(IDictionary<string, SemVersionRange> dictionary) : base(dictionary)
    {
    }

    public ProvisionReferenceDictionary(IEnumerable<KeyValuePair<string, SemVersionRange>> collection) : base(collection)
    {
    }

    public ProvisionReferenceDictionary(int capacity) : base(capacity)
    {
    }
}
