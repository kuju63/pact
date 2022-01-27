using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Pact.Cli;

/// <summary>
/// Generator for GitHub Actions Runner
/// </summary>
public class ContainerRunnerGenerator
{
    private static readonly string _imageName = "ghcr.io/catthehacker/ubuntu:act-20.04";

    /// <summary>
    /// Start GitHub Actions runner
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asynchronous result</returns>
    public async Task StartRunnerAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInstalledPodman())
        {
            throw new InvalidOperationException("Podman is not installed. Required podman.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

#if OSX
        var isStart = await StartVMAsync();
        if (isStart)
        {
            await CheckImageAsync(cancellationToken);
            await PullRunnerImageAsync(cancellationToken);
            await RunRunnerAsync(cancellationToken);
        }
#else
        await CheckImageAsync(cancellationToken);
        await PullRunnerImageAsync(cancellationToken);
        await RunRunnerAsync(cancellationToken);
#endif
    }

    private bool ValidateProcessResult(string input, string succeedPattern)
    {
        var reg = new Regex(succeedPattern);
        return reg.IsMatch(input);
    }

    private async Task<bool> CheckImageAsync(CancellationToken cancellationToken = default)
    {
        using (var podman = new Process())
        {
            podman.OutputDataReceived += (o, data) => { Console.WriteLine(data.Data); };
            podman.StartInfo = new ProcessStartInfo("podman", $"image exists {_imageName}");

            if (podman.Start())
            {
                await podman.WaitForExitAsync(cancellationToken);
                if (podman.ExitCode == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async Task<bool> PullRunnerImageAsync(CancellationToken cancellationToken = default)
    {
        using (var podman = new Process())
        {
            podman.OutputDataReceived += (o, data) => { Console.WriteLine(data.Data); };
            podman.StartInfo = new ProcessStartInfo("podman", $"pull {_imageName}");

            if (podman.Start())
            {
                await podman.WaitForExitAsync(cancellationToken);
                if (podman.ExitCode == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private async Task RunRunnerAsync(CancellationToken cancellationToken = default)
    {
        using (var podman = new Process())
        {
            podman.OutputDataReceived += (o, data) => { Console.WriteLine(data.Data); };
            podman.StartInfo = new ProcessStartInfo("podman");
            podman.StartInfo.ArgumentList.Add("run");
            podman.StartInfo.ArgumentList.Add("-d");
            podman.StartInfo.ArgumentList.Add("--name");
            podman.StartInfo.ArgumentList.Add("runner");
            podman.StartInfo.ArgumentList.Add(_imageName);

            if (podman.Start())
            {
                await podman.WaitForExitAsync(cancellationToken);
            }
        }
    }

#if OSX
    private async Task<bool> StartVMAsync(CancellationToken cancellationToken = default)
    {
        var isVMStarted = false;
        using (var podman = new Process())
        {
            podman.StartInfo = new ProcessStartInfo("podman", "system connection list");
            if (podman.Start())
            {
                podman.OutputDataReceived += (o, data) => { Console.WriteLine(data.Data); };
                await podman.WaitForExitAsync(cancellationToken);
                string? output;
                while ((output = (await podman.StandardOutput.ReadLineAsync())) is not null)
                {
                    if (ValidateProcessResult(output!, "(podman-machine-default\\*)+"))
                    {
                        isVMStarted = true;
                    }
                }
            }
        }

        var isInit = false;
        if (!isVMStarted)
        {
            using (var podman = new Process())
            {
                podman.OutputDataReceived += (o, data) => { Console.WriteLine(data.Data); };
                podman.StartInfo = new ProcessStartInfo("podman", "machine init");
                if (podman.Start())
                {
                    await podman.WaitForExitAsync(cancellationToken);
                    if (podman.ExitCode == 0)
                    {
                        isInit = true;
                    }
                    else
                    {
                        var error = await podman.StandardError.ReadToEndAsync();
                        if (error.Contains("VM already exists"))
                        {
                            isInit = true;
                        }
                    }
                }
            }
        }
        else
        {
            isInit = true;
        }

        var isStart = false;
        if (isInit)
        {
            using (var podman = new Process())
            {
                podman.OutputDataReceived += (o, data) => { Console.WriteLine(data.Data); };
                podman.StartInfo = new ProcessStartInfo("podman", "machine start");
                if (podman.Start())
                {
                    await podman.WaitForExitAsync(cancellationToken);
                    if (podman.ExitCode == 0)
                    {
                        isStart = true;
                    }
                    else
                    {
                        var log = await podman.StandardError.ReadToEndAsync();
                        isStart = log.Contains("VM already running");
                    }
                }
            }
        }


        return isStart;
    }
#endif

    private bool IsInstalledPodman()
        => File.Exists("podman");
}
