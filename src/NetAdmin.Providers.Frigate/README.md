NetAdmin.Providers.Frigate
=========================

Provider integration for Frigate (video surveillance) integrations. Implements connectors to Frigate APIs to surface camera events and metadata.

Key points:
- Conforms to Provider Architecture described in .opencode/skills/provider-architecture/SKILL.md
- Reuse shared models from NetAdmin.Core.
- The NVR editor can link Frigate camera definitions to existing camera inventory items or create new items, synchronizing every configured FFmpeg input into the camera's stream definitions.

Build
- Build with `dotnet build`.
