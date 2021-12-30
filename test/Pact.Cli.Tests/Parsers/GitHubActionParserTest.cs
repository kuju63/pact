using System;
using Pact.Cli.Parsers;
using Xunit;

namespace Pact.Cli.Tests.Parsers;

public class GitHubActionParserTest
{
    private const string RequirementErrorYaml1 = @"
name: sample
jobs:
    sample:
    runs-on: ubuntu-latest
";
    private const string RequirementErrorYaml2 = @"
name: sample
on: push
";

    [Theory]
    [InlineData(RequirementErrorYaml1)]
    [InlineData(RequirementErrorYaml2)]
    public void ParseActionThrowngitHubParseException(string yaml)
    {
        Assert.Throws<GitHubActionParserException>(() => GitHubActionParser.ParseAction(yaml));
    }

    [Fact]
    public void ParseActionSetOnString()
    {
        var yaml = @"
name: sample
on: push
jobs:
  sample:
    runs-on: ubuntu-latest";
        var actual = GitHubActionParser.ParseAction(yaml);
        Assert.Equal("push", actual.OnString);
    }

    [Fact]
    public void ParseActionSetOnStringArray()
    {
        var yaml = @"
name: sample
on: [push, pull_request]
jobs:
  sample:
    runs-on: ubuntu-latest";

        var actual = GitHubActionParser.ParseAction(yaml);
        Assert.Collection(actual.OnStringArray, x => Assert.Equal("push", x), x => Assert.Equal("pull_request", x));
    }

    [Fact]
    public void ParseActionSetOnEvent()
    {
        var yaml = @"
name: sample
on:
  push:
  pull_request:
    types: review_requested
jobs:
  sample:
    runs-on: ubuntu-latest";

        var actual = GitHubActionParser.ParseAction(yaml);
        Assert.Collection(
            actual.OnEvent,
            (x) =>
            {
                Assert.Equal("push", x.Key);
            },
            (x) =>
            {
                Assert.Equal("pull_request", x.Key);
                Console.WriteLine(x.Value);
            });
    }
}
