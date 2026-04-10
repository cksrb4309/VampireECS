# Agent Prompt Templates For {{PROJECT_NAME}}

## Why This Exists

In this project, the prompt is not the harness by itself.

The harness works when these pieces are combined:

- your task request
- `AGENTS.md`
- automated validation
- task-specific tests or acceptance checks

This file can be used by both Codex and Claude Code.

You do not need to write the full template for every request.

If you say something short like:

```text
이 기능이 가끔 안 돼.
```

the agent should still expand that into:

- task goal
- likely safe edit scope
- completion condition
- matching validation path

Use the full template only when you want tighter control.

If a confirmed mistake happens during operation, the agent should:

- keep the current primary domain
- classify it using `docs/HarnessMistakes/README.md`
- update the matching domain file
- update the matching category file

If you ask to inspect the other agent's work, the agent should:

- check only the matching `docs/AgentHandoffs/pending/**` folder first
- avoid reading consumed handoff files by default
- move any understood pending note into `docs/AgentHandoffs/consumed/**`

If the task is expected to produce long shell output, the agent should:

- check `docs/RTK.md` first
- prefer RTK-compressed shell workflows where available
- avoid treating RTK as a replacement for raw file reads or structured search tools

If the project uses Obsidian-friendly docs, the agent should:

- keep documentation notes in the project root docs tree, not under `Assets/`
- use `docs/ProjectWiki/index.md` as the main hub note
- add `[[wiki links]]` only where they materially improve navigation

If delegation is allowed in the current session, the agent should:

- check `docs/SubAgents.md` before spawning any sub-agent
- keep the main agent on the critical path
- delegate only bounded side tasks with clear ownership
- avoid delegating overlapping write scopes or guarded serialized asset edits

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

Use this template when the task is unclear and you want the agent to gather context before acting.

```text
작업 목표:
- [알고 싶은 것 또는 풀고 싶은 문제 한 줄]

역할:
- 아직 코드를 수정하지 마.
- 작업을 시작하기 전에, 최선의 결과를 내기 위해 필요한 컨텍스트를 파악할 수 있게
  질문을 3~4개 해줘. 한 번에 하나씩 물어봐. 내 답변을 받은 뒤 다음 질문으로 넘어가.
- 질문이 끝나면 파악한 내용을 요약하고 작업 계획을 제안해줘.
- 계획을 내가 승인하면 그 때 실행에 들어가.
```

이 방식을 **Reverse Prompting**이라고 부른다.
일반적인 "푸시 프롬프팅"(내가 정보를 넣어주는 방식) 대신,
에이전트가 목표를 파악한 뒤 스스로 필요한 컨텍스트를 수집한다.

짧은 요청에도 이 방식을 쓰면 잘못된 가정으로 시작하는 것을 막을 수 있다.

```text
예시:

전투 시스템이 이상하게 동작해. 고치기 전에 질문 3~4개 해줘.
한 번에 하나씩 물어봐.
```

## Template 5: Harness Update

하네스 킷을 최신 버전으로 업데이트할 때 사용한다.

```text
하네스 킷을 업데이트해줘.
킷 경로: [unity-agent-kit 폴더 경로]
```

에이전트는 `FOR_CLAUDE_CODE.md`의 "하네스 업데이트 워크플로"를 따른다.
변경 계획을 먼저 제시하고 승인받은 뒤 실행한다.

## Template 6: Cross-Agent Handoff Check

다른 에이전트가 남긴 unread 작업 내역만 확인하고 싶을 때 사용한다.

```text
Codex 작업 내역 확인해줘.

`docs/AgentHandoffs/pending/codex-to-claude/`만 먼저 확인하고,
unread handoff note가 있으면 관련된 것만 읽어줘.
이해한 note는 `docs/AgentHandoffs/consumed/codex-to-claude/`로 이동해줘.
없으면 handoff note가 없다고만 알려주고 다른 handoff 파일은 읽지 마.
```

```text
Claude Code 작업 내역 확인해줘.

`docs/AgentHandoffs/pending/claude-to-codex/`만 먼저 확인하고,
unread handoff note가 있으면 관련된 것만 읽어줘.
이해한 note는 `docs/AgentHandoffs/consumed/claude-to-codex/`로 이동해줘.
없으면 handoff note가 없다고만 알려주고 다른 handoff 파일은 읽지 마.
```

## One-Line Shortcut

```text
[기능/버그]를 수정해줘. 관련 파일 전부 읽고 분석한 다음 수정해. AGENTS.md 규칙을 따르고, 수정은 [허용 범위] 안에서만 해줘. 완료 조건은 [조건]이고, 검증은 가능한 자동으로 수행해줘. Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 알려줘.
```

## Template 7: Delegation Allowed

Use this when you want the agent to split bounded side tasks in parallel.

```text
이 작업은 delegation을 허용할게.

단, 메인 에이전트가 critical path와 최종 검증을 직접 맡고,
sub-agent는 read-only 조사, 테스트/로그 확인, 문서 정리, 명확히 분리된 코드 범위에만 써줘.
같은 파일을 여러 에이전트가 동시에 수정하지 말고, guarded asset은 sub-agent에게 맡기지 마.
```
