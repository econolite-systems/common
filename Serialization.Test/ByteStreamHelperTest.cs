// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using FluentAssertions;
using Xunit;

namespace Econolite.Ode.Serialization.Test;

public class ByteStreamHelperTest
{
    [Fact]
    public void ByteStreamHelperTest01()
    {
        var input = " 02 03 04  05  ";

        byte[] expected = {2, 3, 4, 5};
        var result = ByteStreamHelper.Convert(input);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ByteStreamHelperTest02()
    {
        var input = "02,F30405";

        byte[] expected = {2, 0xF3, 4, 5};
        var result = ByteStreamHelper.Convert(input);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ByteStreamHelperTest03()
    {
        byte[] input = {1, 2, 3, 4, 5, 0xde, 0xad, 0xbe, 0xef};

        var expected = "01 02 03 04 05 DE AD BE EF";

        var result = ByteStreamHelper.Convert(input);

        result.Should().Be(expected);
    }
}
