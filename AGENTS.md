# NetAdmin Agent & Developer Guidelines

This repository supports both **Visual Studio (GitHub Copilot)** and **OpenCode**.
All reusable skills are defined concisely in English under `.opencode/skills/` and serve as the single source of truth:

- [Code Style & Maximum Reuse](.opencode/skills/code-style-and-reuse/SKILL.md) — C# 14 / .NET 10 conventions, DRY rules, and mandatory shared base classes in `NetAdmin.Core`.
- [Provider Architecture](.opencode/skills/provider-architecture/SKILL.md) — Rules for creating dedicated `NetAdmin.Providers.<Name>` projects, plugins, UI modules, and product catalog definitions.
- [Blazor UI & EF Core](.opencode/skills/blazor-ui-and-efcore/SKILL.md) — Blazor InteractiveServer patterns, EF Core migration rules (`MigrateAsync`), and encrypted credential storage.
- [Writing Skills](.opencode/skills/writing-skills/SKILL.md) — Rules, frontmatter format, and registration checklist for authoring new skills.

## Key Non-Negotiables
1. **Maximum Code Reuse**: Put shared methods, HTTP helpers, UI module bases, and Blazor editor bases in `NetAdmin.Core`.
2. **One Project Per Provider**: Every new integration/provider must be created in its own `src/NetAdmin.Providers.<Name>` project.
3. **English UI & Clean Categories**: Keep all UI text in English and keep `HardwareCategory` display names clean without parenthetical examples.
4. **EF Core Migrations**: Always use EF Core migrations (`MigrateAsync`) instead of raw `ALTER TABLE` statements.
5. **Track Progress**: Keep [TODO.md](TODO.md) updated as features and integrations are planned or completed.
