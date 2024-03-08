// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Common.Records;
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
public class GuidRecordRepositoryBaseTest : IdRepositoryBaseTest<Guid, TestGuidRecordRepository, TestGuidRecord>
{
    public GuidRecordRepositoryBaseTest(MongoFixture fixture) : base(fixture)
    {
    }

    protected override Guid Id { get; } = Guid.NewGuid();

    protected override string ExpectedJsonIdFilter => "UUID(\"" + Id + "\")";

    [Fact]
    public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
    {
        var target = CreateRepository();
        var expected = new TestGuidRecord(Id);
        var testCollection = Mock.Of<IMongoCollection<TestGuidRecord>>();

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TestGuidRecord>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(Context)
            .Setup(x => x.AddCommand(It.IsAny<Func<CancellationToken, Task>>()))
            .Callback(async (Func<CancellationToken, Task> func) => { await func.Invoke(CancellationToken.None).ConfigureAwait(false); })
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.ReplaceOneAsync(
                It.Is<FilterDefinition<TestGuidRecord>>(filter => filter.RenderToJson().Contains(ExpectedJsonIdFilter)),
                expected,
                It.IsAny<ReplaceOptions>(),
                default))
            .Returns(Task.FromResult((ReplaceOneResult) new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
            .Verifiable();


        target.Update(expected);


        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    protected override TestGuidRecordRepository CreateRepository()
    {
        return new TestGuidRecordRepository(Context, LoggerFactory.CreateLogger<TestGuidRecordRepository>());
    }

    protected override TestGuidRecord CreateDocument()
    {
        return new(Id);
    }
}

public class TestGuidRecordRepository : GuidDocumentRecordRepositoryBase<TestGuidRecord>
{
    public TestGuidRecordRepository(IMongoContext context, ILogger logger)
        : base(context, logger)
    {
    }
}

public record TestGuidRecord(Guid Id) : GuidIndexedRecordBase(Id);
