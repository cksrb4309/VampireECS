# Harness Mistakes

This folder records repeatable agent-operation mistakes in this repository.

The goal is not simple retrospective logging. The goal is to strengthen rules, prompts, and validation so the same class of mistake is less likely to recur.

## Navigation

- Root hub: [[docs/HarnessMistakes/README]]
- Domain index: [[docs/HarnessMistakes/domains/README]]
- Category index: [[docs/HarnessMistakes/categories/README]]

## Usage Rules

- At task start, read `domains/README.md` first and choose one primary domain.
- Read only that domain file unless it explicitly says to preload extra category context.
- When a mistake is confirmed, assign one primary category.
- If needed, mention secondary categories, but record the incident under the primary category file.
- Append incidents to the target category file's `Incident Log`.
- If the same category repeats, strengthen the operating rule and validation, not just the note.

## Category Index

- [[docs/HarnessMistakes/categories/scope-boundary]]
  - out-of-scope edits, unrelated-change damage, guarded-asset mistakes
- [[docs/HarnessMistakes/categories/context-drift]]
  - misunderstanding current project structure or state
- [[docs/HarnessMistakes/categories/validation-gap]]
  - missing validation or declaring completion on weak validation
- [[docs/HarnessMistakes/categories/behavioral-regression]]
  - minimal gate passed, intended behavior still broken
- [[docs/HarnessMistakes/categories/tooling-automation-gap]]
  - bad execution flow caused by tool, editor, or automation misunderstanding
- [[docs/HarnessMistakes/categories/communication-handoff]]
  - unclear user escalation, handoff, or completion reporting

## Domain Routing

- [[docs/HarnessMistakes/domains/README]]
- [[docs/HarnessMistakes/domains/combat-gameplay]]
- [[docs/HarnessMistakes/domains/ui-bridge]]
- [[docs/HarnessMistakes/domains/scene-prefab]]
- [[docs/HarnessMistakes/domains/data-assets]]
- [[docs/HarnessMistakes/domains/vfx-presentation]]
- [[docs/HarnessMistakes/domains/validation-tooling]]
- [[docs/HarnessMistakes/domains/docs-harness]]

## Incident Entry Format

```text
### YYYY-MM-DD - [short title]
- Trigger:
- Impact:
- Detection:
- Rule Change:
- Validation Change:
- Follow-up Files:
```

## Category Priority

When several categories appear applicable, prefer this order for the primary category:

1. scope-boundary
2. validation-gap
3. context-drift
4. behavioral-regression
5. tooling-automation-gap
6. communication-handoff
