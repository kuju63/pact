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

        if (HasProperty(workflowYaml, "env"))
        {
            if (DynamicUtil.Is<Dictionary<object, object>>(workflowYaml.env))
            {
                Dictionary<object, object>? env = DynamicUtil.CastTo<Dictionary<object, object>>(workflowYaml.env);
                workflowResult.Env = env?.Where(kv => kv.Key is string && kv.Value is string)
                                         .Select(kv => ((string)kv.Key, (string)kv.Value))
                                         .ToDictionary(t => t.Item1, t => t.Item2);
            }
        }

        SetOnEvent(workflowYaml.on, workflowResult);

        SetJobs(workflowYaml.jobs, workflowResult);

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
        if (DynamicUtil.Is<Dictionary<object, object>>(on))
        {
            var eventMap = DynamicUtil.CastTo<Dictionary<object, object>>(on);
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
        else if (DynamicUtil.Is<List<object>>(on))
        {
            List<object> events = DynamicUtil.CastTo<List<object>>(on);
            workflow.OnStringArray = events.Where(x => x is string && IsSupportedTrigger((string)x)).Select(x => (string)x).ToArray();
        }
        else if (DynamicUtil.Is<string>(on))
        {
            var eventName = DynamicUtil.CastTo<string>(on);
            if (IsSupportedTrigger(eventName))
            {
                workflow.OnString = eventName;
            }
        }
    }

    private static void SetJobs(dynamic jobs, Workflow workflow)
    {
        if (DynamicUtil.Is<Dictionary<object, object>>(jobs))
        {
            Dictionary<object, object>? jobsMap = DynamicUtil.CastTo<Dictionary<object, object>>(jobs);
            foreach (var kv in jobsMap)
            {
                if (kv.Key is string jobKey)
                {
                    var job = new Job();
                    if (kv.Value is not null)
                    {
                        Dictionary<object, object>? jobMap = DynamicUtil.CastTo<Dictionary<object, object>>(kv.Value);
                        if (jobMap is not null)
                        {
                            foreach (var kv2 in jobMap)
                            {
                                if (kv2.Key is string jobKeyName)
                                {
                                    // pact is supported these property. But GitHub Actions has more properties.
                                    switch (jobKeyName)
                                    {
                                        case "name":
                                            if (DynamicUtil.Is<string>(kv2.Value))
                                            {
                                                job.Name = DynamicUtil.CastTo<string?>(kv2.Value);
                                            }
                                            break;
                                        case "needs":
                                            if (DynamicUtil.Is<List<object>>(kv2.Value))
                                            {
                                                job.Needs = DynamicUtil.CastTo<List<object>>(kv2.Value)?.Where(x => x is string)
                                                    .Select(x => (string)x)
                                                    .ToArray();
                                            }
                                            break;
                                        case "runs-on":
                                            if (DynamicUtil.Is<string>(kv2.Value))
                                            {
                                                job.RunsOn = DynamicUtil.CastTo<string?>(kv2.Value);
                                            }
                                            else if (DynamicUtil.Is<List<object>>(kv2.Value))
                                            {
                                                var list = DynamicUtil.CastTo<List<object>>(kv2.Value);
                                                job.RunsOnArray = list?.Where(l => l is string).Select(l => (string)l).ToArray();
                                            }
                                            break;
                                        case "env":
                                            if (DynamicUtil.Is<Dictionary<object, object>>(kv2.Value))
                                            {
                                                var env = DynamicUtil.CastTo<Dictionary<object, object>>(kv2.Value);
                                                if (env is not null)
                                                {
                                                    job.Env = env.Where(x => x.Key is string && x.Value is string)
                                                        .Select(x => new KeyValuePair<string, string>((string)x.Key, (string)x.Value))
                                                        .ToDictionary(x => x.Key, x => x.Value);
                                                }
                                            }
                                            break;
                                        case "steps":
                                            // TODO parse steps
                                            if (DynamicUtil.Is<List<object>>(kv2.Value))
                                            {
                                                var steps = DynamicUtil.CastTo<List<object>>(kv2.Value);
                                                job.Steps = steps?.Select(o =>
                                                {
                                                    Step step = new();
                                                    if (DynamicUtil.Is<Dictionary<object, object>>(o))
                                                    {
                                                        var stepDictionary = DynamicUtil.CastTo<Dictionary<object, dynamic>>(o);
                                                        if (stepDictionary is not null)
                                                        {
                                                            foreach (KeyValuePair<object, dynamic> stepInfo in stepDictionary)
                                                            {
                                                                if (stepInfo.Key is string keyName)
                                                                {
                                                                    switch (keyName)
                                                                    {
                                                                        case "name":
                                                                            step.Name = DynamicUtil.CastTo<string>(stepInfo.Value);
                                                                            break;
                                                                        case "id":
                                                                            step.Id = DynamicUtil.CastTo<string>(stepInfo.Value);
                                                                            break;
                                                                        case "working-directory":
                                                                            step.WorkingDirectory = DynamicUtil.CastTo<string>(stepInfo.Value);
                                                                            break;
                                                                        case "shell":
                                                                            {
                                                                                var shell = DynamicUtil.CastTo<string>(stepInfo.Value);
                                                                                step.Shell = shell switch
                                                                                {
                                                                                    "bash" => Shell.Bash,
                                                                                    "pwsh" => Shell.Pwsh,
                                                                                    "python" => Shell.Python,
                                                                                    "sh" => Shell.Sh,
                                                                                    "cmd" => Shell.Cmd,
                                                                                    "powershell" => Shell.Powershell,
                                                                                    _ => null
                                                                                };
                                                                                break;
                                                                            }
                                                                        case "uses":
                                                                            step.Uses = DynamicUtil.CastTo<string>(stepInfo.Value);
                                                                            break;
                                                                        case "run":
                                                                            step.Run = DynamicUtil.CastTo<string>(stepInfo.Value);
                                                                            break;
                                                                        case "with":
                                                                            {
                                                                                Dictionary<object, object>? inputArg = DynamicUtil.CastTo<Dictionary<object, object>>(stepInfo.Value);
                                                                                if (inputArg is not null)
                                                                                {
                                                                                    step.With = inputArg.Where(arg => arg.Key is string && arg.Value is string)
                                                                                                        .Select(arg => new KeyValuePair<string, string>((string)arg.Key, (string)arg.Value))
                                                                                                        .ToDictionary(kp => kp.Key, kp => kp.Value);
                                                                                }
                                                                                break;
                                                                            }
                                                                        case "env":
                                                                            {
                                                                                Dictionary<object, object>? inputArg = DynamicUtil.CastTo<Dictionary<object, object>>(stepInfo.Value);
                                                                                if (inputArg is not null)
                                                                                {
                                                                                    step.Env = inputArg.Where(arg => arg.Key is string && arg.Value is string)
                                                                                                        .Select(arg => new KeyValuePair<string, string>((string)arg.Key, (string)arg.Value))
                                                                                                        .ToDictionary(kp => kp.Key, kp => kp.Value);
                                                                                }
                                                                                break;
                                                                            }
                                                                        default:
                                                                            Console.WriteLine($"Does not support property({keyName})");
                                                                            break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    return step;

                                                }).ToArray();
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    workflow.Jobs.Add(jobKey, job);
                }
            }
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
