// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using MetaCraft.Core.Locales;

namespace MetaCraft.Core.Serialization;

public sealed class SemVersionKeyConverter : JsonConverter<SemVersionKey>
{
    private static readonly SemVersionConverter ValueConverter = new();

    public override SemVersionKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = ValueConverter.Read(ref reader, typeToConvert, options);
        return value == null
            ? null
            : new SemVersionKey(value);
    }

    public override void Write(Utf8JsonWriter writer, SemVersionKey value, JsonSerializerOptions options)
    {
        ValueConverter.Write(writer, value.Value, options);
    }

    public override SemVersionKey ReadAsPropertyName(ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options)
    {
        var value = ValueConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException(Lc.L("Version values cannot be null when used as a property name."));

        return new SemVersionKey(value);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, 
        SemVersionKey value, 
        JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.Value.ToString());
    }
}
