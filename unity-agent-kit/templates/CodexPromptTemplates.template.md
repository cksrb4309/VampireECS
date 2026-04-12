# Codex Prompt Templates For {{PROJECT_NAME}}

이 파일은 레거시 호환용 템플릿입니다.

새 프로젝트에는 가능하면 `docs/AgentPromptTemplates.md`를 사용하고,
이미 `docs/CodexPromptTemplates.md`를 쓰는 프로젝트에서만 이 이름을 유지하세요.

운영 중 실수가 확정되면 `docs/HarnessMistakes/README.md`를 기준으로 범주를 고르고 해당 category 파일을 갱신하세요.

## Why This Exists

In this project, the prompt is not the harness by itself.

The harness works when these pieces are combined:

- your task request
- `AGENTS.md`
- automated validation
- task-specific tests or acceptance checks

You do not need to write the full template for every request.

If you say something short like:

```text
이 기능이 가끔 안 돼.
```

Codex should still expand that into:

- task goal
- likely safe edit scope
- completion condition
- matching validation path

Use the full template only when you want tighter control.

If you ask to inspect Claude Code work, Codex should:

- check only `docs/AgentHandoffs/pending/claude-to-codex/` first
- read only unread notes that match the current task
- move understood notes to `docs/AgentHandoffs/consumed/claude-to-codex/`
- avoid rereading `consumed/**` unless you explicitly ask

## Base Template

```text
작업 목표:
- [원하는 기능/버그 수정 한 줄]

배경:
- [현재 증상 또는 목적]
- [관련 시스템/파일 경로]

수정 허용 범위:
- [예: runtime code folders]
- [필요 시 tests]

수정 금지 범위:
- [예: scenes/prefabs/data assets/project settings]

완료 조건:
- [조건 1]
- [조건 2]
- [조건 3]

순서:
- 관련 파일 전부 읽고 분석한 다음 수정해.

검증:
- AGENTS.md 기준으로 검증해줘.
- Unity 에디터가 닫혀 있으면 batch 검증을 실행해줘.
- Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 알려줘.
- 이번 변경에 맞는 테스트가 필요하면 최소 범위로 추가해줘.

보고 방식:
- 변경 파일
- 검증 결과
- 남은 리스크나 수동 확인 항목
```

## Template 1: Code-Only Task

Use this when the task should stay in runtime code and tests.

## Template 2: Code Plus Data Task

Use this when ScriptableObject or config asset edits are part of the request.

## Template 3: Scene Or Prefab Task

Use this only when you intentionally allow guarded serialized assets.

## Template 4: Investigation First

Use this when you want analysis only and no code changes yet.

## Template 5: Claude Code Handoff Check

```text
Claude Code 작업 내역 확인해줘.

`docs/AgentHandoffs/pending/claude-to-codex/`만 먼저 확인하고,
unread handoff note가 있으면 관련된 것만 읽어줘.
읽은 note는 `docs/AgentHandoffs/consumed/claude-to-codex/`로 이동해줘.
없으면 handoff가 없다고만 알려주고 다른 handoff 파일은 읽지 마.
```

## One-Line Shortcut

```text
[기능/버그]를 수정해줘. 관련 파일 전부 읽고 분석한 다음 수정해. AGENTS.md 규칙을 따르고, 수정은 [허용 범위] 안에서만 해줘. 완료 조건은 [조건]이고, 검증은 가능한 자동으로 수행해줘. Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 알려줘.
```
