# Round 1 — general
**Date:** 2026-08-30
**Scope reviewed:** branch task/005 vs origin/main; tools/transform/Program.cs and surrounding call sites

## Summary

The change ports heroicons-style `ResolveTemplatePath()` so `template.scriban` is found from the binary/output dir (and fallbacks) instead of process CWD, and switches non-`.svg` handling from `return` to `continue`. Product diff is limited to `tools/transform/Program.cs`; SVG rewrite, component naming, package version (`16.27.1`), and NuGet/publish paths are untouched. Risk is low: the CI failure mode is directly addressed, and a repo-root `dotnet run --project tools/transform/transform.csproj -- <in> <out>` smoke (without `--no-launch-profile`) exited 0, wrote `GithubIcon.razor` with `@attributes=Attributes`, and skipped a non-svg fixture.

## Issues
