# Review framework — task 005

**Date:** 2026-08-30
**Host task:** kanban/in-progress/005-fix-scheduled-icon-sync-missing-templatescriban/
**Diff scope:** branch `task/005-fix-scheduled-icon-sync-missing-templatescriban` vs `origin/main`; product change is `tools/transform/Program.cs` (commit `aa5e131`). Kanban kitchen/results commits are process, not product.
**Plan / brief:** Scheduled CI `update-icons` failed with `FileNotFoundException` for `template.scriban` because transform read it from process CWD (repo root). Port heroicons `ResolveTemplatePath` candidate list; skip non-`.svg` with `continue` instead of `return`. Do not change icon markup, package version, or clone/tag handling.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** grok review oracle (2026-08-30)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-1/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
