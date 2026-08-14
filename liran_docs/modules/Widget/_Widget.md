# Widget 模块

## `CQV-TASK-WIDGET-001` 迁移共享 Widget 契约
- Requirement：`CQV-WIDGET-001..002`、`CQV-DATA-001..009`。平台：共享。目标：版本化快照支持两端 Widget。
- 前置：PROTO-001、DATA 任务。读取：WidgetSnapshot.swift、Writer。预计修改：共享 Schema、Swift/C# Codec。
- 步骤：保持计划名、窗口、四指标、可用性、更新时间；原子编码。禁止：把完整 App Server 响应写入快照。
- 测试：编解码、版本、损坏文件。证据：Fixture。回退：显示不可用。状态：待开发。

## `CQV-TASK-WIDGET-002` 迁移 macOS App Group
- Requirement：`CQV-WIDGET-001`、`CQV-BRAND-002`。平台：macOS。目标：新团队前缀 Group 在 Developer ID 直分发可用。
- 前置：BRAND-002、WIDGET-001。读取：entitlement、pitfall、build script。预计修改：App/Widget entitlement 和 plist。
- 步骤：使用同一合法 Group；脚本读取预期值；签名后比对。禁止：无团队前缀旧 Group 或只改一端。
- 测试：codesign entitlement、真机读写、系统日志。证据：Widget 时间线。回退：阻止发布。状态：待开发。

## `CQV-TASK-WIDGET-003` 迁移 macOS Small/Medium UI
- Requirement：`CQV-WIDGET-001`、`CQV-VIS-002..004`。平台：macOS。目标：保持现有 Figma/生产尺寸、字体、指标和状态。
- 前置：WIDGET-001、BRAND-005。读取：QuotaViewWidget.swift、AGENTS。预计修改：Widget UI 和资源。
- 步骤：更名；更新图标；验证深浅、本地化、不可用。禁止：自绘外层系统容器。
- 测试：Widget Preview、归档、真机。证据：截图矩阵。回退：保持生产布局。状态：待开发。

## `CQV-TASK-WIDGET-004` Windows Widget Provider 可行性门禁
- Requirement：`CQV-WIDGET-002`。平台：Windows。目标：用实际 SDK/分发条件决定原生 Provider。
- 前置：WIN-001、WIDGET-001。读取：目标 Windows SDK 官方文档和 Runner 能力。预计修改：Spike/决策记录。
- 步骤：验证 API 可用、GitHub 直分发、注册、刷新、本地数据和尺寸。禁止：仅凭文档猜测可行。
- 测试：最小 Provider 构建/注册；不可行记录具体证据。证据：ADR。回退：选择 Compact Host。状态：待开发。

## `CQV-TASK-WIDGET-005` 实现 Windows 原生 Provider
- Requirement：`CQV-WIDGET-002`。平台：Windows。目标：若门禁通过，交付 Small/Medium 等价 Widget。
- 前置：WIDGET-004=可行。读取：UI 壳、共享契约。预计修改：Provider 工程。
- 步骤：注册；读取快照；刷新；主题/语言；错误状态。禁止：联网或读取凭据。
- 测试：注册、刷新、尺寸、Golden。证据：Runner/内部证据。回退：Compact Host。状态：条件待开发。

## `CQV-TASK-WIDGET-006` 实现 Compact Widget Host
- Requirement：`CQV-WIDGET-002`。平台：Windows。目标：原生 Provider 不可行时交付无边框、可固定桌面的等价宿主。
- 前置：WIDGET-004=不可行。读取：Compact UI 清单。预计修改：WinUI Host、设置、启动策略。
- 步骤：Small/Medium 模式；固定位置；置底/桌面语义；锁定交互；同一 ViewModel。禁止：取消 Widget 功能。
- 测试：布局、持久化、多屏、Golden。证据：内部截图。回退：恢复默认位置。状态：条件待开发。

## `CQV-TASK-WIDGET-007` Widget 跨平台回归
- Requirement：`CQV-WIDGET-001..002`。平台：双平台。目标：数据、状态、主题、语言和视觉一致。
- 前置：WIDGET-002/003 与 005 或 006。读取：测试矩阵。预计修改：Fixture/视觉测试。
- 步骤：所有快照状态；过期/损坏；浅深；中英；Small/Medium。禁止：以 Preview 代替真机 macOS。
- 测试：Codec、Golden、macOS 真机。证据：矩阵。回退：隐藏不可用指标。状态：待开发。
