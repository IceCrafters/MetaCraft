// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Archive;
using MetaCraft.Core.Scopes.Referral;
using MetaCraft.Core.Serialization;
using Moq;
using Semver;

namespace MetaCraft.Tests;

public class ReferralTests
{
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

        var database = new PackageReferralDatabase(storageMock.Object);

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

        var database = new PackageReferralDatabase(storageMock.Object);

        // Act
        var result = database.Locate(new RangedPackageReference("example", SemVersionRange.GreaterThan(new SemVersion(1, 0, 0))));

        // Assert
        Assert.True(result.HasValue);
        Assert.Equal(expectedVer, result.Value.Version);
    }
}
