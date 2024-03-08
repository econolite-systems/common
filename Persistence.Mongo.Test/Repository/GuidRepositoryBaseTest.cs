// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Common.Entities;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Econolite.Ode.Persistence.Mongo.Test.Helpers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Mongo.Test.Repository;

[Collection(nameof(MongoCollection))]
public class GuidRepositoryBaseTest : IdRepositoryBaseTest<Guid, TestGuidRepository, TestGuidDocument>
{
    public GuidRepositoryBaseTest(MongoFixture fixture) : base(fixture)
    {
    }

    protected override Guid Id { get; } = Guid.NewGuid();

    protected override string ExpectedJsonIdFilter => "UUID(\"" + Id + "\")";

    [Fact]
    public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
    {
        var target = CreateRepository();
        var expected = new TestGuidDocument {Id = Id};
        var testCollection = Mock.Of<IMongoCollection<TestGuidDocument>>();

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TestGuidDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(Context)
            .Setup(x => x.AddCommand(It.IsAny<Func<CancellationToken, Task>>()))
            .Callback(async (Func<CancellationToken, Task> func) => { await func.Invoke(CancellationToken.None).ConfigureAwait(false); })
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.ReplaceOneAsync(
                It.Is<FilterDefinition<TestGuidDocument>>(filter => filter.RenderToJson().Contains(ExpectedJsonIdFilter)),
                expected,
                It.IsAny<ReplaceOptions>(),
                default))
            .Returns(Task.FromResult((ReplaceOneResult) new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
            .Verifiable();


        target.Update(expected);


        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    protected override TestGuidRepository CreateRepository()
    {
        return new TestGuidRepository(Context, LoggerFactory.CreateLogger<TestGuidRepository>());
    }

    protected override TestGuidDocument CreateDocument()
    {
        return new();
    }
}

public class TestGuidRepository : GuidDocumentRepositoryBase<TestGuidDocument>
{
    public TestGuidRepository(IMongoContext context, ILogger logger)
        : base(context, logger)
    {
    }
}

public class TestGuidDocument : IndexedEntityBase<Guid>
{
}
