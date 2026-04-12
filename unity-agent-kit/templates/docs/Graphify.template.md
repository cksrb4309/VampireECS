# Graphify

`graphify` is an optional knowledge-graph layer for repository orientation.

Use it to understand structure faster. Do not treat it as a replacement for:

- `AGENTS.md`
- `CLAUDE.md`
- harness validation commands
- guarded asset rules

## Purpose

Use `graphify` for:

- architecture orientation
- ECS or gameplay system relationship mapping
- code + docs + notes overview
- repeated structure questions with lower token cost

Do not use `graphify` as proof that a Unity change is safe or complete.

## Install

Choose the agent environment you actually use.

### Codex

```powershell
pip install graphifyy
graphify install --platform codex
```

Codex parallel extraction also needs this in `~/.codex/config.toml`:

```toml
[features]
multi_agent = true
```

### Claude Code (Windows)

```powershell
pip install graphifyy
graphify install
```

## Harness Compatibility Rule

In a harness-managed Unity project, do **not** use these commands as the default workflow:

```powershell
graphify codex install
graphify claude install
```

Reason:

- they may modify `AGENTS.md`
- they may modify `CLAUDE.md`
- they may install hook behavior that overlaps with the harness

If you intentionally want those mutations, review and merge them manually instead of letting them overwrite the project harness.

## Initial Build

Run in the Unity project root:

```powershell
tools\graphify-refresh.cmd .
```

Recommended ignores:

```gitignore
Library/
Temp/
Logs/
Obj/
Build/
Builds/
UserSettings/
graphify-out/
.graphify_*.json
__pycache__/
Assets/**/graphify-out/
```

If needed, place them in `.graphifyignore`.

## Agent Usage Rule

If `graphify-out/GRAPH_REPORT.md` exists:

- read it before broad architecture questions
- read it before wide grep/glob exploration across the whole repository
- prefer it for high-level system orientation

Still read raw source files before making non-trivial edits.

## Refresh Workflow (Explicit Only)

This project does not use git hooks or background watchers. Refresh is manual only.

```powershell
tools\graphify-refresh.cmd .
```

## Recommended Policy

- after a meaningful code/doc/image update: run `tools\graphify-refresh.cmd .`
- before asking large structure questions: refresh once if the graph might be stale

## Query Examples

```powershell
graphify query "show the combat flow" --graph graphify-out/graph.json
graphify query "what connects SpawnSystem to DamageSystem?" --graph graphify-out/graph.json
graphify path "ChainLightningSystem" "DamageTextEvent"
graphify explain "CombatSetting"
```

## Expected Outputs

`graphify-out/` should usually contain:

- `GRAPH_REPORT.md`
- `graph.json`
- `graph.html`
- `manifest.json`

## Reporting Rule

If the agent used `graphify` during a task, report:

- whether `GRAPH_REPORT.md` was read
- whether the graph was refreshed
- which command ran (`tools\graphify-refresh.cmd .`)
- whether the graph may still be stale
