# ProjectWiki

This folder is the durable project knowledge hub.

Use it for long-lived notes, system references, and ADRs. Keep top-level harness docs outside `Assets/`.

## Structure

```text
docs/ProjectWiki/
- README.md          hub note
- index.md           central index
- log.md             append-only activity log
- systems/           system notes
- assets/            asset notes
- adr/               architecture decision records
```

## When to Create a Note

- system investigation or design: `systems/<SystemName>.md`
- asset or config reference: `assets/<AssetName>.md`
- architecture decision: `adr/ADR-<NNN>-<slug>.md`
- reusable project knowledge that should survive handoffs: use a durable note here, not a transient handoff note

## Writing Rules

- Keep notes concise and stable.
- Add backlinks only when they materially improve navigation.
- Keep `index.md` updated when you add a durable note.
- Append only to `log.md`.
