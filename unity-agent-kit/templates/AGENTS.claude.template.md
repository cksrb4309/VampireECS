# {{PROJECT_NAME}} Agent Guide — Claude Code

<!-- 선택형 템플릿:
     공유 프로젝트 기본값은 AGENTS.template.md의 자기완결형 AGENTS다.
     이 파일은 사용자가 Claude Code 전용 경량 AGENTS를 명시적으로 원할 때만 사용한다. -->

<!-- CLAUDE.md가 세션 시작 시 자동 로드된다.
     Repo Map, Validation, Guarded Assets는 CLAUDE.md에 있으므로 여기서 반복하지 않는다.
     Architecture Rules, Visual/Data Flow는 docs/project-context.md에 있다. -->

## Scope

This repository is a Unity project on version `{{UNITY_VERSION}}`.

Replace this section with a short, project-specific description:

- what the project is
- what the current engineering focus is
- which gameplay or tooling loop must stay stable while the codebase changes

Read `docs/AgentPromptTemplates.md` before non-trivial edits.

If `docs/Obsidian.md` exists, treat the repository root as the documentation vault and keep harness docs outside `Assets/`.
If `graphify-out/GRAPH_REPORT.md` exists, read it before broad architecture questions or wide repository exploration.
Use `docs/Graphify.md` to decide whether the graph should be refreshed first.
If the task will produce long shell output, read `docs/RTK.md` before wide Bash-based exploration or log-heavy validation.
If the session supports delegation and the user has allowed it, read `docs/SubAgents.md` before splitting bounded side tasks.

## Image Analysis Pipeline

이미지 분석 작업(UI 검증, 색상 확인, 비주얼 버그 등)에는 로컬 MCP 파이프라인을 사용한다.

- `ollama-vision` → 장면 설명, UI 구조, 텍스트 추출 (먼저 사용)
- `image-tools` → 픽셀 색상, 지배색, 거리 측정 (정밀 수치 필요 시만)

이 MCP는 전역(`~/.claude.json`)에 등록되어 있다. 프로젝트별 `.mcp.json` 수정 불필요.
운영 규칙 전문: `.claude/image-analysis.md`

Ollama가 꺼져 있으면 파이프라인이 응답하지 않는다. 작업 전 `describe_image` 호출로 가용성을 확인한다.

## Context Management

대화가 아래 조건 중 하나라도 해당되면 `/compact`를 제안한다.

- 작업 단위가 3개 이상 전환됐을 때
- 파일 수정이 10개 이상 쌓였을 때
- 컨텍스트 압축이 이미 한 번 발생했을 때

제안 시 실행할 명령어 전체를 코드 블록으로 제공한다.

```
/compact [완료 작업 목록] | [현재 상태 — git 커밋 등] | [미완료 항목]
```

## Tool Use Pattern

작업 시작 전 관련 파일을 전부 먼저 읽는다.

- **읽기 턴**: Read, Grep, Glob을 한 번에 병렬로 실행한다.
- **분석**: 읽은 내용을 바탕으로 수정 범위와 계획을 확정한다.
- **쓰기 턴**: Edit, Write, Bash를 순차 실행한다.

읽기와 쓰기를 여러 턴에 걸쳐 섞지 않는다.
파일 하나 읽고 수정하고를 반복하면 API 왕복이 선형으로 늘어난다.

## Graph Orientation

If the project uses `graphify`:

- use `graphify-out/GRAPH_REPORT.md` for high-level orientation
- use raw source files for implementation decisions
- treat `docs/Graphify.md` as the refresh manual
- do not run `graphify codex install` or `graphify claude install` by default in a harness-managed project

## Shell Output Compression

If `docs/RTK.md` exists:

- use it as the operating manual for shell-output compression
- prefer RTK paths for shell-heavy `git`, `rg`/`grep`, test, and batch-validation logs
- do not assume RTK is installed without checking or user confirmation
- do not treat RTK as a replacement for raw source reading, `Read/Grep/Glob`, `unity-cli`, or `unity-prefab-parser-mcp`

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

Use `docs/AgentHandoffs/**` as a narrow bridge between Claude Code and Codex.

When the user asks to check Codex work:

- inspect `docs/AgentHandoffs/pending/codex-to-claude/` first
- if only placeholder files exist, do not read more handoff context
- read only the relevant pending note(s), newest first
- once understood, move them to `docs/AgentHandoffs/consumed/codex-to-claude/`
- do not reread `consumed/**` notes unless the user explicitly asks

When writing or updating long-lived docs:

- keep them in the root-level docs tree, not under `Assets/`
- prefer stable note names
- add `[[wiki links]]` only when they improve navigation
- keep `docs/ProjectWiki/index.md` as the main hub note

When handing work to Codex:

- write a compact note to `docs/AgentHandoffs/pending/claude-to-codex/`
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

→ `docs/project-context.md` (Architecture Rules 섹션) 참조

## Task Routing Hints

Write project-specific routing examples:

- pure code change → compile + smoke
- UI/state sync → smoke + manual check
- data/config change → validate ScriptableObject references
- scene/prefab change → unity-cli + smoke

## Mistake Learning Loop

If a harness mistake or repeatable agent mistake is confirmed:

- keep the current primary domain in mind
- classify one primary category using `docs/HarnessMistakes/README.md`
- update the matching `docs/HarnessMistakes/categories/*.md` file
- update the matching `docs/HarnessMistakes/domains/*.md` file
- if the lesson changes normal operating rules, mirror it into this file, `CLAUDE.md`, prompt templates, validation tools, or tests
- report which domain/category mistake files were updated

## Response Expectations

When reporting work:

- list the files changed
- state which validation commands ran
- call out unvalidated risk, especially on scenes, prefabs, VFX, or ScriptableObject assets

Do not claim a Unity task is complete without a validation result.
