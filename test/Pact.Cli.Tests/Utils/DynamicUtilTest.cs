using System;
using System.Collections.Generic;
using Pact.Cli.Utils;
using Xunit;

namespace Pact.Cli.Tests.Utils;

public class DynamicUtilTest
{
    private const byte byteNumber = 1;

    private const int intNumber = int.MaxValue;

    private const long longNumber = long.MaxValue;

    private const string s = "sample";

    private const char c = 'c';

    [Fact]
    public void IsByteReturnTrue()
    {
        dynamic arg = byteNumber;
        Assert.True(DynamicUtil.Is<byte>(arg));
    }

    [Theory]
    [InlineData(intNumber)]
    [InlineData(longNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void IsByteReturnFalse(dynamic arg)
    {
        Assert.False(DynamicUtil.Is<byte>(arg));
    }

    [Fact]
    public void IsIntReturnTrue()
    {
        dynamic arg = intNumber;
        Assert.True(DynamicUtil.Is<int>(arg));
    }

    [Theory]
    [InlineData(byteNumber)]
    [InlineData(longNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void IsIntReturnFalse(dynamic arg)
    {
        Assert.False(DynamicUtil.Is<int>(arg));
    }

    [Fact]
    public void IsLongReturnTrue()
    {
        Assert.True(DynamicUtil.Is<long>(longNumber));
    }

    [Theory]
    [InlineData(byteNumber)]
    [InlineData(intNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void IsLongReturnFalse(dynamic arg)
    {
        Assert.False(DynamicUtil.Is<long>(arg));
    }

    [Fact]
    public void IsArrayReturnTrue()
    {
        dynamic arg = Array.Empty<string>();
        Assert.True(DynamicUtil.Is<string[]>(arg));
    }

    [Fact]
    public void IsDictionaryTrue()
    {
        dynamic arg = new Dictionary<string, object>();
        Assert.True(DynamicUtil.Is<Dictionary<string, object>>(arg));
    }

    [Fact]
    public void IsDictionaryFalseByWrongTypeParameter()
    {
        dynamic arg = new Dictionary<int, string>();
        Assert.False(DynamicUtil.Is<Dictionary<long, object>>(arg));
    }

    [Fact]
    public void IsDictionaryFalseByWrongType()
    {
        dynamic arg = new List<string>();
        Assert.False(DynamicUtil.Is<Dictionary<long, object>>(arg));
    }

    [Fact]
    public void IsListTrue()
    {
        dynamic arg = new List<object>();
        Assert.True(DynamicUtil.Is<List<object>>(arg));
    }

    [Fact]
    public void CastToByteReturnCastedvalue()
    {
        dynamic arg = byteNumber;
        Assert.Equal(byteNumber, DynamicUtil.CastTo<byte>(arg));
    }

    [Theory]
    [InlineData(intNumber)]
    [InlineData(longNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void CastToByteReturnDefault(dynamic arg)
    {
        Assert.Equal(default(byte), DynamicUtil.CastTo<byte>(arg));
    }

    [Fact]
    public void CastToStringReturnCastedValue()
    {
        dynamic arg = "sample";
        Assert.Equal(s, DynamicUtil.CastTo<string>(arg));
    }

    [Theory]
    [InlineData(c)]
    [InlineData(byteNumber)]
    [InlineData(intNumber)]
    [InlineData(longNumber)]
    public void CastToStringReturnDefault(dynamic arg)
    {
        Assert.Null(DynamicUtil.CastTo<string>(arg));
    }
}
