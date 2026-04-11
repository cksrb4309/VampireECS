# Harness Install Spec

이 문서는 Unity 프로젝트에 하네스를 적용할 때 Codex와 Claude Code가 공통으로 따라야 하는 설치 규격이다.

## 목표

현재 Unity 프로젝트를 조사해서, 해당 프로젝트에 맞는 하네스를 설치하거나 갱신한다.

최소 산출물은 다음과 같다.

- 프로젝트 맞춤형 `AGENTS.md`
- 프로젝트 맞춤형 `CLAUDE.md`
- `docs/AgentPromptTemplates.md`
  - 기존 프로젝트가 `docs/CodexPromptTemplates.md`를 이미 쓰고 있다면 그 파일을 유지하거나 병행 갱신해도 된다
- `docs/Obsidian.md`
  - Obsidian vault 친화 문서 운영 규칙
- `docs/RTK.md`
  - shell output 압축 운영 문서
- `docs/SubAgents.md`
  - 서브 에이전트 / 병렬 위임 운영 문서
- `docs/HarnessMistakes/**`
  - 실수 범주 파일과 학습 루프 문서
- `docs/AgentHandoffs/**`
  - Codex ↔ Claude Code handoff note와 pending/consumed 이동 구조
- optional `docs/Graphify.md`
  - 구조 이해용 knowledge graph 운영 문서
- `.claude/settings.json`
- `.claude/hooks/guard-assets-check.ps1`
- `.mcp.json`
- `.claude/unity-cli.md`
- 가능하면 `.claude/image-analysis.md`
- 가능하면 `.claude/claude-desktop-config.md`
- `tools/` 검증 스크립트
- `Assets/Editor/HarnessValidation/BatchValidationRunner.cs`
  - 기존 프로젝트가 `Assets/Editor/CodexValidation/BatchValidationRunner.cs`를 이미 쓰고 있다면 기존 경로를 유지하거나 점진적으로 옮겨도 된다
- 가능하면 최소 1개의 프로젝트 맞춤형 EditMode smoke 테스트

기존 문서가 이미 있다면 무조건 덮어쓰지 말고, 현재 상태와 어긋난 부분만 갱신한다.

## 기본 절차

1. 이 파일과 `templates/**`를 읽는다.
2. 프로젝트 구조를 스캔한다.
3. 기존 `README.md`, `AGENTS.md`, `CLAUDE.md`, `docs/**`, `tools/**`, `Assets/Editor/**`, `Assets/99_Tests/**`를 확인한다.
4. 프로젝트 구조에 맞게 문서와 검증 도구를 설치하거나 갱신한다.
5. 실수 학습 루프 문서를 설치하거나 기존 구조와 맞춘다.
6. 가능한 검증을 수행한다.
7. 변경 파일, 검증 결과, 잔여 리스크를 보고한다.

## 설치 위치 규칙

- `unity-agent-kit`와 하네스 산출물은 Unity 프로젝트 **루트**에 둔다.
- `Assets/` 아래에는 두지 않는다.
- 이유: `Assets/` 아래의 문서 폴더와 markdown 파일에도 Unity `.meta` 파일이 생기기 때문이다.
- 문서 vault는 루트의 `docs/**`, `AGENTS.md`, `CLAUDE.md` 계층으로 운영한다.

## 스캔 기준

다음을 우선 확인한다.

- `ProjectSettings/ProjectVersion.txt`
- `Assets/` 하위 주요 폴더
- `Assets/**/*.unity`
- `Assets/**/*.prefab`
- `Assets/**/Config*.asset`, `Assets/**/ScriptableObject/**`
- `Assets/01_Scripts/**` 또는 동등한 코드 폴더
- 기존 테스트 폴더
- 기존 에디터 툴링
- 기존 문서와 설계 메모

필요하면 `scripts/scan-unity-project.ps1`를 보조적으로 사용한다.

## 기본 판단 규칙

- 처음에는 code-only scope를 기본값으로 둔다.
- 씬, 프리팹, ScriptableObject, 입력 자산, `ProjectSettings/**`는 guarded asset으로 취급한다.
- 해당 자산을 건드리지 않고도 하네스를 설치할 수 있으면 먼저 그 방식으로 끝낸다.
- 검증용 주 씬이나 주 프리팹이 명확하면 검증 게이트에 포함한다.
- 주 씬 후보가 여러 개면 가장 테스트/전투 성격이 강한 씬을 고르거나, 명확하지 않으면 사용자에게 한 번만 짧게 확인한다.
- 기본 설치 모드는 **공유 프로젝트용 dual-agent 모드**다.
- 즉, Codex가 시작해도 Claude Code용 프로젝트 로컬 설정까지 같이 설치하고, Claude Code가 시작해도 Codex가 바로 읽을 수 있는 문서 구조를 같이 맞춘다.
- Claude Desktop이 설치된 환경이라면 전역 설정 파일도 companion 대상으로 본다.
- 전역 설정 파일은 전체 덮어쓰기가 아니라 필요한 `mcpServers` 항목만 병합한다.
- `rtk`는 기본 포함되는 shell output 압축 계층으로 문서화한다.
- RTK 실제 설치와 업데이트는 프로젝트가 아니라 사용자 전역 환경에서 관리한다.
- RTK는 `AGENTS.md`, `CLAUDE.md`, 프로젝트 로컬 hook 소유권을 가져가지 않는다.
- RTK 설치 여부를 추정하지 말고, 설치/확인/우회 규칙은 `docs/RTK.md`에 적는다.
- 긴 shell output이 예상되는 조사·git·test·batch 검증 로그 읽기에서는 RTK 경로를 우선 고려한다.
- 직접 파일 읽기, 구조화된 Read/Grep/Glob, Unity parser 계열에는 RTK 효과를 과대평가하지 않는다.
- 서브 에이전트는 플랫폼/세션 정책이 허용하고, 사용자가 delegation을 허용했을 때만 사용한다.
- 서브 에이전트는 critical path를 직접 막는 urgent work보다 read-only 조사, 로그/테스트 triage, 문서 정리, disjoint write scope 작업에 우선 사용한다.
- 같은 파일 또는 같은 serialized asset surface를 여러 에이전트에게 동시에 맡기지 않는다.
- 내부 sub-agent 결과는 메인 에이전트가 통합하고 최종 validation은 메인 에이전트가 책임진다.
- `graphify`는 선택적 구조 이해 계층으로만 다룬다.
- 하네스가 이미 있는 프로젝트에서는 `graphify codex install`, `graphify claude install` 같은 always-on 설치를 기본값으로 사용하지 않는다.
- 대신 `docs/Graphify.md`에 설치/갱신 규칙을 적고, `graphify-out/GRAPH_REPORT.md`가 있을 때만 구조 이해에 활용한다.

## 설치 대상

### 0. unity-agent-kit

???? ??? `unity-agent-kit/` ??? ????.

```
kit-commit: [git -C unity-agent-kit rev-parse HEAD ? ??]
installed: [?? ?? YYYY-MM-DD]
kit-path: [?? ??]
agent: codex | claude-code
```

? ??? ?? ???? ?? ??? ???? ????.

### 1. AGENTS.md

반드시 프로젝트별로 맞춤화한다.

**공유 프로젝트 기본값:**

- 기본값은 `AGENTS.md`를 `AGENTS.template.md` 기준의 **자기완결형**으로 설치하는 것이다.
- 이유: Codex는 `AGENTS.md`에서 직접 작업 규칙을 읽고, Claude Code는 `CLAUDE.md`와 `.claude/**`를 추가로 사용하므로 공유 프로젝트에서는 이 편이 더 안전하다.
- `AGENTS.claude.template.md`는 사용자가 **Claude Code 전용 경량 AGENTS**를 명시적으로 원할 때만 사용한다.
- 사용자가 그런 요청을 하지 않았다면, Claude Code가 설치를 시작했더라도 `AGENTS.md`는 Codex 호환 자기완결형으로 유지한다.

Codex 전용 포함 내용:

- 프로젝트 범위와 목표
- 주요 폴더 맵
- safe / explicit / read-only write boundary
- existing worktree rule
- default request interpretation
- architecture rules
- high-risk assets
- validation workflow
- response expectations

Claude Code 전용 포함 내용 (CLAUDE.md와 중복 제외):

- 프로젝트 범위와 목표
- safe / explicit / read-only write boundary
- existing worktree rule
- default request interpretation
- architecture rules → docs/project-context.md 포인터
- task routing hints
- mistake learning loop
- response expectations

### 2. CLAUDE.md

반드시 현재 프로젝트 구조를 설명하는 컨텍스트 문서로 갱신한다.

**100줄 이하를 엄수한다.** CLAUDE.md는 매 세션 자동 로드되므로 내용이 길수록 토큰 낭비가 발생한다.

포함해야 할 내용 (CLAUDE.md 직접 기입):

- 프로젝트 개요 (엔진, 장르, 아키텍처)
- 주요 런타임 루프 (4~6단계)
- 폴더 맵 (최상위 폴더만)
- 검증 커맨드
- guarded asset 목록
- 참조 파일 포인터 (AGENTS.md, project-context.md, ProjectWiki, HarnessMistakes)

별도 파일로 분리해야 할 내용 (docs/project-context.md에 기입):

- 핵심 시스템/모듈 상세
- 시각 표현 흐름 (Visual Flow)
- 데이터/설정 흐름 (Data Flow)
- 시스템 초기화 순서 / 업데이트 단계

### 3. docs/project-context.md

`templates/docs/project-context.template.md`을 기반으로 설치한다.

역할:
- CLAUDE.md에서 분리된 상세 컨텍스트 파일이다.
- Core Systems, Visual Flow, Data Flow, System Ordering/Lifecycle, Architecture Rules를 담는다.
- 자동 로드되지 않는다. 에이전트가 관련 작업 시에만 읽는다.
- CLAUDE.md의 `Reference` 섹션에서 포인터로 참조된다.

### 4. docs/AgentPromptTemplates.md

짧은 요청도 내부적으로 하네스 형태로 확장된다는 기준을 포함한다.

반드시 포함할 내용:

- 긴 템플릿은 선택 사항
- 짧은 버그 리포트나 기능 요청도 내부적으로 목표/수정 범위/완료 조건/검증으로 확장
- code-only, code+data, scene/prefab, investigation-first 예문
- Codex와 Claude Code 둘 다 이 파일을 참고해도 무방하다는 점
- 긴 shell output이 예상되는 작업에서는 `docs/RTK.md`를 기준으로 RTK 경로를 우선 고려하되, raw file reading 대체재로 취급하지 않는다는 점
- 병렬 위임이 허용된 세션에서는 `docs/SubAgents.md`를 기준으로 bounded side task만 분리한다는 점

### 5. docs/HarnessMistakes/**

반드시 설치하거나 기존 구조와 맞춘다.

최소 포함 대상:

- `docs/HarnessMistakes/README.md`
- `docs/HarnessMistakes/domains/README.md`
- `docs/HarnessMistakes/categories/README.md`
- `docs/HarnessMistakes/domains/combat-gameplay.md`
- `docs/HarnessMistakes/domains/ui-bridge.md`
- `docs/HarnessMistakes/domains/scene-prefab.md`
- `docs/HarnessMistakes/domains/data-assets.md`
- `docs/HarnessMistakes/domains/vfx-presentation.md`
- `docs/HarnessMistakes/domains/validation-tooling.md`
- `docs/HarnessMistakes/domains/docs-harness.md`
- `docs/HarnessMistakes/categories/scope-boundary.md`
- `docs/HarnessMistakes/categories/context-drift.md`
- `docs/HarnessMistakes/categories/validation-gap.md`
- `docs/HarnessMistakes/categories/behavioral-regression.md`
- `docs/HarnessMistakes/categories/tooling-automation-gap.md`
- `docs/HarnessMistakes/categories/communication-handoff.md`

규칙:

- 작업 시작 시 먼저 `docs/HarnessMistakes/domains/README.md`를 읽고 primary domain을 고른다.
- 그 다음 해당 `domains/<domain>.md` 파일만 읽는다.
- 해당 domain 파일에 `Preload extra mistake context: no` 라고 되어 있으면 category 파일은 추가로 읽지 않는다.
- 해당 domain 파일에 `Preload extra mistake context: yes` 라고 되어 있으면, `Preload category files`에 적힌 category 파일만 추가로 읽는다.
- 실수가 확인되면 1개의 primary category를 정한다.
- 필요하면 secondary category를 덧붙이되, 실제 기록은 primary category 파일에 남긴다.
- 실수가 확정되면 해당 domain 파일의 `Known recurring mistakes`와 preload 여부도 함께 갱신한다.
- 범주 파일에는 최소한 `what happened`, `why it happened`, `what rule changed`, `what validation/test changed`를 남긴다.
- 같은 종류의 실수가 반복되면 새 항목을 append하고, 예방 규칙을 더 강하게 갱신한다.
- 실수 기록은 단순 회고가 아니라, 이후 에이전트가 참고할 운영 규칙이어야 한다.

### 6. tools/

기본적으로 아래 파일을 만든다.

- `tools/unity-batch.ps1`
- `tools/compile-unity.cmd`
- `tools/smoke-unity.cmd`
- `tools/test-editmode.cmd`
- `tools/test-playmode.cmd`
- `tools/validate-unity.cmd`

`templates/tools/**`를 복사한 뒤 프로젝트 실정에 맞게 맞춘다.

### 7. Assets/Editor/HarnessValidation/

`BatchValidationRunner.cs`를 설치한다.

최소 기능:

- smoke validation
- strict smoke validation
- editor menu items
- validation log folder open

가능하면 추가:

- project-specific EditMode smoke scenarios
- deterministic core-system checks

기본 메뉴 이름은 에이전트 중립적으로 유지한다.

- `Tools/Harness Validation/Run Smoke Validation`
- `Tools/Harness Validation/Run Strict Smoke Validation`
- `Tools/Harness Validation/Run EditMode Smoke Tests`
- `Tools/Harness Validation/Run Full Validation`

기존 프로젝트가 이미 `Tools/Codex Validation/*`에 익숙하다면, 이름을 유지해도 된다.

### 8. Assets/99_Tests/

핵심 시스템 중 deterministic하고 editor-only로 재현 가능한 것이 있다면 최소 1개의 smoke test를 추가한다.

예:

- damage apply
- inventory stat application
- ability unlock flow
- progression request consumption

프로젝트 구조상 직접 참조가 어려우면 reflection 기반 테스트를 허용한다.

### 9. docs/ProjectWiki/

`templates/docs/ProjectWiki/**`를 복사한다.

에이전트가 작업하면서 발견한 프로젝트 지식을 누적하는 위키다.
초기 설치 시에는 빈 상태로 복사하고, 이후 작업마다 에이전트가 채워간다.

구성:
- `README.md` — 위키 운영 규칙 (에이전트가 작업 시 참조)
- `index.md` — 전체 페이지 카탈로그 (새 페이지 추가 시 갱신)
- `log.md` — 작업 이력 (append-only)
- `systems/_template.md` — 시스템 페이지 템플릿
- `assets/_template.md` — 씬/프리팹/에셋 페이지 템플릿
- `adr/_template.md` — 아키텍처 결정 기록 템플릿

페이지 생성 기준:
- 시스템을 조사하거나 수정했을 때 → `systems/<SystemName>.md`
- 씬/프리팹/에셋 의존성을 파악했을 때 → `assets/<AssetName>.md`
- 아키텍처 결정 이유를 추론/확인했을 때 → `adr/ADR-<NNN>-<slug>.md`

규칙:
- 새 페이지를 만들면 반드시 `index.md`에 한 줄 추가한다.
- 작업이 끝나면 `log.md`에 항목을 append한다.
- 기존 페이지와 모순이 발견되면 두 페이지 모두 갱신한다.
- 코드/씬/프리팹 원본과 모순되면 원본이 우선이다.

### 10. docs/AgentHandoffs/

`templates/docs/AgentHandoffs/**`를 복사한다.

이 폴더는 Codex와 Claude Code가 서로 넘겨야 하는 짧은 작업 인계 메모를 저장한다.
장기 지식은 `docs/ProjectWiki/**`에 남기고, 현재 작업을 넘기는 요약은 `docs/AgentHandoffs/**`에 남긴다.

최소 포함 대상:

- `docs/AgentHandoffs/README.md`
- `docs/AgentHandoffs/_handoff-template.md`
- `docs/AgentHandoffs/pending/codex-to-claude/.gitkeep`
- `docs/AgentHandoffs/pending/claude-to-codex/.gitkeep`
- `docs/AgentHandoffs/consumed/codex-to-claude/.gitkeep`
- `docs/AgentHandoffs/consumed/claude-to-codex/.gitkeep`

규칙:

- 모든 작업에서 이 폴더를 자동으로 읽지 않는다.
- 사용자가 `Codex 작업 내역 확인해줘`, `Claude Code 작업 내역 확인해줘`처럼 명시했을 때 우선 확인한다.
- 또는 다른 에이전트가 끝낸 작업을 바로 이어받는 상황이 명확할 때만 확인한다.
- 먼저 대상 `pending/<other-to-self>/` 폴더의 파일명만 확인한다.
- `.gitkeep`만 있거나 실제 handoff note가 없으면 추가 handoff 파일은 읽지 않는다.
- pending note가 있으면 현재 작업과 관련된 note만 읽고, 최신 파일부터 본다.
- 읽어서 이해한 note는 같은 작업 턴 안에서 `consumed/<other-to-self>/`로 이동한다.
- 이미 `consumed/**` 아래로 이동한 note는 사용자가 다시 보라고 하지 않는 한 재독하지 않는다.
- 같은 주제의 handoff를 같은 방향으로 다시 남길 때, 아직 상대 에이전트가 읽지 않은 최신 pending note가 있으면 새 파일을 남발하지 말고 그 note를 갱신해도 된다.
- handoff note는 길게 쓰지 않는다. diff 전문, 장문 회고, 전체 로그 대신 다음 작업자가 바로 이어서 움직일 정보만 남긴다.

권장 파일명:

- `YYYY-MM-DD-HHMM-topic.md`

권장 포함 내용:

- From / To / Date
- 작업 목표와 현재 상태
- 실제 변경 파일
- 수행한 검증과 미실행 검증
- 다음 에이전트가 먼저 읽을 파일
- 남은 리스크와 다음 액션

### 11. docs/Obsidian.md

`templates/docs/Obsidian.template.md`를 `docs/Obsidian.md`로 복사한다.

규칙:

- `docs/Obsidian.md`는 기본 산출물이다.
- 프로젝트 루트를 Obsidian vault로 열 수 있다는 전제를 문서화한다.
- 하네스 문서와 위키는 `Assets/` 밖의 루트 문서 계층에 유지한다.
- `docs/ProjectWiki/index.md`를 허브 note로 삼는 최소 운영 규칙을 포함한다.
- 장기 지식과 짧은 handoff를 분리하는 규칙을 포함한다.
- `.obsidian/` 폴더 자체는 킷의 소유 범위로 강제하지 않는다.
- Obsidian `Excluded files`에 최소 `Library/**`, `Temp/**`, `Logs/**`, `Obj/**`, `UserSettings/**`, `.git/**`를 권장한다.
- `docs/HarnessMistakes/**`는 제외하지 않고, root/domain/category 허브 링크로 한 클러스터를 이루게 유지한다.

에이전트 동작:

- 새 위키 문서를 만들면 `docs/ProjectWiki/index.md`에서 발견 가능하게 만든다.
- 문서 간 연결이 실제로 유용할 때만 `[[wiki link]]`를 추가한다.
- handoff note는 짧게 유지하고, 긴 설명은 `docs/ProjectWiki/**`로 넘긴다.

### 12. docs/Graphify.md

`graphify`를 구조 이해 보조 계층으로 쓸 계획이 있거나, 사용자가 이를 원하면 `templates/docs/Graphify.template.md`를 `docs/Graphify.md`로 복사한다.

규칙:

- `graphify`는 선택 사항이다. 모든 프로젝트에 강제하지 않는다.
- 설치 위치는 Unity 프로젝트 루트 기준이다.
- `graphify-out/`은 project-local artifact로 취급한다.
- `docs/Graphify.md`에는 최소한 아래를 적는다:
  - 설치 명령
  - initial build 명령
  - refresh 명령 (`graphify . --update`)
  - watch 명령 (`graphify . --watch`)
  - optional git hook (`graphify hook install`)
  - Codex에서 `multi_agent = true` 필요 여부
  - always-on install (`graphify codex install`, `graphify claude install`)을 기본값으로 쓰지 않는 이유

권장 운영:

- 활동 중인 개발 세션: `graphify . --watch`
- watch를 안 쓰는 경우: 의미 있는 변경 뒤 `graphify . --update`
- 커밋/브랜치 전환 후 동기화가 중요하면: `graphify hook install`

에이전트 동작:

- `graphify-out/GRAPH_REPORT.md`가 있으면 넓은 구조 질문 전에 먼저 읽는다.
- `graphify-out/graph.json` 질의가 필요하면 `docs/Graphify.md`에 적힌 워크플로를 따른다.
- 그래프가 오래되었을 수 있으면 refresh 여부를 사용자에게 보고한다.

### 13. docs/RTK.md

`templates/docs/RTK.template.md`를 `docs/RTK.md`로 복사한다.

규칙:

- `docs/RTK.md`는 기본 산출물이다. RTK 미설치 상태에서도 문서는 남긴다.
- RTK는 shell command output 압축 계층이다. 하네스 규칙, 검증, Graphify, Unity parser를 대체하지 않는다.
- 실제 RTK 바이너리 설치와 전역 훅 연결은 사용자 전역 환경에서만 관리한다.
- 에이전트는 RTK 설치 여부를 추정하지 말고 `docs/RTK.md` 기준으로만 안내한다.
- 프로젝트 로컬 `AGENTS.md`, `CLAUDE.md`, `.claude/settings.json`의 소유권을 RTK에 넘기지 않는다.
- Claude Code의 `Read`, `Grep`, `Glob` 같은 구조화 도구와 직접 파일 읽기는 RTK 자동 재작성 대상이 아니라는 점을 명시한다.
- Unity 프로젝트 기준으로 효과가 큰 명령 예시를 포함한다:
  - `git status`, `git diff`, `git log`
  - `rg`/`grep`
  - `tools\\compile-unity.cmd`, `tools\\smoke-unity.cmd`, `tools\\validate-unity.cmd`
  - 테스트 러너 및 빌드 로그
- 효과가 낮은 경로도 명시한다:
  - raw source file reading
  - 구조화된 검색 도구
  - `unity-cli`, `unity-prefab-parser-mcp`, 이미지 분석 MCP

에이전트 동작:

- 긴 shell output이 예상되는 작업에서는 먼저 `docs/RTK.md`를 본다.
- RTK가 설치되어 있으면 shell-heavy 조사와 로그 읽기에서 RTK 경로를 우선 고려한다.
- RTK가 미설치되어 있으면 하네스는 그대로 진행하고, 필요 시 수동 전역 설치 가이드만 제시한다.

### 14. docs/SubAgents.md

`templates/docs/SubAgents.template.md`를 `docs/SubAgents.md`로 복사한다.

규칙:

- `docs/SubAgents.md`는 기본 산출물이다. 실제 sub-agent 사용 여부와 무관하게 문서는 남긴다.
- 이 문서는 internal delegation 규칙만 다룬다. `docs/AgentHandoffs/**`는 top-level agent 간 handoff 규칙으로 유지한다.
- sub-agent 사용은 플랫폼과 세션 정책이 허용하고, 사용자가 delegation을 허용했을 때만 활성화한다.
- 메인 에이전트는 blocking design decision, guarded serialized asset, final validation, final report를 직접 맡는다.
- sub-agent는 아래 작업을 우선 대상으로 한다:
  - read-only codebase exploration
  - test/log/build failure triage
  - 문서/위키/요약 초안 작성
  - disjoint write scope의 보조 구현
- sub-agent는 아래 작업에 기본적으로 쓰지 않는다:
  - 같은 파일을 메인 에이전트와 함께 수정하는 작업
  - scene/prefab/ScriptableObject 같은 guarded serialized asset 수정
  - 메인 에이전트의 바로 다음 행동을 막는 urgent blocking work

에이전트 동작:

- delegation이 허용되지 않으면 이 문서는 참고만 하고 일반 단일 에이전트 흐름으로 진행한다.
- delegation이 허용되면 메인 에이전트가 ownership을 명시한 bounded task만 분리한다.
- sub-agent 결과는 메인 에이전트가 다시 검토하고 통합한다.

## README 갱신 기준

README는 전체를 다시 써도 되지만, 최소한 아래는 최신 상태여야 한다.

- 현재 프로젝트의 실제 상태
- 주요 전투/성장/표현 계층
- 하네스와 검증 흐름
- 에디터가 열렸을 때와 닫혔을 때의 검증 경로

README가 이미 충분히 좋고 하네스 설명만 빠져 있으면 필요한 섹션만 추가해도 된다.

## 성공 패턴 승격 루프

실수뿐 아니라, 특히 잘 된 작업도 규칙으로 굳혀야 한다.

작업이 의도한 대로 완벽하게 끝났고 그 방식을 반복하고 싶다면:

1. 해당 작업에서 효과적이었던 접근 방식을 한 줄로 요약한다.
2. 그 접근 방식이 특정 request 유형에 일반화 가능한지 판단한다.
3. 가능하면 `AGENTS.md`의 architecture rules 또는 default request interpretation에 반영한다.
4. 필요하면 `docs/AgentPromptTemplates.md`의 해당 템플릿에도 추가한다.

이 루프를 통해 잘 된 작업의 패턴이 규칙으로 축적되고, 에이전트가 매번 같은 수준의 결과를 낼 수 있게 된다.

## 실수 학습 루프

실수나 재발 가능한 실패가 확인되면, 에이전트는 다음 순서를 따른다.

1. primary domain을 확인한다.
2. primary category를 분류한다.
3. `docs/HarnessMistakes/categories/*.md` 중 해당 파일을 갱신한다.
4. 해당 `docs/HarnessMistakes/domains/<domain>.md` 파일의 `Known recurring mistakes`, `Preload extra mistake context`, `Preload category files`를 갱신한다.
5. 그 실수가 일반 규칙으로 승격되어야 하면 `AGENTS.md`, `CLAUDE.md`, `docs/AgentPromptTemplates.md`, 검증 도구, 테스트 중 필요한 위치에 반영한다.
6. 변경 보고에서 어떤 domain/category 파일을 갱신했는지 명시한다.

## 작업 시작 시 도메인 참조 규칙

작업 시작 시의 실수 컨텍스트 로딩 규칙은 아래와 같다.

1. `docs/HarnessMistakes/domains/README.md`를 읽는다.
2. 현재 작업의 primary domain을 하나 고른다.
3. 해당 domain 파일 하나만 읽는다.
4. 그 domain 파일에 `Preload extra mistake context: no`라고 적혀 있으면 추가 실수 파일은 열지 않는다.
5. `Preload extra mistake context: yes`라고 적혀 있으면 `Preload category files`에 적힌 category 파일만 읽는다.

즉, 모든 실수 파일을 매번 다 읽지 않는다. domain 파일이 추가 로딩 여부를 결정하는 게이트 역할을 한다.

## 작업 시작 시 handoff 참조 규칙

작업 시작 시 handoff 로딩 규칙은 아래와 같다.

1. handoff는 기본적으로 자동 로드하지 않는다.
2. 사용자가 다른 에이전트의 작업 내역 확인을 요청했는지 먼저 본다.
3. 요청이 있거나, 직전 작업을 다른 에이전트가 넘긴 상황이 명확하면 대응되는 `docs/AgentHandoffs/pending/<other-to-self>/` 폴더의 파일명만 먼저 확인한다.
4. pending 폴더에 실제 note가 없으면 더 읽지 않는다.
5. 실제 note가 있으면 현재 작업과 관련된 것만 최신순으로 읽는다.
6. 이해가 끝난 note는 즉시 `consumed/<other-to-self>/`로 이동한다.
7. `consumed/**` 아래 파일은 사용자가 다시 확인하라고 요청할 때만 연다.

즉, handoff도 모든 기록을 매번 읽지 않는다. `pending` 폴더에 unread note가 있을 때만 필요한 범위로 읽고, 읽은 뒤에는 폴더 이동으로 재독을 막는다.

## Graph-First Orientation Rule

프로젝트에 `docs/Graphify.md`와 `graphify-out/GRAPH_REPORT.md`가 둘 다 존재하면:

1. 넓은 구조 질문이나 아키텍처 질문 전에는 `graphify-out/GRAPH_REPORT.md`를 먼저 읽는다.
2. 그래프가 오래되었을 가능성이 높으면 `docs/Graphify.md`의 갱신 규칙을 따른다.
3. 그래프를 읽었다고 해서 raw source reading을 생략하지 않는다.
4. `graphify`가 하네스 문서나 검증 게이트를 대체한다고 설명하지 않는다.

## RTK-First Shell Output Rule

프로젝트에 `docs/RTK.md`가 존재하면:

1. 긴 shell output이 예상되는 조사, git 상태 확인, diff 검토, 테스트 로그 읽기, batch validation 로그 읽기 전에는 `docs/RTK.md`를 먼저 확인한다.
2. RTK가 설치되어 있으면 shell command output은 RTK 경로를 우선 고려한다.
3. RTK가 설치되어 있지 않으면 raw shell output으로 진행하되, 필요하면 `docs/RTK.md`의 전역 설치 가이드를 안내한다.
4. RTK가 있다고 해서 raw source reading, 구조화 도구, Unity parser 사용을 생략하지 않는다.
5. RTK가 프로젝트 로컬 `AGENTS.md`, `CLAUDE.md`, hook 레이어를 대체한다고 설명하지 않는다.

## Sub-Agent Delegation Rule

프로젝트에 `docs/SubAgents.md`가 존재하면:

1. delegation이 허용된 세션에서만 이 규칙을 적용한다.
2. 메인 에이전트는 critical path와 최종 검증 책임을 유지한다.
3. sub-agent는 read-only 조사, 로그/테스트 triage, 문서 정리, disjoint write scope 작업에만 우선 사용한다.
4. 같은 파일이나 같은 guarded serialized asset surface를 여러 에이전트에게 동시에 맡기지 않는다.
5. internal sub-agent 결과와 cross-agent handoff는 혼동하지 않는다. `docs/AgentHandoffs/**`는 Codex와 Claude Code 사이 인계용으로만 유지한다.

다음 중 하나에 해당하면 실수 학습 루프를 반드시 돌린다.

- 사용자가 "이건 실수였다", "다시 이러면 안 된다", "재발 방지 기록을 남겨라" 같은 식으로 명시
- 검증 누락, 범위 위반, 잘못된 가정, 잘못된 완료 보고 같은 하네스 실패가 명확히 확인됨
- 최소 안전 게이트는 통과했지만 의도된 동작을 놓친 구조적 원인이 드러남

## 검증 규칙

에디터가 닫혀 있으면 가능한 자동 검증을 직접 수행한다.

기본 우선순위:

1. `tools\compile-unity.cmd`
2. `tools\smoke-unity.cmd`
3. `tools\test-editmode.cmd` if applicable
4. `tools\validate-unity.cmd` if applicable

에디터가 열려 있어서 batchmode를 못 돌리면:

- 에디터 메뉴 경로를 설치한다.
- 사용자가 실행할 메뉴를 알려준다.
- 정상 결과가 무엇인지 구체적으로 적는다.

## 보고 형식

반드시 보고할 것:

- 변경 파일
- 수행한 검증
- 에디터가 열려 있어서 직접 돌리지 못한 검증
- 남은 리스크

## Claude Code companion outputs

아래 항목은 Claude Code가 프로젝트에 들어왔을 때 바로 작업할 수 있게 만드는 **프로젝트 로컬 companion outputs**다.
공유 프로젝트 기본값에서는 **Codex가 설치를 시작했더라도 같이 설치한다.**
자세한 설치 규칙은 `FOR_CLAUDE_CODE.md`의 "Claude Code companion outputs" 섹션을 참고한다.

### .claude/settings.json

`templates/.claude/settings.template.json`을 기반으로 설치한다.

- `permissions.allow`: `tools/*` 검증 스크립트 실행 허용
- `permissions.deny`: `.unity`, `.prefab`, `ProjectSettings/**` 결정론적 편집 차단
- `{{GUARDED_DATA_PATH}}`는 프로젝트 실제 ScriptableObject/Config 경로로 교체한다.

### .claude/hooks/guard-assets-check.ps1

`templates/.claude/hooks/guard-assets-check.ps1`을 복사한다.

- PreToolUse 훅으로 동작하며 guarded asset 편집 시도를 차단하고 이유를 에이전트에 전달한다.
- settings.json deny와 함께 이중 방어선을 구성한다.
- `$guardedPatterns` 배열에 프로젝트별 추가 패턴을 넣는다.

### .mcp.json (Claude Code project-local)

`templates/.mcp.template.json`을 `.mcp.json`으로 복사하고 `{{UNITY_PREFAB_PARSER_PATH}}`를 실제 경로로 교체한다.

- `unity-prefab-parser-mcp`를 연결해서 씬/프리팹 파일 읽기 시 토큰 사용량을 절감한다.
- `unity-cli`와 별개로 동작하며 에디터가 닫혀 있어도 사용 가능하다.

Codex 사용자는 `.mcp.json` 대신 `~/.codex/config.toml`에 아래를 추가한다.

```toml
[mcp_servers.unity_prefab_parser]
command = "node"
args = ["{{UNITY_PREFAB_PARSER_PATH}}/dist/index.js"]
```

### .claude/unity-cli.md

`templates/.claude/unity-cli.md`를 복사한다.

- Unity 조작을 `unity-cli`로 수행하는 기준을 정의한다.
- `unity-status`, `unity-exec`, `unity-console-read`, `unity-test-run`, `unity-screenshot` 같은 도구의 사용 경계를 설명한다.
- 연결 확인 방법과 미연결 시 사용자 안내 절차를 포함한다.
- `unity-prefab-parser-mcp` 연결 설정 방법(Claude Code, Codex 모두)을 포함한다.

### .claude/image-analysis.md

로컬 이미지 분석 파이프라인을 사용하는 환경이라면 `templates/.claude/image-analysis.md`를 복사한다.

- UI 검증, 스프라이트 색상 확인, 비주얼 버그 분석 시 사용할 규칙을 담는다.
- 환경에 이 파이프라인이 없다면 생략할 수 있지만, 있다면 Codex 시작이든 Claude Code 시작이든 같이 설치한다.

### .claude/claude-desktop-config.md

Claude Desktop을 사용하는 환경이라면 `templates/.claude/claude-desktop-config.md`를 복사한다.

- Claude Desktop 전역 설정 파일 경로와 병합 규칙을 설명한다.
- 기본 대상 경로:
  - `C:\Users\rlack\AppData\Local\Packages\Claude_pzs8sxrjxfjjc\LocalCache\Roaming\Claude\claude_desktop_config.json`
- 설정은 전체 덮어쓰지 말고 `mcpServers`만 병합한다.
- 기존 `preferences`와 다른 MCP 서버는 유지한다.
- 필요하면 `scripts/merge-claude-desktop-config.ps1`로 안전하게 병합한다.

## Codex companion readiness

Codex가 바로 들어와 작업할 수 있는 상태도 항상 같이 맞춘다.

- `AGENTS.md`는 공유 프로젝트 기본값에서 항상 Codex 호환 자기완결형으로 유지한다.
- `docs/AgentPromptTemplates.md`는 에이전트 중립 템플릿으로 유지한다.
- 프로젝트가 이미 `docs/CodexPromptTemplates.md`를 쓰고 있다면 그 파일도 같이 유지하거나 병행 갱신한다.
- `unity-cli`를 쓸 수 있게 Codex용 `~/.codex/config.toml` 설정 예시를 포함한다.
- `unity-prefab-parser-mcp`를 쓰는 경우 `.claude/unity-cli.md`에 함께 적는다.

## Claude Desktop companion readiness

Claude Desktop을 같이 쓰는 환경이면 아래도 맞춘다.

- Claude Desktop 설정 파일이 존재하는지 확인한다:
  - `C:\Users\rlack\AppData\Local\Packages\Claude_pzs8sxrjxfjjc\LocalCache\Roaming\Claude\claude_desktop_config.json`
- 존재하면 `mcpServers`에 필요한 서버만 병합한다.
- 최소 대상:
  - `unity-cli` for Unity editor control
  - `unity-parser` if `unity-prefab-parser-mcp`를 쓴다면
  - `image-tools` if 로컬 이미지 분석 파이프라인을 쓴다면
  - `ollama-vision` if 로컬 이미지 분석 파이프라인을 쓴다면
- 기존 `executor`, `preferences` 등 unrelated entry는 보존한다.
- 병합은 가능하면 `scripts/merge-claude-desktop-config.ps1`를 사용한다.

## 피해야 할 것

- 기존 사용자 변경을 되돌리는 것
- 프로젝트 구조를 확인하지 않고 템플릿을 그대로 덮어쓰는 것
- guarded asset을 이유 없이 수정하는 것
- 테스트 이름만 있고 실제로는 무엇을 검증하는지 모호한 상태로 끝내는 것
- `docs/ProjectWiki/log.md`를 handoff inbox처럼 남용하는 것
- handoff note를 읽은 뒤에도 `pending/**`에 그대로 둬서 다음 에이전트가 다시 읽게 만드는 것
