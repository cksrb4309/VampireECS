# VampireECS

Unity 6 기반의 3D 뱀서라이크 전투 수직 슬라이스 프로젝트입니다.

이 저장소의 목표는 완성형 콘텐츠를 빠르게 늘리는 것이 아니라, `전투 상태는 ECS`, `입력/UI/표현은 브리지 계층`이라는 구조를 유지한 채 전투 루프와 성장 루프를 안정적으로 굴리는 것입니다.

## 현재 상태

- Unity 6 `6000.3.11f1`
- DOTS/ECS 기반 전투 루프
- 주 전투 씬: `Assets/07_Scenes/Test_Combat.unity`
- 주 공격 계열
  - `Shooter`
  - `Aura`
  - `Boomerang`
  - `Chain Lightning`
  - `Meteor Strike`
  - `Black Hole`
- 성장 루프
  - 경험치 획득
  - 레벨업 요청
  - 시간 정지형 보상 선택
  - ScriptableObject 기반 해금/스택 강화
- 시각 표현
  - VFX Graph 기반 데미지 텍스트
  - 오라 뷰 매니저
  - 라인 렌더러 기반 체인 라이트닝
  - 라인 렌더러 기반 메테오 경고/임팩트
  - 라인 렌더러 기반 블랙홀 프레젠테이션

## 핵심 게임 루프

1. 플레이어가 이동하고 조준한다.
2. ECS 전투 시스템이 오라, 투사체, 체인 라이트닝, 메테오, 블랙홀 공격을 처리한다.
3. 적이 피해를 받고 사망하면 경험치 이벤트가 생성된다.
4. 경험치가 누적되면 레벨업 선택 UI가 열린다.
5. 선택한 보상이 ECS 컴포넌트 또는 해금 상태에 반영된다.
6. 전투가 재개되고 다음 성장 루프로 이어진다.

## 아키텍처 요약

### ECS Core

- `Assets/01_Scripts/ECS/**`
- 전투 상태, 공간 분할, 적 스폰/이동, 공격 생성, 피해 처리, 경험치 처리
- 성능에 민감한 흐름을 중심으로 유지

### Bridge / Presentation

- `Assets/01_Scripts/UI/**`
- `Assets/01_Scripts/System/**`
- `Assets/01_Scripts/Presentation/**`
- 입력, UI, 카메라, 런타임 VFX 표현 담당

### Visual Event Flow

- `Assets/01_Scripts/ECS/Visual/**`
- ECS가 시각 이벤트 엔티티를 발행
- 프레젠테이션 계층이 이를 소비하고 즉시 정리
- 현재 대표 흐름
  - `DamageTextPresentationSystem`
  - `ChainLightningPresentationSystem`
  - `MeteorStrikePresentationSystem`
  - `BlackHolePresentationSystem`

## 능력 스탯 모델

현재 공격 능력은 공통적으로 `기본값(Base)`과 `강화값(Bonus)`을 분리하는 구조를 사용합니다.

- `Unlock...Config`
  - 능력 해금과 함께 기본 성능을 정의
- `...BaseStatsData`
  - ECS 런타임에서 능력의 기본 성능을 보관
- `...StatsConfig`
  - 레벨업 보상으로 더해지는 강화값을 정의
- `...StatsData`
  - ECS 런타임에서 누적된 강화값을 보관
- `CombatStatsData`
  - 모든 능력에 공통으로 곱해지는 전역 전투 배율

대표 계산식은 다음 형태를 따릅니다.

```text
최종값 = BaseValue * (1 + LocalBonusRate) * GlobalCombatMultiplier
```

개수형 수치나 반경 보정처럼 비율이 아닌 값은 별도의 bonus 필드로 가산합니다.

## 주요 폴더

```text
Assets/
├── 00_Core/                     프로젝트 설정 자산
├── 01_Scripts/
│   ├── Ability/                 능력, 해금, 스탯 적용
│   ├── ECS/                     전투 ECS 코어
│   ├── Presentation/            런타임 표현 계층
│   ├── System/                  카메라, 시간 정지 등
│   └── UI/                      입력/UI 브리지
├── 06_Prefabs/                  씬, 플레이어, 적, VFX 프리팹
├── 07_Scenes/                   테스트 전투 씬
├── 09_Data/                     ScriptableObject 구성 데이터
├── 99_Tests/                    EditMode 테스트 및 확장 예정 테스트
└── Editor/CodexValidation/      프로젝트 전용 검증 엔트리
tools/                           Unity 배치 검증 스크립트
```

## 현재 구현 범위

### 전투

- 플레이어 이동
- 마우스 조준
- 투사체 생성 및 충돌 처리
- 오라 기반 범위 피해
- 체인 라이트닝 점프 공격
- 메테오 타겟팅/지연 폭발
- 블랙홀 생성/흡인/도트 피해
- 피해 적용과 사망 판정

### 성장과 해금

- 경험치 누적
- 레벨업 선택 UI 요청
- 기본 전투 스탯 강화
- Shooter, Aura, Chain Lightning, Meteor Strike, Black Hole 해금

### 표현

- 데미지 텍스트 이벤트 기반 출력
- 오라 VFX 관리
- 체인 라이트닝 세그먼트 시각화
- 메테오 텔레그래프/임팩트 시각화
- 블랙홀 지속 필드 시각화
- 트레일 렌더링 보조 시스템

## 검증과 하네스

이 저장소는 프롬프트만으로 작업하지 않고, 문서화된 하네스와 검증 게이트를 같이 사용합니다.

### 작업 규칙 문서

- [AGENTS.md](AGENTS.md)
  - 수정 허용 범위, 검증 기본값, guarded asset 규칙
- [CLAUDE.md](CLAUDE.md)
  - 코드 구조와 시스템 맥락
- [docs/CodexPromptTemplates.md](docs/CodexPromptTemplates.md)
  - Codex 요청 템플릿과 짧은 요청 해석 기준
- [docs/AgentPromptTemplates.md](docs/AgentPromptTemplates.md)
  - Claude Code 요청 템플릿과 짧은 요청 해석 기준

### 배치 검증

에디터가 닫혀 있을 때:

```powershell
tools\compile-unity.cmd
tools\smoke-unity.cmd
tools\test-editmode.cmd
tools\validate-unity.cmd
```

### 에디터 메뉴 검증

에디터가 열려 있을 때:

- `Tools/Codex Validation/Run Smoke Validation`
- `Tools/Codex Validation/Run Strict Smoke Validation`
- `Tools/Codex Validation/Run EditMode Smoke Tests`
- `Tools/Codex Validation/Run Full Validation`

### 현재 검증이 확인하는 것

- `Test_Combat` 씬 존재 여부
- `CombatSetting.prefab` 존재 여부
- 핵심 입력/데이터 자산 존재 여부
- 씬/프리팹의 missing MonoBehaviour 참조
- 기본 EditMode smoke 시나리오
  - `ApplyDamageSystem` 치명타 처리
  - 존재하지 않는 타깃 이벤트 소비

## 현재 한계

- 적 종류와 전투 패턴 수가 아직 적습니다.
- PlayMode 수준의 자동화 검증은 아직 얕습니다.
- 씬, 프리팹, VFX, ScriptableObject 튜닝은 여전히 수동 확인 비중이 높습니다.
- 게임 오버, 메타 진행, 장기 콘텐츠 루프는 본격 구현 전입니다.

## Claude Code 설정

이 저장소는 Claude Code와 함께 사용할 수 있도록 설정되어 있습니다.

### Unity MCP 서버

`.mcp.json`이 프로젝트 루트에 있으며, 로컬 Unity MCP 서버(`http://localhost:9979`)에 연결합니다.
Claude Code를 열기 전에 Unity 에디터에서 MCP 서버를 먼저 실행해야 합니다.

### 워크트리 브랜치 규칙

Claude Code는 작업 시 `claude/` 접두사를 붙인 브랜치를 사용합니다.

```
claude/<작업명>   예) claude/hardcore-roentgen
```

`--worktree` 플래그로 실행할 경우 격리된 임시 worktree가 자동 생성됩니다.

### 새 대화 시작 방법

`CLAUDE.md`와 `AGENTS.md`가 자동으로 로드되므로 프로젝트 구조를 재설명할 필요가 없습니다.

```text
[기능/버그]를 수정해줘. AGENTS.md 규칙 따르고,
수정은 [허용 범위]에서만 해줘.
Unity 에디터 열려 있으면 검증 메뉴 알려줘.
```

## 문서

- 프로젝트 개요: [README.md](README.md)
- 작업 규칙: [AGENTS.md](AGENTS.md)
- 코드 컨텍스트: [CLAUDE.md](CLAUDE.md)
- Codex 프롬프트 템플릿: [docs/CodexPromptTemplates.md](docs/CodexPromptTemplates.md)
- Claude Code 프롬프트 템플릿: [docs/AgentPromptTemplates.md](docs/AgentPromptTemplates.md)
- 기획 초안: [docs/GAME_DESIGN_DRAFT.md](docs/GAME_DESIGN_DRAFT.md)
