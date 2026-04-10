# Obsidian

This project treats the repository root as the documentation vault.

## Placement Rule

- Keep generated notes in the project root docs tree.
- Do not create documentation folders under `Assets/`.
- Use `docs/ProjectWiki/**` for durable notes, ADRs, and system references.
- Use `[[wiki links]]` only when they improve navigation.

## Minimum Vault Rules

- Root docs live in the repository, not in `Assets/`.
- Handoff notes should be concise and move from `pending/` to `consumed/` once read.
- Long-lived project knowledge should live in `docs/ProjectWiki/**`.

## Recommended Linking Pattern

- System note: `[[docs/ProjectWiki/systems/SomeSystem]]`
- Asset note: `[[docs/ProjectWiki/assets/SomeAsset]]`
- ADR: `[[docs/ProjectWiki/adr/ADR-001-some-decision]]`

## Local Graph Guidance

- Use `docs/ProjectWiki/index.md` as the main hub note.
- Keep orphan notes to a minimum.
- Do not move harness documentation under `Assets/`.

## Excluded Files

Recommended Obsidian excludes:

```text
Library/**
Temp/**
Logs/**
Obj/**
UserSettings/**
.git/**
```

`Library/PackageCache/**` is usually excluded by the broader `Library/**` rule.
