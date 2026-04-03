# Codex Prompt Templates For VampireECS

## Why This Exists

In this project, the prompt is not the harness by itself.

The harness works when these pieces are combined:

- your task request
- [AGENTS.md](C:\Users\rlack\Desktop\git\VampireECS\AGENTS.md)
- automated validation
- task-specific tests or acceptance checks

`tools\validate-unity.cmd` and `Tools/Codex Validation/Run Full Validation` do not understand your feature request.
They only check whether the changed project still passes the defined safety gates.

That means the prompt should describe:

- the outcome you want
- the safe edit boundary
- the acceptance criteria
- the expected validation evidence

You do not need to write the full template for every request.

If you report something short like:

```text
이 스킬이 가끔 데미지가 안 들어가.
```

Codex should still expand that request into the harness internally by:

- inferring the likely task goal
- starting in the safest code-only scope
- avoiding guarded serialized assets unless clearly required
- choosing the matching validation path
- reporting what changed, what was validated, and what still needs manual confirmation

Use the full template when you want tighter control over scope or completion criteria.

## Prompt Structure

Use this shape by default.

```text
작업 목표:
- 무엇을 바꾸고 싶은지 한 문장으로 적는다.

배경:
- 현재 문제, 재현 조건, 관련 시스템을 적는다.

수정 허용 범위:
- 수정 가능한 폴더나 정확한 파일 경로를 적는다.

수정 금지 범위:
- 건드리면 안 되는 폴더나 자산을 적는다.

완료 조건:
- 기능적으로 어떤 상태가 되면 끝인지 적는다.
- 가능하면 관찰 가능한 조건으로 적는다.

검증:
- 에디터가 닫혀 있으면 Codex가 실행할 명령을 적는다.
- 에디터가 열려 있으면 내가 실행할 메뉴를 적는다.
- 기능 변경에 맞는 테스트 추가가 필요하면 명시한다.

보고 방식:
- 변경 파일
- 검증 결과
- 남은 리스크
```

## Default Validation Language

This project already has a default gate.

Use this sentence unless the task needs something stricter:

```text
검증은 AGENTS.md 기준으로 수행해줘. Unity 에디터가 닫혀 있으면 tools\validate-unity.cmd를 실행하고, 열려 있으면 내가 Tools/Codex Validation/Run Full Validation를 실행할 수 있게 필요한 안내와 기대 결과를 같이 알려줘.
```

Important: Codex can run the batch command when the editor is closed.
If the editor is already open, Codex cannot click the Unity menu for you. In that case, Codex should tell you which menu to run and how to interpret the result.

## What Good Prompts Include

- exact gameplay intent
- exact allowed edit scope
- whether scene, prefab, or ScriptableObject edits are allowed
- whether automated tests should be added or updated
- what proof counts as done

## What Weak Prompts Miss

Avoid prompts like this:

```text
체인 라이트닝 좀 개선해줘.
```

That is too vague. It does not define:

- desired behavior
- allowed asset changes
- validation scope
- definition of done

## Base Template

Copy this and fill in the brackets.

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
- Unity 에디터가 열려 있으면 내가 Tools/Codex Validation/Run Full Validation를 실행할 수 있게 메뉴와 기대 결과를 안내해줘.
- 이번 변경에 맞는 테스트가 필요하면 최소 범위로 추가해줘.

보고 방식:
- 변경 파일
- 검증 결과
- 남은 리스크나 수동 확인 항목
```

## Template 1: ECS Code-Only Task

Use this when the task should stay in code and tests only.

```text
작업 목표:
- 적 처치 시 체인 라이트닝이 최대 1회 추가 점프하도록 전투 로직을 수정해줘.

배경:
- 현재는 첫 타격 이후 추가 점프가 발생하지 않는다.
- ECS 전투 로직 안에서 해결하고 싶고, 씬/프리팹/ScriptableObject는 건드리고 싶지 않다.

수정 허용 범위:
- Assets/01_Scripts/ECS/**
- Assets/99_Tests/**

수정 금지 범위:
- Assets/06_Prefabs/**
- Assets/07_Scenes/**
- Assets/09_Data/**
- ProjectSettings/**

완료 조건:
- 적이 사망하는 타이밍에 체인 라이트닝이 조건에 맞으면 1회 더 점프한다.
- 기존 데미지 처리 흐름을 깨지 않는다.
- 관련 ECS smoke test가 추가되거나 갱신된다.

검증:
- AGENTS.md 기준으로 검증해줘.
- Unity 에디터가 닫혀 있으면 tools\validate-unity.cmd와 필요한 테스트를 실행해줘.
- Unity 에디터가 열려 있으면 내가 Tools/Codex Validation/Run Full Validation를 실행할 수 있게 안내해줘.

보고 방식:
- 변경 파일
- 검증 결과
- 남은 리스크
```

## Template 2: Code Plus Data Task

Use this when tuning data assets is part of the request.

```text
작업 목표:
- 신규 무기 Chain Lightning의 기본 수치와 해금 흐름까지 연결해줘.

배경:
- 코드만 추가하는 것이 아니라 실제 플레이 가능한 기본값이 필요하다.
- 현재 능력 데이터가 `해금 자산의 기본값`과 `스탯 자산의 보너스값`으로 나뉘어 있다면 그 의미를 유지해야 한다.

수정 허용 범위:
- Assets/01_Scripts/ECS/**
- Assets/01_Scripts/Ability/**
- Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/ChainLightningStatsConfig.asset
- Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock/UnlockChainLightningConfig.asset
- Assets/99_Tests/**

수정 금지 범위:
- Assets/07_Scenes/**
- Assets/06_Prefabs/Scene/CombatSetting.prefab
- ProjectSettings/**

완료 조건:
- Chain Lightning 관련 코드와 데이터가 연결된다.
- 기존 해금/보상 흐름과 충돌하지 않는다.
- 데이터 자산 경로가 바뀌지 않는다.
- 해금 자산과 스탯 자산의 값 의미가 섞이지 않는다.

검증:
- AGENTS.md 기준으로 검증해줘.
- 데이터 자산 변경 리스크를 따로 설명해줘.
- Unity 에디터가 닫혀 있으면 tools\validate-unity.cmd를 실행해줘.
- Unity 에디터가 열려 있으면 내가 Tools/Codex Validation/Run Full Validation를 실행하고 확인할 수 있게 기대 결과를 알려줘.

보고 방식:
- 변경 파일
- 검증 결과
- 데이터 자산 관련 수동 확인 포인트
```

## Template 3: Scene Or Prefab Task

Use this only when you intentionally allow serialized asset changes.

```text
작업 목표:
- Test_Combat 씬에서 체인 라이트닝 VFX가 보이도록 최소 설정만 추가해줘.

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
- Test_Combat에서 체인 라이트닝 VFX 참조가 정상 연결된다.
- missing script나 끊어진 참조가 생기지 않는다.
- 변경이 필요한 자산만 수정한다.

검증:
- AGENTS.md 기준으로 검증해줘.
- 에디터가 닫혀 있으면 strict smoke까지 고려해줘.
- 에디터가 열려 있으면 내가 Tools/Codex Validation/Run Full Validation를 실행할 수 있게 안내해줘.
- 씬/프리팹 변경은 수동 플레이 확인이 필요한 부분을 별도로 적어줘.

보고 방식:
- 변경 파일
- 검증 결과
- 수동 플레이 확인 항목
```

## Template 4: Investigation First

Use this when you do not want code changes yet.

```text
작업 목표:
- Chain Lightning이 왜 시각적으로만 보이고 실제 데미지가 적용되지 않는지 원인만 분석해줘. 아직 수정은 하지 마.

배경:
- ECS 로직, 브리지, 프레젠테이션 중 어디가 원인인지 모르겠다.

수정 허용 범위:
- 이번 요청에서는 파일 수정 금지

완료 조건:
- 원인 후보를 우선순위대로 정리한다.
- 실제 수정이 필요한 파일과 예상 변경 범위를 제안한다.
- 바로 이어서 수정 요청할 수 있게 다음 프롬프트 초안을 같이 준다.

검증:
- 수정 작업이 아니므로 코드 변경은 하지 않는다.

보고 방식:
- 원인 후보
- 근거가 된 파일
- 다음 수정 프롬프트 초안
```

## Practical Rule

If the prompt does not answer these four questions, it is still weak:

1. What exactly should change?
2. What is allowed to be edited?
3. What evidence proves the work is done?
4. What should be validated automatically?

## One-Line Shortcut

If you want a short version, use this form:

```text
[기능/버그]를 수정해줘. AGENTS.md 규칙을 따르고, 수정은 [허용 범위] 안에서만 해줘. 완료 조건은 [조건]이고, 검증은 가능한 자동으로 수행해줘. Unity 에디터가 열려 있으면 내가 실행할 메뉴와 기대 결과를 같이 알려줘.
```
