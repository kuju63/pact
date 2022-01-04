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
    protected GitHubActionParser()
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
        {
            return false;
        }

        if (obj is ExpandoObject)
        {
            return ((IDictionary<string, object>)obj).ContainsKey(name);
        }

        return obj.GetType().GetProperty(name) != null;
    }

    private static void SetOnEvent(dynamic on, Workflow workflow)
    {
        if (DynamicUtil.Is<Dictionary<object, object>>(on, out Dictionary<object, object> eventMap))
        {
            var events = new Dictionary<string, TriggerEvent>();
            foreach (var kv in eventMap)
            {
                if (kv.Key is string eventName && IsSupportedTrigger(eventName))
                {
                    events.Add(eventName, GetTriggerEventInfo(eventName, kv.Value));
                }
            }

            workflow.OnEvent = events;
        }
        else if (DynamicUtil.Is<List<object>>(on, out List<object> events))
        {
            workflow.OnStringArray = events.Where(x => x is string && IsSupportedTrigger((string)x)).Select(x => (string)x).ToArray();
        }
        else if (DynamicUtil.Is<string>(on, out string eventName) && IsSupportedTrigger(eventName))
        {
            workflow.OnString = eventName;
        }
    }

    private static TriggerEvent GetTriggerEventInfo(string eventName, dynamic yaml)
        => eventName switch
        {
            "push" => ParsePushEvent(yaml),
            "pull_request" => ParsePullRequestEvent(yaml),
            _ => throw new NotSupportedException($"{eventName} is not supported.")
        };

    private static bool IsSupportedTrigger(string eventName) =>
        eventName switch
        {
            "push" => true,
            "pull_request" => true,
            _ => false
        };
}
