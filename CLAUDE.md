# VampireECS Context

## Overview

- Genre: 3D vampire-survivor-like action prototype
- Engine: Unity 6 `6000.3.11f1`
- Primary architecture: DOTS/ECS for combat state and hot gameplay paths
- Secondary architecture: MonoBehaviour bridge/presentation layer for input, UI, camera, and runtime visuals

This project is not aiming for "everything in ECS".
The important split is:

- ECS owns combat state, damage flow, entity simulation, and progression requests
- bridge/presentation code owns input plumbing, UI, camera, and view-side rendering

Use [AGENTS.md](AGENTS.md) for write boundaries and validation rules.

## Main Runtime Loop

1. Input bridges update ECS-facing player input state.
2. ECS combat systems run movement, targeting, attack generation, and damage flow.
3. Death and experience systems emit progression events.
4. UI bridges pause time and present level-up choices.
5. ability/unlock code applies the chosen reward back into ECS data.
6. presentation systems consume ECS visual events and spawn or update view objects.

## Repo Map

```text
Assets/
├── 00_Core/                         project settings and input assets
├── 01_Scripts/
│   ├── Ability/                     unlocks, stat configs, reward application
│   ├── Core/                        managers, DI lifetime scope, shared utilities
│   ├── ECS/
│   │   ├── Attack/                  aura, shooter, chain lightning
│   │   ├── Combat/                  health, damage, death
│   │   ├── Common/                  spatial partitioning and helpers
│   │   ├── Enemy/                   spawning and follow logic
│   │   ├── Experience/              exp gain and level-up requests
│   │   ├── Player/                  move and rotation
│   │   ├── SystemGroups/            deterministic update ordering
│   │   └── Visual/                  ECS visual events and presentation bridges
│   ├── Presentation/                runtime VFX/view managers
│   ├── System/                      time pause, camera, scene systems
│   └── UI/                          binders, controllers, ECS bridges
├── 06_Prefabs/                      player, enemy, scene, and effect prefabs
├── 07_Scenes/                       main test scene
├── 09_Data/                         ScriptableObject config assets
├── 99_Tests/                        EditMode test assembly
└── Editor/CodexValidation/          editor/batch validation code
tools/                               compile and validation wrappers
```

## Combat Systems

### Attack

Key components:

- `AuraData`, `AuraStatsData`
- `ShooterData`, `ShooterStatsData`, `ShooterCanFireData`
- `ChainLightningData`, `ChainLightningStatsData`
- `ProjectileData`, `FactionData`, `CombatStatsData`

Key systems:

- `AuraSystem`
- `ProjectileSpawnSystem`
- `ProjectileMoveSystem`
- `ProjectileTriggerSystem`
- `AuraRenderSystem`
- `AuraCleanupSystem`
- `ChainLightningSystem`

### Combat / Death

- `ApplyDamageSystem`
  - consumes `DamageEventData`
  - reduces `HealthData`
  - adds `DeadTag`
  - emits `DamageTextEvent`
- `EnemyDeathSystem`
  - removes dead enemies
  - emits experience gain
- `PlayerDeathSystem`
  - handles player death flow

### Enemy / Player / Progression

- `EnemySpawnSystem`
- `EnemyFollowSystem`
- `EnemyTargetDirectionSystem`
- `PlayerMoveSystem`
- `PlayerRotationSystem`
- `ExperienceSystem`
- `ExperienceBridge`

## Visual Flow

Visual events are a one-way bridge from ECS to managed presentation.

Important ECS-side visual pieces:

- `DamageTextEvent`
- `ChainLightningVisualEvent`
- `DamageTextPresentationSystem`
- `ChainLightningPresentationSystem`
- `RenderTrailSystem`

Important managed presentation pieces:

- `DamageTextProvider`
- `DamageTextVfxBatchEmitter`
- `AuraViewManager`
- `ChainLightningViewManager`
- `LineChainLightningView`

Current intent:

- ECS decides when a visual should happen
- presentation code decides how to render it
- ECS core should not directly own pooled scene objects or VFX lifecycle

## Progression / Data Flow

Ability configs live under `Assets/01_Scripts/Ability/**`.
Serialized tuning data lives under `Assets/09_Data/**`.

Current unlock/tuning families:

- Aura
- Shooter
- Chain Lightning
- shared combat stats

Important rule:

- if behavior needs tuning, prefer adding or updating aligned config assets instead of hardcoding values in runtime systems

## System Group Ordering

Combat order matters.
The project uses explicit system groups under `Assets/01_Scripts/ECS/SystemGroups/**`.

The high-level flow is:

1. spatial preparation
2. spatial index build
3. attack setup and movement
4. physics trigger detection
5. damage application
6. destruction cleanup
7. presentation event consumption

Do not attach new high-frequency systems loosely when a matching update group already exists.

## Validation

Project-specific validation exists in two forms.

### Batch

- `tools\compile-unity.cmd`
- `tools\smoke-unity.cmd`
- `tools\test-editmode.cmd`
- `tools\validate-unity.cmd`

### Editor Menu

- `Tools/Codex Validation/Run Smoke Validation`
- `Tools/Codex Validation/Run Strict Smoke Validation`
- `Tools/Codex Validation/Run EditMode Smoke Tests`
- `Tools/Codex Validation/Run Full Validation`

Validation entry point:

- `Assets/Editor/CodexValidation/BatchValidationRunner.cs`

Current smoke coverage focuses on:

- `Test_Combat` scene presence
- `CombatSetting.prefab` presence
- core config/input assets
- missing scripts in the main scene and scene settings prefab
- `ApplyDamageSystem` smoke scenarios

## Guarded Assets

These assets are high-risk because they are frequently tuned in the editor and easy to break:

- `Assets/07_Scenes/Test_Combat.unity`
- `Assets/06_Prefabs/Scene/CombatSetting.prefab`
- `Assets/09_Data/ScriptableObject/**`
- `Assets/00_Core/ProjectSetting/InputSystem_Actions.inputactions`
- `ProjectSettings/EditorBuildSettings.asset`

Assume scene, prefab, VFX, and ScriptableObject edits may conflict with ongoing manual work unless the task explicitly allows them.

## Practical Guidance

- Start in `Assets/01_Scripts/ECS/**` for gameplay bugs.
- Check `Assets/01_Scripts/ECS/Visual/**` and `Assets/01_Scripts/Presentation/VFX/**` for visual mismatches.
- Check `Assets/01_Scripts/UI/Bridge/**` when gameplay state and UI diverge.
- Check both `Ability/**` and `09_Data/**` when an unlock or stat change is involved.
- Prefer the smallest edit surface that can solve the problem.
