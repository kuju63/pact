namespace Pact.Cli.Models.Workflows;

public class Job
{
    /// <summary>
    /// Get or set Job name.
    /// </summary>
    /// <value>Job name</value>
    public string? Name { get; set; }

    /// <summary>
    /// Get or set need jobs that complete successfully before this hob will run.
    /// </summary>
    /// <value>Depends job</value>
    public string[]? Needs { get; set; }

    /// <summary>
    /// Get or set permissions of GITHUB_TOKEN.
    /// </summary>
    /// <value>Token permissions</value>
    public string[]? Permissions { get; set; }

    /// <summary>
    /// Get or set runs-on property
    /// </summary>
    /// <value></value>
    public string? RunsOn { get; set; }

    /// <summary>
    /// Get or set runs-on property by array
    /// </summary>
    /// <value></value>
    public string[]? RunsOnArray { get; set; }

    /// <summary>
    /// Get or set string of environment
    /// </summary>
    /// <value></value>
    public string? Environment { get; set; }

    /// <summary>
    /// Get or set object of environment
    /// </summary>
    /// <value></value>
    public object? EnvironmentObject { get; set; }

    /// <summary>
    /// Get or set environment variable per jobs.
    /// </summary>
    /// <value></value>
    public Dictionary<string, string>? Env { get; set; }

    /// <summary>
    /// Get or set setting of shell
    /// </summary>
    /// <value></value>
    public object? Defaults { get; set; }

    /// <summary>
    /// Get or set condition that execute job
    /// </summary>
    /// <value></value>
    public string? If { get; set; }

    /// <summary>
    /// Get or set tasks
    /// </summary>
    /// <value></value>
    public Step[]? Steps { get; set; }

    /// <summary>
    /// Get or set workflow timeout
    /// </summary>
    /// <value></value>
    public int TimeoutMinutes { get; set; } = 360;

    /// <summary>
    /// Get or set build matrix
    /// </summary>
    /// <value></value>
    public object? Strategy { get; set; }

    /// <summary>
    /// Get or set value of "continue-on-error"
    /// </summary>
    /// <value></value>
    public bool ContinueOnError { get; set; } = false;

    /// <summary>
    /// Get or set container
    /// </summary>
    /// <value></value>
    public string? Container { get; set; }

    /// <summary>
    /// Get or set setting object of container
    /// </summary>
    /// <value></value>
    public object? ContainerObject { get; set; }

    /// <summary>
    /// Get or set additional containers
    /// </summary>
    /// <value></value>
    public object? Services { get; set; }

    /// <summary>
    /// Get or set setting of concurrency ensures that only a single job
    /// or workflow using the same concurrency group will run at a time.
    /// </summary>
    /// <value></value>
    public string? Concurrency { get; set; }

    /// <summary>
    /// Get or set setting of concurrency ensures that only a single job
    /// or workflow using the same concurrency group will run at a time.
    /// </summary>
    /// <value></value>
    public object? ConcurrencyObject { get; set; }
}
