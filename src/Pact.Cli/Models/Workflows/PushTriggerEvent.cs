namespace Pact.Cli.Models.Workflows;

/// <summary>
/// Class of trigger event for push event
/// </summary>
public class PushTriggerEvent : TriggerEvent
{
    /// <summary>
    /// Get or set triggered branches
    /// </summary>
    /// <value>Branch name</value>
    public string[]? Branches { get; set; }

    /// <summary>
    /// Get or set triggered tags
    /// </summary>
    /// <value>Tag name</value>
    public string[]? Tags { get; set; }

    /// <summary>
    /// Get or set trigger path
    /// </summary>
    /// <value>Path of glob pattern</value>
    public string[]? Paths { get; set; }

    /// <summary>
    /// Get or set ignored branch
    /// </summary>
    /// <value>Ignored branch name</value>
    public string[]? BranchesIgnore { get; set; }

    /// <summary>
    /// Get or set ignored tag
    /// </summary>
    /// <value>Ignored tag</value>
    public string[]? TagsIgnore { get; set; }

    /// <summary>
    /// Get or set ignored path
    /// </summary>
    /// <value>Ignored path of glob pattern</value>
    public string[]? PathsIgnore { get; set; }
}
