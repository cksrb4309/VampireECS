# VampireECS Agent Guide

## Scope

This repository is a Unity 6 (`6000.3.11f1`) DOTS/ECS vertical slice for a 3D Vampire Survivors-like combat loop.

The primary goal is not content volume. The primary goal is to keep the combat-growth loop stable while the ECS architecture grows:

- combat state stays in ECS
- input, UI, and presentation stay in bridge layers
- data and serialized assets stay consistent
- performance-sensitive paths remain Burst/job friendly

Read [README.md](C:\Users\rlack\Desktop\git\VampireECS\README.md) first, then use [CLAUDE.md](C:\Users\rlack\Desktop\git\VampireECS\CLAUDE.md) as architecture context.
Use [CodexPromptTemplates.md](C:\Users\rlack\Desktop\git\VampireECS\docs\CodexPromptTemplates.md) when writing task prompts for this repo.

## Repo Map

- `Assets/01_Scripts/ECS/**`
  - gameplay-critical ECS systems, components, system groups
- `Assets/01_Scripts/ECS/Visual/**`
  - ECS-side visual event components and presentation bridge systems
- `Assets/01_Scripts/Ability/**`
  - reward, unlock, and stat application logic
- `Assets/01_Scripts/UI/**`
  - ECS-to-UI bridge, binders, controllers
- `Assets/01_Scripts/Presentation/**`
  - VFX and view-side presentation managers
- `Assets/01_Scripts/System/**`
  - camera, pause, and game-level MonoBehaviour systems
- `Assets/09_Data/**`
  - ScriptableObject configs for stats and unlocks
- `Assets/06_Prefabs/**`
  - player, enemy, scene, and VFX prefabs
- `Assets/07_Scenes/Test_Combat.unity`
  - current main validation scene
- `Assets/99_Tests/**`
  - project EditMode tests and future Unity Test Runner coverage
- `Assets/Editor/CodexValidation/**`
  - editor menu entries and batch validation entry points
- `tools/**`
  - batch wrappers for compile, smoke, and test gates
- `Assets/10_ThirdParty/**`
  - vendor code, treat as read-only

## Default Write Boundaries

Safe by default:

- `Assets/01_Scripts/**`
- `Assets/99_Tests/**`
- `Assets/Editor/**` for tooling or validation only
- `docs/**`
- root markdown files
- `tools/**`

Explicit request required:

- `Assets/06_Prefabs/**`
- `Assets/07_Scenes/**`
- `Assets/08_UI/**`
- `Assets/09_Data/**`
- `Assets/00_Core/ProjectSetting/**`
- `ProjectSettings/**`
- `Packages/manifest.json`
- `Packages/packages-lock.json`

Read-only unless the task is specifically about that dependency:

- `Assets/10_ThirdParty/**`
- `Assets/Plugins/**`
- `Library/**`
- `Temp/**`
- `UserSettings/**`
- `Assets/_Recovery/**`
- `Assets/SceneDependencyCache/**`

Never hand-edit `.meta` files unless the task is explicitly about asset recovery.

## Existing Worktree Rule

This project often has active manual tuning in scenes, prefabs, VFX, and ScriptableObject assets.

- Do not revert unrelated user changes.
- If a task is code-only, stay in code-only paths.
- If serialized assets are already dirty and your task does not require them, leave them alone.
- Assume scene, prefab, and data asset edits can conflict with ongoing manual editor work.

## Default Request Interpretation

Do not require the user to restate the full harness template every time.

When the user gives a short request such as a bug report, symptom report, or feature ask, expand it into the harness internally.

Default behavior:

- infer the task goal from the user request
- read this file plus the relevant code before editing
- assume code-only scope first
- stay inside `Assets/01_Scripts/**`, `Assets/99_Tests/**`, `Assets/Editor/**`, `tools/**`, and docs unless the task clearly requires more
- do not change scenes, prefabs, ScriptableObject assets, input assets, package files, or project settings unless the user explicitly allows it or the task cannot be completed without them
- choose the smallest validation gate that matches the change
- report changed files, validation result, and any remaining manual checks

Ask for clarification only when one of these is true:

- the request is too ambiguous to define a correct completion condition
- the task likely requires serialized asset changes in guarded paths
- there are multiple plausible gameplay interpretations with materially different implementations
- validation cannot prove the intended behavior without a user decision

## Architecture Rules

### ECS Core

- Keep combat logic in `Assets/01_Scripts/ECS/**`.
- New ECS systems must declare their update group explicitly.
- Follow the current pipeline under `ECS/SystemGroups` instead of attaching systems loosely.
- Prefer event entities or buffers over direct cross-system coupling.
- Avoid managed allocations in hot `ISystem` or job paths.
- Preserve Burst/job friendliness for systems that run every frame.

### Bridge and Presentation

- `UI`, `Presentation`, and `System` layers may use `MonoBehaviour`, `UniTask`, and scene objects.
- ECS systems must not depend on UI objects, camera objects, or scene-only view logic.
- ECS-to-UI communication should stay in bridge classes such as `UI/Bridge/**`.
- `ECS/Visual/**` is the one allowed ECS-side presentation bridge. Keep it one-way: ECS emits visual event entities, managed presentation consumes them, and the event entities are destroyed immediately after use.
- View spawning or VFX object pooling belongs in `Presentation/**` or `UI/**`, not in ECS core systems.

### Data and Ability Flow

- New combat behavior that needs tuning should usually come with data changes in `Assets/09_Data/**`.
- Keep unlock/config assets aligned with the code path that consumes them.
- For attack abilities, keep base tuning in `Unlock...Config` and matching `...BaseStatsData`.
- Keep stackable upgrade tuning in `...StatsConfig` and matching runtime `...StatsData`.
- When using the current stat model, prefer `Base * (1 + local bonus) * CombatStats` for percentage-based growth, and use explicit additive bonus fields for count/radius/timer values that should not compound.
- Do not rename or move existing config assets casually. Asset references are more important than folder neatness.

### Code Style

- Match the current project style: no namespace declarations in runtime code unless the file already uses one.
- Keep files small and specific rather than building large manager classes.
- Prefer explicit names that mirror gameplay concepts already used in the repo: `DamageEvent`, `PlayerExpData`, `...Bridge`, `...System`, `...Config`.
- Runtime code must never reference `UnityEditor`.

## High-Risk Areas

Treat these as guarded surfaces:

- `Assets/07_Scenes/Test_Combat.unity`
- `Assets/06_Prefabs/Scene/CombatSetting.prefab`
- `Assets/06_Prefabs/Player/**`
- `Assets/06_Prefabs/Enemy/**`
- `Assets/09_Data/ScriptableObject/**`
- `Assets/00_Core/ProjectSetting/InputSystem_Actions.inputactions`
- `ProjectSettings/EditorBuildSettings.asset`

If a request touches one of these, name the exact asset in the task and validate immediately after the change.

## Validation Workflow

Run validation after every meaningful change.

Minimum gate for code-only work:

1. `tools\compile-unity.cmd`
2. `tools\smoke-unity.cmd`

When you add or change project tests:

1. `tools\test-editmode.cmd`
2. `tools\test-playmode.cmd` if the task touches scene-dependent presentation or gameplay flow

`tools\test-editmode.cmd` currently runs project-specific EditMode smoke scenarios through `BatchValidationRunner.RunProjectEditModeSmokeTests`.
Keep `Assets/99_Tests/**` for Unity test assembly coverage and direct system tests, but do not assume the batch wrapper runs every NUnit test automatically.

Default combined gate:

```powershell
tools\validate-unity.cmd
```

If the Unity Editor is already open, run the same checks from the editor menu instead of the batch scripts:

- `Tools/Codex Validation/Run Smoke Validation`
- `Tools/Codex Validation/Run Strict Smoke Validation`
- `Tools/Codex Validation/Run EditMode Smoke Tests`
- `Tools/Codex Validation/Run Full Validation`
- `Tools/Codex Validation/Open Validation Log Folder`

Optional strict gate that also requires `Test_Combat` to be enabled in build settings:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\unity-batch.ps1 -Task smoke -StrictBuildSettings
```

The batch tooling auto-resolves the Unity editor version from `ProjectSettings/ProjectVersion.txt`. If auto-discovery fails, pass `-UnityExe` or set `UNITY_EXE`.

## Smoke Validation Intent

The smoke validator is project-specific. It checks:

- `Assets/07_Scenes/Test_Combat.unity` exists
- `Assets/06_Prefabs/Scene/CombatSetting.prefab` exists
- core config assets exist
- `InputSystem_Actions.inputactions` exists
- the main scene and combat prefab have no missing MonoBehaviour references
- lack of project tests is reported as a warning

The current EditMode smoke gate additionally exercises `ApplyDamageSystem` for lethal damage handling and missing-target event consumption.

## Task Routing Hints

Use the smallest safe scope that can finish the task.

- Pure ECS bug fix or refactor:
  - stay in `Assets/01_Scripts/ECS/**`
- UI sync issue:
  - inspect `Assets/01_Scripts/UI/**` and the relevant observable data
- new reward or unlock behavior:
  - likely touches both `Assets/01_Scripts/Ability/**` and `Assets/09_Data/**`
- VFX presentation change:
  - likely touches `Assets/01_Scripts/ECS/Visual/**`, `Assets/01_Scripts/Presentation/VFX/**`, `Assets/04_Effects/**`, and one or more prefabs
- tooling or validation:
  - stay in `Assets/Editor/CodexValidation/**`, `Assets/99_Tests/**`, and `tools/**`

## Response Expectations

When reporting work:

- list the files changed
- state which validation commands ran
- call out any unvalidated risk, especially scene, prefab, VFX, or ScriptableObject changes

Do not claim a Unity task is complete without a validation result.
