# Review framework — task 002

**Date:** 2026-07-26
**Host task:** kanban/in-progress/002-bring-repo-in-line-with-timewarp-standards/
**Diff scope:** branch `main` vs `origin/main` (commits from kanban rename through standards implementation; focus on compliance scaffolding)
**Plan / brief:** Bring repo in line with TimeWarp standards — `ganda repo audit` clean; align with timewarp-heroicons (MSBuild/CPM, dev-cli, workflow.yml, memsearch, vscode, dirs). No icon regeneration.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** orchestrator grok; implementer 019f9ead-c628-7bc3-af57-ddbe18532f68

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
- Verify `ganda repo audit` still reports 0 failed if claiming compliance
