// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using MetaCraft.Common.Json;
using MetaCraft.Core.Archive;
using Semver;

namespace MetaCraft.Archive.Json;

public class RangedReferenceDictionaryConverter : JsonConverter<RangedPackageReferenceDictionary>
{
    private static readonly SemVersionRangeConverter VersionRangeConverter = new();
    
    public override RangedPackageReferenceDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object.");
        }

        var dictionary = new RangedPackageReferenceDictionary();
        while (true)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name.");
            }
            
            var key = reader.GetString();
            
            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected version range string.");
            }
            
            var value = reader.GetString();

            if (key == null || value == null)
            {
                throw new JsonException("Expected non-null key and value.");
            }
            
            var range = VersionRangeConverter.Read(ref reader, typeof(SemVersionRange), options);
            if (range == null)
            {
                throw new JsonException("Expected SemVersionRange.");
            }
            
            dictionary.Add(key, range);
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, RangedPackageReferenceDictionary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var (id, range) in value)
        {
            writer.WritePropertyName(id);
            VersionRangeConverter.Write(writer, range, options);
        }
        
        writer.WriteEndObject();
    }
}
