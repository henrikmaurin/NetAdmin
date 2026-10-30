# Copilot Instructions

## Project Guidelines
- Use EF Core migrations (MigrateAsync) instead of raw ALTER TABLE statements, keep the application UI language in English, and keep category names clean without parenthetical examples.
- Maximize shared methods, base classes, and components in `NetAdmin.Core`. Every new provider must have its own `src/NetAdmin.Providers.<Name>` project.
- Follow the shared OpenCode skills (defined in English) for all architecture and coding standards:
  - [AGENTS.md](../AGENTS.md)
  - [.opencode/skills/code-style-and-reuse/SKILL.md](../.opencode/skills/code-style-and-reuse/SKILL.md)
  - [.opencode/skills/provider-architecture/SKILL.md](../.opencode/skills/provider-architecture/SKILL.md)
  - [.opencode/skills/blazor-ui-and-efcore/SKILL.md](../.opencode/skills/blazor-ui-and-efcore/SKILL.md)
  - [.opencode/skills/writing-skills/SKILL.md](../.opencode/skills/writing-skills/SKILL.md)
  - [TODO.md](../TODO.md)