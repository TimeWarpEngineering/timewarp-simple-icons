#region Purpose
// Syncs Blazor icon components from upstream simple-icons/simple-icons releases
#endregion
#region Design
// Compares the package version in source/Directory.Build.props with the latest GitHub
// release, regenerates components when stale, optionally commits/pushes and publishes to NuGet
// Package version tracks upstream simple-icons tag (e.g. 16.27.1)
#endregion

namespace DevCli.Commands;

[NuruRoute("update-icons", Description = "Sync icon components from upstream simple-icons releases")]
internal sealed class UpdateIconsCommand : ICommand<Unit>
{
  [Option("push", Description = "Commit and push when icons were updated")]
  public bool Push { get; set; }

  [Option("publish", Description = "Pack and push to NuGet after updating (requires --api-key)")]
  public bool Publish { get; set; }

  [Option("api-key", Description = "NuGet API key from OIDC Trusted Publishing")]
  public string? ApiKey { get; set; }

  internal sealed class Handler : ICommandHandler<UpdateIconsCommand, Unit>
  {
    private const string SimpleIconsRepo = "https://github.com/simple-icons/simple-icons.git";
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(UpdateIconsCommand command, CancellationToken ct)
    {
      string? repoRoot = Git.FindRoot();
      if (repoRoot is null)
      {
        Terminal.WriteErrorLine("Error: could not find repository root.");
        Environment.ExitCode = 1;
        return Value;
      }

      string propsPath = Path.Combine(repoRoot, "source", "Directory.Build.props");
      string currentVersion = ReadVersion(propsPath);
      string latestUpstream = await FetchLatestSimpleIconsVersionAsync(ct);

      Terminal.WriteLine($"Current package version:   {currentVersion}");
      Terminal.WriteLine($"Latest simple-icons release: {latestUpstream}");

      if (string.Equals(currentVersion, latestUpstream, StringComparison.Ordinal))
      {
        Terminal.WriteLine("Icons are already up to date.".Green());
        return Value;
      }

      string newVersion = latestUpstream;
      string cloneDir = Path.Combine(Path.GetTempPath(), $"simple-icons-{latestUpstream}-{Guid.NewGuid():N}");

      try
      {
        if (!await CloneSimpleIconsAsync(latestUpstream, cloneDir, ct)) return Value;
        if (!await TransformIconsAsync(repoRoot, cloneDir, ct)) return Value;

        UpdateVersion(propsPath, newVersion);
        UpdateReleases(repoRoot, newVersion);

        if (!await BuildAsync(repoRoot, ct)) return Value;

        if (command.Push)
        {
          if (!await CommitAndPushAsync(repoRoot, latestUpstream, ct)) return Value;
        }

        if (command.Publish)
        {
          if (!await PackAndPushAsync(repoRoot, newVersion, command.ApiKey, ct)) return Value;
        }

        Terminal.WriteLine($"\nUpdated icons to simple-icons {latestUpstream} (package {newVersion}).".Green());
      }
      finally
      {
        if (Directory.Exists(cloneDir))
        {
          Directory.Delete(cloneDir, recursive: true);
        }
      }

      return Value;
    }

    private static string ReadVersion(string propsPath)
    {
      XDocument doc = XDocument.Load(propsPath);
      string? version = doc.Descendants("Version").FirstOrDefault()?.Value;
      if (string.IsNullOrWhiteSpace(version))
      {
        throw new InvalidOperationException($"Could not read <Version> from {propsPath}");
      }

      return version;
    }

    private static async Task<string> FetchLatestSimpleIconsVersionAsync(CancellationToken ct)
    {
      using HttpClient client = new();
      client.DefaultRequestHeaders.UserAgent.ParseAdd("timewarp-simple-icons-dev-cli");
      using HttpResponseMessage response = await client.GetAsync(
        "https://api.github.com/repos/simple-icons/simple-icons/releases/latest",
        ct);
      response.EnsureSuccessStatusCode();

      await using Stream stream = await response.Content.ReadAsStreamAsync(ct);
      using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
      string tag = document.RootElement.GetProperty("tag_name").GetString()
        ?? throw new InvalidOperationException("simple-icons latest release is missing tag_name");

      return tag.StartsWith('v') ? tag[1..] : tag;
    }

    private async Task<bool> CloneSimpleIconsAsync(string version, string cloneDir, CancellationToken ct)
    {
      Terminal.WriteLine($"\nCloning simple-icons {version}...");
      // simple-icons tags are bare version numbers (e.g. 16.27.1), not v-prefixed
      int exitCode = await Shell.Builder("git")
        .WithArguments("clone", "--depth", "1", "--branch", version, SimpleIconsRepo, cloneDir)
        .WithNoValidation()
        .RunAsync(ct);

      if (exitCode != 0)
      {
        Terminal.WriteErrorLine("Failed to clone simple-icons repository.".Red());
        Environment.ExitCode = exitCode;
        return false;
      }

      return true;
    }

    private async Task<bool> TransformIconsAsync(string repoRoot, string cloneDir, CancellationToken ct)
    {
      string inputPath = Path.Combine(cloneDir, "icons");
      string outputPath = Path.Combine(repoRoot, "source", "timewarp-simple-icons", "icons");
      string transformProject = Path.Combine(repoRoot, "tools", "transform", "transform.csproj");

      if (!Directory.Exists(inputPath))
      {
        Terminal.WriteErrorLine($"simple-icons icons directory not found: {inputPath}".Red());
        Environment.ExitCode = 1;
        return false;
      }

      Terminal.WriteLine("\nRegenerating Blazor icon components...");
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("run", "--project", transformProject, "--", inputPath, outputPath)
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .RunAsync(ct);

      if (exitCode != 0)
      {
        Terminal.WriteErrorLine("Icon transform failed.".Red());
        Environment.ExitCode = exitCode;
        return false;
      }

      return true;
    }

    private static void UpdateVersion(string propsPath, string newVersion)
    {
      XDocument doc = XDocument.Load(propsPath);
      XElement? versionElement = doc.Descendants("Version").FirstOrDefault()
        ?? throw new InvalidOperationException($"Could not find <Version> in {propsPath}");
      versionElement.Value = newVersion;
      doc.Save(propsPath);
    }

    private static void UpdateReleases(string repoRoot, string newVersion)
    {
      string releasesPath = Path.Combine(repoRoot, "releases.md");
      string newSection = $"## {newVersion}\n\n* Update to simple-icons version {newVersion}\n\n";

      if (!File.Exists(releasesPath))
      {
        File.WriteAllText(releasesPath, $"# Releases\n\n{newSection}");
        return;
      }

      string existing = File.ReadAllText(releasesPath);
      const string heading = "# Releases\n\n";
      if (existing.StartsWith(heading, StringComparison.Ordinal))
      {
        File.WriteAllText(releasesPath, $"{heading}{newSection}{existing[heading.Length..]}");
        return;
      }

      File.WriteAllText(releasesPath, $"{heading}{newSection}{existing}");
    }

    private async Task<bool> BuildAsync(string repoRoot, CancellationToken ct)
    {
      Terminal.WriteLine("\nVerifying build after icon update...");
      int exitCode = await DotNet.Build(Path.Combine(repoRoot, "timewarp-simple-icons.slnx"))
        .WithConfiguration("Release")
        .WithWorkingDirectory(repoRoot)
        .RunAsync(ct);

      if (exitCode != 0)
      {
        Terminal.WriteErrorLine("Build failed after icon update.".Red());
        Environment.ExitCode = exitCode;
        return false;
      }

      return true;
    }

    private async Task<bool> CommitAndPushAsync(string repoRoot, string upstreamVersion, CancellationToken ct)
    {
      Terminal.WriteLine("\nCommitting and pushing changes...");

      await ConfigureGitBotIdentityAsync(repoRoot, ct);

      int exitCode = await Shell.Builder("git")
        .WithArguments("add", "source/", "releases.md", "source/Directory.Build.props")
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .RunAsync(ct);
      if (exitCode != 0) return Fail(exitCode);

      CommandOutput status = await Shell.Builder("git")
        .WithArguments("status", "--porcelain")
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .CaptureAsync(ct);
      if (string.IsNullOrWhiteSpace(status.Stdout))
      {
        Terminal.WriteLine("No changes to commit.");
        return true;
      }

      exitCode = await Shell.Builder("git")
        .WithArguments("commit", "-m", $"chore: update simple-icons to {upstreamVersion}")
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .RunAsync(ct);
      if (exitCode != 0) return Fail(exitCode);

      exitCode = await Shell.Builder("git")
        .WithArguments("push", "origin", "HEAD")
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .RunAsync(ct);
      if (exitCode != 0) return Fail(exitCode);

      return true;
    }

    private static async Task ConfigureGitBotIdentityAsync(string repoRoot, CancellationToken ct)
    {
      await Shell.Builder("git").WithArguments("config", "user.name", "github-actions[bot]")
        .WithWorkingDirectory(repoRoot).WithNoValidation().RunAsync(ct);
      await Shell.Builder("git").WithArguments("config", "user.email", "41898282+github-actions[bot]@users.noreply.github.com")
        .WithWorkingDirectory(repoRoot).WithNoValidation().RunAsync(ct);
    }

    private async Task<bool> PackAndPushAsync(string repoRoot, string version, string? apiKey, CancellationToken ct)
    {
      string artifactsDir = Path.Combine(repoRoot, "artifacts", "packages");
      Directory.CreateDirectory(artifactsDir);
      string projectPath = Path.Combine(repoRoot, "source", "timewarp-simple-icons", "timewarp-simple-icons.csproj");

      Terminal.WriteLine("\nPacking NuGet package...");
      int exitCode = await DotNet.Pack(projectPath)
        .WithConfiguration("Release")
        .WithOutput(artifactsDir)
        .WithWorkingDirectory(repoRoot)
        .RunAsync(ct);
      if (exitCode != 0) return Fail(exitCode);

      string packagePath = Path.Combine(artifactsDir, $"timewarp-simple-icons.{version}.nupkg");
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

      Terminal.WriteLine("Pushing package to NuGet.org...");
      exitCode = await DotNet.NuGet()
        .Push(packagePath)
        .WithSource("https://api.nuget.org/v3/index.json")
        .WithApiKey(apiKey)
        .WithSkipDuplicate()
        .RunAsync(ct);
      if (exitCode != 0) return Fail(exitCode);

      Terminal.WriteLine("Package pushed to NuGet.org.".Green());
      return true;
    }

    private bool Fail(int exitCode)
    {
      Environment.ExitCode = exitCode;
      return false;
    }
  }
}
