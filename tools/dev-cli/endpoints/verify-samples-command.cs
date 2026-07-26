#region Purpose
// Verifies that any code samples in the repository compile
#endregion
#region Design
// Builds tests/sample-app in Release via Amuru DotNet.Build
// Streams MSBuild output via RunAsync; fails the process on non-zero exit
// Discovers the repository root dynamically using Git.FindRoot()
#endregion

namespace DevCli.Commands;

[NuruRoute("verify-samples", Description = "Verify code samples compile")]
internal sealed class VerifySamplesCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<VerifySamplesCommand, Unit>
  {
    private readonly ITerminal Terminal;
    private CancellationToken Ct;
    private string RepoRoot = null!;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(VerifySamplesCommand command, CancellationToken ct)
    {
      Ct = ct;

      if (!FindRepoRoot()) return Value;
      if (!await BuildSampleAppAsync()) return Value;

      Terminal.WriteLine("\nSamples verified successfully!".Green());
      return Value;
    }

    private bool FindRepoRoot()
    {
      string? root = Git.FindRoot();
      if (root is null)
      {
        Terminal.WriteErrorLine("Error: could not find repository root.");
        Environment.ExitCode = 1;
        return false;
      }

      RepoRoot = root;
      Terminal.WriteLine("Verifying samples...");
      return true;
    }

    private async Task<bool> BuildSampleAppAsync()
    {
      string projectPath = Path.Combine(RepoRoot, "tests", "sample-app", "sample-app.csproj");
      Terminal.WriteLine($"\nBuilding {projectPath} (Release)...");

      CommandResult command = DotNet.Build(projectPath)
        .WithConfiguration("Release")
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .Build();

      int exitCode = await command.RunAsync(Ct);
      if (exitCode != 0)
      {
        Terminal.WriteErrorLine("Sample verification failed!".Red());
        Environment.ExitCode = exitCode;
        return false;
      }

      return true;
    }
  }
}
