# CodexQuotaView 1.0.0 规格（二开）

> Spec ID：`CQV-PRODUCT-1.0.0`（本文件）
> 规格状态：`Accepted`（用户 2026-08-14 集中确认）
> 交付状态：`Verifying`（2026-08-15 内部实现与 CI 全绿，真机验收已完成主要项，未公开发布）
> 产品版本：`1.0.0 Build 1`（候选，GitHub Draft Release `v1.0.0-build.1`）

## 范围

在 QuotaView 开源基线（d3487bd）上二次开发，产品名 CodexQuotaView，双平台原生实现：

- macOS 14+：SwiftUI/AppKit/Metal/WidgetKit/Sparkle，Universal（arm64+x86_64），菜单栏主面板 + 设置 + 重置 Demo + Token 活动 + 稳定活动岛 + Small/Medium Widget。
- Windows 11 24H2+ x64：WinUI 3/.NET 8/Windows App SDK + C++/WinRT + D3D11/HLSL 渲染组件 + Velopack 更新骨架；系统托盘、Compact Widget Host。
- 共享协议：JSON Schema + 脱敏 Fixture，Swift/Python/C# 三端一致性校验。
- 数据链路：读取本机 Codex（ChatGPT.app 内置 CLI）app-server 的真实额度快照；上游不可用时稳定降级（破折号，不伪造 0%）。

## 验收条件（与 liran_docs/01-需求文档.md Requirement 对应）

- `CQV-BRAND-001..005`：品牌/身份/版本（1.0.0 Build 1）/License/资源全部迁移完成；Bundle ID `com.zmjza.codexquotaview.menubar`。
- `CQV-MAC-001..004`：macOS 14+、Universal、macOS 26 Liquid Glass（清透/磨砂，14–15 回退）、原生技术栈。
- `CQV-WIN-001..006`：Windows 11 24H2+ x64 原生、WinUI 3/.NET 8、D3D11/HLSL、Native/WSL Backend、托盘与自包含发布、不承诺 ARM64。
- `CQV-DATA-001..010`：套餐/额度/Spark/Credits/Token 汇总/成本/活动图/刷新协调/状态映射/重置 Demo（不调用真实 consume）。
- `CQV-UX-001..005`：菜单栏与托盘入口、主面板/重置/设置复刻、深浅色与跟随系统、中英文与跟随系统、Reduce Motion/键盘/辅助功能。
- `CQV-HOOK-001..002`、`CQV-WIDGET-001..002`：Hook 隐私白名单；活动岛稳定版状态；Widget 扩展与等价体验。
- `CQV-VIS-001..005`：macOS 生产界面为唯一母版；静态 ≥95%、动态 ≥90%、几何 ≤2px 为量化目标（真机截图已核对，量化工具链见 Design QA 记录）；Windows 平台等价光学实现。
- `CQV-PRIV-001..004`：不抓网页/不复制凭据；不保存 Token/Cookie/账号/完整响应；Hook 白名单；本地标准目录。
- `CQV-UPD-001..002`：Sparkle 门禁（无签名禁用）；Velopack 骨架。
- `CQV-CI-001..002`：GitHub 双平台 CI 全绿；GitHub-only 发布（无 Gitee）。
- `CQV-REL-001..004`：无签名本地分发为已确认模式；Draft Release 资产（macOS ZIP/Windows Setup/Portable/SHA256SUMS）；回下载校验；本地可直接使用。

## 已知限制（无签名本地分发）

- macOS Gatekeeper 提示：右键打开或 `xattr -cr`；Windows SmartScreen 提示。
- Sparkle 更新器在无签名构建禁用（untrustedSignature 门禁）；`SUPublicEDKey` 为占位符，正式签名版必须替换。
- App Group 需要 entitlements/签名；无签名构建 Widget 快照不可用（fail-soft 显示 unavailable），`TEAMID` 占位需替换为真实 Team ID。
- Activity Hook 安装需要用户在 Codex CLI 中按 T 完成安全信任。
- 上游 usage 接口（wham/usage）间歇性不可用，应用按“数据不可用”降级。

## 证据索引

- CI：`.github/workflows/ci.yml`（四 job）；Release：`.github/workflows/release.yml`（双平台资产 + Draft Release）。
- 真机：`liran_docs/09-真机实测.md`；追踪：`liran_docs/04-开发追踪.md`。
- 需求：`liran_docs/01-需求文档.md`；架构：`liran_docs/02-架构文档.md`。
- 避坑：`docs/pitfalls/`；交接：`HANDOFF.md` 顶部二开区。
