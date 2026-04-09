# VampireECS Project Context

This file is detailed context for agent work. It complements `CLAUDE.md` and is not intended for automatic preload.

## Core Systems

### ECS Combat

Role:
- Owns combat state, attack execution, damage application, enemy behavior, progression events, and system ordering.

Key paths:
- `Assets/01_Scripts/ECS/Attack/**`
- `Assets/01_Scripts/ECS/Combat/**`
- `Assets/01_Scripts/ECS/Enemy/**`
- `Assets/01_Scripts/ECS/SystemGroups/**`

### Bridges And Presentation

Role:
- Converts ECS state into UI, VFX, and scene-side behavior without pulling managed references into ECS core.

Key paths:
- `Assets/01_Scripts/UI/Bridge/**`
- `Assets/01_Scripts/ECS/Visual/**`
- `Assets/01_Scripts/Presentation/**`
- `Assets/01_Scripts/System/**`

## Visual Flow

```text
ECS gameplay systems
    -> emit state changes or visual event entities
Bridge / presentation systems
    -> consume ECS state or one-shot visual events
Managed view / VFX layer
    -> pooled objects, UI widgets, camera, LineRenderer/VFX Graph
```

Typical examples:

- damage -> `DamageEventData` -> apply damage -> `DamageTextEvent` -> presentation system -> damage text provider
- meteor / chain lightning / black hole visuals -> ECS visual event or ECS visual binding -> managed pooled view

## Data Flow

- Ability tuning assets live under `Assets/09_Data/ScriptableObject/**`
- Unlock assets own base tuning and attach runtime components
- Stats assets own stackable bonuses
- Runtime attack formulas generally follow:
  - `Base * (1 + local bonus) * CombatStats`
- Count/radius/timer values use explicit additive bonus fields when compounding would distort balance

## System Ordering / Lifecycle

Bootstrap:

1. scene loads `Test_Combat`
2. `EntitiesSubScene` loads authored ECS content
3. bootstrap/singleton setup initializes player, combat settings, ability config application

Update phases:

- preparation/setup groups: spatial indexing, target prep, event setup
- attack/combat groups: attack execution, damage event production, lethal handling
- destruction/cleanup groups: dead entity cleanup and event disposal
- visual bridge systems: consume ECS visual events after gameplay state is final for the frame

## Architecture Rules

- Keep combat logic in ECS and Burst/job-friendly paths where practical.
- Keep UI, camera, VFX object management, and presentation pooling in managed bridge layers.
- Prefer event entities and narrow data components over direct cross-system coupling.
- Treat scenes, prefabs, ScriptableObject tuning assets, and project settings as guarded surfaces.
- Prefer code-only scope first unless the task clearly requires serialized asset changes.
