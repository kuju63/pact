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
        SetOnEvent(workflowYaml.on, workflowResult);

        return workflowResult;
    }

    private static bool HasProperty(dynamic obj, string name)
    {
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
                events.Add((string)kv.Key, kv.Value);
            }
            workflow.OnEvent = events;
        }
        else if (((Type)on.GetType()).GetInterface("System.Collections.IList") is not null)
        {
            workflow.OnStringArray = ((IList<object>)on).Select(x => (string)x).ToArray();
        }
        else if (((Type)on.GetType()).Name == "String")
        {
            workflow.OnString = on;
        }
    }
}
