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

## Next Up (Backlog)
- [ ] **Frigate Camera Auto-Sync**: Import or link Frigate camera definitions directly to `CameraHardwareDevice` inventory items and RTSP stream definitions
- [ ] **Home Assistant Entity Discovery**: Correlate Home Assistant device tracker / network entities with NetAdmin MAC/IP inventory
- [ ] **Proxmox VE Provider (`NetAdmin.Providers.Proxmox`)**: Query node status, VMs, and LXC containers for `HardwareCategory.Server`
- [ ] **MQTT / Zigbee2MQTT Provider (`NetAdmin.Providers.Zigbee2Mqtt`)**: Inspect coordinator status and Zigbee mesh devices
- [ ] **Automated Network Discovery**: Scheduled background ARP/DHCP/mDNS scan with one-click device registration
- [ ] **Backup & Export**: Encrypted export/import of SQLite inventory and settings
