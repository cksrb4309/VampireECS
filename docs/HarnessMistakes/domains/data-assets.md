# Domain: Data Assets

## Scope

ScriptableObject, config asset, unlock/stat/tuning 데이터, 참조 일관성.

## Typical Areas

- config assets (`Assets/09_Data/ScriptableObject/Config_ScriptableObject/Stats/**`)
- unlock assets (`Assets/09_Data/ScriptableObject/Config_ScriptableObject/Unlock/**`)
- stat tuning assets
- address/reference consistency (BatchValidationRunner RequiredAssetPaths)

## Routing Metadata

- Preload extra mistake context: no
- Preload category files: none

## Known Recurring Mistakes

- none recorded

## Update Rule

데이터 자산 경로, 참조, 수치 적용 실수가 반복되면 이 파일을 갱신하고 preload를 켠다.
