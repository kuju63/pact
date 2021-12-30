namespace Pact.Cli.Models.Workflows;

/// <summary>
/// Class of workflow
/// </summary>
public class Workflow
{
    /// <summary>
    /// Get or set workflow name
    /// </summary>
    /// <value>Workflow name</value>
    public string? Name { get; set; }

    /// <summary>
    /// Get or set "on" property of string
    /// </summary>
    /// <value>Trigger event name</value>
    public string? OnString { get; set; }

    /// <summary>
    /// Get or set "on" property of array
    /// </summary>
    /// <value>Array of trigger event name</value>
    public string[] OnStringArray { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Get or set "on" property of object
    /// </summary>
    /// <value></value>
    public Dictionary<string, object> OnEvent { get; set; } = new Dictionary<string, object>();

    public Dictionary<string, object> Jobs { get; set; } = new Dictionary<string, object>();
}
