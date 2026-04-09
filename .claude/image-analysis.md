# Image Analysis

This project can use local image-analysis MCP tools when a task depends on screenshots, materials, UI layouts, or visual debugging.

Available companion tools on this machine:

- `ollama-vision`
- `image-tools`

Use them for:

- reading UI screenshots
- checking color usage in materials or textures
- comparing expected and actual scene output
- measuring layout positions from screenshots

Do not use them as a substitute for source inspection when the issue is clearly in code or serialized data.

If a visual task depends on a screenshot:

1. describe the screenshot with `ollama-vision`
2. use `image-tools` only for precise measurements or color sampling
3. report inferred observations separately from measured values

This project does not require these tools to be installed inside the repo. They are global companion tools only.
