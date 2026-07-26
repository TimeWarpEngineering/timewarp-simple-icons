# Bring repo in line with TimeWarp standards

## Description

Update `timewarp-simple-icons` so it matches the current TimeWarp `timewarp-xxx` repo standard: pass `ganda repo audit` cleanly and follow the same layout/tooling patterns as sibling package repos (especially `timewarp-heroicons`, which currently passes all audit checks).

This is scaffolding/compliance work first (MSBuild, CPM, dev CLI, CI workflow, VS Code, memsearch, kanban columns, docs/skills dirs). Product/icon generation changes are out of scope unless required for audit.

## Requirements

- `ganda repo audit` reports **0 failed** checks (warnings may still be addressed if fixable).
- Repo structure matches TimeWarp conventions used by package repos:
  - Root: `Directory.Build.props`, `Directory.Packages.props`, `BannedSymbols.txt`, `.envrc`, `*.slnx`, `msbuild/repository.props`
  - `source/Directory.Build.props`, `bin/dev`, `tools/dev-cli` (Nuru-based), `documentation/`, `skills/`, `kanban/archived/`
  - `.github/workflows/workflow.yml`, memsearch scaffold (`.memsearch.toml`, `.githooks/*`), VS Code window-icon config
- Prefer `ganda repo audit --fix` for auto-fixable items; hand-align anything remaining by copying patterns from a green peer repo.
- Existing package still builds/tests after structural changes (no silent breakage of `source/timewarp-simple-icons`).
- Do not invent custom one-off structure; align with peer repos and skills (`tw-dev-cli`, `tw-csharp`, `tw-nuru`, `tw-git`, `tw-kanban`).

## Checklist

### Bootstrap (auto-fix)

- [x] Run `ganda repo audit` and capture baseline (currently ~15 failures)
- [x] Run `ganda repo audit --fix` as the first structural step
- [x] Re-run `ganda repo audit` and list remaining failures

### Align remaining failures (reference: timewarp-heroicons)

- [x] MSBuild / CPM: root + `source/` Directory.Build.props, `Directory.Packages.props`, `BannedSymbols.txt`, `msbuild/repository.props`
- [x] Dev CLI: `bin/dev`, `tools/dev-cli` with TimeWarp.Nuru, region annotations, `--capabilities`
- [x] Solution: add/update `.slnx` (keep or retire legacy `.sln` per peer pattern)
- [x] Env: `.envrc` with `PATH_add bin`
- [x] Dirs: `documentation/`, `skills/`, `kanban/archived/`
- [x] CI: `.github/workflows/workflow.yml` (and reconcile with any existing publish workflow)
- [x] Memsearch: `.memsearch.toml`, `.githooks/post-commit` + `post-merge`, `core.hooksPath=.githooks`
- [x] VS Code window-icon: avatar SVG, `.vscode/tasks.json`, `window.title`, `timewarp.blurImagePath`
- [x] NuGet package metadata / icon wiring if audit surfaces it after scaffold

### Verify

- [x] `ganda repo audit` clean
- [x] `bin/dev --capabilities` works (or peer-equivalent)
- [x] Solution builds; sample app / tests still run
- [x] Commit compliance work in conventional commits

## Notes

- **Gold standard peer:** `/home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-heroicons/main` — `ganda repo audit` → 21 pass / 0 fail.
- **Already done:** kebab-case `kanban/` columns; done task `001-convert-publish-workflow-to-powershell.md`.

### Implementation plan (2026-07-26)

**Defaults (locked):** net10.0; keep package version `16.27.1`; keep license `CC0-1.0` and authors; branch `main`; keep `.sln` + make `.slnx` authoritative; move root `*.ps1` → `scripts/` (don’t delete); replace secret NuGet publish with OIDC/`dev workflow` + retire `publish.yml` once covered.

**Critical pre-existing bugs (Linux):** `tests/sample-app` ProjectReference uses capital `Source/`; `transform.ps1` same — fix to `source/`.

#### Phase A — Bootstrap
1. `ganda repo audit` → baseline
2. `ganda repo audit --fix` first structural step
3. Re-audit; **immediately** rewrite scaffolded `source/Directory.Build.props` (template uses wrong version/license/authors)
4. Do not commit `.memsearch/memory/*` or bin/obj

#### Phase B — MSBuild / CPM / TFM
1. Align root `Directory.Build.props`, `msbuild/repository.props`, `BannedSymbols.txt` with heroicons
2. Rewrite `source/Directory.Build.props`: Version `16.27.1`, CC0-1.0, PackageIcon `logo.png`, README pack, package URLs/tags
3. Ensure `assets/logo.png` (lowercase); keep `Logo.png` for README if desired
4. Slim package csproj → net10.0, CPM PackageReferences without Version
5. Sample-app + transform → net10.0, CPM, fix `source/` path
6. `Directory.Packages.props` pins from heroicons (+ AspNetCore 10.x, Scriban, Nuru/Amuru/Build.Tasks)
7. Optional `global.json` like heroicons
8. Verify: `dotnet restore` + build package/sample/transform

#### Phase C — Solution
1. Populate empty `.slnx` with package, sample-app, transform (folder groups like heroicons)
2. `dotnet build timewarp-simple-icons.slnx -c Release`

#### Phase D — Dev CLI
1. Keep scaffold; port heroicons endpoints: build/test/workflow against `.slnx`
2. Description: `Development CLI for timewarp-simple-icons`
3. Capabilities must include: build, check-version, clean, self-install, test, verify-samples, workflow
4. Replace stub `workflow` with mode-aware PR/merge/release (pack `timewarp-simple-icons`)
5. Optional: `update-icons` port from process-release.ps1 (needed before deleting scheduled publish)
6. `#region Purpose` on all tools/dev-cli .cs; `dotnet run tools/dev-cli/dev.cs -- self-install`; `./bin/dev --capabilities`

#### Phase E — CI
1. Align `.github/workflows/workflow.yml` with heroicons (main branch, net10, OIDC nuget login)
2. Optional sync-icons job/schedule once `update-icons` exists
3. Delete `publish.yml` + move PS1 scripts under `scripts/` with fixed paths
4. Confirm Trusted Publishing for package id on nuget.org (note if not verifiable locally)

#### Phase F — Scaffold leftovers
Confirm documentation/, skills/, kanban/archived/, memsearch, .githooks + core.hooksPath, vscode window-icon assets

#### Phase G — Verify + commit
- `ganda repo audit` → Failed: 0
- `./bin/dev --capabilities`, build slnx, pack nupkg with logo+readme
- Conventional commits; no secrets/bin dumps

**Risks:** dual workflows; scaffold version overwrite; empty slnx; CPM NU1008; Source vs source; stub workflow; Trusted Publishing.

### Session

- Orchestrator: grok (2026-07-26) — phases 1–2; plan locked
- Plan agent: 019f9eaa-0dfe-7553-bf09-9787e8bbd237

## Results

(Fill when complete)
