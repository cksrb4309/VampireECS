# guard-shell-check.ps1
# PreToolUse hook: block dangerous shell commands
# Only inspects tool_input.command — does not scan description or other fields

param()

$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }

try {
    $payload = $raw | ConvertFrom-Json
} catch {
    exit 0
}

$command = $payload.tool_input.command
if ([string]::IsNullOrWhiteSpace($command)) { exit 0 }

$dangerousPatterns = @(
    '(?i)\brm(\.exe)?\b.*-(?:r|rf|fr)\b',
    '(?i)\bRemove-Item\b.*-Recurse\b',
    '(?i)\bRemove-Item\b.*-Force\b',
    '(?i)\bgit\s+reset\s+--hard\b',
    '(?i)\bgit\s+clean\s+-fdx?\b',
    '(?i)\bdel(\.exe)?\b.*\/[sqf]+\b',
    '(?i)\brmdir(\.exe)?\b.*\/[sqf]+\b',
    '(?i)\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item)\b.*graphify-out[/\\]',
    '(?i)\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item)\b.*Assets[/\\].*\.(md|txt|markdown)\b',
    '(?i)\b(Set-Content|Add-Content|Out-File|New-Item|Copy-Item|Move-Item|Remove-Item)\b.*(?:Library|Temp|Logs|Obj)[/\\]'
)

foreach ($pattern in $dangerousPatterns) {
    if ($command -match $pattern) {
        Write-Host "[HARNESS] DANGEROUS SHELL COMMAND BLOCKED"
        Write-Host "Command: $command"
        Write-Host "Matched pattern: $pattern"
        Write-Host "Use RTK (docs/RTK.md) for noisy output."
        Write-Host "Request explicit user approval before destructive operations."
        exit 2
    }
}

exit 0
