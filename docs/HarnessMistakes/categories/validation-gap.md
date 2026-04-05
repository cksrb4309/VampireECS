# Validation Gap Mistakes

## Definition

수행했어야 할 검증을 생략했거나, 불충분한 검증만으로 완료 처리한 경우입니다.

## Typical Signals

- compile만 하고 smoke/test를 생략함
- 에디터가 열려 있어서 검증을 못 돌렸는데도 완료처럼 보고함
- 새 에셋 경로를 BatchValidationRunner에 추가하지 않고 머지함

## Prevention Rules

- 에디터가 열려 있으면 `Tools/Codex Validation/Run Full Validation` 메뉴를 안내한다
- 새 ScriptableObject 에셋을 추가할 때는 BatchValidationRunner.RequiredAssetPaths를 함께 갱신한다
- 검증이 불가능한 경우 그 사실을 명시하고 사용자에게 수동 확인을 요청한다

## Incident Log

- none recorded
