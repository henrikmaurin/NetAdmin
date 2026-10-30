---
name: provider-architecture
description: How to create and structure a new NetAdmin provider project.
---

# Provider Architecture

## One Project Per Provider
- Every new hardware or service integration must live in its own Razor class library project: `src/NetAdmin.Providers.<Name>/NetAdmin.Providers.<Name>.csproj`.
- Use `<Project Sdk="Microsoft.NET.Sdk.Razor">`, target `net10.0`, reference `Microsoft.AspNetCore.App` and `..\NetAdmin.Core\NetAdmin.Core.csproj`.
- Add the project to `NetAdmin.slnx` (under `/src/Providers/`) and reference it from `src/NetAdmin.Web/NetAdmin.Web.csproj`.

## Required Provider Components
1. **Core Contract**: Define `I<Name>Provider : INetworkIntegration` and immutable DTO records in `NetAdmin.Core/Providers/INetworkCapabilities.cs`.
2. **Plugin Registration**: Implement a `NetAdminProviderPluginBase` subclass in `ServiceCollectionExtensions.cs` so `AddNetAdminProviderPlugins` discovers it automatically at startup.
3. **API Provider**: Inherit `HttpIntegrationProviderBase` and resolve base URLs, tokens, and credentials dynamically via `IEncryptedSettingsService`.
4. **Hardware UI Module**: Inherit `HardwareUiModuleBase<TDevice>` (or `IHardwareUiModule`), declare `GetCredentialFields(HardwareDevice)` and `GetCapabilities(HardwareDevice)`, and wire `EditorComponentType`, `HeaderActionsComponentType`, and `CardSummaryComponentType`.
5. **Product Catalog**: Define strongly typed `HardwareProductDefinition` subclasses under `Products/` for automatic catalog discovery.
