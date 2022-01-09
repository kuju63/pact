using System;
using Pact.Cli.Models.Workflows;
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
    private const string FullYaml1 = @"
name: sample workflow
on:
  push:
    branches:
      - main
      - master
    tags:
      - v*.*.*
    paths:
      - src/**/*.cs
    branches-ignore:
      - test/*
    tags-ignore:
      - v*.*.*-beta.*
    paths-ignore:
      - build/*
      - dist/*
  pull_request:
    branches:
      - main
      - master
    tags:
      - v*.*.*
    paths:
      - src/**/*.cs
    branches-ignore:
      - test/*
    tags-ignore:
      - v*.*.*-beta.*
    paths-ignore:
      - build/*
      - dist/*
    types:
      - opened
      - synchronize
      - reopened
env:
  SAMPLE_ENV1: sample1
  SAMPLE_ENV2: sample2
defaults:
  run:
    shell: pwsh
    working-directory: work/sample
permissions: read-all
jobs:
  build:
    name: sample job1
    permissions: read-all
    runs-on: ubuntu-18.04
    env:
      JOB_SAMPLE_ENV1: sample3
      JOB_SAMPLE_ENV2: 1
    defaults:
      run:
        shell: bash
        working-directory: jobwork/sample
    if: ${{ github.event_name == 'push' }}
    steps:
      - name: checkout
        id: checkout
        uses: actions/checkout@v2
        with:
          fetch-depth: 1
      - name: run echo
        id: echo
        working-directory: step/sample
        shell: bash
        env:
          STEP_SAMPLE_ENV1: sample5
        run: |
          echo $STEP_SAMPLE_ENV1
  archive:
    runs-on: [self-hosted, linux, gpu]
    needs: [build]
    steps:
      - name: sample
        run: |
          echo ${{ github.event_name }}
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
    public void ParseActionSetOnStringNull()
    {
        var yaml = @"
name: sample
on: issues
jobs:
  sample:
    runs-on: ubuntu-latest";
        var actual = GitHubActionParser.ParseAction(yaml);
        Assert.Null(actual.OnString);
    }

    [Fact]
    public void ParseActionSetOnStringArray()
    {
        var yaml = @"
name: sample
on: [push, pull_request, issues]
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
  issues:
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

    [Fact]
    public void ParseActionFull()
    {
        var actual = GitHubActionParser.ParseAction(FullYaml1);
        Assert.Equal("sample workflow", actual.Name);
        Assert.Collection(
            actual.OnEvent,
            (kv) =>
            {
                Assert.Equal("push", kv.Key);
                Assert.IsType<PushTriggerEvent>(kv.Value);
                var triggerEvent = (PushTriggerEvent)kv.Value;
                Assert.Collection(
                    triggerEvent.Branches,
                    x => Assert.Equal("main", x),
                    x => Assert.Equal("master", x));
                Assert.Collection(
                    triggerEvent.BranchesIgnore,
                    x => Assert.Equal("test/*", x));
                Assert.Collection(
                    triggerEvent.Paths,
                    x => Assert.Equal("src/**/*.cs", x));
                Assert.Collection(
                    triggerEvent.PathsIgnore,
                    x => Assert.Equal("build/*", x),
                    x => Assert.Equal("dist/*", x));
                Assert.Collection(
                    triggerEvent.Tags,
                    x => Assert.Equal("v*.*.*", x));
                Assert.Collection(
                    triggerEvent.TagsIgnore,
                    x => Assert.Equal("v*.*.*-beta.*", x));
            },
            (kv) =>
            {
                Assert.Equal("pull_request", kv.Key);
                Assert.IsType<PullRequestTriggerEvent>(kv.Value);
                var triggerEvent = (PullRequestTriggerEvent)kv.Value;
                Assert.Collection(
                    triggerEvent.Branches,
                    x => Assert.Equal("main", x),
                    x => Assert.Equal("master", x));
                Assert.Collection(
                    triggerEvent.BranchesIgnore,
                    x => Assert.Equal("test/*", x));
                Assert.Collection(
                    triggerEvent.Paths,
                    x => Assert.Equal("src/**/*.cs", x));
                Assert.Collection(
                    triggerEvent.PathsIgnore,
                    x => Assert.Equal("build/*", x),
                    x => Assert.Equal("dist/*", x));
                Assert.Collection(
                    triggerEvent.Tags,
                    x => Assert.Equal("v*.*.*", x));
                Assert.Collection(
                    triggerEvent.TagsIgnore,
                    x => Assert.Equal("v*.*.*-beta.*", x));
                Assert.Collection(
                    triggerEvent.Types,
                    x => Assert.Equal("opened", x),
                    x => Assert.Equal("synchronize", x),
                    x => Assert.Equal("reopened", x));
            });
        Assert.Collection(
            actual.Jobs,
            (kv) =>
            {
                Assert.Equal("build", kv.Key);
                Assert.Equal("sample job1", kv.Value.Name);
                Assert.Equal("ubuntu-18.04", kv.Value.RunsOn);
                Assert.Null(kv.Value.RunsOnArray);
                Assert.Null(kv.Value.Needs);
                Assert.Collection(
                    kv.Value.Env,
                    kv2 =>
                    {
                        Assert.Equal("JOB_SAMPLE_ENV1", kv2.Key);
                        Assert.Equal("sample3", kv2.Value);
                    },
                    kv2 =>
                    {
                        Assert.Equal("JOB_SAMPLE_ENV2", kv2.Key);
                        Assert.Equal("1", kv2.Value);
                    });
                Assert.Collection(
                    kv.Value.Steps,
                    step =>
                    {
                        Assert.Equal("checkout", step.Name);
                        Assert.Equal("checkout", step.Id);
                        Assert.Equal("actions/checkout@v2", step.Uses);
                        Assert.Collection(
                            step.With,
                            with =>
                            {
                                Assert.Equal("fetch-depth", with.Key);
                                Assert.Equal("1", with.Value);
                            });
                    },
                    step =>
                    {
                        Assert.Equal("run echo", step.Name);
                        Assert.Equal("echo", step.Id);
                        Assert.Equal("step/sample", step.WorkingDirectory);
                        Assert.Equal(Shell.Bash, step.Shell);
                        Assert.Collection(
                            step.Env,
                            env =>
                            {
                                Assert.Equal("STEP_SAMPLE_ENV1", env.Key);
                                Assert.Equal("sample5", env.Value);
                            });
                        Assert.Equal("echo $STEP_SAMPLE_ENV1\n", step.Run);
                    }
                );
            },
            (kv) =>
            {
                Assert.Equal("archive", kv.Key);
                Assert.Null(kv.Value.Name);
                Assert.Collection(
                    kv.Value.RunsOnArray,
                    x => Assert.Equal("self-hosted", x),
                    x => Assert.Equal("linux", x),
                    x => Assert.Equal("gpu", x));
                Assert.Collection(
                    kv.Value.Needs,
                    x => Assert.Equal("build", x));
                Assert.Collection(
                    kv.Value.Steps,
                    step =>
                    {
                        Assert.Equal("sample", step.Name);
                        Assert.Equal("echo ${{ github.event_name }}\n", step.Run);
                    });
            }
        );
        Assert.Collection(
            actual.Env,
            (kv) =>
            {
                Assert.Equal("SAMPLE_ENV1", kv.Key);
                Assert.Equal("sample1", kv.Value);
            },
            (kv) =>
            {
                Assert.Equal("SAMPLE_ENV2", kv.Key);
                Assert.Equal("sample2", kv.Value);
            });
    }
}
