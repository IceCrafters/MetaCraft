// SPDX-FileCopyrightText: 2024 WithLithum <WithLithum@outlook.com>
// SPDX-License-Identifier: GPL-3.0-or-later

using MetaCraft.Core.Scopes;
using MetaCraft.Core.Transactions;
using Moq;

namespace MetaCraft.Tests;

public class FinalActionTransactionTests
{
    [Fact]
    public void FinalAction_ExecutedFails_ThrowsTransaction()
    {
        // Arrange
        var actionA = new Mock<ITransaction>();
        actionA.Setup(x => x.Commit(It.IsAny<ITransactionAgent>()))
            .Throws(() => new TransactionException());

        var actionB = new Mock<ITransaction>();
        actionB.Setup(x => x.Commit(It.IsAny<ITransactionAgent>()));

        var transaction = new FinalActionTransaction(Mock.Of<IPackageContainer>(),
            actionA.Object,
            actionB.Object);

        // Act
        var exception = Record.Exception(() => transaction.Commit(Mock.Of<ITransactionAgent>()));

        // Assert
        Assert.IsType<TransactionException>(exception);
    }

    [Fact]
    public void FinalAction_BothFails_ThrowsAggregate()
    {
        // Arrange
        var actionA = new Mock<ITransaction>();
        actionA.Setup(x => x.Commit(It.IsAny<ITransactionAgent>()))
            .Throws(() => new TransactionException());

        var actionB = new Mock<ITransaction>();
        actionB.Setup(x => x.Commit(It.IsAny<ITransactionAgent>()))
            .Throws(() => new TransactionException());

        var transaction = new FinalActionTransaction(Mock.Of<IPackageContainer>(),
            actionA.Object,
            actionB.Object);

        // Act
        var exception = Record.Exception(() => transaction.Commit(Mock.Of<ITransactionAgent>()));

        // Assert
        Assert.IsType<AggregateException>(exception);
    }
}
