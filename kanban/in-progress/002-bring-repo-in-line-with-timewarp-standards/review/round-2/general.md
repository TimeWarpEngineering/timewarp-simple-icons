# Round 2 — general
**Date:** 2026-07-26
**Scope reviewed:** re-verify M1–M5 + fix delta

## Summary
All five prior findings (M1–M5) re-verify as **fixed**. Live product tree matches the disposition notes from round-1/merged.md: `.sln` uses lowercase `source\`, solution items point at `scripts/*.ps1`, `process-release.ps1` and `.github/scripts/` are gone, README documents net10 + OIDC/`bin/dev`, and `verify-samples` builds `tests/sample-app` Release and fails on non-zero. No new bugs observed in the fix delta; remaining PowerShell helpers under `scripts/` (e.g. `publish.ps1` still requiring `$Nuget_Key`) are intentional local one-offs called out by the README, not regressions.

## Prior findings (status)

### M1 — Severity: bug — Status: fixed
- Verification: `timewarp-simple-icons.sln:8` projects `source\timewarp-simple-icons\timewarp-simple-icons.csproj`; solution folder name is `source` (line 6). No capital `Source\` path remains in the `.sln`. Matches on-disk `source/` tree. Authoritative `.slnx` already used `source/` and is unchanged/correct.

### M2 — Severity: suggestion — Status: fixed
- Verification: No `process-release.ps1` under the repo (only historical mentions in kanban task/review docs). `.github/` contains only `workflows/workflow.yml` — no `.github/scripts/` directory. Scheduled/icon sync path is `bin/dev update-icons` via `workflow.yml` `sync-icons` job.

### M3 — Severity: suggestion — Status: fixed
- Verification: README badge is `dotnet-10.0` (line 1). Publishing section documents OIDC Trusted Publishing, `bin/dev workflow`, `bin/dev update-icons`, and helper scripts under `scripts/`; version location is `source/Directory.Build.props`. “Commands used” uses `.\source\...`. No `dotnet-6.0` badge, root script paths, or primary `$Nuget_Key` publish instructions remain in the README.

### M4 — Severity: suggestion — Status: fixed
- Verification: `tools/dev-cli/endpoints/verify-samples-command.cs` is no longer a stub. `BuildSampleAppAsync` builds `tests/sample-app/sample-app.csproj` with configuration Release via Amuru `DotNet.Build` / `RunAsync`, sets `Environment.ExitCode` and returns false on non-zero. Success path prints “Samples verified successfully!”. No `TODO: Implement` remains.

### M5 — Severity: nit — Status: fixed
- Verification: Scripts solution folder lists exactly the five files present under `scripts/`: `cline.ps1`, `publish.ps1`, `transform.ps1`, `update.ps1`, `watch-sample.ps1`. Non-existent `update-transform-publish.ps1` and root-level script paths are gone.

## Issues
(none)
