// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using MetaCraft.Common.Resources;

namespace MetaCraft.Common.Json;

public class SemVersionKeyConverter : JsonConverter<SemVersionKey>
{
    private readonly SemVersionConverter _converter = new();
    
    public override SemVersionKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new SemVersionKey(_converter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException(CommonMessages.SemVersionNullAsKey));
    }

    public override void Write(Utf8JsonWriter writer, SemVersionKey value, JsonSerializerOptions options)
    {
        _converter.Write(writer, value.Value, options);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, SemVersionKey value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.Value.ToString());
    }
}