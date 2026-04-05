# Agent Prompt Templates For VampireECS

## Why This Exists

이 파일은 Claude Code 전용 프롬프트 가이드다.
Codex 요청 템플릿은 [docs/CodexPromptTemplates.md](CodexPromptTemplates.md)에 있으며, 두 파일 모두 동일한 하네스를 참조한다.

프롬프트 하나만으로 하네스가 작동하지 않는다.

하네스가 효과를 내려면 이것들이 함께 작동해야 한다.

- 작업 요청
- [AGENTS.md](../AGENTS.md) — 수정 허용 범위, guarded asset, 검증 기준
- 자동 검증 (`tools\validate-unity.cmd` 또는 에디터 메뉴)
- 작업별 완료 조건과 검증 증거

## 짧은 요청도 된다

요청을 길게 적지 않아도 된다.

아래처럼 짧게 요청해도:

```text
이 스킬이 가끔 데미지가 안 들어가.
```

Claude Code는 내부적으로 다음과 같이 확장해서 처리한다.

- 작업 목표 추론
- code-only scope 우선 적용
- guarded asset은 명시적 허용 없이 건드리지 않음
- 가장 좁은 검증 게이트 선택
- 변경 파일, 검증 결과, 수동 확인 항목 보고

Unity 에디터가 열려 있으면 Claude Code가 batchmode를 돌릴 수 없다.
이 경우 Claude Code는 사용자가 눌러야 할 메뉴와 기대 결과를 구체적으로 안내한다.

## Base Template

```text
작업 목표:
- [원하는 기능/버그 수정 한 줄]

배경:
- [현재 증상 또는 목적]
- [관련 시스템/파일 경로가 있으면 적기]

수정 허용 범위:
- [예: Assets/01_Scripts/ECS/**]
- [필요 시 Assets/99_Tests/**]

수정 금지 범위:
- [예: Assets/07_Scenes/**, Assets/06_Prefabs/**, Assets/09_Data/**]

완료 조건:
- [조건 1]
- [조건 2]
- [조건 3]

검증:
- AGENTS.md 기준으로 검증해줘.
- Unity 에디터가 닫혀 있으면 tools\validate-unity.cmd를 실행해줘.
- Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 알려줘.
- 이번 변경에 맞는 테스트가 필요하면 최소 범위로 추가해줘.

보고 방식:
- 변경 파일
- 검증 결과
- 남은 리스크나 수동 확인 항목
```

## Template 1: ECS Code-Only

코드와 테스트만 건드리는 작업.

```text
작업 목표:
- [ECS 버그 수정 또는 로직 변경 한 줄]

배경:
- [증상 또는 목적]

수정 허용 범위:
- Assets/01_Scripts/ECS/**
- Assets/99_Tests/**

수정 금지 범위:
- Assets/06_Prefabs/**
- Assets/07_Scenes/**
- Assets/09_Data/**
- ProjectSettings/**

완료 조건:
- [기능적 완료 조건]
- 기존 데미지 처리 흐름을 깨지 않는다.
- 관련 EditMode smoke 시나리오가 통과한다.

검증:
- AGENTS.md 기준으로 검증해줘.
- Unity 에디터가 닫혀 있으면 tools\validate-unity.cmd를 실행해줘.
- Unity 에디터가 열려 있으면 Tools/Codex Validation/Run Full Validation 메뉴와 기대 결과를 안내해줘.
```

## Template 2: Code + Data

ScriptableObject 자산 변경이 함께 필요한 작업.

```text
작업 목표:
- [코드와 데이터를 함께 변경하는 한 줄 목표]

배경:
- [목적 또는 현재 상태]

수정 허용 범위:
- Assets/01_Scripts/ECS/**
- Assets/01_Scripts/Ability/**
- Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/[TargetStatsConfig].asset
- Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock/[TargetUnlockConfig].asset
- Assets/99_Tests/**

수정 금지 범위:
- Assets/07_Scenes/**
- Assets/06_Prefabs/Scene/CombatSetting.prefab
- ProjectSettings/**

완료 조건:
- 코드와 데이터가 연결된다.
- 기존 해금/보상 흐름과 충돌하지 않는다.
- 데이터 자산 경로가 바뀌지 않는다.

검증:
- AGENTS.md 기준으로 검증해줘.
- 데이터 자산 변경 리스크를 따로 설명해줘.
- Unity 에디터가 닫혀 있으면 tools\validate-unity.cmd를 실행해줘.
- Unity 에디터가 열려 있으면 Tools/Codex Validation/Run Full Validation 메뉴와 기대 결과를 안내해줘.
```

## Template 3: Scene / Prefab

serialized asset 변경이 명시적으로 허용된 작업.

```text
작업 목표:
- [씬 또는 프리팹 변경이 필요한 한 줄 목표]

배경:
- 이번 작업은 씬/프리팹 변경이 필요하다는 것을 알고 있다.

수정 허용 범위:
- Assets/01_Scripts/Presentation/**
- Assets/06_Prefabs/Scene/CombatSetting.prefab
- Assets/07_Scenes/Test_Combat.unity

수정 금지 범위:
- ProjectSettings/**
- Packages/**
- Assets/10_ThirdParty/**

완료 조건:
- missing script나 끊어진 참조가 생기지 않는다.
- 변경이 필요한 자산만 수정한다.

검증:
- AGENTS.md 기준으로 검증해줘.
- 에디터가 닫혀 있으면 strict smoke까지 고려해줘.
- 에디터가 열려 있으면 Tools/Codex Validation/Run Full Validation 메뉴와 기대 결과를 안내해줘.
- 씬/프리팹 변경은 수동 플레이 확인이 필요한 항목을 별도로 적어줘.
```

## Template 4: Investigation First

분석만 원하고 코드 변경은 아직 원하지 않을 때.

```text
작업 목표:
- [현상 설명]. 아직 수정은 하지 마.

배경:
- [어느 계층 — ECS / 브리지 / 프레젠테이션 — 이 원인인지 모르겠다]

수정 허용 범위:
- 이번 요청에서는 파일 수정 금지

완료 조건:
- 원인 후보를 우선순위대로 정리한다.
- 근거가 된 파일을 명시한다.
- 바로 이어서 수정 요청할 수 있게 다음 프롬프트 초안을 같이 준다.

검증:
- 수정 작업이 아니므로 코드 변경은 하지 않는다.
```

## One-Line Shortcut

```text
[기능/버그]를 수정해줘. AGENTS.md 규칙을 따르고, 수정은 [허용 범위] 안에서만 해줘. 완료 조건은 [조건]이고, 검증은 가능한 자동으로 수행해줘. Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 알려줘.
```

## 에디터 메뉴 검증 경로

Unity 에디터가 열려 있을 때 Claude Code가 안내하는 메뉴:

| 메뉴 | 용도 |
|------|------|
| `Tools/Codex Validation/Run Smoke Validation` | 자산 존재 + missing script 확인 |
| `Tools/Codex Validation/Run Strict Smoke Validation` | 위 + build settings 포함 |
| `Tools/Codex Validation/Run EditMode Smoke Tests` | ApplyDamageSystem smoke 시나리오 |
| `Tools/Codex Validation/Run Full Validation` | 전체 검증 (기본 게이트) |

정상 결과: 각 메뉴 실행 후 `Validation passed` 다이얼로그가 나타나고 Console에 `[BatchValidation] ... passed` 로그가 찍힌다.
