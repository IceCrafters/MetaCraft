// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace MetaCraft.Common.Json;

public class SemVersionRangeConverter : JsonConverter<SemVersionRange>
{
    public override SemVersionRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str == null)
        {
            throw new JsonException("Expected a string.");
        }

        if (!SemVersionRange.TryParse(str, out var result))
        {
            throw new JsonException("Invalid semantic version range.");
        }
        
        return result;
    }

    public override void Write(Utf8JsonWriter writer, SemVersionRange value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
