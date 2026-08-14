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

- 2026-08-15 更新：内部实现、CI 四 job 全绿、macOS 真机验收主要项与 Windows 内部验收完成；候选资产进入 Draft Release v1.0.0-build.1（macOS Universal ZIP、Windows Setup EXE、Windows Portable ZIP、SHA256SUMS）。
- 正式发布前需：替换 SUPublicEDKey 占位符、TEAMID/App Group 真实值、签名与公证（如启用）、回下载复核与用户最终验收。
- 已知限制：无签名本地分发（Gatekeeper/SmartScreen 提示）、Sparkle 更新器在无签名构建禁用、无签名构建 Widget 快照不可用、Activity Hook 需用户在 Codex CLI 按 T 信任。
