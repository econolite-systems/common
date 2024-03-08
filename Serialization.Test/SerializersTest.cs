// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using FluentAssertions;
using Xunit;

namespace Econolite.Ode.Serialization.Test;

public class SerializersTest
{
    [Fact]
    private void GuidTest()
    {
        var raw = Guid.Parse("DEADBEEF-8BAD-F00D-0000-000000000001");

        var testresults = Serializers.Guid(raw);

        var expected = ByteStreamHelper.Convert("EFBEADDE AD8B 0DF0 0000 000000000001");
        testresults.Should().BeEquivalentTo(expected);
    }
}
