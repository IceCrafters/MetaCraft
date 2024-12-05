// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Platform;
using MetaCraft.Core.Scopes;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Core.Serialization;
using MetaCraft.Core.Transactions;
using MetaCraft.Tests.Util;
using Moq;
using Semver;
using Xunit.Abstractions;

namespace MetaCraft.Tests;

public class ReferralTests
{
    private readonly TestTransactionAgent _agent;

    public ReferralTests(ITestOutputHelper outputHelper)
    {
        _agent = new TestTransactionAgent(outputHelper);
    }

    [Fact]
    public void UpdateTransaction_ProvidedVirtualPackage_VirtualClausePresent()
    {
        // Arrange
        var parent = new Mock<IPackageScope>();
        var target = new Mock<IPackageContainer>();

        parent.SetupGet(x => x.Container).Returns(target.Object);
        parent.SetupGet(x => x.Root).Returns(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

        target.SetupGet(x => x.Parent).Returns(parent.Object);
        target.Setup(x => x.EnumeratePackages()).Returns(["example"]);
        target.Setup(x => x.EnumerateVersions("example")).Returns([new SemVersion(2, 3, 0)]);
        target.Setup(x => x.GetSerial()).Returns(1122L);

        // Add package
        target.Setup(x => x.InspectLocal("example", It.IsAny<SemVersion>()))
            .Returns(new Core.PackageManifest
            {
                Id = "example",
                Version = new SemVersion(1, 0, 0),
                PackageTime = DateTime.MinValue,
                Platform = PlatformIdentifier.Current,
                Provides = new PackageReferenceDictionary { {"provided", new SemVersion(1, 2, 0)} }
            });

        // Setup database
        var preference = new Mock<IReferralPreferenceProvider>();
        preference
            .Setup(x => x.GetPreferredId(It.IsAny<string>(), It.IsAny<SemVersion>()))
            .Returns("does not matter");

        var dbStore = new Mock<IReferralDatabaseStore>();

        var db = new PackageReferralDatabase(dbStore.Object, preference.Object);
        parent.SetupGet(x => x.Referrals).Returns(db);

        var transaction = new UpdateReferrersTransaction(target.Object, new UpdateReferrersTransaction.Parameters
        {
            IgnoreSerial = true // force regeneration
        });

        // Act
        transaction.Commit(_agent);

        // Assert
        dbStore.Verify(x => x.WriteFile("provided", It.Is<ReferralIndexDictionary>(
            x => x.ContainsKey(new SemVersionKey(new SemVersion(1, 2, 0)))
        )));
    }

    [Fact]
    public void UpdateTransaction_ProvidedVirtualPackage_InsertsCorrectly()
    {
        // Arrange
        var parent = new Mock<IPackageScope>();
        var target = new Mock<IPackageContainer>();

        parent.SetupGet(x => x.Container).Returns(target.Object);
        parent.SetupGet(x => x.Root).Returns(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

        target.SetupGet(x => x.Parent).Returns(parent.Object);
        target.Setup(x => x.EnumeratePackages()).Returns(["example"]);
        target.Setup(x => x.EnumerateVersions("example")).Returns([new SemVersion(2, 3, 0)]);
        target.Setup(x => x.GetSerial()).Returns(1122L);

        // Add package
        target.Setup(x => x.InspectLocal("example", It.IsAny<SemVersion>()))
            .Returns(new Core.PackageManifest
            {
                Id = "example",
                Version = new SemVersion(2, 3, 0),
                PackageTime = DateTime.MinValue,
                Platform = PlatformIdentifier.Current,
                Provides = new PackageReferenceDictionary { {"provided", new SemVersion(1, 2, 0)} }
            });

        // Setup database
        var preference = new Mock<IReferralPreferenceProvider>();
        preference
            .Setup(x => x.GetPreferredId(It.IsAny<string>(), It.IsAny<SemVersion>()))
            .Returns("does not matter");

        var dbStore = new Mock<IReferralDatabaseStore>();

        var db = new PackageReferralDatabase(dbStore.Object, preference.Object);
        parent.SetupGet(x => x.Referrals).Returns(db);

        var transaction = new UpdateReferrersTransaction(target.Object, new UpdateReferrersTransaction.Parameters
        {
            IgnoreSerial = true // force regeneration
        });

        // Act
        transaction.Commit(_agent);

        // Assert
        dbStore.Verify(x => x.WriteFile("provided", It.Is<ReferralIndexDictionary>(
            x => x[new SemVersionKey(new SemVersion(1, 2, 0))].Referrers.ContainsKey("example-2.3.0")
        )));
    }

    [Fact]
    public void UpdateTransaction_ValidPreferredPresent_UsePreferred()
    {
        // Arrange
        var parent = new Mock<IPackageScope>();
        var target = new Mock<IPackageContainer>();

        parent.SetupGet(x => x.Container).Returns(target.Object);
        parent.SetupGet(x => x.Root).Returns(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

        target.SetupGet(x => x.Parent).Returns(parent.Object);
        target.Setup(x => x.EnumeratePackages()).Returns(["another", "example"]);
        // The correct one is example. It is sorted as the second item, by default selection.
        target.Setup(x => x.EnumerateVersions("another")).Returns([new SemVersion(2, 0, 0)]);
        target.Setup(x => x.EnumerateVersions("example")).Returns([new SemVersion(1, 0, 0)]);
        target.Setup(x => x.GetSerial()).Returns(1122L);

        // Add packages
        target.Setup(x => x.InspectLocal("another", It.IsAny<SemVersion>()))
            .Returns(new Core.PackageManifest
            {
                Id = "another",
                Version = new SemVersion(2, 0, 0),
                PackageTime = DateTime.MinValue,
                Platform = PlatformIdentifier.Current,
                Provides = new PackageReferenceDictionary { {"provided", new SemVersion(1, 2, 0)} }
            });

        target.Setup(x => x.InspectLocal("example", It.IsAny<SemVersion>()))
            .Returns(new Core.PackageManifest
            {
                Id = "example",
                Version = new SemVersion(1, 0, 0),
                PackageTime = DateTime.MinValue,
                Platform = PlatformIdentifier.Current,
                Provides = new PackageReferenceDictionary { {"provided", new SemVersion(1, 2, 0)} }
            });

        // Setup database
        var preference = new Mock<IReferralPreferenceProvider>();
        preference.Setup(x => x.GetPreferredId("provided", It.IsAny<SemVersion>())).Returns("example-1.0.0");
        // preference.Setup(x => x.GetPreferredId(It.IsAny<string>(), It.IsAny<SemVersion>())).Returns("does not matter");

        var dbStore = new Mock<IReferralDatabaseStore>();

        var db = new PackageReferralDatabase(dbStore.Object, preference.Object);
        parent.SetupGet(x => x.Referrals).Returns(db);

        var transaction = new UpdateReferrersTransaction(target.Object, new UpdateReferrersTransaction.Parameters
        {
            IgnoreSerial = true
        });

        // Act
        transaction.Commit(_agent);

        // Assert
        dbStore.Verify(x => x.WriteFile("provided", It.Is<ReferralIndexDictionary>(
            x => x[new SemVersionKey(new SemVersion(1, 2, 0))].Current == "example-1.0.0"
        )));
    }

    [Fact]
    public void ContainsClause_MatchingVersion_ReturnsTrue()
    {
        // Arrange
        var store = new Mock<IReferralDatabaseStore>();
        store.Setup(x => x.ReadFile("example"))
            .Returns(new ReferralIndexDictionary
            {
                { 
                    /*  key  */ new SemVersionKey(new SemVersion(1, 0, 0)), 
                    /* value */ new PackageReferralIndex(
                        new PackageReferrerDictionary(), current: "does not matter here") 
                }
            });

        var database = new PackageReferralDatabase(store.Object,
            Mock.Of<IReferralPreferenceProvider>());
        
        // Act
        var result = database.ContainsClause("example", new SemVersion(1, 0, 0));
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ContainsClause_VersionArgumentWithNoMatchingVersion_ReturnsFalse()
    {
        // Arrange
        var store = new Mock<IReferralDatabaseStore>();
        store.Setup(x => x.ReadFile("example"))
            .Returns(new ReferralIndexDictionary());

        var database = new PackageReferralDatabase(store.Object,
            Mock.Of<IReferralPreferenceProvider>());
        
        // Act
        var result = database.ContainsClause("example", new SemVersion(1, 0, 0));
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ContainsClause_NoMatchingFile_ReturnsFalse()
    {
        // Arrange
        var store = new Mock<IReferralDatabaseStore>();
        store.Setup(x => x.ReadFile("example"))
            .Returns(() => null);
        
        var database = new PackageReferralDatabase(store.Object,
            Mock.Of<IReferralPreferenceProvider>());
        
        // Act
        var result = database.ContainsClause("example", new SemVersion(1, 0, 0));
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ContainsClause_VersionIsNullAndClauseTreeExists_ReturnsTrue()
    {
        // Arrange
        var store = new Mock<IReferralDatabaseStore>();
        store.Setup(x => x.ReadFile("example"))
            .Returns(new ReferralIndexDictionary
            {
                { new SemVersionKey(new SemVersion(1, 0, 0)), new PackageReferralIndex([],
                    "which is current does not matter here") }
            });

        var database = new PackageReferralDatabase(store.Object,
            Mock.Of<IReferralPreferenceProvider>());
        
        // Act
        var result = database.ContainsClause("example", null);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void ReferralDatabase_MultipleReferrer_SelectCurrent()
    {
        // Arrange
        var expected = new PackageReference("test2-1.3.0", new SemVersion(1, 3, 0));

        var testData = new ReferralIndexDictionary
        {
            {
                new SemVersionKey(new SemVersion(1, 3, 0)),
                new PackageReferralIndex(new PackageReferrerDictionary()
                {
                    { "test1", new PackageReference("test1-1.3.0", new SemVersion(1, 3, 0)) },
                    { "test2", expected }
                }, 
                current: "test2")
            }
        };

        var storageMock = new Mock<IReferralDatabaseStore>();
        storageMock.Setup(x => x.ReadFile("example"))
            .Returns(testData);

        var database = new PackageReferralDatabase(storageMock.Object, new NullReferralPreferenceProvider());

        // Act
        var result = database.Locate(new RangedPackageReference(
            "example", 
            SemVersionRange.All)
        );

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expected, result.Value);
    }

    [Fact]
    public void ReferralDatabase_MultipleMatching_SelectLatestMatch()
    {
        // Arrange
        var expectedVer = new SemVersion(1, 3, 0);

        var testData = new ReferralIndexDictionary
        {
            {
                new SemVersionKey(new SemVersion(1, 2, 0)),
                new PackageReferralIndex(new PackageReferrerDictionary()
                {
                    { "test1", new PackageReference("test1-1.2.0", new SemVersion(1, 2, 0)) }
                }, 
                current: "test1")
            },
            {
                new SemVersionKey(expectedVer), // 1.3.0
                new PackageReferralIndex(new PackageReferrerDictionary()
                {
                    { "test1", new PackageReference("test1-1.3.0", new SemVersion(1, 3, 0)) }
                }, 
                current: "test1")
            }
        };

        var storageMock = new Mock<IReferralDatabaseStore>();
        storageMock.Setup(x => x.ReadFile("example"))
            .Returns(testData);

        var database = new PackageReferralDatabase(storageMock.Object, new NullReferralPreferenceProvider());

        // Act
        var result = database.Locate(new RangedPackageReference("example", SemVersionRange.GreaterThan(new SemVersion(1, 0, 0))));

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedVer, result.Value.Version);
    }
}
