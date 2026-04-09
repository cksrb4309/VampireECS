# Harness Mistake Domains

This file routes mistake context so agents do not read every mistake file on every task.

## Navigation

- Root hub: [[docs/HarnessMistakes/README]]
- Category index: [[docs/HarnessMistakes/categories/README]]

## Usage Rules

1. Choose one primary domain for the current task.
2. Read only that domain file first.
3. If it says `Preload extra mistake context: no`, stop there.
4. If it says `Preload extra mistake context: yes`, read only the listed category files.

## Domain Index

- [[docs/HarnessMistakes/domains/combat-gameplay]]
  - combat logic, ECS abilities, enemies, damage, progression loops
- [[docs/HarnessMistakes/domains/ui-bridge]]
  - UI sync, input bridge, observable/controller/binder issues
- [[docs/HarnessMistakes/domains/scene-prefab]]
  - scenes, prefabs, inspector links, missing references, structural edits
- [[docs/HarnessMistakes/domains/data-assets]]
  - ScriptableObject config, unlock/stat tuning, reference consistency
- [[docs/HarnessMistakes/domains/vfx-presentation]]
  - VFX, presentation managers, view objects, visual bridges
- [[docs/HarnessMistakes/domains/validation-tooling]]
  - compile/smoke/test flow, editor validation menus, automation, batchmode
- [[docs/HarnessMistakes/domains/docs-harness]]
  - README, AGENTS, CLAUDE, prompt templates, harness docs
