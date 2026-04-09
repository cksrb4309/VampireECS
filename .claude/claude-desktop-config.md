# Claude Desktop Config

If Claude Desktop is used with this project, keep the global config in merge mode.

Target file on this machine:

`C:\Users\rlack\AppData\Local\Packages\Claude_pzs8sxrjxfjjc\LocalCache\Roaming\Claude\claude_desktop_config.json`

Rules:

- never replace the whole file just for this project
- preserve existing `preferences`
- preserve unrelated `mcpServers`
- merge only the entries this project actually needs

Recommended entries for this project:

- `unity-cli`
- `unity-parser`
- `ollama-vision`
- `image-tools`

Use global MCP config for Claude Desktop. Keep project-specific MCP additions in repo-local `.mcp.json` only when they are useful across both Codex and Claude Code.
