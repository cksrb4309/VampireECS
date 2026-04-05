# Harness Mistake Domains

작업 시작 시 모든 실수 파일을 다 읽지 않기 위한 라우팅 문서입니다.

## 사용 규칙

1. 먼저 현재 작업의 primary domain을 1개 고릅니다.
2. 해당 domain 파일 하나만 읽습니다.
3. 그 domain 파일에 `Preload extra mistake context: no` 라고 적혀 있으면 category 파일은 더 읽지 않습니다.
4. `Preload extra mistake context: yes` 라고 적혀 있으면 `Preload category files`에 적힌 category 파일만 읽습니다.

즉, domain 파일이 실수 컨텍스트 추가 로딩 여부를 결정하는 게이트입니다.

## Domain Index

- `combat-gameplay.md`
  - 전투 로직, ECS/능력/적/피해/진행 루프
- `ui-bridge.md`
  - UI 표시, 입력 브리지, observable sync, controller/binder
- `scene-prefab.md`
  - 씬, 프리팹, 인스펙터 연결, missing reference, 배치/구조 변경
- `data-assets.md`
  - ScriptableObject, config asset, unlock/stat tuning, address/reference consistency
- `vfx-presentation.md`
  - VFX, presentation manager, view object, runtime visual bridge
- `validation-tooling.md`
  - compile/smoke/test, editor menu validation, automation, batchmode limits
- `docs-harness.md`
  - README, AGENTS, CLAUDE, prompt templates, mistake docs 자체

## Domain Selection Hints

- gameplay bug, ability bug, progression bug:
  - `combat-gameplay.md`
- UI가 상태와 안 맞음, 입력 반영이 어긋남:
  - `ui-bridge.md`
- 씬/프리팹 연결, 오브젝트 참조, 부트스트랩 문제:
  - `scene-prefab.md`
- 수치/해금/ScriptableObject 문제:
  - `data-assets.md`
- VFX나 화면 표현 문제:
  - `vfx-presentation.md`
- 검증, 자동화, 메뉴 실행, 배치 스크립트 문제:
  - `validation-tooling.md`
- 하네스 문서, 운영 규칙, 요청 템플릿 문제:
  - `docs-harness.md`
