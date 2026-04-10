# Sub-Agents

This document defines how to use internal sub-agents or delegated parallel work in this project.

It is not the same as cross-agent handoff:

- `docs/AgentHandoffs/**` is for top-level handoff between Codex and Claude Code
- `docs/SubAgents.md` is for internal delegation rules within one working session

## Activation Rule

Use this document only when:

- the current platform/session supports delegation, and
- the user has allowed sub-agent or parallel work

If either condition is false, ignore this document and continue with a single main agent.

## Main Agent Responsibility

The main agent keeps ownership of:

- the critical path
- ambiguous design decisions
- guarded serialized assets
- final code integration
- final validation
- the final report to the user

Sub-agents assist the main agent. They do not replace it.

## Good Delegation Targets

Prefer sub-agents for:

- read-only codebase exploration
- tracing a narrow subsystem or file cluster
- build/test/log failure triage
- documentation, wiki, or summary drafting
- bounded code changes with a clearly disjoint write scope

Examples:

- one sub-agent maps a combat system flow
- one sub-agent checks failing test output
- one sub-agent drafts ProjectWiki updates

## Bad Delegation Targets

Do not use sub-agents by default for:

- urgent blocking work needed for the very next step
- overlapping file edits
- the same module under active main-agent editing
- `.unity`, `.prefab`, ScriptableObject, or other guarded serialized assets
- broad refactors without explicit ownership

## Ownership Rule

Every delegated code task must have explicit ownership.

Before delegating, define:

- the exact file or folder scope
- whether it is read-only or write-enabled
- the expected output

One file area should have one writer at a time.

If ownership becomes ambiguous, keep the work on the main agent.

## Integration Rule

When a sub-agent returns:

1. Review the result quickly.
2. Integrate or refine it on the main path.
3. Run the same project validation gates you would have run without delegation.

Sub-agent output is not considered done until the main agent validates it.

## Handoff Separation

Do not use `docs/AgentHandoffs/**` to store internal sub-agent notes by default.

Use `docs/AgentHandoffs/**` only when:

- Codex is handing work to Claude Code, or
- Claude Code is handing work to Codex

If a sub-agent discovers lasting project knowledge, put the durable part in `docs/ProjectWiki/**` instead.

## Reporting Rule

If sub-agents were used during a task, report:

- whether delegation was enabled
- which bounded tasks were delegated
- the owned scope of each sub-agent
- whether any result required manual integration or follow-up validation
