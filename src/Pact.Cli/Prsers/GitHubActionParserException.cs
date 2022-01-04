using System.Runtime.Serialization;

namespace Pact.Cli.Parsers;

/// <summary>
/// Exception class of parser error for GitHub Actions workflow
/// </summary>
[Serializable]
public class GitHubActionParserException : Exception
{
    /// <summary>
    /// Get or set property name of occurring error
    /// </summary>
    /// <value>Property name</value>
    public string? PropertyName { get; init; }

    /// <summary>
    /// Initialize new instance of <see cref="GitHubActionsParserException"/> class.
    /// </summary>
    public GitHubActionParserException()
    {
    }

    /// <summary>
    /// Initialize new instance of <see cref="GitHubActionsParserException"/> class.
    /// </summary>
    /// <param name="propertyName">Parse failure property</param>
    public GitHubActionParserException(string propertyName) : base($"Failed to parse workflow by {propertyName}.")
    {
        PropertyName = propertyName;
    }

    /// <summary>
    /// Initialize new instance of <see cref="GitHubActionParserException"/> class.
    /// </summary>
    /// <param name="propertyName">Error property name</param>
    /// <param name="message">Error message</param>
    /// <param name="inner">Inner exception</param>
    public GitHubActionParserException(string propertyName, string message, Exception inner) : base(message, inner)
    {
        PropertyName = propertyName;
    }

    /// <summary>
    /// Initialize new instance of <see cref="GitHubActionParserException"/> class.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    /// <param name="propertyName">Error property name</param>
    protected GitHubActionParserException(
        SerializationInfo info,
        StreamingContext context,
        string propertyName) : base(info, context)
    {
        PropertyName = propertyName;
    }
}
