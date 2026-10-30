NetAdmin.Infrastructure
=======================

Infrastructure project for NetAdmin. Implements data access, persistence, background services, and integration points required by the application.

Key points
- This project holds EF Core DbContext, migrations, repository implementations, and background worker services.
- Keep infrastructure-specific logic here; shared abstractions belong in NetAdmin.Core.

EF Core migrations and database
- Migrations are kept in this project. Recommended commands (run from the repository root):
  - Add migration: `dotnet ef migrations add <Name> --project src/NetAdmin.Infrastructure --startup-project src/NetAdmin.Web`
  - Apply migrations: `dotnet ef database update --project src/NetAdmin.Infrastructure --startup-project src/NetAdmin.Web`

- Programmatic migrations: prefer calling `MigrateAsync` during application startup (Web project's Program.cs) to apply migrations automatically in CI/CD or container startup.

Local development tips
- For local development, a simple SQLite connection string is convenient: `Data Source=local.db`.
- Keep secrets out of source control; use user secrets or environment variables for sensitive configuration in development.

Build
- Build with `dotnet build`.
