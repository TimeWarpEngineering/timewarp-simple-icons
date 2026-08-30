# Fix scheduled icon sync missing template.scriban

## Description

Scheduled CI on `timewarp-simple-icons` fails during icon sync because `tools/transform` reads `template.scriban` from the process CWD (repo root), not from the transform project. Port the path resolution already used in `timewarp-heroicons` so weekly `update-icons --push --publish` can bump 16.27.1 → 16.29.0 and publish.

## Requirements

- `tools/transform` locates `template.scriban` without depending on CWD.
- `bin/dev update-icons` succeeds when invoked from repo root (same as CI `workflow` sync mode).
- Non-svg files in the upstream icons dir do not abort the whole transform (`return` vs `continue`).
- Do not change icon markup, package version, or NuGet publish unless a successful local sync actually regenerates icons (then version/releases follow existing `update-icons` behavior).
- Do not `kanban done` and do not `gh pr create` / **tw-pr** create. Host `open-pr` opens the PR. Write `## Results` including `### How to validate`.

## Checklist

- [ ] Resolve `template.scriban` from binary / project dir (see heroicons `ResolveTemplatePath`)
- [ ] Skip non-`.svg` files with `continue` instead of `return`
- [ ] Prove transform from repo-root CWD (repro of CI)
- [ ] `bin/dev build` (and tests if any) green
- [ ] Write `## Results` + `### How to validate`

## Notes

### Failure

- Run: https://github.com/TimeWarpEngineering/timewarp-simple-icons/actions/runs/33307775988
- Workflow: CI/CD #15, event `schedule`, job `ci`, SHA `5f5c657` on `origin/main`
- Mode: Sync (`update-icons --push --publish`)
- Log:
  - Current package `16.27.1`, latest simple-icons `16.29.0`
  - Clone of `simple-icons` 16.29.0 succeeded (warning `refs/tags/16.29.0 is not a commit` then detached HEAD `9e22f29`)
  - `FileNotFoundException: Could not find file '.../timewarp-simple-icons/template.scriban'`
  - at `tools/transform/Program.cs:37` `File.ReadAllText("template.scriban")`
  - wrapper: `Icon transform failed.` then CliWrap exit 134

### Root cause

`tools/dev-cli/endpoints/update-icons-command.cs` `TransformIconsAsync` runs:

```text
dotnet run --project tools/transform/transform.csproj -- <clone>/icons <repo>/source/timewarp-simple-icons/icons
```

with `.WithWorkingDirectory(repoRoot)`.

The PowerShell helper `scripts/transform.ps1` does `Push-Location tools/transform`, so local one-offs work. CI does not.

`template.scriban` lives at `tools/transform/template.scriban` and is already `CopyToOutputDirectory=PreserveNewest`.

### Known-good sibling

`timewarp-heroicons` `tools/transform/Program.cs` already has `ResolveTemplatePath()` for this exact CI failure. Copy the candidate list (binary dir, project dir via `../../../template.scriban`, CWD, `tools/transform` under CWD). Keep simple-icons SVG rewrite / component naming; do not copy heroicons size/namespace logic.

Also fix in the same file: `if (iconExtension != ".svg") return;` exits `Main` on the first non-svg. Change to `continue`.

Do not expand into git-tag clone warning unless transform still fails after the path fix.

### Origin-home note

Local origin-home `main` is 6 commits ahead of `origin/main` (unpushed task 004 audit-clean / path casing). Claim started from origin-home. Do not revert those commits; do not add more 004 work. Keep this task's product diff scoped to the transform CWD bug.

### Validate (implementer)

From the claim worktree, with CWD = repo root (not `tools/transform`):

```bash
# Minimal repro of the CI crash — must not FileNotFoundException
dotnet run --project tools/transform/transform.csproj -- /tmp/does-not-need-full-upstream-if-you-use-a-tiny-svg-dir <out-dir>
```

Prefer a tiny temp dir with one `.svg` (and one non-svg file to prove `continue`). Full `bin/dev update-icons` without `--push --publish` is also valid if network is available; do not publish from the implementer.

`bin/dev build` must pass.

## Session

- Orchestrator: grok session 480965 (2026-08-30)
- Created: 480965 (2026-08-30)
