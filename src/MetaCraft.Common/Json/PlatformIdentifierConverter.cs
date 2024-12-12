// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using MetaCraft.Common.Platform;

namespace MetaCraft.Common.Json;

public sealed class PlatformIdentifierConverter : JsonConverter<PlatformIdentifier>
{
    public override PlatformIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str == null)
        {
            throw new JsonException("Platform identifier should not be null.");
        }

        try
        {
            return PlatformIdentifier.Parse(str);
        }
        catch (FormatException e)
        {
            throw new JsonException(e.Message, e);
        }
    }

    public override void Write(Utf8JsonWriter writer, PlatformIdentifier value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
