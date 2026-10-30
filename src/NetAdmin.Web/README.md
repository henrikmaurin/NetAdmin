NetAdmin.Web
============

Blazor InteractiveServer web UI for NetAdmin. Provides the user interface, pages, and interactive components for managing devices and providers.

Key points
- Built with Blazor InteractiveServer patterns. See .opencode/skills/blazor-ui-and-efcore/SKILL.md for UI conventions and guidance.
- The Web project depends on NetAdmin.Core and NetAdmin.Infrastructure for domain and data access.

Run (development)
1. Ensure dependencies are built: `dotnet build`.
2. Provide any required environment variables (example below).
3. Start the app: `dotnet run --project src/NetAdmin.Web`.

Environment / configuration
- Configuration follows standard .NET patterns (appsettings.json, appsettings.Development.json, and environment variables).
- Typical environment variables you may need to provide during development:
  - `ConnectionStrings__Default`: connection string for the application database (e.g. SQLite, SQL Server).
  - `ASPNETCORE_ENVIRONMENT`: set to `Development` for local development.

Database migrations
- Run EF Core migrations from the Infrastructure project. Example (from repo root):
  - `dotnet ef migrations add <Name> --project src/NetAdmin.Infrastructure --startup-project src/NetAdmin.Web`
  - `dotnet ef database update --project src/NetAdmin.Infrastructure --startup-project src/NetAdmin.Web`
- The app also calls `MigrateAsync` during startup in many deployments — prefer programmatic migrations for CI/CD to avoid drift.

Testing
- Run UI and integration tests with `dotnet test` from the repository root or the tests folder.
