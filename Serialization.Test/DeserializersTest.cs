// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using FluentAssertions;
using Xunit;

namespace Econolite.Ode.Serialization.Test;

public class DeserializersTest
{
    [Fact]
    private void GuidTest()
    {
        var raw = ByteStreamHelper.Convert("EFBEADDE AD8B 0DF0 0000 000000000001");

        var testresults = Deserializers.Guid(raw);

        var expected = Guid.Parse("DEADBEEF-8BAD-F00D-0000-000000000001");
        testresults.Should().Be(expected);
    }

    [Fact]
    private void InvalidArrayGuidTest()
    {
        var act = () =>
        {
            var raw = ByteStreamHelper.Convert("EFBEADDE AD8B 0DF0 0000");

            var testresults = Deserializers.Guid(raw);
        };
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    private void NullGuidTest()
    {
        var act = () =>
        {
            byte[]? raw = null;
            var testresults = Deserializers.Guid(raw);
        };
        act.Should().Throw<ArgumentNullException>();
    }
}
