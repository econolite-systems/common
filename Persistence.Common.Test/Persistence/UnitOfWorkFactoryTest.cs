// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Common.Persistence;
using Econolite.Ode.Persistence.Common.Repository;
using Moq;
using Xunit;

namespace Econolite.Ode.Persistence.Common.Test.Persistence;

public class UnitOfWorkFactoryTest
{
    [Fact]
    public void GivenARepository_WhenCreateUnitOfWorkIsCalled_ThenUnitOfWorkIsReturned()
    {
        var repository = Mock.Of<IRepository>();
        var target = new UnitOfWorkFactory();

        var actual = target.CreateUnitOfWork(repository);

        Assert.IsType<UnitOfWork>(actual);
    }
}
