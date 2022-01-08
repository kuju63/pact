namespace Pact.Cli.Models.Workflows;

public class Step
{
    /// <summary>
    /// Get or set unique identifier for the step
    /// </summary>
    /// <value>Unique identifier</value>
    public string? Id { get; set; }

    /// <summary>
    /// Get or set running condition
    /// </summary>
    /// <value>Condition</value>
    public string? If { get; set; }

    /// <summary>
    /// Get or set name for displaying step
    /// </summary>
    /// <value>Display name</value>
    public string Name { get; set; }

    /// <summary>
    /// Get or set action.
    /// </summary>
    /// <value>Using action</value>
    public string? Uses { get; set; }

    /// <summary>
    /// Get or set command line programs
    /// </summary>
    /// <value>Command-line programs</value>
    public string? Run { get; set; }

    /// <summary>
    /// Get or set working directory.
    /// <para>
    /// This setting is enabled only to use command ¥-line programs
    /// </para>
    /// </summary>
    /// <value>Working directory</value>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Get or set default shell setting
    /// </summary>
    /// <value>Default shell</value>
    public Shell? Shell { get; set; }

    /// <summary>
    /// Get or set input parameters defined by the action
    /// </summary>
    /// <value>Input parameters</value>
    public Dictionary<string, string>? With { get; set; }

    /// <summary>
    /// Get or set environment variables
    /// </summary>
    /// <value>Environment variables</value>
    public Dictionary<string, string>? Env { get; set; }

    /// <summary>
    /// Get or set maximum number of minutes to run the step before killing the process
    /// </summary>
    /// <value>Timeout number of minutes</value>
    public int? TimeoutMinuts { get; set; }
}
