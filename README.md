# VampireECS

Unity 6 `6000.3.11f1` 기반의 3D Vampire Survivors 스타일 전투 프로토타입입니다.  
이 프로젝트는 콘텐츠 양보다 `전투 루프를 ECS 중심으로 안정적으로 설계하고 확장하는 것`에 초점을 두고 있습니다.

전투 상태와 핵심 게임플레이는 DOTS/ECS가 담당하고, 입력, UI, 카메라, VFX 같은 씬 의존 로직은 브리지 레이어에서 처리합니다.  
즉, "전부 ECS로 옮기는 것"이 아니라 `성능 민감한 전투 루프만 ECS로 관리하는 구조`를 목표로 했습니다.

## 기술 스택

`Unity` `C#` `DOTS/ECS` `Burst` `Jobs` 

## 프로젝트 목표

- 전투 상태를 ECS에 유지하고, 프레젠테이션 의존성을 분리하기
- 공격 종류가 늘어나도 구조가 무너지지 않도록 시스템과 데이터를 정리하기
- 레벨업, 보상 선택, 능력치 적용까지 하나의 전투-성장 루프로 연결하기

## 구현한 핵심 내용

### 1. ECS 중심 전투 루프

- 플레이어 입력은 브리지 계층에서 ECS 친화적인 상태로 전달
- ECS 시스템이 이동, 타겟팅, 공격 생성, 피해 처리, 사망, 경험치 획득을 담당
- 레벨업 시 시간을 멈추고 보상 선택 UI를 열어 성장 루프로 연결

대표 시스템:
- `Assets/01_Scripts/ECS/Player/Systems/PlayerMoveSystem.cs`
- `Assets/01_Scripts/ECS/Enemy/Systems/EnemySpawnSystem.cs`
- `Assets/01_Scripts/ECS/Combat/Systems/ApplyDamageSystem.cs`
- `Assets/01_Scripts/ECS/Combat/Systems/EnemyDeathSystem.cs`
- `Assets/01_Scripts/ECS/Experience/ExperienceSystem.cs`

전투 루프:

```text
Input Bridge
  -> ECS player state
  -> movement / targeting / attack generation
  -> damage application / death cleanup
  -> experience / level-up request
  -> reward selection
  -> stat application back into ECS
```

### 2. 공격 계열 확장 구조

현재 전투 계열:

- `Shooter`
- `Aura`
- `Boomerang`
- `Orbit`
- `Chain Lightning`
- `Meteor Strike`
- `Black Hole`

단일 하드코딩 스킬이 아니라, 공격 계열별 설정과 런타임 데이터를 분리해서 확장 가능한 구조로 구성했습니다.  
새 공격을 추가할 때는 ECS 시스템, 설정 데이터, 시각 연출 계층이 같은 규칙으로 따라오도록 정리했습니다.

관련 파일:
- `Assets/01_Scripts/Ability/Config/Unlock/UnlockShooterConfig.cs`
- `Assets/01_Scripts/Ability/Config/Unlock/UnlockChainLightningConfig.cs`
- `Assets/01_Scripts/Ability/Config/Stats/ShooterStatsConfig.cs`
- `Assets/01_Scripts/Ability/Config/Stats/ChainLightningStatsConfig.cs`
- `Assets/01_Scripts/Ability/AbilityRewardGenerator.cs`

### 3. 데이터 기반 성장과 능력치 적용

능력 해금과 스택형 강화 수치를 분리해서 관리합니다.

- `Unlock...Config`
  - 해금 시점과 기본 성능 정의
- `...StatsConfig`
  - 레벨업/보상으로 누적되는 강화 수치 정의
- `...BaseStatsData`
  - 런타임 기준 성능값
- `...StatsData`
  - 누적 보너스 수치
- `CombatStatsData`
  - 전체 전투 공통 배율

의도한 계산 방식:

```text
final stat = base * (1 + local bonus) * combat multiplier
```

이 구조로 공격력 같은 배율형 수치와, 개수/반경/타이머 같은 비배율 수치를 분리해서 다룰 수 있게 했습니다.

관련 파일:
- `Assets/01_Scripts/Ability/Config/Unlock/UnlockAbilityConfig.cs`
- `Assets/01_Scripts/Ability/Config/Stats/AbilityStatsConfig.cs`
- `Assets/01_Scripts/Ability/Config/Stats/CombatStatsConfig.cs`
- `Assets/01_Scripts/Ability/Config/Stats/PlayerStatApplier.cs`

### 4. ECS와 연출 계층의 분리

이 프로젝트에서 중요한 설계 포인트는 `ECS가 VFX 오브젝트를 직접 관리하지 않는다`는 점입니다.

ECS는 "무슨 일이 일어났는지"만 이벤트 엔티티로 발행하고,  
실제 연출 생성과 뷰 갱신은 Presentation 계층이 담당합니다.

흐름:

```text
ECS combat systems
  -> visual event entity
  -> presentation bridge system
  -> managed VFX / pooled view objects
```

대표 파일:
- `Assets/01_Scripts/ECS/Visual/Systems/ChainLightningPresentationSystem.cs`
- `Assets/01_Scripts/ECS/Visual/Systems/MeteorStrikePresentationSystem.cs`
- `Assets/01_Scripts/ECS/Visual/Systems/BlackHolePresentationSystem.cs`

이 방식으로 전투 로직과 씬 오브젝트 수명주기를 분리해, 시스템 간 결합도를 낮추고 연출 변경 비용을 줄이려 했습니다.

## 아키텍처 요약

### 레이어 분리

- `ECS`
  - 전투 상태, 이동, 공격, 피해, 사망, 경험치, 전투 이벤트
- `UI / System / Presentation`
  - 입력, 카메라, 레벨업 UI, 데미지 텍스트, VFX, 뷰 오브젝트
- `Data`
  - ScriptableObject 기반 해금/성장/전투 수치 설정

### 시스템 흐름

```text
1. spatial preparation
2. spatial index build
3. attack setup and movement
4. hit detection
5. damage application
6. destruction cleanup
7. presentation event consumption
```

핵심은 새 시스템을 느슨하게 붙이는 것이 아니라, 이미 정의된 ECS 시스템 그룹 순서 안에 맞춰 넣는 것입니다.

## 저장소 구조

```text
Assets/01_Scripts/ECS/            전투 핵심 ECS 시스템과 컴포넌트
Assets/01_Scripts/ECS/Visual/     ECS 시각 이벤트와 연출 브리지
Assets/01_Scripts/Ability/        해금, 보상, 능력치 적용 로직
Assets/01_Scripts/UI/             ECS-to-UI 브리지와 UI 컨트롤러
Assets/01_Scripts/Presentation/   VFX, 뷰 매니저, 풀링된 연출 오브젝트
Assets/09_Data/                   ScriptableObject 기반 전투/성장 데이터
Assets/99_Tests/                  EditMode 테스트
Assets/Editor/CodexValidation/    검증 메뉴와 배치 진입점
tools/                            컴파일, 스모크, 테스트 실행 스크립트
```

## 실행

메인 검증 씬:

- `Assets/07_Scenes/Test_Combat.unity`

주요 씬 설정 프리팹:

- `Assets/06_Prefabs/Scene/CombatSetting.prefab`

## 이 프로젝트에서 보여주고 싶은 것

이 저장소는 단순히 "뱀서라이크를 만들었다"보다 아래 내용을 보여주기 위한 작업물입니다.

- ECS를 어디에 써야 하는지 판단하고 구조를 분리한 설계 능력
- 전투 루프를 데이터 기반으로 확장 가능하게 만든 시스템 설계
- 연출과 핵심 로직을 분리해 유지보수성과 확장성을 확보한 방식

## 문서

프로젝트 내부 문서:

- [AGENTS.md](AGENTS.md)
- [CLAUDE.md](CLAUDE.md)
- [docs/project-context.md](docs/project-context.md)
- [docs/CodexPromptTemplates.md](docs/CodexPromptTemplates.md)
- [docs/AgentPromptTemplates.md](docs/AgentPromptTemplates.md)
