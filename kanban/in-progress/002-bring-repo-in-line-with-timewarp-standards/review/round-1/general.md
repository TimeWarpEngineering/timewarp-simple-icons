# Round 1 — general
**Date:** 2026-07-26
**Scope reviewed:** origin/main...HEAD compliance scaffolding

## Summary
Scaffolding largely matches the heroicons-aligned TimeWarp package layout: net10 + CPM, `source/Directory.Build.props` metadata (16.27.1, CC0-1.0, `logo.png`), Nuru `tools/dev-cli` with real `workflow`/`update-icons`, and `.github/workflows/workflow.yml` on `main` with OIDC + weekly schedule. Path casing in live ProjectReferences/scripts is fixed to `source/`. Remaining gaps are mostly legacy leftovers (broken `.sln`, dead `process-release.ps1`, stale README publish docs) and the intentional `verify-samples` stub.

## Issues

### Issue 1 — Severity: bug
- File: timewarp-simple-icons.sln:8
- Description: Legacy solution still references `Source\timewarp-simple-icons\timewarp-simple-icons.csproj` (capital `Source`). On case-sensitive Linux this fails to resolve; the authoritative tree is `source/`. Plan said keep `.sln` alongside `.slnx`, but the kept file was not updated with the Source→source fix that sample-app/transform received.
- Suggestion: Retarget the package project path to `source\timewarp-simple-icons\timewarp-simple-icons.csproj` (and refresh solution-item script paths under `scripts/`), or drop/ignore the `.sln` if `.slnx` is the only supported entrypoint and document that.
- Status: open

### Issue 2 — Severity: suggestion
- File: .github/scripts/process-release.ps1:27
- Description: Leftover from the retired `publish.yml` path. It still reads `<Version>` from `source/timewarp-simple-icons/timewarp-simple-icons.csproj` (version now lives only in `source/Directory.Build.props`), clones/transforms via Windows-style `tools\transform`, and calls `./publish.ps1` at repo root after scripts moved to `scripts/publish.ps1`. Not wired into `workflow.yml`, but it is wrong if anyone runs it.
- Suggestion: Delete `.github/scripts/process-release.ps1` (and empty scripts dir if unused) now that `dev update-icons` covers scheduled sync/publish; or rewrite it to match the C# command (prefer delete).
- Status: open

### Issue 3 — Severity: suggestion
- File: README.md:1
- Description: Contributor/publish docs are pre-standards: badge claims `dotnet-6.0`; steps set version in the csproj, run root `update.ps1`/`transform.ps1`/`publish.ps1`, and require `$Nuget_Key`. CI/dev-cli now use OIDC Trusted Publishing and `dev update-icons` / `dev workflow`. Paths in “Commands used” still show `.\Source\...`.
- Suggestion: Point publish flow at `bin/dev update-icons` / GitHub release + OIDC; fix TFM badge to net10; correct paths to `source/` and `scripts/`; drop secret-key primary instructions.
- Status: open

### Issue 4 — Severity: suggestion
- File: tools/dev-cli/endpoints/verify-samples-command.cs:25
- Description: `verify-samples` is a hard-coded success stub (`TODO: Implement…`). Capabilities advertise the command, but it never builds `tests/sample-app` or other samples. Acceptable as peer scaffold copy, still a gap vs the task’s “sample app / tests still run” verify story (sample build is only covered indirectly via `build`/`workflow` if the slnx is built).
- Suggestion: Implement by building `tests/sample-app/sample-app.csproj` (Release) and failing on non-zero exit, or remove the route until real sample verification exists.
- Status: open

### Issue 5 — Severity: nit
- File: timewarp-simple-icons.sln:19
- Description: Solution items still list root `publish.ps1`, `transform.ps1`, `update.ps1`, `watch-sample.ps1`, and non-existent `update-transform-publish.ps1` after the scripts were moved under `scripts/`.
- Suggestion: Point solution items at `scripts/*.ps1` or remove the Scripts folder from the `.sln`.
- Status: open
