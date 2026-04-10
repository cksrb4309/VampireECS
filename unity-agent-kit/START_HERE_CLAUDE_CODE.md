# Start Here For Claude Code

아래 요청문을 그대로 보내면 됩니다.

```text
프로젝트 루트의 `unity-agent-kit` 폴더를 기준으로 이 Unity 프로젝트에 에이전트 운영 구조를 설치해줘. `Assets/` 아래에는 문서 폴더를 만들지 마.

먼저 `unity-agent-kit/FOR_CLAUDE_CODE.md`, `unity-agent-kit/HARNESS_INSTALL_SPEC.md`, `unity-agent-kit/templates/**`를 읽고, 현재 프로젝트 구조를 조사한 뒤, 이 프로젝트에 맞는 운영 문서, 검증 흐름, 에이전트 설정을 설치하거나 갱신해줘.

기본 원칙은:
- code-only scope 우선
- guarded asset은 꼭 필요할 때만
- 검증은 가능한 자동으로
- Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 안내
- Codex와 Claude Code가 함께 쓸 수 있게 `docs/AgentHandoffs/**` handoff 구조도 설치
- Claude Code로 시작하더라도 Codex가 바로 읽을 수 있는 문서 구조까지 같이 설치
- Claude Desktop 설정파일이 있으면 그쪽 `mcpServers`도 보존 merge 방식으로 같이 맞춰
- `docs/Obsidian.md`도 기본 산출물로 같이 설치하고, 프로젝트 루트를 Obsidian vault로 열 수 있게 문서 링크 규칙을 맞춰
- `docs/RTK.md`도 기본 산출물로 같이 설치하되, 실제 RTK 바이너리/전역 훅 설치는 전역 가이드 기준으로만 다뤄
- `docs/SubAgents.md`도 기본 산출물로 같이 설치하되, sub-agent는 delegation이 허용된 세션에서만 bounded side task에 써
- 구조 이해용으로 `graphify`를 붙일 계획이 있으면 `docs/Graphify.md`도 같이 설치하되, 기존 하네스 문서/훅을 덮어쓰는 `graphify claude install`/`graphify codex install`은 기본값으로 쓰지 마

작업 후에는 변경 파일, 검증 결과, 남은 리스크를 보고해줘.
```

## 기존 하네스가 이미 있는 경우

```text
프로젝트 루트의 `unity-agent-kit` 폴더를 기준으로 현재 에이전트 운영 구조를 점검하고 최신 프로젝트 상태에 맞게 갱신해줘. `Assets/` 아래 문서 폴더는 만들지 마.

`unity-agent-kit/FOR_CLAUDE_CODE.md`와 `unity-agent-kit/HARNESS_INSTALL_SPEC.md`를 먼저 읽고, 기존 AGENTS.md, README.md, CLAUDE.md, docs, tools, Assets/Editor, Assets/99_Tests를 비교해서 필요한 부분만 수정해줘.

기존 사용자 작업은 되돌리지 말고, 문서와 검증 흐름이 실제 프로젝트 상태와 어긋나는 부분만 맞춰줘.
Codex와 Claude Code를 같이 쓰는 handoff 구조가 빠져 있으면 그것도 보강해줘.
Claude Code로 시작해도 Codex가 바로 작업할 수 있는 설정까지 같이 맞춰줘.
Claude Desktop 설정파일이 있으면 그 설정도 안전하게 merge해줘.
`docs/Obsidian.md`도 기본 문서로 같이 설치하고, Obsidian wiki-link와 허브 note 규칙도 정리해줘.
`docs/RTK.md`도 기본 문서로 같이 설치하고, RTK는 shell output 압축 계층으로만 정리해줘.
`docs/SubAgents.md`도 기본 문서로 같이 설치하고, sub-agent는 delegation 허용 시의 bounded side task 규칙으로만 정리해줘.
프로젝트에서 `graphify`를 쓴다면 그래프 갱신 운영 문서도 같이 정리해줘.
```
