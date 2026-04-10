# For Codex

이 파일은 Codex 전용 진입 문서다.

프로젝트 루트에 `unity-agent-kit`가 있고 사용자가 이 폴더를 언급했다면:

1. 이 파일을 읽는다.
2. [HARNESS_INSTALL_SPEC.md](C:\Users\rlack\Desktop\노션\unity-agent-kit\HARNESS_INSTALL_SPEC.md)를 읽는다.
3. `templates/**`를 읽는다.
4. 공용 설치 규격에 따라 현재 프로젝트에 맞는 하네스를 설치하거나 갱신한다.

## Codex 전용 보조 규칙

- 기본 설치 모드는 **shared-project dual-agent 모드**다. Codex가 시작했더라도 Claude Code가 바로 붙을 수 있게 프로젝트 로컬 설정까지 같이 설치한다.
- **읽기-분석-쓰기 순서를 지킨다**: Read/Grep/Glob은 한 번에 병렬로 실행하고, 분석 후 Edit/Write/Bash를 실행한다. 읽기와 쓰기를 여러 턴에 걸쳐 섞지 않는다. 파일 하나 읽고 수정하고를 반복하지 않는다.
- 가능한 경우 직접 검증을 실행한다.
- Unity 에디터가 이미 열려 있어 batchmode를 돌릴 수 없으면, 사용자가 눌러야 할 메뉴와 기대 결과를 구체적으로 안내한다.
- 짧은 사용자 요청도 내부적으로 목표/수정 범위/완료 조건/검증으로 확장해서 처리한다.
- `AGENTS.md`는 공유 프로젝트 기본값에서 Codex 호환 자기완결형으로 유지한다. Claude Code용 경량형 `AGENTS.claude.template.md`는 사용자가 명시적으로 원할 때만 선택한다.
- `docs/Obsidian.md`는 기본 산출물로 함께 설치한다. 문서와 위키는 프로젝트 루트 vault 기준으로 유지하고 `Assets/` 아래에 만들지 않는다.
- Claude Code companion outputs도 같이 설치한다: `.claude/settings.json`, `.claude/hooks/guard-assets-check.ps1`, `.mcp.json`, `.claude/unity-cli.md`, 가능하면 `.claude/image-analysis.md`.
- Claude Desktop 설정 파일이 존재하면 그것도 companion 대상으로 본다. `claude_desktop_config.json`은 전체 덮어쓰지 말고 `mcpServers`만 병합한다.
- 가능하면 `scripts/merge-claude-desktop-config.ps1`를 사용해서 `unity-parser`와 필요한 이미지 분석 MCP를 병합한다.
- `docs/RTK.md`는 기본 산출물로 함께 설치한다. RTK는 shell output 압축 계층이며, 실제 바이너리 설치와 훅 연결은 전역 가이드로만 다룬다.
- shell-heavy 조사, `git` 상태 확인, `rg`/`grep`, 테스트 로그, Unity batch 검증 로그 읽기에는 `docs/RTK.md` 기준으로 RTK 경로를 우선 고려한다.
- RTK 설치 여부를 추정하지 말고, 확인/설치/우회는 항상 `docs/RTK.md` 기준으로 안내한다.
- RTK가 있다고 해서 raw source reading, `Read/Grep/Glob`, `unity-cli`, `unity-prefab-parser-mcp`를 대체한다고 가정하지 않는다.
- `docs/SubAgents.md`는 기본 산출물로 함께 설치한다. Codex sub-agent는 delegation이 허용된 세션에서만 사용하고, 메인 에이전트가 ownership과 최종 검증 책임을 유지한다.
- sub-agent는 read-only 조사, 테스트/로그 triage, 문서 정리, disjoint write scope 작업에만 우선 사용한다.
- 같은 파일이나 같은 guarded serialized asset 영역을 여러 에이전트에게 동시에 맡기지 않는다.
- `docs/AgentHandoffs/**`는 Codex ↔ Claude Code handoff용이다. Codex 내부 sub-agent 결과 저장소로 쓰지 않는다.
- 프로젝트가 `graphify`를 쓰더라도, 하네스가 이미 있는 상태에서는 `graphify codex install`로 `AGENTS.md`나 훅 레이어를 덮어쓰지 않는다.
- `docs/Graphify.md`와 `graphify-out/GRAPH_REPORT.md`가 있으면 구조 질문 전에 먼저 읽되, 그래프가 stale할 수 있으면 `docs/Graphify.md`의 refresh 규칙을 따른다.
- `unity-prefab-parser-mcp` 경로나 guarded data 경로 같은 플레이스홀더는 실제 환경 값으로 채운다. 안전하게 추론할 수 없으면 한 번만 짧게 확인한다.
- 기존 프로젝트가 이미 `docs/CodexPromptTemplates.md`나 `Assets/Editor/CodexValidation/*`를 쓰고 있다면, 무조건 이름을 바꾸지 말고 현재 프로젝트 관성에 맞춰 유지하거나 점진적으로 이동한다.
- 실수나 하네스 실패가 확인되면 `HARNESS_INSTALL_SPEC.md`의 실수 학습 루프를 적용해서 primary category를 분류하고, 해당 `docs/HarnessMistakes/categories/*.md` 파일을 갱신한다.
- 작업 시작 시에는 `docs/HarnessMistakes/domains/README.md`로 primary domain을 고르고, 해당 domain 파일을 먼저 읽는다. 그 domain 파일이 `Preload extra mistake context: yes`일 때만 연결된 category 파일을 추가로 읽는다.
- 사용자가 `Claude Code 작업 내역 확인해줘`라고 하면 먼저 `docs/AgentHandoffs/pending/claude-to-codex/`의 파일명만 확인한다.
- `.gitkeep` 외에 실제 note가 없으면 handoff가 없다고만 보고하고 다른 handoff 파일은 읽지 않는다.
- unread handoff note가 있으면 현재 작업과 관련된 note만 읽고, 이해가 끝난 파일은 `docs/AgentHandoffs/consumed/claude-to-codex/`로 이동한다.
- `consumed/**` 아래 handoff note는 사용자가 다시 보라고 하지 않는 한 재독하지 않는다.
- Codex 작업을 Claude Code에 넘길 필요가 생기면 `docs/AgentHandoffs/pending/codex-to-claude/`에 짧은 handoff note를 남긴다.
- Obsidian을 쓰는 프로젝트라면 장기 문서는 `docs/ProjectWiki/**`에 남기고, 필요한 곳에만 `[[wiki link]]`를 추가한다.
