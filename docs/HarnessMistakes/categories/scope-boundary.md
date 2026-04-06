# Scope Boundary Mistakes

## Definition

허용되지 않은 파일이나 자산을 건드렸거나, 필요한 범위를 넘어서 수정했거나, unrelated user changes를 훼손한 경우입니다.

## Typical Signals

- guarded asset을 명시적 허용 없이 수정함
- code-only 작업인데 씬/프리팹/ScriptableObject를 함께 수정함
- 사용자가 작업 중이던 dirty 파일을 건드림

## Prevention Rules

- task가 code-only로 해결 가능하면 절대 씬/프리팹/SO를 건드리지 않는다
- 수정 전 git status를 확인해서 사용자 dirty 파일을 파악한다
- guarded asset에 접근이 필요하면 작업 전 사용자에게 명시적으로 확인한다

## Incident Log

- none recorded
