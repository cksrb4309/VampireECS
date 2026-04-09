# Unity CLI

This project uses the local `unity-cli` MCP bridge for editor-aware checks.

Use it for:

- editor connection checks
- play/pause/stop
- console reads and clears
- running editor-side validation or tests
- safe editor-side C# execution when YAML editing would be risky

Preferred usage in this project:

1. Check editor connectivity first.
2. If the editor is open, prefer the editor validation menu instead of batchmode.
3. Keep combat logic changes in code paths first. Touch serialized assets only when required.

Project-specific validation menu:

- `Tools/Codex Validation/Run Smoke Validation`
- `Tools/Codex Validation/Run Strict Smoke Validation`
- `Tools/Codex Validation/Run EditMode Smoke Tests`
- `Tools/Codex Validation/Run Full Validation`

Expected results:

- smoke: no compile errors, core scene/prefab references intact
- full: smoke plus project EditMode smoke tests

Do not use `unity-cli` as a replacement for reading source files. Read code and docs first, then use the editor bridge for validation or asset-safe operations.
