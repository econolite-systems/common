// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Mongo.Context;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace Econolite.Ode.Persistence.Mongo.Test.Helpers;

[ExcludeFromCodeCoverage]
public static class RepositorySetup
{
    public static void SetupInsertOne<TDocument>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, TDocument expected)
    {
        SetupContextForGetCollection(collectionName, context, testCollection);
        SetupContextForAddCommand(context);
        Mock.Get(testCollection)
            .Setup(x => x.InsertOneAsync(expected, null, default))
            .Returns(Task.FromResult(testCollection))
            .Verifiable();
    }

    public static void SetupInsertOneWithParent<TDocument>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, TDocument expected, string expectedJsonFilter,
        string expectedJsonUpdate, int parentModifiedCount = 1)
    {
        SetupContextForGetCollection(collectionName, context, testCollection);
        SetupContextForAddCommand(context);
        Mock.Get(testCollection)
            .Setup(x => x.InsertOneAsync(expected, null, default))
            .Returns(Task.FromResult(testCollection))
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.UpdateOneAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.Is<UpdateDefinition<TDocument>>(definition =>
                    definition.RenderToJson().Contains(expectedJsonUpdate)),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(Task.FromResult(
                (UpdateResult) new UpdateResult.Acknowledged(1, parentModifiedCount, new BsonInt64(1))))
            .Verifiable();
    }

    public static void SetupUpdateOneWithParent<TDocument>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, TDocument expected, string expectedJsonIdFilter,
        string expectedJsonFilter, string expectedJsonUpdate, int parentModifiedCount = 1)
    {
        SetupContextForGetCollection(collectionName, context, testCollection);
        SetupContextForAddCommand(context);

        Mock.Get(testCollection)
            .Setup(x => x.ReplaceOneAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonIdFilter)),
                expected,
                It.IsAny<ReplaceOptions>(),
                default))
            .Returns(Task.FromResult((ReplaceOneResult) new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.UpdateManyAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.Is<UpdateDefinition<TDocument>>(definition =>
                    definition.RenderToJson().Contains(expectedJsonUpdate)),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(Task.FromResult(
                (UpdateResult) new UpdateResult.Acknowledged(1L, parentModifiedCount, new BsonInt64(1L))))
            .Verifiable();
    }

    public static void SetupUpdateOneWithParentAndLoadDocument<TDocument>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, TDocument expected, string expectedJsonIdFilter,
        string expectedJsonFilter, string expectedJsonUpdate, int parentModifiedCount = 1)
    {
        var expectedList = new[] {expected};
        SetupQueryCollection(collectionName, context, testCollection, expectedList, expectedJsonIdFilter);
        SetupContextForAddCommand(context);

        Mock.Get(testCollection)
            .Setup(x => x.ReplaceOneAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonIdFilter)),
                expected,
                It.IsAny<ReplaceOptions>(),
                default))
            .Returns(Task.FromResult((ReplaceOneResult) new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.UpdateManyAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.Is<UpdateDefinition<TDocument>>(definition =>
                    definition.RenderToJson().Contains(expectedJsonUpdate)),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(Task.FromResult(
                (UpdateResult) new UpdateResult.Acknowledged(1L, parentModifiedCount, new BsonInt64(1L))))
            .Verifiable();
    }

    public static void SetupDeleteOneWithParent<TDocument>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, string expectedJsonIdFilter, string expectedJsonFilter,
        string expectedJsonUpdate, int parentModifiedCount = 1)
    {
        SetupContextForGetCollection(collectionName, context, testCollection);
        SetupContextForAddCommand(context);

        Mock.Get(testCollection)
            .Setup(x => x.DeleteOneAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonIdFilter)),
                default))
            .Returns(Task.FromResult((DeleteResult) new DeleteResult.Acknowledged(1L)))
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.UpdateOneAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.Is<UpdateDefinition<TDocument>>(definition =>
                    definition.RenderToJson().Contains(expectedJsonUpdate)),
                It.IsAny<UpdateOptions>(),
                default))
            .Returns(Task.FromResult(
                (UpdateResult) new UpdateResult.Acknowledged(1L, parentModifiedCount, new BsonInt64(1L))))
            .Verifiable();
    }

    public static void SetupDistinctQueryCollection<TDocument, TResult>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, TResult[] expected,
        string expectedJsonFilter)
    {
        var findCollection = SetupFindCollection(collectionName, context, testCollection, expected);

        Mock.Get(testCollection)
            .Setup(x => x.DistinctAsync(
                It.IsAny<FieldDefinition<TDocument, TResult>>(),
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.IsAny<DistinctOptions>(),
                default))
            .Returns(Task.FromResult(findCollection))
            .Verifiable();
    }

    public static void SetupQueryCollection<TDocument, TResult>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection, TResult[] expected,
        string expectedJsonFilter)
    {
        var findCollection = SetupFindCollection(collectionName, context, testCollection, expected);

        Mock.Get(testCollection)
            .Setup(x => x.FindAsync(
                It.Is<FilterDefinition<TDocument>>(filter => filter.RenderToJson().Contains(expectedJsonFilter)),
                It.IsAny<FindOptions<TDocument, TResult>>(),
                default))
            .Returns(Task.FromResult(findCollection))
            .Verifiable();
    }

    public static IAsyncCursor<TResult> SetupFindCollection<TDocument, TResult>(string collectionName,
        IMongoContext context, IMongoCollection<TDocument> testCollection,
        TResult[] expected)
    {
        var findCollection = Mock.Of<IAsyncCursor<TResult>>();
        var index = 0;
        SetupContextForGetCollection(collectionName, context, testCollection);

        Mock.Get(findCollection)
            .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken ct) => { index++; })
            .Returns(() => index < expected.Length);

        Mock.Get(findCollection)
            .Setup(x => x.Current)
            .Returns(expected);

        return findCollection;
    }

    public static void SetupContextForGetCollection<TDocument>(string collectionName, IMongoContext context,
        IMongoCollection<TDocument> testCollection)
    {
        Mock.Get(context)
            .Setup(x => x.GetCollection<TDocument>(collectionName))
            .Returns(testCollection)
            .Verifiable();
    }

    public static void SetupContextForAddCommand(IMongoContext context)
    {
        Mock.Get(context)
            .Setup(x => x.AddCommand(It.IsAny<Func<CancellationToken, Task>>()))
            // This will run the command in the test
            .Callback(async (Func<Task> func) =>
            {
                try
                {
                    await func.Invoke().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // exception
                }
            })
            .Verifiable();
    }
}
