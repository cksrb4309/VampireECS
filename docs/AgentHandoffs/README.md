# AgentHandoffs

This folder is for active handoff notes between Codex and Claude Code.

Use:

- `docs/ProjectWiki/**` for durable knowledge
- `docs/AgentHandoffs/**` for short-lived task transfer

## Structure

```text
docs/AgentHandoffs/
├── README.md
├── _handoff-template.md
├── pending/
│   ├── codex-to-claude/
│   └── claude-to-codex/
└── consumed/
    ├── codex-to-claude/
    └── claude-to-codex/
```

## Read Rules

- Do not preload this folder automatically for every task.
- Read it only when the user explicitly asks to continue prior work or when an agent is clearly picking up an in-progress transfer.
- Check filenames under the relevant `pending/<other-to-self>/` folder first.
- If there is only `.gitkeep`, there is no pending handoff.

## Move Rules

- After reading and using a handoff note, move it to the matching `consumed/<other-to-self>/` folder in the same task.
- Do not reread `consumed/**` unless the user explicitly asks for historical handoff context.

## Write Rules

- Write a handoff when another top-level agent is likely to continue the task.
- Use path:
  - `pending/codex-to-claude/YYYY-MM-DD-HHMM-topic.md`
  - `pending/claude-to-codex/YYYY-MM-DD-HHMM-topic.md`
- Keep the note short. Put durable knowledge in `docs/ProjectWiki/**` instead.
