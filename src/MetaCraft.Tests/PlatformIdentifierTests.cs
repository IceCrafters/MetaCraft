// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Platform;

namespace MetaCraft.Tests;

public class PlatformIdentifierTests
{
    [Fact]
    public void SystemOnly_Parse_AnyArchitecture()
    {
        // Arrange
        const string strToParse = "linux";
        
        // Act
        var result = PlatformIdentifier.Parse(strToParse);
        
        // Assert
        Assert.Equal(new PlatformIdentifier(PlatformSystem.Linux, PlatformArchitecture.Any),
            result);
    }
    
    [Fact]
    public void LinuxX64_ToString_CorrectFormat()
    {
        // Arrange
        var instance = new PlatformIdentifier(PlatformSystem.Linux, PlatformArchitecture.X64);
        
        // Act
        var result = instance.ToString();
        
        // Assert
        Assert.Equal("linux-x64", result);
    }

    [Fact]
    public void LinuxX64_Parse_CorrectValue()
    {
        // Arrange
        const string strToParse = "linux-x64";
        
        // Act
        var result = PlatformIdentifier.Parse(strToParse);
        
        // Assert
        Assert.Equal(new PlatformIdentifier(PlatformSystem.Linux, PlatformArchitecture.X64),
            result);
    }

    [Theory]
    [InlineData("linux-x86", PlatformSystem.Linux, PlatformArchitecture.X86)]
    [InlineData("musllinux-x86", PlatformSystem.MuslLinux, PlatformArchitecture.X86)]
    [InlineData("musllinux-x64", PlatformSystem.MuslLinux, PlatformArchitecture.X64)]
    [InlineData("windows-x86", PlatformSystem.Windows, PlatformArchitecture.X86)]
    [InlineData("windows-x64", PlatformSystem.Windows, PlatformArchitecture.X64)]
    [InlineData("freebsd-x86", PlatformSystem.FreeBsd, PlatformArchitecture.X86)]
    [InlineData("freebsd-x64", PlatformSystem.FreeBsd, PlatformArchitecture.X64)]
    public void Others_Parse_CorrectValue(string toParse, PlatformSystem system, PlatformArchitecture arch)
    {
        // Arrange
        var expected = new PlatformIdentifier(system, arch);
        
        // Act
        var result = PlatformIdentifier.Parse(toParse);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AnyArchitectureSameSystem_Covers_True()
    {
        // Arrange
        var instanceA = new PlatformIdentifier(PlatformSystem.Windows, PlatformArchitecture.Any);
        var instanceB = new PlatformIdentifier(PlatformSystem.Windows, PlatformArchitecture.Arm64);
        
        // Act
        var result = instanceA.Covers(instanceB);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void AnySystemSameArchitecture_Covers_True()
    {
        // Arrange
        var instanceA = new PlatformIdentifier(PlatformSystem.Any, PlatformArchitecture.X64);
        var instanceB = new PlatformIdentifier(PlatformSystem.FreeBsd, PlatformArchitecture.X64);
        
        // Act
        var result = instanceA.Covers(instanceB);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void CompletelyDifferentPlatforms_Covers_False()
    {
        // Arrange
        var instanceA = new PlatformIdentifier(PlatformSystem.Windows, PlatformArchitecture.X64);
        var instanceB = new PlatformIdentifier(PlatformSystem.FreeBsd, PlatformArchitecture.X64);
        
        // Act
        var result = instanceA.Covers(instanceB);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void Equal_Covers_True()
    {
        // Arrange
        var instanceA = new PlatformIdentifier(PlatformSystem.Windows, PlatformArchitecture.X64);
        var instanceB = new PlatformIdentifier(PlatformSystem.Windows, PlatformArchitecture.X64);
        
        // Act
        var result = instanceA.Covers(instanceB);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void AllAny_Covers_True()
    {
        // Arrange
        var instanceA = new PlatformIdentifier(PlatformSystem.Any, PlatformArchitecture.Any);
        // Yes I know this is really an impossible combination
        var instanceB = new PlatformIdentifier(PlatformSystem.Mac, PlatformArchitecture.LoongArch64);
        
        // Act
        var result = instanceA.Covers(instanceB);
        
        // Assert
        Assert.True(result);
    }
}
