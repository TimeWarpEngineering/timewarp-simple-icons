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

- [x] Resolve `template.scriban` from binary / project dir (see heroicons `ResolveTemplatePath`)
- [x] Skip non-`.svg` files with `continue` instead of `return`
- [x] Prove transform from repo-root CWD (repro of CI)
- [x] `bin/dev build` (and tests if any) green
- [x] Write `## Results` + `### How to validate`
- [x] Implementation review (effort 1, general) + disposition on this id

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

### Review kitchen

Effort 1, general only, round 1. Disposition **clean** (0 open). Artifacts under `review/` (framework, `round-1/`, `disposition.md`). No sibling apply-findings task.

## Session

- Orchestrator: grok session 480965 (2026-08-30)
- Created: 480965 (2026-08-30)
- Implementer: grok (2026-08-30)
- Review: grok oracle (2026-08-30); general reviewer subagent `01a0532b-4c0f-7a20-964a-9dd1c3a0ae4b`

## Results

`tools/transform` locates `template.scriban` without depending on process CWD, using the same candidate list as timewarp-heroicons: binary dir (`CopyToOutputDirectory`), project dir via `../../../template.scriban`, CWD, and `tools/transform` under CWD. Non-`.svg` files in the input directory are skipped with `continue` so they no longer abort `Main`.

Simple-icons SVG rewrite (inject `@attributes=Attributes` on the known `<svg>` opening tag) and component naming (leading non-letter → `_`, `Icon` suffix) are unchanged. Package version remains `16.27.1`; no icon tree regeneration and no NuGet publish.

### Files changed

- `tools/transform/Program.cs`

### Decisions

- Ported `ResolveTemplatePath` and parse-the-template-once from heroicons; did not copy size/namespace logic.
- Did not change `update-icons` clone/tag handling (out of scope unless transform still failed after the path fix).
- Did not run a full `update-icons` sync; local proof used a tiny SVG fixture from repo-root CWD and from `/tmp`.

### Test outcomes

- `dotnet run --project tools/transform/transform.csproj -- <in> <out>` from repo root: exit 0, no `FileNotFoundException` for `template.scriban`.
- Same with `--no-launch-profile` and CWD=`/tmp`: exit 0, `GithubIcon.razor` written.
- Non-svg fixture files produced no components; both fixture SVGs transformed.
- `dotnet run tools/dev-cli/dev.cs -- build`: succeeded, 0 warnings, 0 errors. Packed package still `timewarp-simple-icons.16.27.1.nupkg`.
- No dedicated test project in the solution besides `tests/sample-app`.

### How to validate

**Smoke**

From the claim worktree, with CWD = repo root (not `tools/transform`):

```bash
IN=$(mktemp -d)
OUT=$(mktemp -d)
printf 'skip\n' > "$IN/notes.txt"
cat > "$IN/github.svg" <<'EOF'
<svg role="img" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><title>GitHub</title><path d="M12 .297"/></svg>
EOF
dotnet run --project tools/transform/transform.csproj --no-launch-profile -- "$IN" "$OUT"
```

**Expect**

- Exit 0 (no `FileNotFoundException` for `template.scriban` at repo root or `/tmp`)
- `$OUT/GithubIcon.razor` exists, starts with `@namespace TimeWarp.Simple.Icons`, includes `@attributes=Attributes`
- No razor file generated from `notes.txt`

**Automated gate**

```bash
dotnet run tools/dev-cli/dev.cs -- build
# expect: Build succeeded, 0 Warning(s), 0 Error(s)
```

`./bin/dev` is gitignored; the runfile invocation above is the same `build` endpoint. `./bin/dev build` is equivalent after `dotnet run tools/dev-cli/dev.cs -- self-install`.

**Not in scope:** full `update-icons --push --publish` (network + NuGet). After this lands, scheduled CI should be able to bump 16.27.1 → latest simple-icons.

### Review

- **Rounds:** 1
- **Effort / roster:** 1 — general only
- **Counts (final):** bug 0/0/0, suggestion 0/0/0, nit 0/0/0 (open/fixed/wontfix)
- **Disposition:** clean (no issues raised; 0 open)
- **Wontfix / escalations:** none
- **Paths:**
  - `kanban/in-progress/005-fix-scheduled-icon-sync-missing-templatescriban/review/review-framework.md`
  - `kanban/in-progress/005-fix-scheduled-icon-sync-missing-templatescriban/review/round-1/general.md`
  - `kanban/in-progress/005-fix-scheduled-icon-sync-missing-templatescriban/review/round-1/merged.md`
  - `kanban/in-progress/005-fix-scheduled-icon-sync-missing-templatescriban/review/disposition.md`
