# VampireECS Project Context

## Core Systems

### Combat Core

- ECS owns combat state, targeting, attack spawning, hit detection, damage application, and death cleanup.
- `ApplyDamageSystem`, `EnemyDeathSystem`, and the attack family systems are the most important runtime paths.

### Player and Enemy Flow

- Player input is bridged into ECS-facing state.
- Player movement and rotation stay in ECS.
- Enemy spawn, follow, direction, and teleport logic stay in ECS.

### Ability Flow

- Unlock configs define base tuning and unlock behavior.
- Stats configs define stackable upgrade values.
- Runtime `...BaseStatsData` and `...StatsData` are applied to the player entity.
- Shared combat tuning comes from `CombatStatsData`.

## Visual Flow

```text
ECS combat systems
  -> visual event entities
  -> presentation bridge systems
  -> managed VFX / view managers
```

Important presentation pieces:

- `DamageTextPresentationSystem`
- `ChainLightningPresentationSystem`
- `MeteorStrikePresentationSystem`
- `BlackHolePresentationSystem`
- `DamageTextProvider`
- `AuraViewManager`
- `ChainLightningViewManager`
- `MeteorStrikeViewManager`
- `BlackHoleViewManager`

## Data Flow

- Config assets live under `Assets/09_Data/ScriptableObject/**`
- Combat tuning is usually split between `Unlock...Config` and `...StatsConfig`
- Scene setup lives in `Assets/07_Scenes/Test_Combat.unity`
- Scene bootstrap prefab lives in `Assets/06_Prefabs/Scene/CombatSetting.prefab`

## System Ordering / Lifecycle

Bootstrap order:

1. scene and scene-settings initialization
2. player and enemy authoring bake
3. spatial partition build
4. attack setup and movement
5. damage application and death cleanup
6. presentation event consumption

Update order:

- early: spatial preparation and attack setup
- default: movement, targeting, hit detection, and damage application
- late: cleanup and presentation

## Architecture Rules

- Keep gameplay logic in ECS.
- Keep UI and presentation in bridge layers.
- Keep hot paths Burst/job friendly.
- Keep serialized assets aligned with the code that consumes them.
