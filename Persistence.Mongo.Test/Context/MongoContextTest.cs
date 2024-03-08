// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Common.Entities;
using Econolite.Ode.Persistence.Mongo.Client;
using Econolite.Ode.Persistence.Mongo.Context;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test.Context;

public class MongoContextTest
{
    private readonly IMongoDbClient _client = Mock.Of<IMongoDbClient>();
    private readonly IClientProvider _clientProvider = Mock.Of<IClientProvider>();
    private readonly IMongoDatabase _database = Mock.Of<IMongoDatabase>();
    private readonly ILogger<MongoContext> _logger = Mock.Of<ILogger<MongoContext>>();
    private readonly ILoggerFactory _loggerFactory = Mock.Of<ILoggerFactory>();

    public MongoContextTest()
    {
        Mock.Get(_loggerFactory).Setup(x => x.CreateLogger(typeof(MongoContext).FullName!)).Returns(_logger);
        Mock.Get(_clientProvider).Setup(x => x.Client).Returns(_client);
        Mock.Get(_clientProvider).Setup(x => x.Database).Returns(_database);
    }

    [Fact]
    public async Task GivenClientIsNull_WhenSaveChangesCalled_InvalidOperationExceptionIsThrown()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        Mock.Get(_clientProvider).Setup(x => x.Client).Returns(null as IMongoDbClient);


        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => target.SaveChangesAsync());


        Assert.Equal("Mongo check failed. Client is null", result.Message);
    }

    [Fact]
    public async Task GivenDatabaseIsNull_WhenSaveChangesCalled_InvalidOperationExceptionIsThrown()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        Mock.Get(_clientProvider).Setup(x => x.Database).Returns(null as IMongoDatabase);


        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => target.SaveChangesAsync());


        Assert.Equal("Mongo check failed. Database is null", result.Message);
    }

    [Fact]
    public void GivenClientIsNull_WhenGetCollectionCalled_InvalidOperationExceptionIsThrown()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        Mock.Get(_clientProvider).Setup(x => x.Client).Returns(null as IMongoDbClient);


        var result =
            Assert.Throws<InvalidOperationException>(
                () => target.GetCollection<TestDocument>(typeof(TestDocument).Name));


        Assert.Equal("Mongo check failed. Client is null", result.Message);
    }

    [Fact]
    public void GivenDatabaseIsNull_WhenGetCollectionCalled_InvalidOperationExceptionIsThrown()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        Mock.Get(_clientProvider).Setup(x => x.Database).Returns(null as IMongoDatabase);


        var result =
            Assert.Throws<InvalidOperationException>(
                () => target.GetCollection<TestDocument>(typeof(TestDocument).Name));


        Assert.Equal("Mongo check failed. Database is null", result.Message);
    }

    [Fact]
    public void GivenMongoClientAndDatabase_WhenCollectionRequested_DatabaseGetCollectionIsCalled()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        var expected = Mock.Of<IMongoCollection<TestDocument>>();

        Mock.Get(_database)
            .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).FullName, null))
            .Returns(expected)
            .Verifiable();


        var actual = target.GetCollection<TestDocument>(typeof(TestDocument).FullName!);


        Mock.Verify(Mock.Get(_database));
        Assert.Equal(expected, actual);
    }

    [Fact]
    [SuppressMessage("Blocker Code Smell",
        "S2699:Tests should include assertions",
        Justification = "Just a check to ensure that if no writes are made then, with the session being null, " +
                        "calling Dispose on it within the Dispose method of MongoContext does not throw an Exception")]
    public void GivenANoActiveSession_WhenDisposed_NoExceptionIsThrown()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);

        target.Dispose();
    }

    [Fact]
    public async Task GivenAnActiveSession_WhenDisposed_TheSessionIsDisposed()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        var sessionHandle = Mock.Of<IClientSessionHandle>();

        Mock.Get(_client)
            .Setup(x => x.CanSupportTransactions)
            .Returns(true)
            .Verifiable();

        Mock.Get(_client)
            .Setup(x => x.StartSessionAsync(null, default))
            .ReturnsAsync(sessionHandle)
            .Verifiable();


        await target.SaveChangesAsync();


        target.Dispose();


        Mock.Get(sessionHandle).Verify(x => x.Dispose());
        Mock.Verify(Mock.Get(_client));
    }

    [Fact]
    public async Task GivenTransactionsAreSupported_WhenSaveChangesCalled_TheCommandsAreActionedWithinATransaction()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        var sessionHandle = Mock.Of<IClientSessionHandle>();

        Mock.Get(_client)
            .Setup(x => x.CanSupportTransactions)
            .Returns(true)
            .Verifiable();

        Mock.Get(sessionHandle)
            .Setup(x => x.StartTransaction(null))
            .Verifiable();

        Mock.Get(sessionHandle)
            .Setup(x => x.CommitTransactionAsync(default))
            .Returns(Task.FromResult(0))
            .Verifiable();

        Mock.Get(_client)
            .Setup(x => x.StartSessionAsync(null, default))
            .ReturnsAsync(sessionHandle)
            .Verifiable();

        var commandHasRun = false;

        target.AddCommand((_) => Task.Run(() => commandHasRun = true));


        var (success, errors) = await target.SaveChangesAsync();


        Assert.True(commandHasRun);
        Mock.Verify(Mock.Get(sessionHandle));
    }

    [Fact]
    public async Task
        GivenTransactionsAreSupported_WhenSaveChangesCalled_TheCommandsAreActionedWithinATransactionWithExceptions()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);
        var sessionHandle = Mock.Of<IClientSessionHandle>();

        Mock.Get(_client)
            .Setup(x => x.CanSupportTransactions)
            .Returns(true)
            .Verifiable();

        Mock.Get(sessionHandle)
            .Setup(x => x.StartTransaction(null))
            .Verifiable();

        Mock.Get(sessionHandle)
            .Setup(x => x.AbortTransactionAsync(default))
            .Returns(Task.FromResult(0))
            .Verifiable();

        Mock.Get(_client)
            .Setup(x => x.StartSessionAsync(null, default))
            .ReturnsAsync(sessionHandle)
            .Verifiable();

        var commandHasRun = false;

        target.AddCommand((_) => Task.Run(() => throw new Exception("This is an exception")));
        target.AddCommand((_) => Task.Run(() => throw new Exception("This is an exception")));
        target.AddCommand((_) => Task.Run(() => commandHasRun = true));

        var (success, errors) = await target.SaveChangesAsync();

        success.Should().BeFalse();
        errors.Should().NotBeNull();
        commandHasRun.Should().BeTrue();
        Mock.Verify(Mock.Get(sessionHandle));
        Mock.Get(sessionHandle).Verify(x => x.CommitTransactionAsync(default), Times.Never);
    }

    [Fact]
    public async Task GivenTransactionsAreNotSupported_WhenSaveChangesCalled_TheCommandsAreActionedWithoutATransaction()
    {
        var target = new MongoContext(_clientProvider, _loggerFactory);

        Mock.Get(_client)
            .Setup(x => x.CanSupportTransactions)
            .Returns(false)
            .Verifiable();

        var commandHasRun = false;

        target.AddCommand((_) => Task.Run(() => commandHasRun = true));

        await target.SaveChangesAsync();

        Assert.True(commandHasRun);
        Mock.Get(_client).Verify(x => x.StartSessionAsync(null, default), Times.Never);
    }

    [Fact]
    public async Task
        GivenTransactionsAreNotSupported_WhenSaveChangesCalled_TheCommandsAreActionedWithoutATransactionWithException()
    {
        // arrange
        var target = new MongoContext(_clientProvider, _loggerFactory);

        Mock.Get(_client)
            .Setup(x => x.CanSupportTransactions)
            .Returns(false)
            .Verifiable();

        var commandHasRun = false;

        target.AddCommand((_) => Task.Run(() => throw new Exception("This is an exception!")));
        target.AddCommand((_) => Task.Run(() => throw new Exception("This is an exception!")));
        target.AddCommand((_) => Task.Run(() => commandHasRun = true));
        // act
        var (success, errors) = await target.SaveChangesAsync();

        // assert
        success.Should().BeFalse();
        errors.Should().NotBeNull();
        Assert.True(commandHasRun);
        Mock.Get(_client).Verify(x => x.StartSessionAsync(null, default), Times.Never);
    }

    public class TestDocument : IndexedEntityBase<Guid>
    {
    }
}
