Contributing to NetAdmin
========================

This repository follows a few simple rules and conventions to keep contributions consistent.

Language and documentation
- All documentation, README files, and PR descriptions MUST be written in English.

Commits and branches
- Use clear, imperative commit messages, e.g. `Add provider for Acme devices`.
- Branch names: `feature/<short-description>`, `fix/<short-description>`, or `chore/<short-description>`.

Pull request checklist
1. Title and description in English and explain why the change is necessary.
2. Include unit tests for new logic where applicable.
3. Update or add README.md for any new project or provider explaining purpose, build, and run instructions.
4. Run `dotnet build` and `dotnet test` locally; ensure tests pass.
5. If database schema changes are required, include EF Core migrations in `src/NetAdmin.Infrastructure` and document how to apply them.

Code style and guidance
- Follow C# 14 / .NET 10 conventions and reuse rules in `.opencode/skills/code-style-and-reuse/SKILL.md`.
- For provider projects, follow `.opencode/skills/provider-architecture/SKILL.md`.

Testing and CI
- Add or update tests where behavioral changes are introduced.
- CI should run `dotnet build` and `dotnet test` for the repository.

Adding new projects
- Every new project must include a README.md in English that explains:
  - Purpose of the project
  - How to build and run it locally
  - Any environment variables or configuration required
  - How to run tests

Contact
- If in doubt, open a draft PR and ask for feedback on the exact changes.
