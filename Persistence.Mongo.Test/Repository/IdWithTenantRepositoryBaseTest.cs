// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Econolite.Ode.Persistence.Mongo.Test.Helpers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test.Repository;

[Collection(nameof(MongoCollection))]
public abstract class IdWithTenantRepositoryBaseTest<TId, TTenant, TIdDocumentRepository, TIdDocument>
    where TIdDocument : class, IIndexedEntityWithTenant<TId, TTenant>
    where TIdDocumentRepository : DocumentRepositoryWithTenantBase<TIdDocument, TId, TTenant>
    where TId : new()
    where TTenant : new()
{
    private readonly ILogger<TIdDocumentRepository> _logger = Mock.Of<ILogger<TIdDocumentRepository>>();
    protected readonly MongoFixture Fixture;

    protected IdWithTenantRepositoryBaseTest(MongoFixture fixture)
    {
        Fixture = fixture;
        Mock.Get(LoggerFactory)
            .Setup(x => x.CreateLogger(typeof(TIdDocumentRepository).FullName!))
            .Returns(_logger);
        // Mock.Get(_logger)
        //     .Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<TIdDocumentRepository>(),
        //         It.IsAny<Exception?>(), It.IsAny<Func<TIdDocumentRepository, Exception?, string>>()));
    }

    protected IMongoContext Context { get; } = Mock.Of<IMongoContext>();

    protected ILoggerFactory LoggerFactory { get; } = Mock.Of<ILoggerFactory>();

    protected abstract TId Id { get; }
    protected abstract string ExpectedJsonIdFilter { get; }

    [Fact]
    public void GivenADocumentRepository_WhenAddIsCalled_CollectionIsTakenFromContextAndInsertIsCalled()
    {
        // arrange
        var target = CreateRepository();
        var expected = CreateDocument();
        var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(Context)
            .Setup(x => x.AddCommand(It.IsAny<Func<CancellationToken, Task>>()))
            .Callback(async (Func<CancellationToken, Task> func) => { await func.Invoke(CancellationToken.None).ConfigureAwait(false); })
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.InsertOneAsync(expected, null, default))
            .Returns(Task.FromResult(testCollection))
            .Verifiable();

        // act
        target.Add(expected);

        // assert
        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    [Fact]
    public void GivenADocumentRepository_WhenRemoveIsCalled_CollectionIsTakenFromContextAndDeleteOneIsCalled()
    {
        // arrange
        var target = CreateRepository();
        var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(Context)
            .Setup(x => x.AddCommand(It.IsAny<Func<CancellationToken, Task>>()))
            .Callback(async (Func<CancellationToken, Task> func) => { await func.Invoke(CancellationToken.None).ConfigureAwait(false); })
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.DeleteOneAsync(
                It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Contains(ExpectedJsonIdFilter)),
                default))
            .Returns(Task.FromResult((DeleteResult) new DeleteResult.Acknowledged(1L)))
            .Verifiable();

        // act
        target.Remove(Id);

        // assert
        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    [Fact]
    public async Task GivenADocumentRepository_WhenGetAllAsyncIsCalled_CollectionIsTakenFromContext()
    {
        // arrange
        var target = CreateRepository();
        var tenant = CreateTenant();
        var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
        var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
        const string expectedJsonFilter = "";
        var index = 0;
        var expected = new[] {CreateDocument(), CreateDocument()};

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(findCollection)
            .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken ct) => { index++; })
            .Returns(() => index < expected.Length);

        Mock.Get(findCollection)
            .Setup(x => x.Current)
            .Returns(expected);

        Mock.Get(testCollection)
            .Setup(x => x.FindAsync(
                It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                default))
            .Returns(Task.FromResult(findCollection))
            .Verifiable();

        // act
        var actual = await target.GetAllAsync(tenant).ConfigureAwait(false);

        // assert
        Assert.Equal(expected, actual);
        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    [Fact]
    public void GivenADocumentRepository_WhenGetAllIsCalled_CollectionIsTakenFromContext()
    {
        // arrange
        var target = CreateRepository();
        var tenant = CreateTenant();
        var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
        var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
        const string expectedJsonFilter = "";
        var index = 0;
        var expected = new[] {CreateDocument(), CreateDocument()};

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(findCollection)
            .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken ct) => { index++; })
            .Returns(() => index < expected.Length);

        Mock.Get(findCollection)
            .Setup(x => x.Current)
            .Returns(expected);

        Mock.Get(testCollection)
            .Setup(x => x.FindSync(
                It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                default))
            .Returns(findCollection)
            .Verifiable();

        // act
        var actual = target.GetAll(tenant);

        // assert
        Assert.Equal(expected, actual);
        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    [Fact]
    public async Task GivenADocumentRepository_WhenGetByIdAsyncIsCalled_CollectionIsTakenFromContext()
    {
        // arrange
        var target = CreateRepository();
        var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
        var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
        var expected = CreateDocument();
        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(findCollection)
            .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true);

        Mock.Get(findCollection)
            .Setup(x => x.Current)
            .Returns(new[] {expected});

        Mock.Get(testCollection)
            .Setup(x => x.FindAsync(
                It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Contains(ExpectedJsonIdFilter)),
                It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                default))
            .Returns(Task.FromResult(findCollection))
            .Verifiable();

        // act
        var actual = await target.GetByIdAsync(Id).ConfigureAwait(false);

        // assert
        Assert.Equal(expected, actual);
        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    [Fact]
    public void GivenADocumentRepository_WhenGetByIdIsCalled_CollectionIsTakenFromContext()
    {
        // arrange
        var target = CreateRepository();
        var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
        var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
        var expected = CreateDocument();
        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(findCollection)
            .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true);

        Mock.Get(findCollection)
            .Setup(x => x.Current)
            .Returns(new[] {expected});

        Mock.Get(testCollection)
            .Setup(x => x.FindSync(
                It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Contains(ExpectedJsonIdFilter)),
                It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                default))
            .Returns(findCollection)
            .Verifiable();

        // act
        var actual = target.GetById(Id);

        // assert
        Assert.Equal(expected, actual);
        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    [Fact]
    public async Task GivenAnErrorGettingTheCollection_WhenGetAllIsCalled_ExceptionIsThrown()
    {
        // arrange
        var target = CreateRepository();
        var tenant = CreateTenant();
        Mock.Get(Context)
            .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
            .Throws(new Exception("Test error message"))
            .Verifiable();

        // act
        var actual = await Assert.ThrowsAsync<Exception>(async () =>
                await target.GetAllAsync(tenant).ConfigureAwait(false))
            .ConfigureAwait(false);

        // assert
        Assert.Equal("Test error message", actual.Message);
        Mock.Verify(Mock.Get(Context));
    }

    protected abstract TIdDocumentRepository CreateRepository();
    protected abstract TIdDocument CreateDocument();
    protected abstract TTenant CreateTenant();
}
