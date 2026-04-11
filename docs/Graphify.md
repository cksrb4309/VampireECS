# Graphify

`graphify` is an optional knowledge-graph layer for repository orientation.

Use it to understand structure faster. Do not treat it as a replacement for AGENTS, CLAUDE, harness validation, or guarded asset rules.

## Purpose

Use `graphify` for:

- architecture orientation
- ECS or gameplay system relationship mapping
- code + docs + notes overview
- repeated structure questions with lower token cost

## Refresh Workflow

Run this from the Unity project root when the graph may be stale:

```powershell
tools\\graphify-refresh.cmd .
```

The refresh script always writes to the repo-root `graphify-out/` folder, even when the scan target is a subfolder such as `Assets\\01_Scripts`.

## Agent Rule

If repo-root `graphify-out/GRAPH_REPORT.md` exists, read it before broad architecture questions or wide repository exploration.
