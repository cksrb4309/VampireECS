# RTK

`rtk` is an optional shell-output compression layer. It is useful when the task depends on long shell output.

Use RTK for:

- `git status`, `git diff`, `git log`
- `rg` / `grep`
- test output
- Unity batch validation logs
- build or lint logs

Do not treat RTK as a replacement for:

- `AGENTS.md`
- `CLAUDE.md`
- source file reading
- `unity-cli`
- `unity-parser`

## Install Policy

RTK is not a project dependency. Install and maintain it globally.

Codex global path:

- `C:\Users\rlack\.codex\RTK.md`

Typical checks:

```powershell
rtk --version
rtk gain
rtk init --show
```

## Project Usage Rule

If a task is shell-heavy:

1. prefer RTK for long command output
2. still read raw source files before non-trivial edits
3. do not assume RTK helps with structured editor-side tools

If RTK is not available, continue without it and report that shell output stayed raw.
