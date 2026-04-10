# VampireECS Context

## Overview

- Genre: 3D vampire-survivor-like action prototype
- Engine: Unity 6 `6000.3.11f1`
- Primary architecture: DOTS/ECS for combat state and hot gameplay paths
- Secondary architecture: MonoBehaviour bridge and presentation layers for input, UI, camera, and runtime visuals

The project is not aiming for everything in ECS. The important split is:

- ECS owns combat state, damage flow, entity simulation, and progression requests
- bridge and presentation code owns input plumbing, UI, camera, and view-side rendering

Use `AGENTS.md` for write boundaries and validation rules.
Use `docs/project-context.md` for the current system map.
Use `docs/AgentHandoffs/**` when Codex and Claude Code need a durable handoff.

## Main Runtime Loop

1. Input bridges update ECS-facing player input state.
2. ECS combat systems run movement, targeting, attack generation, and damage flow.
3. Death and experience systems emit progression events.
4. UI bridges pause time and present level-up choices.
5. Ability and unlock code apply the chosen reward back into ECS data.
6. Presentation systems consume ECS visual events and spawn or update view objects.

## Repo Map

- `Assets/01_Scripts/ECS/**`
  - combat logic, entity simulation, system groups
- `Assets/01_Scripts/ECS/Visual/**`
  - ECS-side visual event components and managed presentation bridge systems
- `Assets/01_Scripts/Ability/**`
  - unlock, reward, and stat application logic
- `Assets/01_Scripts/UI/**`
  - ECS-to-UI bridge, binders, controllers
- `Assets/01_Scripts/Presentation/**`
  - runtime VFX and view managers
- `Assets/01_Scripts/System/**`
  - camera, pause, and game-level MonoBehaviour systems
- `Assets/06_Prefabs/**`
  - player, enemy, scene, and VFX prefabs
- `Assets/07_Scenes/Test_Combat.unity`
  - current main validation scene
- `Assets/09_Data/**`
  - ScriptableObject configs for stats and unlocks
- `Assets/99_Tests/**`
  - EditMode test assembly and future Unity Test Runner coverage
- `Assets/Editor/CodexValidation/**`
  - editor menu entries and batch validation entry points
- `tools/**`
  - compile, smoke, and test wrappers

## Combat Systems

### Attack

Representative attack families:

- `Shooter`
- `Aura`
- `Boomerang`
- `Orbit`
- `Chain Lightning`
- `Meteor Strike`
- `Black Hole`

Current stat model:

- `Unlock...Config`
  - unlock behavior and base tuning values
- `...BaseStatsData`
  - baseline runtime performance
- `...StatsConfig`
  - stackable upgrade values
- `...StatsData`
  - accumulated upgrade bonuses on the player
- `CombatStatsData`
  - shared combat-wide multipliers

Typical final-value shape:

```text
final = base * (1 + local bonus rate) * combat multiplier
```

Use additive bonus fields for count, radius, and timer values that should not compound.

### Combat / Death / Progression

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
- `ExperienceSystem`
  - raises level-up requests
- `ExperienceBridge`
  - pauses time and opens the reward UI

### Enemy / Player

- `EnemySpawnSystem`
- `EnemyMoveSystem`
- `EnemyDirectionSystem`
- `EnemyTeleportSystem`
- `PlayerMoveSystem`
- `PlayerRotationSystem`

## Visual Flow

Visual events are a one-way bridge from ECS to managed presentation.

Important ECS-side visual pieces:

- `DamageTextEvent`
- `ChainLightningVisualEvent`
- `MeteorStrikeTelegraphVisualEvent`
- `MeteorStrikeImpactVisualEvent`
- `BlackHoleVisualID`
- `DamageTextPresentationSystem`
- `ChainLightningPresentationSystem`
- `MeteorStrikePresentationSystem`
- `BlackHolePresentationSystem`
- `RenderTrailSystem`

Important managed presentation pieces:

- `DamageTextProvider`
- `DamageTextVfxBatchEmitter`
- `AuraViewManager`
- `ChainLightningViewManager`
- `MeteorStrikeViewManager`
- `BlackHoleViewManager`
- `LineChainLightningView`
- `MeteorStrikeTelegraphView`
- `MeteorStrikeImpactView`
- `LineBlackHoleView`

Current intent:

- ECS decides when a visual should happen
- presentation code decides how to render it
- ECS core does not own pooled scene objects or VFX lifecycle

## System Group Ordering

Combat order matters. The project uses explicit system groups under `Assets/01_Scripts/ECS/SystemGroups/**`.

High-level flow:

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

- `tools\\compile-unity.cmd`
- `tools\\smoke-unity.cmd`
- `tools\\test-editmode.cmd`
- `tools\\validate-unity.cmd`

### Editor Menu

- `Tools/Codex Validation/Run Smoke Validation`
- `Tools/Codex Validation/Run Strict Smoke Validation`
- `Tools/Codex Validation/Run EditMode Smoke Tests`
- `Tools/Codex Validation/Run Full Validation`

Current smoke coverage focuses on:

- `Test_Combat` scene presence
- `CombatSetting.prefab` presence
- core config/input assets
- missing scripts in the main scene and scene settings prefab
- `ApplyDamageSystem` smoke scenarios

## Guarded Assets

Treat these assets as guarded surfaces:

- `Assets/07_Scenes/Test_Combat.unity`
- `Assets/06_Prefabs/Scene/CombatSetting.prefab`
- `Assets/09_Data/ScriptableObject/**`
- `Assets/00_Core/ProjectSetting/InputSystem_Actions.inputactions`
- `ProjectSettings/EditorBuildSettings.asset`

## Claude / Codex Tooling

- `harness-version` records the installed kit version.
- `.mcp.json` wires project-local MCP servers.
- `.claude/settings.json` defines project-local allow/deny rules.
- `docs/Obsidian.md`, `docs/RTK.md`, `docs/SubAgents.md`, and `docs/Graphify.md` describe optional companion workflows.

## Practical Guidance

- Start in `Assets/01_Scripts/ECS/**` for gameplay bugs.
- Check `Assets/01_Scripts/ECS/Visual/**` and `Assets/01_Scripts/Presentation/VFX/**` for visual mismatches.
- Check `Assets/01_Scripts/UI/Bridge/**` when gameplay state and UI diverge.
- Check both `Ability/**` and `09_Data/**` when an unlock or stat change is involved.
- If attack tuning looks inconsistent, verify both the unlock asset base values and the stats asset bonus values.
- Prefer the smallest edit surface that can solve the problem.
