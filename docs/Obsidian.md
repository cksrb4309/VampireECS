# Obsidian

This repository is intended to work as an Obsidian vault from the project root.

## Placement Rules

- Open the project root as the vault.
- Keep documentation under root docs and root markdown files.
- Do not create documentation under `Assets/`.

Reason:

- Unity imports everything under `Assets/` and creates `.meta` files for it.
- Root docs avoid unnecessary Unity churn and keep documentation independent from asset import.

## Recommended Link Pattern

Use wikilinks only when they are actually useful.

Examples:

- `[[docs/ProjectWiki/index]]`
- `[[docs/ProjectWiki/systems/SomeSystem]]`
- `[[docs/ProjectWiki/assets/SomeAsset]]`
- `[[docs/ProjectWiki/adr/ADR-001-some-decision]]`

## Working Structure

- durable knowledge -> `docs/ProjectWiki/**`
- active agent handoffs -> `docs/AgentHandoffs/**`
- local operating rules -> `AGENTS.md`, `CLAUDE.md`, `docs/*.md`
- harness mistake knowledge -> `docs/HarnessMistakes/**`

## Graph And Noise Control

- keep `docs/HarnessMistakes/**` connected as a small cluster centered on `README -> domains/README -> categories/README`
- prefer local graph, backlinks, and search over the global graph view

## Recommended Excluded Files

When the project root is used as an Obsidian vault, generated Unity folders can overwhelm the graph.

Recommended `Excluded files` patterns:

```text
Library/**
Temp/**
Logs/**
Obj/**
UserSettings/**
.git/**
```

The key exclusion is `Library/**`. That also removes `Library/PackageCache/**`, which is usually the largest source of graph noise.

## Local Vault Notes

- `.obsidian/` is optional and does not need to be committed for this harness
- keep filenames stable
- prefer short notes with clear ownership instead of one large knowledge dump
