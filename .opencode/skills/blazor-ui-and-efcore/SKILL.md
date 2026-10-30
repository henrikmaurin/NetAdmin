---
name: blazor-ui-and-efcore
description: Blazor InteractiveServer UI guidelines and EF Core SQLite data rules.
---

# Blazor UI & EF Core Guidelines

## Blazor InteractiveServer
- Use `@rendermode InteractiveServer` on interactive pages.
- Reuse shared component bases (`CatalogDeviceEditorBase<TDevice>`, `TypedDeviceComponentBase<TDevice>`, `DeviceComponentBase`) and CSS classes (`card`, `card-grid`, `form-label`, `form-input`, `form-select`, `action-button`, `info-box`, `tag-badge`).
- Perform cookie sign-in/sign-out via standard HTTP endpoints (`/auth/*`), never by calling `HttpContext.SignInAsync` directly inside a SignalR circuit.

## EF Core & Persistence
- Always use EF Core migrations (`MigrateAsync`) instead of raw `ALTER TABLE` SQL statements.
- Use `IDbContextFactory<AppDbContext>` in stores and services for safe concurrent operations in Blazor Server.
- Store all API keys, passwords, and tokens encrypted via `IEncryptedSettingsService`.
