# Tooling Automation Gap Mistakes

## Definition

도구 동작 방식, 배치 검증 한계, 에디터 상태, 자동화 가능 범위를 잘못 이해해서 생긴 실수입니다.

## Typical Signals

- 에디터가 열린 상태에서도 batchmode가 될 거라고 잘못 가정
- GUI 메뉴를 에이전트가 직접 실행할 수 있다고 오해
- tools\ 스크립트가 없는데 실행하려 함

## Prevention Rules

- Unity 에디터가 열려 있으면 batchmode 스크립트를 실행하지 않는다
- 에디터가 열려 있으면 사용자가 직접 실행할 메뉴 경로와 기대 결과를 안내한다
- tools\ 스크립트 실행 전 파일 존재를 확인한다

## Incident Log

- none recorded
