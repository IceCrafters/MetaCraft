// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Archive.References;
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
    
    private static MockPackageScope CreateNoOpReferralScope(MockPackageContainer container)
    {
        return new MockPackageScope(container, new PackageReferralDatabase(new NoOpReferralStore(),
            new NullReferralPreferenceProvider()));
    }
    
    private static MockPackageScope CreateRealReferralScope(MockPackageContainer container)
    {
        return new MockPackageScope(container, new PackageReferralDatabase(new MockReferralStore(),
            new NullReferralPreferenceProvider()));
    }

    [Fact]
    public void HasDependents_NoDependentPackages_ReturnsFalse()
    {
        // Arrange
        // empty container
        var container = new MockPackageContainer(1122L);
        var scope = new MockPackageScope(container, new PackageReferralDatabase(new NoOpReferralStore(), 
            new NullReferralPreferenceProvider()));

        var depChecker = new DependencyChecker(scope);
        
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
        var container = new MockPackageContainer(1122L)
            .WithPackage("dependent",
                new SemVersion(1, 0, 0),
                DateTime.MinValue,
                dependencies: [new RangedPackageReference("example-lib", SemVersionRange.All)]);
        var scope = new MockPackageScope(container, new PackageReferralDatabase(new NoOpReferralStore(),
            new NullReferralPreferenceProvider()));

        var depChecker = new DependencyChecker(scope);
        
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
        var container = new MockPackageContainer(1122L);
        var scope = CreateNoOpReferralScope(container);

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
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesDependencySatisfy_HaveConflictClauseInLocal_ReturnsFalse()
    {
        // Arrange
        var container = new MockPackageContainer(1122L)
            .WithPackage("test", Ver100, DateTime.MinValue, 
                provides: [ new PackageReference("conflicting-clause", Ver100) ]);

        var scope = CreateRealReferralScope(container);

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicting-clause")]),
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void DoesDependencySatisfy_HaveConflictInLocal_ReturnsFalse()
    {
        // Arrange
        var container = new MockPackageContainer(1122L)
            .WithPackage("conflicting", Ver100, DateTime.MinValue);

        var scope = CreateNoOpReferralScope(container);

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicting")])
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisfiedClauseInLocal_ReturnsTrue()
    {
        // Arrange
        var container = new MockPackageContainer(1122L)
            .WithPackage("test", Ver100, DateTime.MinValue,
                provides: [ new PackageReference("referenced", Ver100) ]);
        var scope = CreateRealReferralScope(container);

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("referenced")]),
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisfiedInLocal_ReturnsTrue()
    {
        // Arrange
        var container = new MockPackageContainer(1122L)
            .WithPackage("required", Ver100, DateTime.MinValue);
        var scope = CreateRealReferralScope(container);

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("required")]),
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SatisfiedInSet_ReturnsTrue()
    {
        // Arrange
        var container = new MockPackageContainer(1122L)
            .WithPackage("conflicting", Ver100, DateTime.MinValue);
        var scope = CreateNoOpReferralScope(container);

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
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_SelfProvideConflictingClause_ReturnsTrue()
    {
        // Arrange
        var container = new MockPackageContainer(1122L);
        var scope = CreateNoOpReferralScope(container);

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                conflictsWith: [IsRanged("conflicted")],
                provides: [IsExact("conflicted", Ver100)])
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesDependencySatisfy_MissingDependency_ReturnsFalse()
    {
        // Arrange
        var container = new MockPackageContainer(1122L);
        // Missing the required package.
        var scope = CreateNoOpReferralScope(container);

        var set = new HashSet<PackageManifest>
        {
            ManifestHelper.CreateManifest("example",
                Ver100,
                DateTime.MinValue,
                dependencies: [IsRanged("required")])
        };

        // Act
        var result = DependencyChecker.DoesDependencySatisfy(set, scope, _agent);

        // Assert
        Assert.False(result);
    }
}
