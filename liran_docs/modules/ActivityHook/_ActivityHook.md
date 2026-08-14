# Activity Hook 模块

## `CQV-TASK-HOOK-001` 冻结事件 Schema 白名单
- Requirement：`CQV-HOOK-001`、`CQV-PRIV-003`。平台：共享。目标：只允许 Hash、末级工作区、事件、工具类别、来源、时间。
- 前置：PROTO-001。读取：`CodexActivityModels.swift`。预计修改：共享 Hook Schema。
- 步骤：列允许字段；默认拒绝未知字段；限制字符串/载荷。禁止：提示词、参数、输出、完整路径、会话文件。
- 测试：敏感字段负面 Fixture。证据：Schema 报告。回退：拒绝整个事件。状态：待开发。

## `CQV-TASK-HOOK-002` 迁移 macOS Helper 身份与安装
- Requirement：`CQV-HOOK-001`、`CQV-BRAND-002`。平台：macOS。目标：更新 Helper、Label、路径和 Hook 配置，不破坏用户现有配置。
- 前置：BRAND-002、HOOK-001。读取：ActivityRuntime、Hook main、build script。预计修改：Helper 与安装器。
- 步骤：识别并移除旧 QuotaView handler；原子写新 handler；保留其他用户 Hook。禁止：覆盖整个用户配置。
- 测试：安装/重复安装/卸载/损坏配置。证据：脱敏 diff。回退：恢复备份配置。状态：待开发。

## `CQV-TASK-HOOK-003` 实现 Windows Named Pipe
- Requirement：`CQV-HOOK-001`。平台：Windows。目标：当前用户范围、安全、有界的本地事件管道。
- 前置：WIN-001、HOOK-001。读取：Windows Pipe ACL。预计修改：Hook 与 App IPC。
- 步骤：用户 ACL；认证握手；大小/速率/超时；版本验证。禁止：全局可写 Pipe。
- 测试：错误用户、错误 Token、超大载荷、断连。证据：安全测试。回退：禁用 Hook。状态：待开发。

## `CQV-TASK-HOOK-004` 实现 Windows 有界文件队列
- Requirement：`CQV-HOOK-001`、`CQV-PRIV-004`。平台：Windows。目标：Pipe 不可用时短期缓存脱敏事件。
- 前置：HOOK-003。读取：本地存储规则。预计修改：队列 Store。
- 步骤：原子写；数量/大小/TTL；消费后删除；崩溃恢复。禁止：无限增长或缓存敏感字段。
- 测试：容量、过期、损坏、并发。证据：单测。回退：丢弃事件，不影响主应用。状态：待开发。

## `CQV-TASK-HOOK-005` 迁移稳定 Activity Reducer
- Requirement：`CQV-HOOK-002`。平台：双平台。目标：两端稳定单任务状态一致。
- 前置：HOOK-001、PROTO-003。读取：CodexActivityReducer 和当前生产源码。预计修改：Swift 品牌迁移、C# 实现。
- 步骤：列出全部稳定状态/过渡/超时；对同一事件流比较。禁止：恢复多任务 Preview。
- 测试：事件序列 Fixture。证据：状态快照。回退：回到 idle/error。状态：待开发。

## `CQV-TASK-HOOK-006` 活动桥接诊断与脱敏日志
- Requirement：`CQV-PRIV-002`、`CQV-HOOK-001`。平台：双平台。目标：用户可诊断连接但日志无敏感载荷。
- 前置：HOOK-002..005。读取：现有 Diagnostics。预计修改：诊断模型和设置 UI。
- 步骤：输出状态码、版本、时间、组件存在性；隐藏 Token/路径。禁止：完整事件或配置全文。
- 测试：日志快照和敏感扫描。证据：脱敏样本。回退：只显示通用错误。状态：待开发。

## `CQV-TASK-HOOK-007` Hook 端到端内部测试
- Requirement：`CQV-HOOK-001..002`。平台：双平台。目标：从合成 Hook 到活动 UI 完整跑通。
- 前置：HOOK-002..006、Activity UI 壳。读取：测试矩阵。预计修改：集成测试。
- 步骤：安装/连接/发送/断连/恢复/卸载；检查隐私字段。禁止：使用真实提示词做 Fixture。
- 测试：macOS 集成与 Windows Runner。证据：状态序列。回退：禁用集成不阻塞额度主链路。状态：待开发。
