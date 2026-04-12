# RTK

`rtk` is the shell-output compression layer for this project.

Use it to reduce token consumption when a task depends on long command output.
Do not treat it as a replacement for:

- `AGENTS.md`
- `CLAUDE.md`
- harness validation gates
- `graphify`
- `unity-cli`
- `unity-prefab-parser-mcp`

## Purpose

Use RTK for:

- `git status`, `git diff`, `git log`
- `rg` / `grep`
- test runner output
- Unity batch validation logs
- build and lint output

## Install Policy

`docs/RTK.md` is a default project document, but RTK itself is not a required project dependency.
Install and update RTK in the user-global environment, not inside the Unity project.

## Verify

Use these checks after a global install:

```powershell
rtk --version
rtk gain
rtk init --show
```

## Unity Project Usage

Prefer RTK for shell-heavy tasks such as:

```powershell
rtk git status
rtk git diff
rtk git log -n 10
rtk grep "DamageSystem" .
rtk read Assets/01_Scripts/Combat/DamageSystem.cs
rtk log Logs\Player.log
rtk summary build-output.txt
```

## Agent Usage Rule

When the task is shell-heavy:

1. Check whether RTK is installed or confidently available.
2. Prefer RTK for long shell outputs.
3. Still read raw source files before making non-trivial edits.
4. Do not assume RTK helps with `Read`, `Grep`, `Glob`, or Unity parser tools.
