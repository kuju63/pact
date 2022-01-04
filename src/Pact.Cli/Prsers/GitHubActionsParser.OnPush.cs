using Pact.Cli.Models.Workflows;
using Pact.Cli.Utils;

namespace Pact.Cli.Parsers;

/// <summary>
/// Partial class of <see cref="GitHubActionParser"/>.
/// <para>This is parsed "on" property of Push event</para>
/// </summary>
public partial class GitHubActionParser
{
    /// <summary>
    /// Parse push event
    /// </summary>
    /// <param name="yaml">push event object</param>
    /// <returns>Push event settings</returns>
    private static PushTriggerEvent ParsePushEvent(dynamic yaml)
    {
        var eventInfo = new PushTriggerEvent();
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

        return eventInfo;
    }
}
