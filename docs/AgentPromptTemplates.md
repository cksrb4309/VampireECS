# Agent Prompt Templates For VampireECS

## Why This Exists

The prompt is not the harness by itself.

The harness works when these pieces are combined:

- your task request
- `AGENTS.md`
- automated validation
- task-specific tests or acceptance checks

You do not need to write the full template for every request.

If you ask for a short task, the agent should still expand that request internally into task goal, safe scope, completion condition, and validation path.

If the task is expected to produce long shell output, the agent should check `docs/RTK.md` first.
If delegation is allowed, the agent should check `docs/SubAgents.md` first.
If you are asking about another agent's work, the agent should use `docs/AgentHandoffs/**` first.
If the repository uses Obsidian-style notes, the agent should use `docs/ProjectWiki/index.md` as the main hub note.
If the task needs current system orientation, the agent should also read `docs/project-context.md`.

## Base Template

```text
Task goal:
- [what should change]

Context:
- [current symptom or desired behavior]
- [relevant scene or file path]

Allowed edit scope:
- [safe folders]
- [tests if needed]

Guarded scope:
- [scenes/prefabs/data assets/project settings]

Completion criteria:
- [criterion 1]
- [criterion 2]
- [criterion 3]

Validation:
- AGENTS.md rules first
- if Unity Editor is closed, run batch validation
- if Unity Editor is open, use the editor validation menu and report the result
- add the smallest matching test or smoke check if the task needs one

Report format:
- changed files
- validation results
- any manual follow-up checks
```

## Template 1: Code-Only Task

Use this when the task should stay in runtime code and tests.

## Template 2: Code Plus Data Task

Use this when ScriptableObject or config asset edits are part of the request.

## Template 3: Scene Or Prefab Task

Use this only when you intentionally allow guarded serialized assets.

## Template 4: Investigation First

Use this when you want analysis only and no code changes yet.

## Template 5: Claude Code Handoff Check

```text
Claude Code handoff check.

Check only `docs/AgentHandoffs/pending/claude-to-codex/` first,
read unread notes that match the current task,
and move understood notes to `docs/AgentHandoffs/consumed/claude-to-codex/`.
If there is no unread note, say so and stop.
```

## One-Line Shortcut

```text
[feature/bug]을 수정해줘. AGENTS.md 규칙을 따르고, allowed scope 안에서만 작업해줘. completion criteria는 [criteria]. validation은 가능한 자동으로 해줘. Unity Editor가 열려 있으면 메뉴 검증 결과도 알려줘.
```
