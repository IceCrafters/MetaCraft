// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core;
using MetaCraft.Core.Archive;
using MetaCraft.Core.Dependency;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Testing;
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

    private static PackageReference IsExact(string id, SemVersion version)
    {
        return new PackageReference(id, version);
    }

    private static RangedPackageReference IsRanged(string id, SemVersionRange? range = null)
    {
        return new RangedPackageReference(id, range ?? SemVersionRange.All);
    }

    [Fact]
    public void HasDependents_NoDependentPackages_ReturnsFalse()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        // Empty container
        var container = new MockPackageContainer(scope.Object, 1122L);
        
        scope.SetupGet(x => x.Container).Returns(container);

        var depChecker = new DependencyChecker(scope.Object);
        
        // Act
        var result = depChecker.HasDependents(
            ManifestHelper.CreateManifest("example-lib",
                new SemVersion(1, 0, 0),
                DateTime.MinValue));
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void HasDependents_ThereExistsDependentPackages_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("dependent",
                new SemVersion(1, 0, 0),
                DateTime.MinValue,
                dependencies: [new RangedPackageReference("example-lib", SemVersionRange.All)]);
        
        scope.SetupGet(x => x.Container).Returns(container);

        var depChecker = new DependencyChecker(scope.Object);
        
        // Act
        var result = depChecker.HasDependents(
            ManifestHelper.CreateManifest("example-lib",
                new SemVersion(1, 0, 0),
                DateTime.MinValue));
        
        // Assert
        Assert.True(result);
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

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicting")]),
            ManifestHelper.CreateManifest("conflicting",
                Ver100,
                DateTime.MinValue)
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

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicting-clause")]),
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
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new MockReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicting")])
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisfiedClauseInLocal_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("test", Ver100, DateTime.MinValue,
                provides: [ new PackageReference("referenced", Ver100) ]);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new MockReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("referenced")]),
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisfiedInLocal_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("required", Ver100, DateTime.MinValue);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new MockReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("required")]),
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisfiedInSet_ReturnsTrue()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        var container = new MockPackageContainer(scope.Object, 1122L)
            .WithPackage("conflicting", Ver100, DateTime.MinValue);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("required")]),
            ManifestHelper.CreateManifest("required",
                Ver100,
                DateTime.MinValue)
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

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicted")],
                provides: [IsExact("conflicted", Ver100)])
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_MissingDependency_ReturnsFalse()
    {
        // Arrange
        var scope = new Mock<IPackageScope>();
        // Missing the required 'required' package.
        var container = new MockPackageContainer(scope.Object, 1122L);

        scope.SetupGet(x => x.Container).Returns(container);
        scope.SetupGet(x => x.Referrals).Returns(new PackageReferralDatabase(new MockReferralStore(), 
            new NullReferralPreferenceProvider()));

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("required")])
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope.Object, _agent);

        // Assert
        Assert.False(result);
    }
}
