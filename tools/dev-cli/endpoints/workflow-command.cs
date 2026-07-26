#region Purpose
// Dev CLI command for timewarp-simple-icons CI/CD pipeline
#endregion
#region Design
// PR/merge: clean -> build -> test
// Release: clean -> build -> pack -> push (no test gate)
// Auto-detects mode from GITHUB_EVENT_NAME or --mode
#endregion

namespace DevCli.Commands;

[NuruRoute("workflow", Description = "Run full CI/CD pipeline")]
internal sealed class WorkflowCommand : ICommand<Unit>
{
  [Option("mode", "m", Description = "CI mode: pr, merge, or release (auto-detected from GITHUB_EVENT_NAME if not specified)")]
  public string? Mode { get; set; }

  [Option("api-key", Description = "NuGet API key for publishing (from OIDC Trusted Publishing)")]
  public string? ApiKey { get; set; }

  internal sealed class Handler : ICommandHandler<WorkflowCommand, Unit>
  {
    private readonly ITerminal Terminal;
    private string RepoRoot = null!;
    private string SolutionPath = null!;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(WorkflowCommand command, CancellationToken ct)
    {
      string? root = Git.FindRoot();
      if (root is null)
      {
        Terminal.WriteErrorLine("Error: could not find repository root.");
        Environment.ExitCode = 1;
        return Value;
      }

      RepoRoot = root;
      SolutionPath = Path.Combine(RepoRoot, "timewarp-simple-icons.slnx");
      CiMode mode = DetermineMode(command.Mode);

      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine($"  CI/CD Pipeline - Mode: {mode}");
      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine("");

      if (mode == CiMode.Release)
      {
        await RunReleaseWorkflowAsync(command.ApiKey, ct);
      }
      else
      {
        await RunPrWorkflowAsync(ct);
      }

      return Value;
    }

    private CiMode DetermineMode(string? explicitMode)
    {
      if (!string.IsNullOrEmpty(explicitMode))
      {
        return explicitMode.ToLowerInvariant() switch
        {
          "pr" => CiMode.Pr,
          "merge" => CiMode.Merge,
          "release" => CiMode.Release,
          _ => CiMode.Pr
        };
      }

      string? eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");
      CiMode mode = eventName switch
      {
        "pull_request" => CiMode.Pr,
        "push" => CiMode.Merge,
        "release" => CiMode.Release,
        "workflow_dispatch" => CiMode.Release,
        _ => CiMode.Pr
      };

      Terminal.WriteLine($"Detected GITHUB_EVENT_NAME: {eventName ?? "(not set)"} -> Mode: {mode}");
      return mode;
    }

    private async Task RunPrWorkflowAsync(CancellationToken ct)
    {
      Terminal.WriteLine("Pipeline: clean -> build -> test");
      Terminal.WriteLine("");

      if (!await CleanAsync(ct)) return;
      if (!await BuildAsync(ct)) return;
      if (!await TestAsync(ct)) return;

      Terminal.WriteLine("");
      Terminal.WriteLine("Pipeline SUCCEEDED".Green());
    }

    private async Task RunReleaseWorkflowAsync(string? apiKey, CancellationToken ct)
    {
      Terminal.WriteLine("Pipeline: clean -> build -> pack -> push");
      Terminal.WriteLine("");

      if (!await CleanAsync(ct)) return;
      if (!await BuildAsync(ct)) return;
      if (!await PackAsync(ct)) return;
      if (!await PushAsync(apiKey, ct)) return;

      Terminal.WriteLine("");
      Terminal.WriteLine("Pipeline SUCCEEDED - package published to NuGet.org".Green());
    }

    private async Task<bool> CleanAsync(CancellationToken ct)
    {
      Terminal.WriteLine("Step: Clean");
      int exitCode = await DotNet.Clean(SolutionPath)
        .WithVerbosity("q")
        .WithWorkingDirectory(RepoRoot)
        .RunAsync(ct);
      return ReportFailure(exitCode, "Clean");
    }

    private async Task<bool> BuildAsync(CancellationToken ct)
    {
      Terminal.WriteLine("Step: Build");
      int exitCode = await DotNet.Build(SolutionPath)
        .WithConfiguration("Release")
        .WithWorkingDirectory(RepoRoot)
        .RunAsync(ct);
      return ReportFailure(exitCode, "Build");
    }

    private async Task<bool> TestAsync(CancellationToken ct)
    {
      Terminal.WriteLine("Step: Test");
      int exitCode = await DotNet.Test()
        .WithProject(SolutionPath)
        .WithConfiguration("Release")
        .WithWorkingDirectory(RepoRoot)
        .RunAsync(ct);
      return ReportFailure(exitCode, "Test");
    }

    private async Task<bool> PackAsync(CancellationToken ct)
    {
      Terminal.WriteLine("Step: Pack");
      string artifactsDir = Path.Combine(RepoRoot, "artifacts", "packages");
      Directory.CreateDirectory(artifactsDir);

      int exitCode = await DotNet.Pack(Path.Combine(RepoRoot, "source", "timewarp-simple-icons", "timewarp-simple-icons.csproj"))
        .WithConfiguration("Release")
        .WithOutput(artifactsDir)
        .WithWorkingDirectory(RepoRoot)
        .RunAsync(ct);
      return ReportFailure(exitCode, "Pack");
    }

    private async Task<bool> PushAsync(string? apiKey, CancellationToken ct)
    {
      string version = ReadPackageVersion();
      string packagePath = Path.Combine(RepoRoot, "artifacts", "packages", $"timewarp-simple-icons.{version}.nupkg");

      if (!File.Exists(packagePath))
      {
        Terminal.WriteErrorLine($"Package not found: {packagePath}".Red());
        Environment.ExitCode = 1;
        return false;
      }

      if (string.IsNullOrEmpty(apiKey))
      {
        Terminal.WriteLine("Skipping NuGet push (no API key provided).");
        return true;
      }

      Terminal.WriteLine($"Step: Push timewarp-simple-icons.{version}.nupkg");
      int exitCode = await DotNet.NuGet()
        .Push(packagePath)
        .WithSource("https://api.nuget.org/v3/index.json")
        .WithApiKey(apiKey)
        .WithSkipDuplicate()
        .RunAsync(ct);
      return ReportFailure(exitCode, "Push");
    }

    private bool ReportFailure(int exitCode, string stepName)
    {
      if (exitCode == 0) return true;

      Terminal.WriteErrorLine($"Pipeline FAILED - {stepName} failed".Red());
      Environment.ExitCode = exitCode;
      return false;
    }

    private string ReadPackageVersion()
    {
      string propsPath = Path.Combine(RepoRoot, "source", "Directory.Build.props");
      XDocument doc = XDocument.Load(propsPath);
      return doc.Descendants("Version").FirstOrDefault()?.Value
        ?? throw new InvalidOperationException("Could not determine package version.");
    }
  }
}

internal enum CiMode
{
  Pr,
  Merge,
  Release
}
