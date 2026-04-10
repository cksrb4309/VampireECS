# RTK

`rtk` is the shell-output compression layer for this project.

Use it to reduce LLM token consumption when the work depends on long command output.
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

RTK is most useful when the agent is using shell commands heavily.

RTK is less useful for:

- raw source file reading
- structured search tools
- Claude Code `Read`, `Grep`, `Glob`
- Unity scene/prefab parser workflows

## Install Policy

`docs/RTK.md` is a default project document, but RTK itself is **not** a required project dependency.

The kit does not assume RTK is already installed.
The kit does not give RTK ownership of project-local `AGENTS.md`, `CLAUDE.md`, or `.claude/settings.json`.

Install and update RTK in the user-global environment, not inside the Unity project.

## Codex Global Install

Official quick path:

```powershell
rtk init -g --codex
rtk init --show
```

What this does:

- installs RTK global instructions for Codex
- creates or updates global files such as `~/.codex/RTK.md`
- may update `~/.codex/AGENTS.md` to reference RTK global instructions
- keeps the Unity project harness separate from RTK itself

Review any global changes before relying on them in daily work.

## Claude Code Global Install

Official quick path:

```powershell
rtk init -g
rtk init --show
```

What this does:

- installs RTK global hook/instruction behavior for Claude Code
- keeps the Unity project harness separate from RTK itself

If you already have custom global hook behavior, merge carefully instead of assuming RTK owns the entire setup.

## Verify

Use these checks after global installation:

```powershell
rtk --version
rtk gain
rtk init --show
```

Expected result:

- RTK is callable from the shell
- token savings stats are available
- the active AI tool integration is shown

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

If the AI tool already rewrites shell commands through RTK, normal shell commands may already be compressed.
If not, call `rtk ...` commands explicitly.

For Unity validation commands such as `tools\compile-unity.cmd` or `tools\validate-unity.cmd`, prefer the normal command path when the RTK hook is active. Use explicit `rtk ...` commands only when you know the wrapper you need.

## Agent Usage Rule

When the task is shell-heavy:

1. Check whether RTK is installed or confidently available.
2. Prefer RTK for long shell outputs.
3. Still read raw source files before making non-trivial edits.
4. Do not assume RTK helps with `Read/Grep/Glob` or Unity parser tools.

When the task is structure-heavy:

- use `graphify` first if `graphify-out/GRAPH_REPORT.md` exists

When the task is Unity asset-heavy:

- use `unity-cli` or `unity-prefab-parser-mcp` first

## Troubleshooting

If RTK is not installed:

- continue the task without RTK
- mention that raw shell output may be larger than normal
- point the user to this file for global installation

If shell output still looks uncompressed:

- run `rtk init --show`
- confirm the active AI tool integration
- try explicit `rtk ...` commands instead of relying on transparent rewrite

If RTK conflicts with another global hook setup:

- keep the Unity project harness unchanged
- merge global hook behavior manually
- do not solve the conflict by weakening project-local guarded-asset or validation rules

## Reporting Rule

If RTK was relevant during a task, report:

- whether RTK was available
- whether RTK paths were used
- which shell/log-heavy step benefited from RTK
- whether any shell output was still raw
