// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Econolite.Ode.Persistence.Common.Contexts;
using Econolite.Ode.Persistence.Common.Entities;
using Econolite.Ode.Persistence.Common.Repository;
using Xunit;

namespace Econolite.Ode.Persistence.Common.Test.Repository;

public class RepositoryBaseTest
{
    [Fact]
    public void GivenARepositoryClass_WhenInstantiated_ThenDbContextIsSet()
    {
        var context = new TestContext();

        var target = new TestRepository(context);

        Assert.Equal(context, target.DbContext);
    }

    [Fact]
    public void GivenARepositoryClass_WhenDisposed_ThenDbContextIsDisposed()
    {
        var context = new TestContext();
        var target = new TestRepository(context);

        target.Dispose();

        Assert.True(context.IsDisposed);
    }

    public class TestRepository : RepositoryBase<TestEntity, TestContext, Guid>
    {
        public TestRepository(TestContext context)
            : base(context)
        {
            CollectionName = "Test";
        }

        public override void Add(TestEntity document)
        {
            throw new NotImplementedException();
        }

        public override Task AddAsync(TestEntity document) => throw new NotImplementedException();

        public override Task<TestEntity?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<TestEntity>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public override TestEntity? GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<TestEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TestEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public override void Update(TestEntity document)
        {
            throw new NotImplementedException();
        }

        public override void Remove(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    public class TestContext : IDbContext
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        public void AddCommand(Func<CancellationToken, Task> func)
        {
            throw new NotImplementedException();
        }

        public Task<(bool success, string? errors)> SaveChangesAsync(CancellationToken? cancellationToken = null)
        {
            throw new NotImplementedException();
        }
    }

    public class TestEntity : IndexedEntityBase<Guid>
    {
    }
}
