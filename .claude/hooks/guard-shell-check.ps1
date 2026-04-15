# guard-shell-check.ps1
# PreToolUse hook: 위험한 셸 명령 차단
# Codex 버전과 동일한 재귀 페이로드 파싱 방식 적용

param()

$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }

try {
    $payload = $raw | ConvertFrom-Json
} catch {
    exit 0
}

function Get-StringValues {
    param([object]$Value)

    if ($null -eq $Value) { return @() }
    if ($Value -is [string]) { return @($Value) }

    if ($Value -is [System.Collections.IDictionary]) {
        $items = @()
        foreach ($entry in $Value.GetEnumerator()) {
            $items += Get-StringValues -Value $entry.Value
        }
        return $items
    }

    if ($Value -is [System.Collections.IEnumerable] -and -not ($Value -is [string])) {
        $items = @()
        foreach ($item in $Value) {
            $items += Get-StringValues -Value $item
        }
        return $items
    }

    $props = $Value.PSObject.Properties
    if ($props.Count -eq 0) { return @() }

    $items = @()
    foreach ($prop in $props) {
        $items += Get-StringValues -Value $prop.Value
    }
    return $items
}

$dangerousPatterns = @(
    '(?i)\brm(\.exe)?\b.*-(?:r|rf|fr)\b',
    '(?i)\bRemove-Item\b.*-Recurse\b',
    '(?i)\bRemove-Item\b.*-Force\b',
    '(?i)\bgit\s+reset\s+--hard\b',
    '(?i)\bgit\s+clean\s+-fdx?\b',
    '(?i)\bdel(\.exe)?\b.*\/[sqf]+\b',
    '(?i)\brmdir(\.exe)?\b.*\/[sqf]+\b',
    '(?i)\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item)\b.*graphify-out[\\/]',
    '(?i)\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item)\b.*Assets[\\/].*\.(md|txt|markdown)\b',
    '(?i)\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item)\b.*(?:Library|Temp|Logs|Obj)[\\/]'
)

$strings = Get-StringValues -Value $payload
foreach ($candidate in $strings) {
    foreach ($pattern in $dangerousPatterns) {
        if ($candidate -match $pattern) {
            $msg = @"
[HARNESS] DANGEROUS SHELL COMMAND BLOCKED
명령: $candidate

위험한 셸 명령이 차단됐습니다:
  - rm -rf / Remove-Item -Recurse -Force 형태의 재귀 삭제
  - git reset --hard / git clean -fdx 형태의 파괴적 git 명령
  - del /f, rmdir /s 형태의 Windows 파괴적 명령
  - Library/, Temp/, Assets/ 하위 파일 덮어쓰기

노이즈가 많은 명령은 RTK(docs/RTK.md)를 사용한다.
파괴적 작업이 꼭 필요하면 사용자에게 명시적 허가를 요청한다.
"@
            Write-Host $msg
            exit 2
        }
    }
}

exit 0
