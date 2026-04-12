# AgentHandoffs

This folder stores short, durable handoff notes between Codex and Claude Code.

- Use `docs/AgentHandoffs/pending/` for unread handoffs.
- Move understood notes to `docs/AgentHandoffs/consumed/`.
- Keep handoff notes short: task, state, files, validation, next action.
- Do not use this folder for internal sub-agent notes.

## Structure

```text
docs/AgentHandoffs/
- README.md
- _handoff-template.md
- pending/
  - codex-to-claude/
  - claude-to-codex/
- consumed/
  - codex-to-claude/
  - claude-to-codex/
```
