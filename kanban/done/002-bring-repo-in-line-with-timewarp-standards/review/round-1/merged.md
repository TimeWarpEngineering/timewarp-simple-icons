# Round 1 — merged findings
**Date:** 2026-07-26
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 3 | 0 |
| nit | 0 | 1 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed
- File: timewarp-simple-icons.sln:8
- Description: Legacy solution still references capital `Source\...` path; broken on Linux.
- Suggestion: Retarget to `source\...` and fix script solution items under `scripts/`, or document .slnx-only.
- Source: general
- Disposition notes: Retargeted package project path to `source\timewarp-simple-icons\...`; solution folder renamed to `source`.

### M2 — Severity: suggestion — Status: fixed
- File: .github/scripts/process-release.ps1:27
- Description: Leftover broken process-release.ps1 (wrong version location, wrong script paths).
- Suggestion: Delete now that `dev update-icons` covers the flow.
- Source: general
- Disposition notes: Deleted `process-release.ps1` and removed empty `.github/scripts/`; `bin/dev update-icons` is the path forward.

### M3 — Severity: suggestion — Status: fixed
- File: README.md:1
- Description: Stale TFM badge, secret-key publish docs, root script paths, Source casing.
- Suggestion: Align README with net10, OIDC/dev-cli, scripts/, source/.
- Source: general
- Disposition notes: Badge → `dotnet-10.0`; publish section documents OIDC + `bin/dev update-icons` / `bin/dev workflow`; paths use `source/` and `scripts/`.

### M4 — Severity: suggestion — Status: fixed
- File: tools/dev-cli/endpoints/verify-samples-command.cs:25
- Description: verify-samples is a success stub; never builds sample-app.
- Suggestion: Build tests/sample-app Release and fail on non-zero.
- Source: general
- Disposition notes: Builds `tests/sample-app/sample-app.csproj` Release via Amuru `DotNet.Build` / `RunAsync`; sets ExitCode on failure.

### M5 — Severity: nit — Status: fixed
- File: timewarp-simple-icons.sln:19
- Description: Solution items list root scripts and missing update-transform-publish.ps1 after move to scripts/.
- Suggestion: Point at scripts/*.ps1 or remove folder.
- Source: general
- Disposition notes: Solution items now point at `scripts/cline.ps1`, `publish.ps1`, `transform.ps1`, `update.ps1`, `watch-sample.ps1`; removed non-existent `update-transform-publish.ps1`.

## Duplicates / conflicts

- M5 overlaps M1 (.sln cleanup); fix together.
