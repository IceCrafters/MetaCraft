// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace MetaCraft.Common.Json;

public sealed class SemVersionConverter : JsonConverter<SemVersion>
{
    public override SemVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var strValue = reader.GetString();
        if (strValue == null)
        {
            return null;
        }

        if (!SemVersion.TryParse(strValue, out var result))
        {
            throw new JsonException("The provided string is not a valid Semantic Version number.");
            // TODO: add a common Lc module and restore this localization string
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, SemVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}