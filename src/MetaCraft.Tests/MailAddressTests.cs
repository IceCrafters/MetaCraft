// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using System.Net.Mail;
using System.Text;
using System.Text.Json;
using MetaCraft.Core.Serialization;

namespace MetaCraft.Tests;

public class MailAddressTests
{
    [Fact]
    public void AddressString_ConvertFromJson_Valid()
    {
        // Arrange
        const string toParse = "\"Test <test@example.com>\"";
        var converter = new MailAddressConverter();
        
        var data = Encoding.UTF8.GetBytes(toParse);
        var reader = new Utf8JsonReader(data);
        reader.Read();
        
        // Act
        var result = converter.Read(ref reader, typeof(MailAddress), JsonSerializerOptions.Default);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("\"Test\" <test@example.com>", result.ToString());
    }
    
    [Fact]
    public void SingleObjectCollection_ConvertFromJson_Valid()
    {
        // Arrange
        const string toParse = "[\"Bob <bob@example.com>\"]";
        var converter = new MailAddressCollectionConverter();
        var data = Encoding.UTF8.GetBytes(toParse);
        var reader = new Utf8JsonReader(data);
        reader.Read();
        
        // Act
        var result = converter.Read(ref reader, typeof(MailAddressCollection), JsonSerializerOptions.Default);
        
        // Assert
        Assert.NotNull(result);
        Assert.Collection(result,
            x => Assert.Equal("\"Bob\" <bob@example.com>", x.ToString()));
    }
    
    [Fact]
    public void MultiObjectCollection_ConvertFromJson_Valid()
    {
        // Arrange
        const string toParse = "[\"Bob <bob@example.com>\",\"Adam <adam@example.com>\"]";
        var converter = new MailAddressCollectionConverter();
        var data = Encoding.UTF8.GetBytes(toParse);
        var reader = new Utf8JsonReader(data);
        reader.Read();
        
        // Act
        var result = converter.Read(ref reader, typeof(MailAddressCollection), JsonSerializerOptions.Default);
        
        // Assert
        Assert.NotNull(result);
        Assert.Collection(result,
            x => Assert.Equal("\"Bob\" <bob@example.com>", x.ToString()),
            x => Assert.Equal("\"Adam\" <adam@example.com>", x.ToString()));
    }
}
