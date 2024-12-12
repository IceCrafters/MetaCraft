// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaCraft.Common.Json;

public sealed class MailAddressConverter : JsonConverter<MailAddress>
{
    public override MailAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str == null)
        {
            return null;
        }

        if (!MailAddress.TryCreate(str, out var mail))
        {
            throw new JsonException("Invalid mail address.");
        }
        
        return mail;
    }

    public override void Write(Utf8JsonWriter writer, MailAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
