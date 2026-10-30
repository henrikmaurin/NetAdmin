---
name: code-style-and-reuse
description: C# 14 / .NET 10 coding style, DRY rules, and shared base class conventions for NetAdmin.
---

# Code Style & Maximum Reuse

## Core Principles
- **Target**: .NET 10, C# 14, Nullable enabled, ImplicitUsings enabled.
- **Language**: All code, comments, UI labels, and category names must be in English. Keep category display names clean without parenthetical examples.
- **Maximize Shared Code**: Always place reusable logic, base classes, HTTP helpers, and Blazor component bases in `NetAdmin.Core`. Never duplicate boilerplate across provider projects.

## Shared Abstractions in `NetAdmin.Core`
- **`HttpIntegrationProviderBase`**: Inherit for HTTP/REST integrations. Use its shared URL normalization (`NormalizeBaseUrl`), encrypted setting resolution (`GetSettingAsync`), Bearer/Basic header helpers, and JSON fetching (`GetJsonAsync`).
- **`NetAdminProviderPluginBase`**: Inherit in provider plugins to automatically register `IHardwareUiModule` and `HardwareProductDefinition` types from the provider assembly.
- **`HardwareUiModuleBase<TDevice>`**: Inherit for `IHardwareUiModule` implementations to share default `CanHandle`, `Matches`, and `EnsureDeviceType` behavior.
- **`CatalogDeviceEditorBase<TDevice>`**: Inherit in Blazor device editors to reuse manufacturer/model catalog selection and custom fallback logic.
- **`DeviceComponentBase` / `TypedDeviceComponentBase<TDevice>`**: Inherit for header actions and card summaries (`AnchorAction`, `ButtonAction`, `InfoBox`, `TagBadge`).

## C# Style Rules
- Prefer primary constructors, `record` types for immutable domain models, pattern matching (`is`, `switch` expressions), and collection expressions (`[]`).
- Pass `CancellationToken ct = default` on all async methods.
- Never store secrets in `HardwareDevice` records; always use `IEncryptedSettingsService` with `device:{DeviceId}:{KeySuffix}` or global keys.
