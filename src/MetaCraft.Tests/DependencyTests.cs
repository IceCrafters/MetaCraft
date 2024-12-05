// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Dependency;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Tests.Util;
using Moq;
using Semver;
using Xunit.Abstractions;

namespace MetaCraft.Tests;

public class DependencyTests
{
    private static readonly SemVersion Ver100 = new(1, 0, 0);
    private readonly TestTransactionAgent _agent;

    public DependencyTests(ITestOutputHelper outputHelper)
    {
        _agent = new TestTransactionAgent(outputHelper);
    }

    private static PackageReferenceDictionary OfSingleExact(string id, SemVersion? version = null)
    {
        var dict = new PackageReferenceDictionary();
        dict.Add(id, version ?? Ver100);
        return dict;
    }

    private static RangedPackageReferenceDictionary OfSingleRanged(string id, SemVersionRange? range = null)
    {
        var dict = new RangedPackageReferenceDictionary();
        dict.Add(id, range ?? SemVersionRange.All);
        return dict;
    }

    [Fact]
    public void DoesDependencySatisfy_HaveConflictInSet_ReturnsFalse()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                ConflictsWith = OfSingleRanged("conflicting")
            },
            new() {
                Id = "conflicting",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesDependencySatisfy_HaveConflictClauseInLocal_ReturnsFalse()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("test", Ver100, DateTime.MinValue, 
                provides: [ new PackageReference("conflicting-clause", Ver100) ]);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                ConflictsWith = OfSingleRanged("conflicting-clause")
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void DoesDependencySatisfy_HaveConflictInLocal_ReturnsFalse()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("conflicting", Ver100, DateTime.MinValue);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                ConflictsWith = OfSingleRanged("conflicting")
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisifiedClauseInLocal_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("test", Ver100, DateTime.MinValue,
                provides: [ new PackageReference("referenced", Ver100) ]);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new MockReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                Dependencies = OfSingleRanged("referenced")
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisifiedInLocal_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("required", Ver100, DateTime.MinValue);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new MockReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                Dependencies = OfSingleRanged("required")
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisifiedInSet_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("conflicting", Ver100, DateTime.MinValue);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                Dependencies = OfSingleRanged("required")
            },
            new() {
                Id = "required",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SelfProvideConflictingClause_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>()
        {
            new() {
                Id = "example",
                PackageTime = DateTime.MinValue,
                Version = Ver100,
                Platform = PlatformIdentifier.Current,
                ConflictsWith = OfSingleRanged("conflicted"),
                Provides = OfSingleExact("conflicted")
            }
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }
}
