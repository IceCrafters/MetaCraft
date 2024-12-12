// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Serialization;
using MetaCraft.Archive.References;
using MetaCraft.Common.Json;
using Semver;

namespace MetaCraft.Archive.Json;

public class PackageReferenceDictionaryConverter : JsonConverter<PackageReferenceDictionary>
{
    private static readonly SemVersionConverter VersionConverter = new();
    
    public override PackageReferenceDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object.");
        }

        var dictionary = new PackageReferenceDictionary();
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
            
            var range = VersionConverter.Read(ref reader, typeof(SemVersionRange), options);
            if (range == null)
            {
                throw new JsonException("Expected SemVersion.");
            }
            
            dictionary.Add(key, range);
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, PackageReferenceDictionary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var (id, ver) in value)
        {
            writer.WritePropertyName(id);
            VersionConverter.Write(writer, ver, options);
        }
        
        writer.WriteEndObject();
    }
}
