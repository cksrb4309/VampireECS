# VampireECS

Unity 6 (6000.3.11f1) DOTS/ECS vertical slice for a 3D vampire-survivor-like combat loop.

The goal is stability over content volume:
- combat state stays in ECS
- input, UI, and presentation stay in bridge layers
- serialized data stays consistent with runtime code
- performance-sensitive paths stay Burst/job friendly

## Current Runtime Shape

- Main validation scene: `Assets/07_Scenes/Test_Combat.unity`
- Current combat prefab: `Assets/06_Prefabs/Scene/CombatSetting.prefab`
- Combat families: Shooter, Aura, Boomerang, Orbit, Chain Lightning, Meteor Strike, Black Hole

## Core Loop

1. Player input bridges update ECS-facing state.
2. ECS combat systems run movement, attack generation, damage, death, and experience.
3. Level-up systems pause time and open reward selection.
4. Unlock/config code applies the selected reward back into ECS data.
5. Presentation systems consume ECS visual events and spawn or update views.

## Harness Files

- [AGENTS.md](AGENTS.md)
- [CLAUDE.md](CLAUDE.md)
- [docs/AgentPromptTemplates.md](docs/AgentPromptTemplates.md)
- [docs/CodexPromptTemplates.md](docs/CodexPromptTemplates.md)
- [docs/project-context.md](docs/project-context.md)
- [docs/Obsidian.md](docs/Obsidian.md)
- [docs/RTK.md](docs/RTK.md)
- [docs/SubAgents.md](docs/SubAgents.md)
- [docs/Graphify.md](docs/Graphify.md)
- [docs/AgentHandoffs/README.md](docs/AgentHandoffs/README.md)
- [docs/HarnessMistakes/README.md](docs/HarnessMistakes/README.md)

## Validation

- `tools\compile-unity.cmd`
- `tools\smoke-unity.cmd`
- `tools\test-editmode.cmd`
- `tools\validate-unity.cmd`

If the Unity Editor is already open, use:

- `Tools/Codex Validation/Run Smoke Validation`
- `Tools/Codex Validation/Run Strict Smoke Validation`
- `Tools/Codex Validation/Run EditMode Smoke Tests`
- `Tools/Codex Validation/Run Full Validation`

## Guarded Assets

Treat these as high-risk edit surfaces:

- `Assets/07_Scenes/Test_Combat.unity`
- `Assets/06_Prefabs/Scene/CombatSetting.prefab`
- `Assets/06_Prefabs/Player/**`
- `Assets/06_Prefabs/Enemy/**`
- `Assets/09_Data/ScriptableObject/**`
- `Assets/00_Core/ProjectSetting/InputSystem_Actions.inputactions`
- `ProjectSettings/EditorBuildSettings.asset`

## Notes

- Use `docs/project-context.md` for the current runtime/system map.
- Use `docs/ProjectWiki/index.md` as the root wiki hub if you keep Obsidian-style notes.
- Keep project-local `.claude/` and `.mcp.json` files in sync with the active harness when the kit changes.
- `unity-agent-kit/` is the installed kit mirror; use its git HEAD when you need the current kit version.
- `docs/AgentHandoffs/**` is the durable bridge between Codex and Claude Code.
- `docs/RTK.md`, `docs/SubAgents.md`, and `docs/Graphify.md` describe optional companion workflows.
