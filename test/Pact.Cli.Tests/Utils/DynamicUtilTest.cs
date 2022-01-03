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
        Assert.True(DynamicUtil.Is<byte>(arg, out byte castvalue));
        Assert.Equal(byteNumber, castvalue);
    }

    [Theory]
    [InlineData(intNumber)]
    [InlineData(longNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void IsByteReturnFalse(dynamic arg)
    {
        Assert.False(DynamicUtil.Is<byte>(arg, out byte castValue));
        Assert.Equal(default(byte), castValue);
    }

    [Fact]
    public void IsIntReturnTrue()
    {
        dynamic arg = intNumber;
        Assert.True(DynamicUtil.Is<int>(arg, out int castvalue));
        Assert.Equal(intNumber, castvalue);
    }

    [Theory]
    [InlineData(byteNumber)]
    [InlineData(longNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void IsIntReturnFalse(dynamic arg)
    {
        Assert.False(DynamicUtil.Is<int>(arg, out int castValue));
        Assert.Equal(default(int), castValue);
    }

    [Fact]
    public void IsLongReturnTrue()
    {
        Assert.True(DynamicUtil.Is<long>(longNumber, out long castValue));
        Assert.Equal(longNumber, castValue);
    }

    [Theory]
    [InlineData(byteNumber)]
    [InlineData(intNumber)]
    [InlineData(s)]
    [InlineData(c)]
    public void IsLongReturnFalse(dynamic arg)
    {
        Assert.False(DynamicUtil.Is<long>(arg, out long castValue));
        Assert.Equal(default(long), castValue);
    }

    [Fact]
    public void IsArrayReturnTrue()
    {
        dynamic arg = Array.Empty<string>();
        Assert.True(DynamicUtil.Is<string[]>(arg, out string[] castValue));
        Assert.Empty(castValue);
    }

    [Fact]
    public void IsDictionaryTrue()
    {
        dynamic arg = new Dictionary<string, object>();
        Assert.True(DynamicUtil.Is<Dictionary<string, object>>(arg, out Dictionary<string, object> castValue));
        Assert.Empty(castValue);
    }

    [Fact]
    public void IsDictionaryFalseByWrongTypeParameter()
    {
        dynamic arg = new Dictionary<int, string>();
        Assert.False(DynamicUtil.Is<Dictionary<long, object>>(arg, out Dictionary<long, object> castValue));
        Assert.Null(castValue);
    }

    [Fact]
    public void IsDictionaryFalseByWrongType()
    {
        dynamic arg = new List<string>();
        Assert.False(DynamicUtil.Is<Dictionary<long, object>>(arg, out Dictionary<long, object> castValue));
        Assert.Null(castValue);
    }

    [Fact]
    public void IsListTrue()
    {
        dynamic arg = new List<object>();
        Assert.True(DynamicUtil.Is<List<object>>(arg, out List<object> castValue));
        Assert.Empty(castValue);
    }
}
