# For Claude Code

이 파일은 Claude Code 전용 진입 문서다.

프로젝트 루트에 `unity-agent-kit`가 있고 사용자가 이 폴더를 언급했다면:

1. 이 파일을 읽는다.
2. [HARNESS_INSTALL_SPEC.md](C:\Users\rlack\Desktop\노션\unity-agent-kit\HARNESS_INSTALL_SPEC.md)를 읽는다.
3. `templates/**`를 읽는다.
4. 공용 설치 규격에 따라 현재 Unity 프로젝트에 맞는 하네스를 설치하거나 갱신한다.

**업데이트 요청**을 받으면: 아래 "하네스 업데이트 워크플로" 섹션을 따른다.

## Claude Code 전용 보조 규칙

- 기본 설치 모드는 **shared-project dual-agent 모드**다. Claude Code가 시작했더라도 Codex가 바로 들어와 작업할 수 있게 문서 구조도 같이 맞춘다.
- **대화가 길어지면 `/compact`를 제안한다**: 아래 조건 중 하나라도 해당되면 사용자에게 `/compact`를 실행하도록 안내한다. 실행할 명령어 전체(요약 내용 포함)를 복사하기 쉽게 코드 블록으로 제공한다.
  - 주제(작업 단위)가 3개 이상 전환됐을 때
  - 파일 수정이 10개 이상 쌓였을 때
  - 컨텍스트 압축이 이미 한 번 발생했을 때
  - 요약 내용 형식: `완료한 작업 목록 | 현재 상태(git 커밋 등) | 미완료 항목`
- **읽기-분석-쓰기 순서를 지킨다**: Read/Grep/Glob은 한 번에 병렬로 실행하고, 분석 후 Edit/Write/Bash를 실행한다. 읽기와 쓰기를 여러 턴에 걸쳐 섞지 않는다. 파일 하나 읽고 수정하고를 반복하지 않는다.
- 공유 프로젝트 기본값에서는 `AGENTS.md`를 Codex 호환 자기완결형으로 유지한다. Claude Code는 `CLAUDE.md`와 `.claude/**`를 추가로 사용하므로 이 편이 둘 다 함께 쓰기 쉽다.
- `AGENTS.claude.template.md`는 사용자가 Claude Code 전용 경량 AGENTS를 명시적으로 원할 때만 선택한다.
- `CLAUDE.md`는 Claude Code가 세션 시작 시 자동으로 로드하는 파일이다. 완성도가 높을수록 매 대화에서 별도 설명 없이 프로젝트 맥락을 즉시 파악할 수 있다.
- `CLAUDE.md`는 **100줄 이하를 목표**로 한다. 이 파일은 매 세션 시작 시 자동으로 전체 로드된다. 길어질수록 매 대화마다 낭비되는 토큰이 선형으로 증가한다.
- 상황 의존적 내용(시스템 상세, 비주얼 흐름, 데이터 흐름, 초기화 순서)은 `docs/project-context.md`에 분리한다. 에이전트는 관련 작업 시에만 이 파일을 읽는다.
- CLAUDE.md에는 항상 필요한 것만 남긴다: 개요, Repo Map 요약, Validation 커맨드, Guarded Assets, 참조 파일 포인터.
- Claude Code는 Read, Write, Edit, Grep, Glob, Bash 도구로 프로젝트 파일을 직접 탐색하고 수정할 수 있다. `scripts/scan-unity-project.ps1`을 실행하지 않아도 구조 파악이 가능하다.
- 가능한 경우 직접 검증을 실행한다.
- Unity 에디터가 이미 열려 있어 batchmode를 돌릴 수 없으면, 사용자가 눌러야 할 메뉴와 기대 결과를 구체적으로 안내한다.
- 짧은 사용자 요청도 내부적으로 목표/수정 범위/완료 조건/검증으로 확장해서 처리한다.
- 기존 프로젝트가 이미 Codex 전용 이름을 일부 쓰고 있어도, 사용자가 명시적으로 원하지 않는 한 강제 개명하지 않는다.
- Codex가 바로 이어서 작업할 수 있게 `AGENTS.md`, `docs/AgentPromptTemplates.md`, 필요 시 `docs/CodexPromptTemplates.md`도 같이 맞춘다.
- `docs/Obsidian.md`는 기본 산출물로 함께 설치한다. 문서와 위키는 프로젝트 루트 vault 기준으로 유지하고 `Assets/` 아래에 만들지 않는다.
- Claude Desktop을 같이 쓰는 환경이면 `claude_desktop_config.json`도 companion 대상으로 취급한다. 기존 `preferences`와 unrelated MCP는 유지하고 필요한 `mcpServers`만 병합한다.
- `docs/RTK.md`는 기본 산출물로 함께 설치한다. RTK는 shell output 압축 계층이며, 실제 바이너리 설치와 전역 훅 연결은 전역 가이드로만 다룬다.
- shell-heavy 조사, `git` 상태 확인, `rg`/`grep`, 테스트 로그, Unity batch 검증 로그 읽기에는 `docs/RTK.md` 기준으로 RTK 경로를 우선 고려한다.
- RTK 설치 여부를 추정하지 말고, 확인/설치/우회는 항상 `docs/RTK.md` 기준으로 안내한다.
- RTK가 있다고 해서 Claude Code의 `Read`, `Grep`, `Glob`, raw source reading, `unity-cli`, `unity-prefab-parser-mcp`를 대체한다고 가정하지 않는다.
- `docs/SubAgents.md`는 기본 산출물로 함께 설치한다. 현재 세션/플랫폼이 delegation을 지원할 때만 sub-agent 규칙을 적용하고, 메인 에이전트가 ownership과 최종 검증 책임을 유지한다.
- sub-agent는 read-only 조사, 테스트/로그 triage, 문서 정리, disjoint write scope 작업에만 우선 사용한다.
- 같은 파일이나 같은 guarded serialized asset 영역을 여러 에이전트에게 동시에 맡기지 않는다.
- `docs/AgentHandoffs/**`는 Codex ↔ Claude Code handoff용이다. 내부 sub-agent 결과 저장소로 쓰지 않는다.
- 프로젝트가 `graphify`를 쓰더라도, 하네스가 이미 있는 상태에서는 `graphify claude install`로 `CLAUDE.md`나 훅 레이어를 덮어쓰지 않는다.
- `docs/Graphify.md`와 `graphify-out/GRAPH_REPORT.md`가 있으면 구조 질문 전에 먼저 읽되, 그래프가 stale할 수 있으면 `docs/Graphify.md`의 refresh 규칙을 따른다.
- 실수나 하네스 실패가 확인되면 `HARNESS_INSTALL_SPEC.md`의 실수 학습 루프를 적용해서 primary category를 분류하고, 해당 `docs/HarnessMistakes/categories/*.md` 파일을 갱신한다.
- 작업 시작 시에는 `docs/HarnessMistakes/domains/README.md`로 primary domain을 고르고, 해당 domain 파일을 먼저 읽는다. 그 domain 파일이 `Preload extra mistake context: yes`일 때만 연결된 category 파일을 추가로 읽는다.
- 사용자가 `Codex 작업 내역 확인해줘`라고 하면 먼저 `docs/AgentHandoffs/pending/codex-to-claude/`의 파일명만 확인한다.
- `.gitkeep` 외에 실제 note가 없으면 handoff가 없다고만 보고하고 다른 handoff 파일은 읽지 않는다.
- unread handoff note가 있으면 현재 작업과 관련된 note만 읽고, 이해가 끝난 파일은 `docs/AgentHandoffs/consumed/codex-to-claude/`로 이동한다.
- `consumed/**` 아래 handoff note는 사용자가 다시 보라고 하지 않는 한 재독하지 않는다.
- Claude Code 작업을 Codex에 넘길 필요가 생기면 `docs/AgentHandoffs/pending/claude-to-codex/`에 짧은 handoff note를 남긴다.
- Obsidian을 쓰는 프로젝트라면 장기 문서는 `docs/ProjectWiki/**`에 남기고, 필요한 곳에만 `[[wiki link]]`를 추가한다.

## Claude Code companion outputs (.claude/)

HARNESS_INSTALL_SPEC.md의 공통 산출물 외에, 공유 프로젝트 기본값에서는 다음을 같이 설치한다.

### .claude/settings.json

`templates/.claude/settings.template.json`을 복사한 뒤 프로젝트에 맞게 맞춘다.

역할:
- `permissions.allow` — `tools/*` 검증 스크립트를 권한 프롬프트 없이 실행할 수 있게 허용한다.
- `permissions.deny` — `.unity`, `.prefab`, `ProjectSettings/**`를 결정론적으로 편집 차단한다. CLAUDE.md 텍스트 규칙만으로는 에이전트가 무시할 수 있으므로, settings.json으로 강제한다.

커스터마이징:
- `{{GUARDED_DATA_PATH}}`를 프로젝트 실제 ScriptableObject/Config 경로로 교체한다 (예: `Assets/02_Data`).
- 프로젝트별 추가 guarded 경로가 있으면 deny 목록에 추가한다.
- 허가 받은 guarded asset을 일시적으로 수정해야 할 때는 해당 deny 항목을 잠시 제거하고 작업 후 복구한다.

### .claude/hooks/guard-assets-check.ps1

`templates/.claude/hooks/guard-assets-check.ps1`을 복사한다.

역할:
- PreToolUse 훅으로 동작하며, Edit/Write 호출 시 파일 경로가 가드 패턴에 맞으면 차단하고 이유와 해결 방법을 에이전트에게 전달한다.
- settings.json의 deny가 경로 패턴으로 처리하지 못하는 케이스(동적 경로, 변수 경로 등)를 보조한다.

커스터마이징:
- 스크립트 내 `$guardedPatterns` 배열에 프로젝트별 정규식 패턴을 추가한다.
- 기본 패턴(`.unity`, `.prefab`, `ProjectSettings`) 외에 Config 에셋 등 프로젝트 고유 경로를 추가한다.

### .mcp.json

`templates/.mcp.template.json`을 `.mcp.json`으로 복사한 뒤 `{{UNITY_PREFAB_PARSER_PATH}}`를 실제 설치 경로로 교체한다.

역할:
- `unity-prefab-parser-mcp`를 Claude Code 세션에 연결한다.
- 씬/프리팹 파일을 Read로 직접 읽는 대신 `parse_unity_prefab` 도구를 사용해서 토큰 사용량을 최대 90% 절감한다.
- `unity-cli`와 별개로, 에디터가 닫혀 있어도 동작한다.

### .claude/unity-cli.md

`templates/.claude/unity-cli.md`를 복사한다.

역할:
- Unity 조작을 `unity-cli`로 수행하는 기준과 파일 직접 수정의 사용 경계를 정의한다.
- 세션 시작 시 MCP 연결 확인 방법과 미연결 시 사용자 안내 절차를 제공한다.
- `.unity`, `.prefab` deny 규칙은 MCP가 연결된 상태에서도 유지한다 (파일 직접 수정 방지 역할).

### .claude/image-analysis.md

`templates/.claude/image-analysis.md`를 복사한다.

역할:
- 로컬 이미지 분석 파이프라인(`ollama-vision` + `image-tools`)의 Unity 작업 맥락 운영 규칙을 정의한다.
- 두 MCP는 Claude 계열 전역 설정에 등록하므로 프로젝트별 `.mcp.json`에 추가할 필요 없다.
- Ollama 서비스 가용성 확인 방법과 미연결 시 안내 절차를 포함한다.

커스터마이징:
- 이 파이프라인이 없는 환경에서는 이 파일 복사를 생략한다.
- 미설치 상태라면 설치 방법을 별도로 안내받아야 한다 (`ollama-vision-mcp`, `image-tools-mcp` 설치 필요).

### .claude/claude-desktop-config.md

`templates/.claude/claude-desktop-config.md`를 복사한다.

역할:
- Claude Desktop 전역 설정 파일 경로와 merge 규칙을 설명한다.
- `mcpServers` 병합 시 기존 `preferences`와 unrelated 서버를 보존하도록 가이드한다.
- 가능하면 `scripts/merge-claude-desktop-config.ps1`를 사용하도록 안내한다.

## Unity CLI 운영 규칙

씬이나 프리팹 관련 작업이 포함된 세션은 아래 절차를 따른다.

1. 세션 시작 시 MCP 연결 상태를 확인한다 (가벼운 MCP 도구 호출로 응답 여부 확인).
2. **연결됨**: `.claude/unity-cli.md`의 도구 표를 기준으로 `unity-cli` 도구를 우선 사용한다.
3. **미연결**: `.claude/unity-cli.md`의 "미연결 시" 절차를 따른다. 사용자 선택 없이 파일 직접 수정으로 독단 진행하지 않는다.

C# 스크립트, `.asmdef`, 하네스 문서 등 MCP 범위 밖의 파일은 MCP 연결 여부와 무관하게 직접 편집한다.

## 로컬 이미지 분석 파이프라인 운영 규칙

이미지 분석이 필요한 작업(UI 검증, 스프라이트 색상, 비주얼 버그 등)은 아래 절차를 따른다.

1. 세션 시작 시 `describe_image` 호출로 `ollama-vision` 가용성을 확인한다.
2. **연결됨**: `ollama-vision` 먼저 사용 → 정밀 수치 필요 시만 `image-tools`로 보완한다.
3. **미연결**: `.claude/image-analysis.md`의 "파이프라인 미연결 시" 절차를 따른다. 독단적으로 이미지 파일을 Read로 직접 읽지 않는다.

이 파이프라인은 Claude Code CLI 또는 Claude Desktop 전역 설정에 등록하므로 프로젝트별 `.mcp.json` 수정 없이 모든 Unity 프로젝트에서 사용 가능하다.

## 하네스 업데이트 워크플로

사용자가 "하네스 업데이트해줘" + 킷 경로를 알려주면 아래 절차를 따른다.

### 1단계 — 버전 확인

Unity ???? ??? `unity-agent-kit` ??? ???? `git -C unity-agent-kit rev-parse HEAD`? ?? `kit-commit` ?? ????.

```
kit-commit: abc1234...
installed: 2026-03-01
kit-path: C:\...\unity-agent-kit
```

파일이 없으면: 구버전(버전 파일 도입 이전) 설치로 간주한다. 전체 비교 모드로 진행한다.

### 2단계 — 변경 파일 목록 추출

킷 경로에서 `git log` + `git diff`로 이전 커밋 이후 변경된 템플릿 파일을 확인한다.

```bash
# 변경된 파일 목록
git -C [킷경로] diff --name-only [kit-commit]..HEAD

# 커밋 요약
git -C [킷경로] log --oneline [kit-commit]..HEAD
```

`unity-agent-kit` ??? ??? git ?????? ?? ? ??? `git diff --name-only`? ?? ? ??? ?? ??? ??? ????.

### 3단계 — 변경 내용 분류 및 적용 계획 수립

변경된 파일을 세 가지로 분류한다.

| 분류 | 설명 | 처리 방식 |
|------|------|-----------|
| **자동 적용 가능** | 하네스 문서, 도구 스크립트, MCP 가이드 등 프로젝트 커스터마이징 없는 파일 | 직접 덮어쓰기 |
| **병합 필요** | AGENTS.md, CLAUDE.md, AgentPromptTemplates.md 등 프로젝트별 커스터마이징이 포함된 파일 | 새 섹션/변경 내용만 추출하여 기존 내용 보존하면서 병합 |
| **검토 필요** | 프로젝트 구조에 의존하는 검증 스크립트, 경로 플레이스홀더 포함 파일 | 사용자에게 변경 내용을 보고하고 판단을 구함 |

계획을 사용자에게 제시하고 승인받은 뒤 실행한다.

### 4단계 — 적용

- 자동 적용 파일은 복사한다.
- 병합 필요 파일은 기존 파일을 읽고, 새 섹션/수정 내용을 식별하여, 프로젝트 맞춤 내용을 보존하면서 업데이트한다.
- 검토 필요 파일은 사용자 확인 후 처리한다.

### 5단계 — 버전 파일 갱신

```
kit-commit: [새 커밋 해시]
installed: [오늘 날짜]
kit-path: [킷 경로]
agent: claude-code
```

### 업데이트 완료 보고 형식

```
하네스 업데이트 완료
이전 버전: [이전 커밋 7자]
현재 버전: [새 커밋 7자]

자동 적용: [파일 목록]
병합 적용: [파일 목록] — 변경 내용: [요약]
건너뜀:   [파일 목록] — 이유: [이유]

수동 확인 필요:
- [항목 1]
```
