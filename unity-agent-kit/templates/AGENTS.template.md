# {{PROJECT_NAME}} Agent Guide

<!-- Shared-project dual-agent 기본값:
     Codex와 Claude Code를 함께 쓰는 프로젝트에서는 이 자기완결형 AGENTS를 기본으로 사용한다. -->

## Scope

This repository is a Unity project on version `{{UNITY_VERSION}}`.

Replace this section with a short, project-specific description that explains:

- what the project is
- what the current engineering focus is
- which gameplay or tooling loop must stay stable while the codebase changes

Read:

- `README.md`
- `CLAUDE.md`
- `docs/AgentPromptTemplates.md`

before making non-trivial edits.

If `docs/Obsidian.md` exists, treat the repository root as the documentation vault and keep harness docs outside `Assets/`.
If repo-root `graphify-out/GRAPH_REPORT.md` exists, read it before broad architecture questions or wide repository exploration.
Use `docs/Graphify.md` to decide whether the graph should be refreshed first.
If the task will produce long shell output, read `docs/RTK.md` before wide Bash-based exploration or log-heavy validation.
If the session supports delegation and the user has allowed it, read `docs/SubAgents.md` before splitting bounded side tasks.

## Image Analysis Pipeline

이미지 분석 작업(UI 검증, 색상 확인, 비주얼 버그 등)에는 로컬 MCP 파이프라인을 사용한다.

- `ollama-vision` → 장면 설명, UI 구조, 텍스트 추출 (먼저 사용)
- `image-tools` → 픽셀 색상, 지배색, 거리 측정 (정밀 수치 필요 시만)

이 MCP는 전역(`~/.codex/config.toml`)에 등록한다. 프로젝트별 설정 불필요.
설정 방법 및 운영 규칙 전문: `.claude/image-analysis.md`

Ollama가 꺼져 있으면 파이프라인이 응답하지 않는다. 작업 전 `describe_image` 호출로 가용성을 확인한다.

## Repo Map

Document the real project structure here.

Suggested sections:

- main runtime code folders
- ability/data/config folders
- presentation/UI folders
- scenes/prefabs
- test folders
- third-party folders

## Tool Use Pattern

작업 시작 전 관련 파일을 전부 먼저 읽는다.

- **읽기 턴**: Read, Grep, Glob을 한 번에 병렬로 실행한다.
- **분석**: 읽은 내용을 바탕으로 수정 범위와 계획을 확정한다.
- **쓰기 턴**: Edit, Write, Bash를 순차 실행한다.

읽기와 쓰기를 여러 턴에 걸쳐 섞지 않는다.
파일 하나 읽고 수정하고를 반복하면 API 왕복이 선형으로 늘어난다.

## Graph Orientation

If the project uses `graphify`:

- use repo-root `graphify-out/GRAPH_REPORT.md` for high-level orientation
- use raw source files for implementation decisions
- treat `docs/Graphify.md` as the refresh manual
- do not run `graphify codex install` or `graphify claude install` by default in a harness-managed project

## Shell Output Compression

If `docs/RTK.md` exists:

- use it as the operating manual for shell-output compression
- prefer RTK paths for shell-heavy `git`, `rg`/`grep`, test, and batch-validation logs
- do not assume RTK is installed without checking or user confirmation
- do not treat RTK as a replacement for raw source reading, `unity-cli`, or `unity-prefab-parser-mcp`

## Default Write Boundaries

Safe by default:

- runtime code folders
- test folders
- editor tooling folders
- docs
- `tools/**`

Explicit request required:

- scenes
- prefabs
- ScriptableObject data
- input assets
- `ProjectSettings/**`
- package files

Read-only unless the task is specifically about that dependency:

- third-party code
- `Library/**`
- `Temp/**`
- user settings
- recovery/cache folders

Never hand-edit `.meta` files unless the task is explicitly about asset recovery.

## Existing Worktree Rule

This project may have active user changes in scenes, prefabs, VFX, or data assets.

- Do not revert unrelated user changes.
- If serialized assets are already dirty and your task does not require them, leave them alone.
- Assume serialized asset edits can conflict with ongoing manual editor work.

## Default Request Interpretation

Do not require the user to restate a full harness template every time.

When the user gives a short request:

- infer the task goal
- read this file and the relevant code before editing
- assume code-only scope first
- pick the smallest matching validation gate
- report changed files, validation result, and remaining manual checks

Ask for clarification only when:

- the task is materially ambiguous
- guarded serialized assets likely need edits
- multiple gameplay interpretations imply different implementations
- validation cannot prove the intended result without a user decision

Before non-trivial work:

- classify one primary domain using `docs/HarnessMistakes/domains/README.md`
- read the matching domain file first
- only read extra mistake category files if that domain file says `Preload extra mistake context: yes`

## Cross-Agent Handoffs

Use `docs/AgentHandoffs/**` as a narrow bridge between Codex and Claude Code.

When the user asks to check Claude Code work:

- inspect `docs/AgentHandoffs/pending/claude-to-codex/` first
- if only placeholder files exist, do not read more handoff context
- read only the relevant pending note(s), newest first
- once understood, move them to `docs/AgentHandoffs/consumed/claude-to-codex/`
- do not reread `consumed/**` notes unless the user explicitly asks

When writing or updating long-lived docs:

- keep them in the root-level docs tree, not under `Assets/`
- prefer stable note names
- add `[[wiki links]]` only when they improve navigation
- keep `docs/ProjectWiki/index.md` as the main hub note

When handing work to Claude Code:

- write a compact note to `docs/AgentHandoffs/pending/codex-to-claude/`
- keep it short: task, state, changed files, validation, next action
- prefer updating the newest unread note on the same topic over creating duplicates

## Sub-Agent Delegation

If `docs/SubAgents.md` exists and delegation is allowed in the current session:

- keep blocking design decisions and final validation on the main agent
- use sub-agents for read-only exploration, log/test triage, document drafting, or disjoint write-scope work
- assign explicit ownership before delegating any code-writing task
- do not let multiple agents edit the same file or the same guarded serialized asset surface
- do not use `docs/AgentHandoffs/**` as storage for internal sub-agent results

## Architecture Rules

Write project-specific rules here.

Typical sections:

- ECS core or gameplay core
- bridge / presentation split
- data / ability flow
- code style and naming
- performance-sensitive rules

## High-Risk Areas

List exact assets and folders that are easy to break and should be treated as guarded surfaces.

Examples:

- main combat scene
- scene setup prefab
- input action asset
- ScriptableObject config folders
- `ProjectSettings/EditorBuildSettings.asset`

## Validation Workflow

Document the project-specific minimum gate here.

Typical structure:

1. `tools\compile-unity.cmd`
2. `tools\smoke-unity.cmd`
3. `tools\test-editmode.cmd` when tests exist
4. `tools\test-playmode.cmd` when scene-dependent behavior is touched

Also document the editor menu path for validation when Unity is already open.

## Mistake Learning Loop

If a harness mistake or repeatable agent mistake is confirmed:

- keep the current primary domain in mind
- classify one primary category using `docs/HarnessMistakes/README.md`
- update the matching `docs/HarnessMistakes/categories/*.md` file
- update the matching `docs/HarnessMistakes/domains/*.md` file
- if the lesson changes normal operating rules, mirror it into this file, `CLAUDE.md`, prompt templates, validation tools, or tests
- report which domain/category mistake files were updated

## Smoke Validation Intent

State exactly what the smoke validator checks in this project.

## Task Routing Hints

Give a few project-specific examples such as:

- pure gameplay bug fix
- UI sync issue
- unlock/config change
- VFX or presentation change
- tooling or validation task

## Response Expectations

When reporting work:

- list the files changed
- state which validation commands ran
- call out unvalidated risk, especially on scenes, prefabs, VFX, or ScriptableObject assets

Do not claim a Unity task is complete without a validation result.
