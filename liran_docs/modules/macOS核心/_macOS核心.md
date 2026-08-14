# macOS 核心模块

## `CQV-TASK-MAC-001` 迁移 SwiftPM/Xcode 目标
- Requirement：`CQV-MAC-001`、`CQV-MAC-004`、`CQV-BRAND-002`。平台：macOS。目标：新目标名下保持现有模块关系。
- 前置：BRAND-002、PROTO-001。读取：Package、Xcode project、Configs。预计修改：项目与 Scheme。
- 步骤：重命产品/Target/模块；修正 import 和测试依赖；保持 macOS 14。禁止：顺带重构业务。
- 测试：`swift test`、Xcode Debug/Release。证据：构建日志。回退：原子恢复项目文件。状态：待开发。

## `CQV-TASK-MAC-002` 迁移菜单栏生命周期
- Requirement：`CQV-UX-001`、`CQV-REL-004`。平台：macOS。目标：新身份启动、Status Item、面板、设置和退出稳定。
- 前置：MAC-001。读取：`QuotaViewApp.swift`、`MenuBarPanelController.swift`。预计修改：App 生命周期和持久化 Key。
- 步骤：更新 autosave/ToolTip；验证多屏锚定、外部点击和 key window。禁止：入口可完全隐藏。
- 测试：生命周期单测和真机操作。证据：日志/录屏或截图。回退：恢复旧控制器行为。状态：待开发。

## `CQV-TASK-MAC-003` 保持数据刷新主链路
- Requirement：`CQV-DATA-001..009`。平台：macOS。目标：在新品牌下复用 App Server、Adapter、Store 和 RefreshCoordinator。
- 前置：MAC-001、PROTO-003。读取：Core 与 Store。预计修改：命名、错误文案、协议适配。
- 步骤：TDD 迁移；保持局部 usage 失败降级；更新消费者。禁止：读取凭据或改变官方方法。
- 测试：现有及新增 Core tests。证据：测试数量和结果。回退：恢复上游链路。状态：待开发。

## `CQV-TASK-MAC-004` 迁移 Liquid Glass 双路径
- Requirement：`CQV-MAC-003`、`CQV-VIS-003`。平台：macOS。目标：macOS 26 原生玻璃、14–15 Material 回退。
- 前置：MAC-002、VIS-001。读取：Panel Controller、Theme、pitfalls。预计修改：玻璃 Surface 与可用性分支。
- 步骤：保留可见后重建门禁；验证清透/磨砂；内容置于采样层之外。禁止：CIGaussianBlur、私有 API、屏幕录制。
- 测试：版本分支单测和 macOS 26 真机。证据：视觉矩阵。回退：切回已验证 Material。状态：待开发。

## `CQV-TASK-MAC-005` Universal 构建与嵌套组件
- Requirement：`CQV-MAC-002`。平台：macOS。目标：App、Core、Widget、Hook、Sparkle 全部 Universal。
- 前置：MAC-001..004。读取：build script、Xcode 配置。预计修改：构建校验。
- 步骤：Release 无签名构建；逐个 `lipo` 检查；修复单架构依赖。禁止：只检查主 App。
- 测试：架构脚本。证据：组件清单。回退：阻止打包，不降低目标。状态：待开发。

## `CQV-TASK-MAC-006` macOS 内部回归
- Requirement：`CQV-MAC-001..004`、`CQV-UX-001..005`。平台：macOS。目标：进入真机前完成自动化和构建。
- 前置：MAC-001..005、相关功能任务。读取：Tests、08 测试矩阵。预计修改：测试。
- 步骤：单测、Release、资源、本地化、敏感扫描；记录未覆盖项。禁止：把内部回归写成真机通过。
- 测试：完整 macOS CI 命令。证据：报告。回退：修复失败后重跑。状态：待开发。
