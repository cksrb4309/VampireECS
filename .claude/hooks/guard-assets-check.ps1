param()

$stdin = [Console]::In.ReadToEnd()
if (-not $stdin) { exit 0 }

try {
    $data = $stdin | ConvertFrom-Json
} catch {
    exit 0
}

$toolName = $data.tool_name
if ($toolName -notin @("Edit", "Write")) { exit 0 }

$filePath = $data.tool_input.file_path
if (-not $filePath) { exit 0 }

$guardedPatterns = @(
    '[/\\]Assets[/\\]07_Scenes[/\\].*\.unity$',
    '[/\\]Assets[/\\]06_Prefabs[/\\]',
    '[/\\]Assets[/\\]09_Data[/\\]ScriptableObject[/\\]',
    '[/\\]Assets[/\\]00_Core[/\\]ProjectSetting[/\\]',
    '[/\\]ProjectSettings[/\\]'
)

foreach ($pattern in $guardedPatterns) {
    if ($filePath -match $pattern) {
        $msg = @"
[HARNESS] GUARDED ASSET BLOCKED
File: $filePath

This path is guarded by the local Unity harness.

Blocked by default:
  - Assets/07_Scenes/**/*.unity
  - Assets/06_Prefabs/**
  - Assets/09_Data/ScriptableObject/**
  - Assets/00_Core/ProjectSetting/**
  - ProjectSettings/**

If this edit is truly required:
  1. confirm the exact guarded asset in the task
  2. check AGENTS.md for the write-boundary rules
  3. temporarily relax the deny rule intentionally
"@
        Write-Host $msg
        exit 1
    }
}

exit 0
