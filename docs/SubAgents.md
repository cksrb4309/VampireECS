# SubAgents

This file defines delegation rules for sessions that support sub-agents.

It is separate from cross-agent handoff:

- `docs/AgentHandoffs/**` is for Codex <-> Claude Code handoff
- `docs/SubAgents.md` is for bounded delegation inside a single session

## Activation Rule

Use sub-agents only when:

- the current platform actually supports delegation, and
- the user has allowed delegated or parallel work

Otherwise ignore this file and keep the work on the main agent.

## Good Delegation Targets

- read-only subsystem exploration
- narrow log or test triage
- documentation drafting
- clearly bounded code changes with non-overlapping write scope

## Bad Delegation Targets

- urgent blocking work on the critical path
- overlapping file edits
- `.unity`, `.prefab`, ScriptableObject, or other guarded asset changes
- broad refactors without explicit ownership

## Ownership Rule

Every delegated task must define:

- exact file or folder scope
- read-only or write-enabled status
- expected output

The main agent retains ownership of:

- guarded assets
- design decisions
- integration
- validation
- final reporting
