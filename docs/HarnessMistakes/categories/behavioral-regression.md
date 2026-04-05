# Behavioral Regression Mistakes

## Definition

최소 안전 게이트는 통과했지만, 사용자가 의도한 동작을 놓쳤거나 실제 플레이 동작이 깨진 경우입니다.

## Typical Signals

- compile/smoke는 통과하지만 기능이 원하는 대로 동작하지 않음
- 새 기능이 기존 흐름을 조용히 깨뜨림
- 데미지/스폰/능력 흐름이 변경됐는데 테스트가 없음

## Prevention Rules

- 기존 기능을 수정할 때는 ApplyDamageSystem smoke 시나리오 등 기존 EditMode 테스트를 반드시 통과시킨다
- 새 공격 능력 추가 시에는 최소한 CastSystem이 스폰하는 프로젝타일 수와 기본 수명이 올바른지 확인한다
- gameplay 흐름 변경 시 변경 전후 차이를 보고서에 명시한다

## Incident Log

- none recorded
