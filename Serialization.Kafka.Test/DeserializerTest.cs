// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Confluent.Kafka;
using Econolite.Ode.Serialization.Kafka.Binary;
using Econolite.Ode.Serialization.Test;
using FluentAssertions;
using Xunit;

namespace Econolite.Ode.Serialization.Kafka.Test;

public class DeserializersTest
{
    [Fact]
    private void GuidTest()
    {
        var raw = ByteStreamHelper.Convert("7A 6D F5 F7 FD 75 85 46 93 51 6B DA FF E5 86 97");

        var tobj = new GuidDeserializer();
        var context = new SerializationContext();
        var testresults = tobj.Deserialize(raw, false, context);
        var expected = Guid.Parse("f7f56d7a-75fd-4685-9351-6bdaffe58697");

        testresults.Should().Be(expected);
    }
}
