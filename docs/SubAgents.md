# Sub-Agents

This document defines how to use delegated sub-agents in this project.

## Activation Rule

Use this document only when the current session supports delegation and the user has allowed it.

If either condition is false, continue with a single main agent.

## Main Agent Responsibility

The main agent keeps ownership of:

- the critical path
- ambiguous design decisions
- guarded serialized assets
- final code integration
- final validation
- the final report to the user

## Good Delegation Targets

Prefer sub-agents for:

- read-only codebase exploration
- tracing a narrow subsystem or file cluster
- build/test/log failure triage
- documentation, wiki, or summary drafting
- bounded code changes with a clearly disjoint write scope

## Bad Delegation Targets

Do not delegate by default:

- urgent blocking work needed for the next step
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

## Integration Rule

When a sub-agent returns:

1. Review the result quickly.
2. Integrate or refine it on the main path.
3. Run the same project validation gates you would have run without delegation.

Sub-agent output is not considered done until the main agent validates it.

## Handoff Separation

Use `docs/AgentHandoffs/**` only for Codex <-> Claude Code handoffs.
Use `docs/ProjectWiki/**` for durable project knowledge.
