using Pact.Cli.Models.Workflows;
using Pact.Cli.Utils;

namespace Pact.Cli.Parsers;

/// <summary>
/// Partial class of <see cref="GitHubActionParser"/>.
/// <para>This is parsed "on" property of PullRequest event</para>
/// </summary>
public partial class GitHubActionParser
{
    private static PullRequestTriggerEvent ParsePullRequestEvent(dynamic yaml)
    {
        var eventInfo = new PullRequestTriggerEvent();

        if (yaml is null)
        {
            return eventInfo;
        }

        if (yaml.ContainsKey("branches"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["branches"]))
            {
                List<object> list = DynamicUtil.CastTo<List<object>>(yaml["branches"]);
                eventInfo.Branches = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("tags"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["tags"]))
            {
                List<object> list = DynamicUtil.CastTo<List<object>>(yaml["tags"]);
                eventInfo.Tags = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("paths"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["paths"]))
            {
                List<object> list = DynamicUtil.CastTo<List<object>>(yaml["paths"]);
                eventInfo.Paths = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("branches-ignore"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["branches-ignore"]))
            {
                List<object> list = DynamicUtil.CastTo<List<object>>(yaml["branches-ignore"]);
                eventInfo.BranchesIgnore = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("tags-ignore"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["tags-ignore"]))
            {
                List<object> list = DynamicUtil.CastTo<List<object>>(yaml["tags-ignore"]);
                eventInfo.TagsIgnore = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("paths-ignore"))
        {
            if (DynamicUtil.Is<List<object>>(yaml["paths-ignore"]))
            {
                List<object> list = DynamicUtil.CastTo<List<object>>(yaml["paths-ignore"]);
                eventInfo.PathsIgnore = list.Where(x => x is string).Select(x => (string)x).ToArray();
            }
        }

        if (yaml.ContainsKey("types"))
        {
            var property = yaml["types"];
            if (DynamicUtil.Is<List<string>>(property))
            {
                List<string> listType = DynamicUtil.CastTo<List<string>>(property);
                eventInfo.Types = listType.ToArray();
            }
            else if (DynamicUtil.Is<string>(property))
            {
                string? type = DynamicUtil.CastTo<string?>(property);
                eventInfo.Types = new[] { type };
            }
        }

        return eventInfo;
    }
}
