NetAdmin
========

Monorepo for the NetAdmin application. This repository contains multiple .NET projects: a shared Core library, infrastructure services, a Blazor Web UI, provider integrations, and unit test projects.

Status badges (placeholders)
- CI / Build: [![build](https://img.shields.io/badge/build-unconfigured-lightgrey)]()
- Tests: [![tests](https://img.shields.io/badge/tests-unconfigured-lightgrey)]()

Quick start
1. Build all projects: `dotnet build` from the repository root.
2. Run the Web UI: `dotnet run --project src/NetAdmin.Web`.
3. Run tests: `dotnet test` from the repository root (runs all test projects).

Database migrations
- Infrastructure and the Web project use EF Core migrations. See `src/NetAdmin.Infrastructure/README.md` for recommended migration and startup patterns.

Documentation and contributing
- All documentation must be in English. See CONTRIBUTING.md for repository rules, commit message style, and PR checklist.

Project READMEs
- Each project folder under src/ and tests/ includes a minimal README.md describing purpose, build, and run instructions.
