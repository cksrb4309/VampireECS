# Graphify

`graphify` is an optional repository knowledge-graph layer for structure understanding.

Use it for:

- architecture orientation
- ECS system relationship mapping
- broad repo structure questions

Do not use it as a replacement for:

- `AGENTS.md`
- `CLAUDE.md`
- validation commands
- guarded asset rules

## Compatibility Rule

This repository already has a local harness. Do not use the following as a default workflow:

```powershell
graphify codex install
graphify claude install
```

Reason:

- those installers can overwrite or mutate existing harness files and hooks

If graphify is adopted, install it manually and merge any config changes instead of letting it overwrite the project harness.

## Suggested Workflow

If `graphify-out/GRAPH_REPORT.md` exists:

- read it before large architecture questions
- refresh it with `graphify . --update` when it is stale

If it does not exist, do not block normal work on it.
