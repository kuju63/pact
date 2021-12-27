using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Pact.Cli.Models;

namespace Pact.Cli
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string[]>(new[] { "--secrets", "-s" }, ""),
                new Option<FileInfo>("--secret-file", "").ExistingOnly(),
                new Option<bool>(new[] { "--dry-run", "-d" }),
                new Option<string>("--job"),
                new Option<bool>(new[] { "--list", "-l" }, "List workflows")
            };
            rootCommand.Handler = CommandHandler.Create<GlobalOption>((options) => { Console.WriteLine(options.ToString()); });

            return await rootCommand.InvokeAsync(args);
        }
    }
}
