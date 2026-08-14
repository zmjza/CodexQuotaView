# CodexQuotaView Changelog

## [Unreleased] 1.0.0 Build 1

### Added

- 从 QuotaView 开源基线二次开发，产品更名为 CodexQuotaView。
- Windows x64 原生工程骨架：WinUI 3/.NET 8 Core、Fixture 校验、Native/WSL Backend、D3D11/HLSL 离屏渲染冒烟。
- 共享 JSON Schema 与脱敏 Fixture，以及 Swift/Python/C# 三端一致性校验。
- GitHub Actions 双平台 CI 首次全绿：macOS Swift、Windows .NET/Fixture、WinUI + D3D11/HLSL 渲染冒烟。

### Changed

- 身份迁移：Bundle ID、App Group、更新源、脚本、资源和可见文案改为 CodexQuotaView 身份。
- 版本基线改为 `1.0.0 Build 1`；上游历史 tag 与 Release 保持不变。

### Pending

- macOS Xcode/Universal 构建、Windows Runner 构建与 CI 验证、双平台视觉门禁、签名公证和正式发布（见 `liran_docs/09-真机实测.md`）。
