# VampireECS — Claude 컨텍스트 파일

## 프로젝트 개요
- **장르**: Vampire Survivors 류 액션 RPG
- **아키텍처**: 100% Unity DOTS/ECS (Unity.Entities 기반)
- **주요 언어**: C#

---

## 폴더 구조

```
Assets/
├── 00_Core/         # 프로젝트 설정 (RP Asset 등)
├── 01_Scripts/      # 모든 C# 스크립트 (139개)
├── 02_Art/          # 3D 모델, 메시
├── 03_Animations/   # 애니메이션
├── 04_Effects/      # VFX, 파티클 (VFX Graph 포함)
├── 05_Audio/        # 사운드
├── 06_Prefabs/      # 프리팹
├── 07_Scenes/       # 씬 (Test_Combat.unity 등)
├── 08_UI/           # UI 프리팹
├── 09_Data/         # ScriptableObject 데이터
└── 10_ThirdParty/   # 외부 라이브러리
```

---

## 스크립트 구조 (`Assets/01_Scripts/`)

### ECS/ — 핵심 게임 로직

#### Attack/ — 공격 시스템
- `Components/Base/AuraData.cs` — 오라(범위 공격) 컴포넌트 (ElapsedTime, OwnerFaction)
- `Components/Base/ShooterData.cs` — 사수 컴포넌트 (Direction, MuzzleOffset, ProjectilePrefab)
- `Components/AuraStatsData.cs` — 오라 스탯 (Damage, AttackSpeed, Radius)
- `Components/ShooterStatsData.cs` — 사수 스탯 (Damage, AttackSpeed, ProjectileSpeed, Count, Duration)
- `Components/ProjectileData.cs` — 투사체 (Direction, Speed, Damage, Duration, OwnerFaction)
- `Components/FactionData.cs` — Faction enum (Player=1, Enemy=2, Flags)
- `Systems/Base/AuraSystem.cs` — 공간 분할 기반 범위 피해 처리
- `Systems/Base/ShooterSystem.cs` — 투사체 생성
- `Systems/ProjectileMoveSystem.cs` — 투사체 이동
- `Systems/ProjectileTriggerSystem.cs` — 투사체 충돌 (ITriggerEventsJob)

#### Combat/ — 전투 시스템
- `Components/HealthData.cs` — 체력 (Current, Max)
- `Components/DamageEventData.cs` — 피해 이벤트 (버퍼 엔티티)
- `Components/DeadTag.cs` — 사망 태그
- `Systems/ApplyDamageSystem.cs` — HealthData 감소 + DeadTag + DamageTextEvent 생성
- `Systems/EnemyDeathSystem.cs` — 적 제거 + ExperienceGainEvent 생성
- `Systems/PlayerDeathSystem.cs` — 플레이어 사망 처리

#### Common/ — 공용 유틸
- `Components/SpatialCell.cs`, `SpatialIndex.cs` — 공간 분할 구조체
- `Systems/SpatialPartitionBuildSystem.cs` — NativeParallelMultiHashMap 빌드
- `Systems/SpatialPartitionUpdateSystem.cs` — 이전 위치 기반 업데이트
- `Utils/SpatialUtility.cs` — WorldToCell, CellToWorldCenter, QueryRadius
- `Utils/EntityUtility.cs` — 싱글톤 엔티티 관리 (비동기 포함)
- `Interfaces/IAddable<T>` — 스탯 누적 합산 인터페이스
- `Interfaces/IInitializableStats<T>` — 스탯 초기화 인터페이스

#### Enemy/ — 적 시스템
- `Components/EnemyTag.cs`, `EnemyMoveData.cs`, `EnemySpawnerData.cs`
- `Systems/EnemyFollowSystem.cs` — 플레이어 추적 이동
- `Systems/EnemySpawnSystem.cs` — 시간 기반 단계(Stage) 스포닝
- `Systems/EnemyTargetDirectionSystem.cs` — 사수형 적 조준 방향 계산

#### Player/ — 플레이어 시스템
- `Components/PlayerTag.cs`, `PlayerInputData.cs` (Move: float2), `PlayerMoveData.cs`, `PlayerExpData.cs`
- `Systems/PlayerMoveSystem.cs` — 가속/감속 이동
- `Systems/PlayerRotationSystem.cs` — DynamicRotationData 기반 회전

#### Experience/ — 경험치 시스템
- `ExperienceGainEvent.cs`, `ExperienceSystem.cs`, `LevelUpUIRequest.cs`
- 흐름: EnemyDeathSystem → ExperienceGainEvent → ExperienceSystem → LevelUpUIRequest → ExperienceBridge → TimePauseController / AbilityRewardGenerator

#### Global/ — 전역 싱글톤 컴포넌트
- `GameTimeScale.cs` — 게임 시간 스케일
- `AbilityPrefabLibrary.cs` — 능력 프리팹 라이브러리

#### Transform/ — 변환 시스템
- DynamicRotation, LockYToZero, ShrinkOverTime, RandomRotation 등

#### SystemGroups/ — 실행 순서 정의
```
CombatRootSystemGroup (SimulationSystemGroup 내)
├── SpatialUpdatePreparationGroup   ← 이전 위치 스냅샷
├── SpatialSetupSystemGroup         ← 공간 인덱스 빌드
├── DamageSetupSystemGroup          ← 이동 처리, 오라/투사체 생성
├── DamageEventSystemGroup          ← 충돌 감지 (Physics)
├── DamageApplySystemGroup          ← HealthData 감소 + 이벤트 생성
├── DestructionCleanupSystemGroup   ← 적 제거
└── DestructionSystemGroup          ← 플레이어 처리
```

---

### UI/ — UI 시스템

#### DamageText/ — 데미지 텍스트 (VFX Graph 기반)
- `DamageTextProvider.cs` — 정적 진입점 `ShowDamageText(Vector3, int, Color)`
- `DamageTextVfxBatchEmitter.cs` — 싱글톤, GraphicsBuffer GPU 배치, 최대 100개 동시
- `DamageTextBufferLayout.cs` — GPU 버퍼 레이아웃 (`TextInstanceData` 64B, `GlyphData` 32B)

#### 기타 UI 폴더
- `Binders/` — UI 데이터 바인딩
- `Bridge/` — ECS ↔ UI 연결 브릿지
- `Controller/` — UI 컨트롤러
- `Bootstrap/` — UI 초기화

---

### Presentation/ — 표현 계층
- `VFX/AuraViewManager.cs` — 오라 VFX 뷰 관리 (싱글톤)
- `VFX/VFXAuraView.cs` — 오라 VFX 개별 컴포넌트

### System/ — 게임 시스템
- `Time/TimePauseController.cs` — GameTimeScale 제어 (일시정지/재개)
- `Camera/FollowCamera.cs` — 플레이어 추적 카메라 (UniTask)

### Ability/ — 능력/스킬 시스템
- `AbilityConfig.cs` — 기본 클래스 (Tier, MaxStack, Icon)
- `Config/Stats/` — AuraStatsConfig, ShooterStatsConfig, CombatStatsConfig
- `Config/Unlock/` — UnlockAbilityConfig, UnlockAuraConfig, UnlockShooterConfig
- `PlayerStatApplier.cs` — 플레이어 스탯 적용 (비동기)

### Core/ — 핵심 유틸
- `Manager/InputManager.cs` — InputAction 매핑
- `Util/BattleSceneLifetimeScope.cs` — DI 생명주기
- `Util/Singleton.cs` — 싱글톤 베이스 클래스

---

## 데미지 처리 전체 흐름

```
[AuraSystem / ProjectileHitDetectionSystem]
        ↓  DamageEventData 생성
[ApplyDamageSystem]
        ↓  HealthData 감소 + DeadTag 추가 + DamageTextEvent 생성
[DamageTextSystem]
        ↓  DamageTextProvider.ShowDamageText() 호출
[DamageTextProvider.ShowDamageText()]
        ↓
[DamageTextVfxBatchEmitter] → VFX Graph → 화면 표시
        ↓
[EnemyDeathSystem]
        ↓  엔티티 제거 + ExperienceGainEvent 생성
[ExperienceSystem]
        ↓  레벨업 시 LevelUpUIRequest 생성
[ExperienceBridge]
        ↓  LevelUpUIRequest 감지 및 소비
[TimePauseController.SetPause(true)]
        ↓
[AbilityRewardGenerator.GenerateRewardChoices()]
```

---

## 핵심 설계 패턴

| 패턴 | 적용 위치 |
|------|-----------|
| `IAddable<T>` | 스탯 누적 합산 (AuraStatsData, ShooterStatsData, CombatStatsData) |
| `IInitializableStats<T>` | 스탯 초기값 설정 |
| `IJobEntity` | 병렬 엔티티 처리 (Burst 컴파일) |
| `ITriggerEventsJob` | 물리 충돌 감지 |
| 싱글톤 컴포넌트 | 전역 상태 (GameTimeScale, AbilityPrefabLibrary) |
| 이벤트 버퍼 엔티티 | DamageEventData, ExperienceGainEvent, LevelUpUIRequest |

---

## 외부 의존성

| 라이브러리 | 용도 |
|-----------|------|
| `Unity.Entities` | ECS 핵심 |
| `Unity.Physics` | 충돌 감지 |
| `Unity.Mathematics` | 수학 연산 |
| `Unity.Burst` | JIT 컴파일 최적화 |
| `Unity.VFX Graph` | 오라/데미지 텍스트 렌더링 |
| `UniTask (Cysharp)` | 비동기 처리 |
| `Odin Inspector` | 에디터 UI 확장 |

---

## 현재 활성 브랜치 작업 이력

- `feature/damage-text-ui` — VFX Graph 기반 데미지 텍스트 UI 구현 중
  - `DamageTextBufferLayout.cs` 신규 추가 (GPU 버퍼 레이아웃)
  - `DamageTextVfxBatchEmitter.cs` 배치 렌더링 로직 구현
  - `TextVFXGraph.vfx` 수정
