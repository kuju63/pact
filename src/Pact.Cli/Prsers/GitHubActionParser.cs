using System.Dynamic;
using Pact.Cli.Models.Workflows;
using Pact.Cli.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Pact.Cli.Parsers;

/// <summary>
/// Parser class of GitHub Actions workflow file.
/// </summary>
public partial class GitHubActionParser
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
            if (DynamicUtil.Is<List<object>>(yaml["branches"], out List<object> list))
            {
                eventInfo.Branches = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("tags"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["tags"], out List<object> list))
            {
                eventInfo.Tags = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("paths"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["paths"], out List<object> list))
            {
                eventInfo.Paths = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("branches-ignore"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["branches-ignore"], out List<object> list))
            {
                eventInfo.BranchesIgnore = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("tags-ignore"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["tags-ignore"], out List<object> list))
            {
                eventInfo.TagsIgnore = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("paths-ignore"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["paths-ignore"], out List<object> list))
            {
                eventInfo.PathsIgnore = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("types"))
        {
            var property = yaml["types"];
            if (DynamicUtil.Is<List<string>>(property, out List<string> listType))
            {
                eventInfo.Types = listType.ToArray();
            }
            else if (DynamicUtil.Is<string>(property, out string type))
            {
                eventInfo.Types = new string[] { type };
            }
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
