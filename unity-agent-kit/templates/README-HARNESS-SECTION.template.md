## Harness And Validation

This repository uses a practical harness, not prompt-only operation.

Recommended linked docs:

- `AGENTS.md`
- `CLAUDE.md`
- `docs/AgentPromptTemplates.md`
- `docs/HarnessMistakes/README.md`
- `docs/Graphify.md` (optional)

Suggested validation commands:

```powershell
tools\compile-unity.cmd
tools\smoke-unity.cmd
tools\test-editmode.cmd
tools\validate-unity.cmd
```

If the Unity Editor is already open, expose matching editor menu items such as:

- `Tools/Harness Validation/Run Smoke Validation`
- `Tools/Harness Validation/Run Strict Smoke Validation`
- `Tools/Harness Validation/Run EditMode Smoke Tests`
- `Tools/Harness Validation/Run Full Validation`

Replace this section with project-specific details about:

- main validation scene
- guarded prefabs or data assets
- current smoke test coverage
- how mistake categorization is recorded after a confirmed failure
- whether `graphify` is used for structure orientation and how it is refreshed
