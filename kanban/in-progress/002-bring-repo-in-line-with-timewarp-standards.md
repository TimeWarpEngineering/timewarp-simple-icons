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

- [ ] Run `ganda repo audit` and capture baseline (currently ~15 failures)
- [ ] Run `ganda repo audit --fix` as the first structural step
- [ ] Re-run `ganda repo audit` and list remaining failures

### Align remaining failures (reference: timewarp-heroicons)

- [ ] MSBuild / CPM: root + `source/` Directory.Build.props, `Directory.Packages.props`, `BannedSymbols.txt`, `msbuild/repository.props`
- [ ] Dev CLI: `bin/dev`, `tools/dev-cli` with TimeWarp.Nuru, region annotations, `--capabilities`
- [ ] Solution: add/update `.slnx` (keep or retire legacy `.sln` per peer pattern)
- [ ] Env: `.envrc` with `PATH_add bin`
- [ ] Dirs: `documentation/`, `skills/`, `kanban/archived/`
- [ ] CI: `.github/workflows/workflow.yml` (and reconcile with any existing publish workflow)
- [ ] Memsearch: `.memsearch.toml`, `.githooks/post-commit` + `post-merge`, `core.hooksPath=.githooks`
- [ ] VS Code window-icon: avatar SVG, `.vscode/tasks.json`, `window.title`, `timewarp.blurImagePath`
- [ ] NuGet package metadata / icon wiring if audit surfaces it after scaffold

### Verify

- [ ] `ganda repo audit` clean
- [ ] `bin/dev --capabilities` works (or peer-equivalent)
- [ ] Solution builds; sample app / tests still run
- [ ] Commit compliance work in conventional commits

## Notes

- **Gold standard peer:** `/home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-heroicons/main` — `ganda repo audit` → 21 pass / 0 fail.
- **Already done in this worktree:** kebab-case `kanban/` columns; legacy done task renamed to `001-convert-publish-workflow-to-powershell.md`.
- **Known baseline failures (2026-07-26):** banned-api/symbols, bin-dev, Directory.Packages.props, directory-structure (documentation/skills/archived), envrc, memsearch, msbuild repository.props, nuru, region-annotations, slnx, source Directory.Build.props, vscode-window-icon, workflow-file.
- `ganda repo audit --fix` may scaffold many files; review diffs carefully (especially workflow vs existing `publish` scripts, and CPM package versions for this library).
- After fix, diff against heroicons for any non-fixable drift (scripts, global.json, assets naming).

## Results

(Fill when complete)
