# guard-assets-check.ps1
# PreToolUse hook: block edits to guarded Unity assets
# Only inspects file_path — does not scan file content or new_string

param()

$raw = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($raw)) { exit 0 }

try {
    $payload = $raw | ConvertFrom-Json
} catch {
    exit 0
}

$filePath = $payload.tool_input.file_path
if ([string]::IsNullOrWhiteSpace($filePath)) { exit 0 }

$guardedPatterns = @(
    '(?i)\.unity$',
    '(?i)\.prefab$',
    '(?i)[/\\]ProjectSettings[/\\]'
)

foreach ($pattern in $guardedPatterns) {
    if ($filePath -match $pattern) {
        Write-Host "[HARNESS] GUARDED ASSET BLOCKED: $filePath"
        Write-Host "This path is read-only by default (.unity / .prefab / ProjectSettings)."
        Write-Host "To edit: check AGENTS.md high-risk assets and request explicit user approval."
        exit 2
    }
}

exit 0
