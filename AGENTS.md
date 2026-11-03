# NetAdmin Agent & Developer Guidelines

This repository supports both **Visual Studio (GitHub Copilot)** and **OpenCode**.
All reusable skills are defined in English under `.opencode/skills/`; the skill files are the single source of truth. OpenCode and Visual Studio should follow these same files rather than maintaining copies:

- [API and Interface Design](.opencode/skills/api-and-interface-design/SKILL.md)
- [Blazor UI and EF Core](.opencode/skills/blazor-ui-and-efcore/SKILL.md)
- [Changelog Generate](.opencode/skills/changelog-generate/SKILL.md)
- [CI/CD and Automation](.opencode/skills/ci-cd-and-automation/SKILL.md)
- [CI Pipeline](.opencode/skills/ci-pipeline/SKILL.md)
- [Code Review and Quality](.opencode/skills/code-review-and-quality/SKILL.md)
- [Code Simplification](.opencode/skills/code-simplification/SKILL.md)
- [Code Style and Reuse](.opencode/skills/code-style-and-reuse/SKILL.md)
- [Constraint-Driven Development](.opencode/skills/constraint-driven-development/SKILL.md)
- [Context Engineering](.opencode/skills/context-engineering/SKILL.md)
- [Debugging and Error Recovery](.opencode/skills/debugging-and-error-recovery/SKILL.md)
- [Deprecation and Migration](.opencode/skills/deprecation-and-migration/SKILL.md)
- [Dependency Audit](.opencode/skills/dependency-audit/SKILL.md)
- [Design MD Reference](.opencode/skills/design-md-reference/SKILL.md)
- [Design Taste Frontend](.opencode/skills/design-taste-frontend/SKILL.md)
- [Design Workflow](.opencode/skills/design-workflow/SKILL.md)
- [Deploy](.opencode/skills/deploy/SKILL.md)
- [Documentation and ADRs](.opencode/skills/documentation-and-adrs/SKILL.md)
- [Doubt-Driven Development](.opencode/skills/doubt-driven-development/SKILL.md)
- [Docker Optimize](.opencode/skills/docker-optimize/SKILL.md)
- [Frontend UI Engineering](.opencode/skills/frontend-ui-engineering/SKILL.md)
- [Git Release](.opencode/skills/git-release/SKILL.md)
- [Git Workflow and Versioning](.opencode/skills/git-workflow-and-versioning/SKILL.md)
- [High-End Visual Design](.opencode/skills/high-end-visual-design/SKILL.md)
- [Image to Code](.opencode/skills/image-to-code/SKILL.md)
- [Industrial Brutalist UI](.opencode/skills/industrial-brutalist-ui/SKILL.md)
- [Incremental Implementation](.opencode/skills/incremental-implementation/SKILL.md)
- [Interview Me](.opencode/skills/interview-me/SKILL.md)
- [Idea Refine](.opencode/skills/idea-refine/SKILL.md)
- [Migration](.opencode/skills/migration/SKILL.md)
- [Minimalist UI](.opencode/skills/minimalist-ui/SKILL.md)
- [Observability and Instrumentation](.opencode/skills/observability-and-instrumentation/SKILL.md)
- [Performance Optimization](.opencode/skills/performance-optimization/SKILL.md)
- [Planning and Task Breakdown](.opencode/skills/planning-and-task-breakdown/SKILL.md)
- [Playwright CLI](.opencode/skills/playwright-cli/SKILL.md)
- [PR Review](.opencode/skills/pr-review/SKILL.md)
- [Provider Architecture](.opencode/skills/provider-architecture/SKILL.md)
- [Redesign Existing Projects](.opencode/skills/redesign-existing-projects/SKILL.md)
- [Security and Hardening](.opencode/skills/security-and-hardening/SKILL.md)
- [Shipping and Launch](.opencode/skills/shipping-and-launch/SKILL.md)
- [Spec-Driven Development](.opencode/skills/spec-driven-development/SKILL.md)
- [Test-Driven Development](.opencode/skills/test-driven-development/SKILL.md)
- [Test Patterns](.opencode/skills/test-patterns/SKILL.md)
- [Using Agent Skills](.opencode/skills/using-agent-skills/SKILL.md)
- [Web Design Guidelines](.opencode/skills/web-design-guidelines/SKILL.md)
- [Writing Skills](.opencode/skills/writing-skills/SKILL.md)

## Key Non-Negotiables
1. **Maximum Code Reuse**: Put shared methods, HTTP helpers, UI module bases, and Blazor editor bases in `NetAdmin.Core`.
2. **One Project Per Provider**: Every new integration/provider must be created in its own `src/NetAdmin.Providers.<Name>` project.
3. **English UI & Clean Categories**: Keep all UI text in English and keep `HardwareCategory` display names clean without parenthetical examples.
4. **EF Core Migrations**: Always use EF Core migrations (`MigrateAsync`) instead of raw `ALTER TABLE` statements.
5. **Track Progress**: Keep [TODO.md](TODO.md) updated as features and integrations are planned or completed.
