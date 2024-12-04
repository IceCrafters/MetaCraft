// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Serialization;
using Semver;

namespace MetaCraft.Tests;

public class SemVersionKeyTests
{
    [Fact]
    public void EqualOperator_IdenticalInstances_ReturnsTrue()
    {
        // Arrange
        var instanceA = new SemVersionKey(new SemVersion(1, 0, 0));
        var instanceB = new SemVersionKey(new SemVersion(1, 0, 0));
        
        // Act
        var result = instanceA == instanceB;
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void Equals_IdenticalInstances_ReturnsTrue()
    {
        // Arrange
        var instanceA = new SemVersionKey(new SemVersion(1, 0, 0));
        var instanceB = new SemVersionKey(new SemVersion(1, 0, 0));
        
        // Act
        var result = instanceA.Equals(instanceB);
        
        // Assert
        Assert.True(result);
    }
}