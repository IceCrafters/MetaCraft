// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive;
using MetaCraft.Core.Projection;
using Semver;

namespace MetaCraft.Tests;

public class ProjectionSpaceTests
{
    [Fact]
    public void GetRelativePathOf_CorrectDeclaration_CorrectResult()
    {
        // Arrange
        var declaration = new AssemblyExportDeclaration
        {
            Name = "Test.Assembly",
            Path = "lib/netstandard2.0",
            Version = new SemVersion(1, 0, 0),
            To = "Test.Assembly.dll"
        };
        
        // Act
        var result = ProjectionSpace.GetRelativePathOf(declaration);
        
        // Assert
        Assert.Equal("test.assembly/1.0.0/lib/netstandard2.0/Test.Assembly.dll", result);
    }
}