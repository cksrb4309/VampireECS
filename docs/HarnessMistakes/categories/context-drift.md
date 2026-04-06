# Context Drift Mistakes

## Definition

프로젝트 구조, 최신 상태, 아키텍처 경계, 기존 문서/코드의 실제 의미를 잘못 이해해서 생긴 실수입니다.

## Typical Signals

- 오래된 README/CLAUDE 설명을 그대로 믿고 잘못된 수정
- 실제 코드 구조와 다른 폴더/시스템 경계를 가정
- 이미 존재하는 기능을 새로 만들거나 이미 제거된 타입을 참조

## Prevention Rules

- 코드 수정 전 관련 파일을 직접 읽어서 현재 상태를 확인한다
- CLAUDE.md에 설명이 있어도 실제 파일로 재확인한다
- 새 능력/시스템 추가 시 기존 유사 패턴 파일을 먼저 읽고 따른다

## Incident Log

- none recorded
