# NetAdmin TODO & Roadmap

## Completed
- [x] Core polymorphic hardware inventory (`HardwareDevice`, `IHardwareUiModule`, `HardwareProductDefinition`)
- [x] Physical cabling & port topology trace (`/hardware`, `/device/{id}`, `/switch/{id}`)
- [x] Room & zone management with inline editing and automatic equipment rename sync (`/rooms`)
- [x] Local administrator authentication (`IAdminAuthService`, PBKDF2-SHA256, Cookie Auth)
- [x] First-run 3-step Onboarding Wizard (`/onboarding`) and System Settings (`/setup`)
- [x] Per-device encrypted credentials & API testing on device details pages
- [x] Shared base classes in `NetAdmin.Core` (`HttpIntegrationProviderBase`, `NetAdminProviderPluginBase`, `HardwareUiModuleBase`, `CatalogDeviceEditorBase`)
- [x] OpenCode & Visual Studio shared skills (`.opencode/skills/*`, `AGENTS.md`, `.github/copilot-instructions.md`)
- [x] `Frigate` NVR provider project (`NetAdmin.Providers.Frigate`) with `HardwareCategory.Nvr`, API client, UI module, and product catalog
- [x] `HomeAssistant` provider project (`NetAdmin.Providers.HomeAssistant`) with `HardwareCategory.HomeAutomation`, API client, UI module, and product catalog

## Backlog
Open tasks are tracked as GitHub issues:
- [#5](https://github.com/henrikmaurin/NetAdmin/issues/5) Frigate Camera Auto-Sync
- [#6](https://github.com/henrikmaurin/NetAdmin/issues/6) Home Assistant Entity Discovery
- [#7](https://github.com/henrikmaurin/NetAdmin/issues/7) Proxmox VE Provider
- [#8](https://github.com/henrikmaurin/NetAdmin/issues/8) MQTT / Zigbee2MQTT Provider
- [#9](https://github.com/henrikmaurin/NetAdmin/issues/9) Automated Network Discovery
- [#10](https://github.com/henrikmaurin/NetAdmin/issues/10) Backup & Export
- [ ] Add links to the UniFi app for UniFi products
