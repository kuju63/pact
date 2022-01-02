using System.Dynamic;
using Pact.Cli.Models.Workflows;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pact.Cli.Parsers;

/// <summary>
/// Parser class of GitHub Actions workflow file.
/// </summary>
public class GitHubActionParser
{
    /// <summary>
    /// Initialize new instance of <see cref="GitHubActionParser" /> class.
    /// <para><see cref="GitHubActionParser"/> can not create new instance from other class,
    /// because <see cref="GitHubActionParser"/> is util class.</para>
    /// </summary>
    private GitHubActionParser()
    {
    }

    /// <summary>
    /// Parse workflow.
    /// </summary>
    /// <param name="workflow">Workflow content</param>
    /// <returns>Workflow object</returns>
    public static Workflow ParseAction(string workflow)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        dynamic workflowYaml = deserializer.Deserialize<ExpandoObject>(workflow);

        if (!HasProperty(workflowYaml, "on"))
        {
            throw new GitHubActionParserException(propertyName: "on");
        }

        if (!HasProperty(workflowYaml, "jobs"))
        {
            throw new GitHubActionParserException(propertyName: "jobs");
        }

        var workflowResult = new Workflow();

        if (HasProperty(workflowYaml, "name"))
        {
            workflowResult.Name = workflowYaml.name;
        }

        SetOnEvent(workflowYaml.on, workflowResult);

        return workflowResult;
    }

    private static bool HasProperty(dynamic obj, string name)
    {
        if (obj is null)
            return false;

        if (obj is ExpandoObject)
            return ((IDictionary<string, object>)obj).ContainsKey(name);

        return obj.GetType().GetProperty(name) != null;
    }

    private static void SetOnEvent(dynamic on, Workflow workflow)
    {
        if (((Type)on.GetType()).GetInterface("System.Collections.IDictionary") is not null)
        {
            var events = new Dictionary<string, object>();
            foreach (var kv in (Dictionary<object, object>)on)
            {
                var eventName = (string)kv.Key;
                if (IsSupportedTrigger(eventName))
                {
                    events.Add(eventName, GetTriggerEventInfo(eventName, kv.Value));
                }
            }
            workflow.OnEvent = events;
        }
        else if (((Type)on.GetType()).GetInterface("System.Collections.IList") is not null)
        {
            workflow.OnStringArray = ((IList<object>)on).Select(x => (string)x).Where(x => IsSupportedTrigger(x)).ToArray();
        }
        else if (((Type)on.GetType()).Name == "String" && IsSupportedTrigger(on))
        {
            workflow.OnString = on;
        }
    }

    private static TriggerEvent GetTriggerEventInfo(string eventName, dynamic yaml)
        => eventName switch
        {
            "push" => ParsePushEvent(yaml),
            "pull_request" => ParsePullRequestEvent(yaml),
            _ => throw new NotSupportedException($"{eventName} is not supported.")
        };

    private static PullRequestTriggerEvent ParsePullRequestEvent(dynamic yaml)
    {
        var eventInfo = new PullRequestTriggerEvent();

        if (yaml is null)
        {
            return eventInfo;
        }

        if (yaml.ContainsKey("branches"))
        {
            eventInfo.Branches = ((List<object>)yaml["branches"]).Select((x) => (string)x).ToArray();
        }

        if (yaml.ContainsKey("tags"))
        {
            eventInfo.Tags = ((List<object>)yaml["tags"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("paths"))
        {
            eventInfo.Paths = ((List<object>)yaml["paths"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("branches-ignore"))
        {
            eventInfo.BranchesIgnore = ((List<object>)yaml["branches-ignore"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("tags-ignore"))
        {
            eventInfo.TagsIgnore = ((List<object>)yaml["tags-ignore"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("paths-ignore"))
        {
            eventInfo.PathsIgnore = ((List<object>)yaml["paths-ignore"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("types"))
        {
            var property = yaml["types"];
            if (((Type)property.GetType()).GetInterface("System.Collections.Generics.IList") is not null)
            {
                eventInfo.Types = ((List<object>)yaml["types"]).Select(x => (string)x).ToArray();
            }
            else if (((Type)property.GetType()).Name == "String")
            {
                eventInfo.Types = new string[] { property };
            }
        }

        return eventInfo;
    }

    private static PushTriggerEvent ParsePushEvent(dynamic yaml)
    {
        var eventInfo = new PushTriggerEvent();
        if (yaml is null)
        {
            return eventInfo;
        }

        if (yaml.ContainsKey("branches"))
        {
            eventInfo.Branches = ((List<object>)yaml["branches"]).Select((x) => (string)x).ToArray();
        }

        if (yaml.ContainsKey("tags"))
        {
            eventInfo.Tags = ((List<object>)yaml["tags"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("paths"))
        {
            eventInfo.Paths = ((List<object>)yaml["paths"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("branches-ignore"))
        {
            eventInfo.BranchesIgnore = ((List<object>)yaml["branches-ignore"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("tags-ignore"))
        {
            eventInfo.TagsIgnore = ((List<object>)yaml["tags-ignore"]).Select(x => (string)x).ToArray();
        }

        if (yaml.ContainsKey("paths-ignore"))
        {
            eventInfo.PathsIgnore = ((List<object>)yaml["paths-ignore"]).Select(x => (string)x).ToArray();
        }

        return eventInfo;
    }

    private static bool IsSupportedTrigger(string eventName) =>
        eventName switch
        {
            "push" => true,
            "pull_request" => true,
            _ => false
        };
}
