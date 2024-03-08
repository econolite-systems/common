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
public class GuidRepositoryWithTenantBaseTest : IdWithTenantRepositoryBaseTest<Guid, Guid, TestGuidWithTenantRepository, TestGuidWithTenantDocument>
{
    public GuidRepositoryWithTenantBaseTest(MongoFixture fixture) : base(fixture)
    {
    }

    protected override Guid Id { get; } = Guid.NewGuid();

    protected override string ExpectedJsonIdFilter => "UUID(\"" + Id + "\")";

    [Fact]
    public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
    {
        var tenant = CreateTenant();
        var target = CreateRepository();
        var expected = new TestGuidWithTenantDocument {Id = Id, TenantId = tenant};
        var testCollection = Mock.Of<IMongoCollection<TestGuidWithTenantDocument>>();

        Mock.Get(Context)
            .Setup(x => x.GetCollection<TestGuidWithTenantDocument>(target.CollectionName))
            .Returns(testCollection)
            .Verifiable();

        Mock.Get(Context)
            .Setup(x => x.AddCommand(It.IsAny<Func<CancellationToken, Task>>()))
            .Callback(async (Func<CancellationToken, Task> func) => { await func.Invoke(CancellationToken.None).ConfigureAwait(false); })
            .Verifiable();

        Mock.Get(testCollection)
            .Setup(x => x.ReplaceOneAsync(
                It.Is<FilterDefinition<TestGuidWithTenantDocument>>(filter => filter.RenderToJson().Contains(ExpectedJsonIdFilter)),
                expected,
                It.IsAny<ReplaceOptions>(),
                default))
            .Returns(Task.FromResult((ReplaceOneResult) new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
            .Verifiable();


        target.Update(expected);


        Mock.Verify(Mock.Get(Context));
        Mock.Verify(Mock.Get(testCollection));
    }

    protected override TestGuidWithTenantRepository CreateRepository()
    {
        return new (Context, LoggerFactory.CreateLogger<TestGuidWithTenantRepository>());
    }

    protected override TestGuidWithTenantDocument CreateDocument()
    {
        return new();
    }

    protected override Guid CreateTenant()
    {
        return Guid.Empty;
    }
}

public class TestGuidWithTenantRepository : DocumentRepositoryWithTenantBase<TestGuidWithTenantDocument, Guid, Guid>
{
    public TestGuidWithTenantRepository(IMongoContext context, ILogger logger)
        : base(context, logger)
    {
    }
}

public class TestGuidWithTenantDocument : IndexedEntityWithTenantBase<Guid, Guid>
{
}
