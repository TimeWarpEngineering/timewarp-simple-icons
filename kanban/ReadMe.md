# Kanban Board

This Kanban board helps manage and track tasks for the project using a simple folder structure.
Each task is represented by a Markdown file, and the status of a task is indicated by the folder it is in.

## Folders

1. **backlog/**: Spec still needs elaboration before work could start.
2. **to-do/**: Ready to work (definition of ready is complete).
3. **in-progress/**: Currently being worked on.
4. **done/**: Completed.
5. **archived/**: Cancelled/obsolete (when present).

## File Naming Convention

Use the kanban CLI so numbering stays consistent:

```bash
ganda kanban create "Short imperative title"   # -> kanban/to-do/NNN-short-imperative-title.md
```

- Flat tasks: `NNN-short-description.md`
- Folder tasks: `NNN-title/task.md` (`--folder`)
- Child tasks: `001-NNN-title.md` (`--parent 001`)

## Workflow

1. Create tasks with `ganda kanban create` (never hand-pick numbers).
2. Elaborate incomplete specs in **backlog/**, then promote to **to-do/** when ready.
3. Move work into **in-progress/** while implementing.
4. Move completed work to **done/**.
5. Commit every create/move/edit.
