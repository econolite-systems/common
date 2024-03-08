// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Threading;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Common.Contexts;
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Persistence.Common.Persistence;
using Econolite.Ode.Persistence.Common.Repository;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Common.Test.Persistence;

public class UnitOfWorkTest
{
    [Fact]
    public void GivenAListOfCommands_WhenCommitAsyncIsNotCalled_ContextSaveChangesIsNotCalled()
    {
        // arrange
        var repository = Mock.Of<IRepository<TestEntity, Guid>>();
        var context = Mock.Of<IDbContext>();

        Mock.Get(repository).Setup(x => x.DbContext).Returns(context);

        // act
        using (new UnitOfWork(repository))
        {
            var testEntity = new TestEntity
            {
                TestProperty = "Value"
            };

            repository.Add(testEntity);
        }

        // verify
        Mock.Get(context).Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task GivenAListOfCommands_WhenCommitAsyncIsCalled_ContextSaveChangesIsCalled()
    {
        // arrange
        var repository = Mock.Of<IRepository<TestEntity, Guid>>();
        var context = Mock.Of<IDbContext>();

        Mock.Get(repository).Setup(x => x.DbContext).Returns(context);

        // act
        using (var target = new UnitOfWork(repository))
        {
            var testEntity = new TestEntity
            {
                TestProperty = "Value"
            };

            repository.Add(testEntity);
            await target.SaveChangesAsync().ConfigureAwait(false);
        }

        // verify
        Mock.Get(context).Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }

    public class TestEntity : IIndexedEntity<Guid>
    {
        public string TestProperty { get; set; } = string.Empty;
        public Guid Id { get; set; }
    }
}
