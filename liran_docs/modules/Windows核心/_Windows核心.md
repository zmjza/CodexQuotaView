# Windows 核心模块

## `CQV-TASK-WIN-001` 创建 WinUI 3 x64 解决方案
- Requirement：`CQV-WIN-001`、`CQV-WIN-002`。平台：Windows。目标：建立 .NET 8/Windows App SDK 原生工程和测试工程。
- 前置：BRAND-003、PROTO-001。读取：架构文档、官方模板。预计修改：`Windows/` 新工程。
- 步骤：创建 App/Core/Protocol/Rendering/Hook/Tests；固定 x64 和 24H2 最低目标；启用自包含。禁止：Electron/WebView。
- 测试：restore/build/test。证据：解决方案结构与日志。回退：移除新工程，不碰 macOS。状态：待开发。

## `CQV-TASK-WIN-002` 实现 Native Backend 探测
- Requirement：`CQV-WIN-004`、`CQV-PRIV-001`。平台：Windows。目标：探测并调用合法 Windows Codex 本地能力。
- 前置：WIN-001、PROTO-002。读取：macOS Client 语义与 Windows Codex 实际安装。预计修改：Core Backend。
- 步骤：探测 PATH/标准安装；有界启动 app-server；超时取消；脱敏错误。禁止：读取凭据文件。
- 测试：Fake process、缺失、超时、崩溃。证据：单测。回退：禁用 Native 并进入 WSL 决策。状态：待开发。

## `CQV-TASK-WIN-003` 实现 WSL 兼容 Backend
- Requirement：`CQV-WIN-004`。平台：Windows。目标：Native 不可用时通过 WSL 运行同一只读协议。
- 前置：WIN-002。读取：WSL 可用发行版和路径规则。预计修改：WSL Backend。
- 步骤：探测 WSL；选择明确发行版；处理 UTF-8/路径/取消；不复制凭据到 Windows。禁止：静默安装 WSL。
- 测试：无 WSL、多发行版、超时、局部失败。证据：Fixture。回退：返回明确不可用。状态：待开发。

## `CQV-TASK-WIN-004` Backend 选择与刷新协调
- Requirement：`CQV-DATA-008`、`CQV-WIN-004`。平台：Windows。目标：Native 优先、WSL 兜底且不双重请求。
- 前置：WIN-002、003、PROTO-003。读取：RefreshCoordinator 语义。预计修改：Core coordinator。
- 步骤：定义优先级、健康缓存、手动重试和取消；发布统一快照。禁止：后端失败时伪造数据。
- 测试：并发/取消/切换表驱动测试。证据：状态序列。回退：固定到最后可用 Backend。状态：待开发。

## `CQV-TASK-WIN-005` 建立 WinUI App Shell
- Requirement：`CQV-WIN-005`、`CQV-UX-001..005`。平台：Windows。目标：应用生命周期、托盘、面板和设置入口可启动。
- 前置：WIN-001、UI 壳阶段确认。读取：UI 接入清单。预计修改：App Shell。
- 步骤：单实例；NotifyIcon；窗口定位；主题/语言入口；崩溃恢复。禁止：在壳阶段接真实凭据。
- 测试：EXE 烟雾、单实例和托盘命令。证据：Runner 日志。回退：最小 Shell。状态：待开发。

## `CQV-TASK-WIN-006` 自包含发布配置
- Requirement：`CQV-WIN-001`、`CQV-REL-004`。平台：Windows。目标：用户无需安装 .NET。
- 前置：WIN-005。读取：csproj、publish profile。预计修改：RID、trim/single-file 决策、运行时资产。
- 步骤：发布 win-x64；验证原生 DLL；决定是否禁用 trim；记录体积。禁止：依赖开发机全局运行时。
- 测试：干净 Windows Runner 启动。证据：依赖清单。回退：使用非 single-file 自包含目录。状态：待开发。

## `CQV-TASK-WIN-007` Windows 辅助功能与 Reduce Motion
- Requirement：`CQV-UX-005`。平台：Windows。目标：Narrator、键盘、焦点、对比和动画降级完整。
- 前置：WIN-005、VIS-001。读取：Windows accessibility API。预计修改：UI Automation 属性和动画策略。
- 步骤：命名控件；焦点顺序；系统动画设置；高对比语义色。禁止：只靠颜色。
- 测试：Automation 属性测试和截图。证据：矩阵。回退：禁用非必要动画。状态：待开发。

## `CQV-TASK-WIN-008` Windows 内部平台回归
- Requirement：`CQV-WIN-001`、`CQV-WIN-002`、`CQV-WIN-003`、`CQV-WIN-004`、`CQV-WIN-005`、`CQV-WIN-006`。平台：Windows。目标：构建、测试、烟雾和离屏证据完整，并证明首版工作流只声明 x64。
- 前置：WIN-001..007 及功能任务。读取：08、09。预计修改：测试/CI。
- 步骤：Release x64、Core tests、HLSL、EXE 烟雾、包结构；确认 manifest、README 和 Artifact 不声明 ARM64；记录为内部测试。禁止：写成真机通过或虚假声明 ARM64。
- 测试：GitHub Windows Runner。证据：Artifact。回退：阻止发布。状态：待开发。
