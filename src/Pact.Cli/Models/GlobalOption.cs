namespace Pact.Cli.Models
{

    /// <summary>
    ///
    /// </summary>
    public record class GlobalOption
    {
        /// <summary>
        /// Initialize new instance of <see cref="GlobalOption"/> class.
        /// </summary>
        /// <param name="secrets">Option of secres</param>
        /// <param name="secretFile">Option of secret file</param>
        /// <param name="dryRun"></param>
        /// <param name="job"></param>
        /// <param name="list"></param>
        public GlobalOption(string[] secrets, FileInfo? secretFile, bool dryRun, string job, bool list)
        {
            Secrets = secrets;
            SecretFile = secretFile;
            DryRun = dryRun;
            Job = job;
            List = list;
        }

        /// <summary>
        /// Get and set GitHub secrets
        /// </summary>
        /// <value>GitHub secrets</value>
        public string[] Secrets { get; init; }

        /// <summary>
        /// Get and set secret file
        /// </summary>
        /// <value>Secret file</value>
        public FileInfo? SecretFile { get; init; }

        /// <summary>
        /// Get and set flag of dry run
        /// </summary>
        /// <value>Flag of dry run</value>
        public bool DryRun { get; init; }

        /// <summary>
        /// Get and set execution job name
        /// </summary>
        /// <value>Job name</value>
        public string Job { get; init; }

        /// <summary>
        /// Get and set flag of list workflows
        /// </summary>
        /// <value>Flag of listing workflows</value>
        public bool List { get; init; }
    }
}
