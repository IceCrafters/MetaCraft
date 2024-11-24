// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-LicenseIdentifier: GPL-3.0-or-later

using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MetaCraft.Core.Serialization;

public class MailAddressCollectionConverter : JsonConverter<MailAddressCollection>
{
    private static readonly MailAddressConverter AddressConverter = new();
    
    public override MailAddressCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected array");
        }
     
        var collection = new MailAddressCollection();

        while (true)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }
            
            var item = AddressConverter.Read(ref reader, typeToConvert, options);
            if (item == null)
            {
                throw new JsonException("Null item is not allowed.");
            }
            
            collection.Add(item);
        }

        return collection;
    }

    public override void Write(Utf8JsonWriter writer, MailAddressCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var address in value)
        {
            AddressConverter.Write(writer, address, options);
        }
        writer.WriteEndArray();
    }
}