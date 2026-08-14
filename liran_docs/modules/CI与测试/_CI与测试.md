# CI 与测试模块

## `CQV-TASK-CI-001` 扩展 macOS CI
- Requirement：`CQV-CI-001`、`CQV-MAC-001..004`。平台：GitHub/macOS。目标：从单一 `swift test` 扩展为测试、App/Widget/Helper Release 和 Universal 门禁。
- 前置：MAC-001。读取：现有 workflow、build script。预计修改：GitHub Actions。
- 步骤：固定 Xcode；运行单测；无签名 Release；架构/资源/版本检查；上传 Artifact。禁止：CI 使用发布私钥。
- 测试：PR workflow。证据：Checks。回退：保留最小单测 job 并修复新增 job。状态：待开发。

## `CQV-TASK-CI-002` 新增 Windows CI
- Requirement：`CQV-CI-001`、`CQV-WIN-001..006`。平台：GitHub/Windows。目标：restore、test、Release x64、HLSL、烟雾、自包含 Artifact。
- 前置：WIN-001。读取：solution、publish profile。预计修改：workflow。
- 步骤：缓存依赖；编译 C++/HLSL；运行 Core/协议测试；启动烟雾；上传。禁止：只编译 Debug。
- 测试：PR workflow。证据：Windows Checks。回退：分离故障 job。状态：待开发。

## `CQV-TASK-CI-003` 跨平台 Fixture 门禁
- Requirement：`CQV-CI-001`、`CQV-DATA-001..009`。平台：GitHub。目标：同一 Fixture 的 Swift/C# 输出严格比较。
- 前置：PROTO-005。读取：Runner。预计修改：workflow/script。
- 步骤：分别生成规范 JSON；下载/合并 Artifact；结构 diff。禁止：忽略未知字段。
- 测试：故意差异失败。证据：diff Artifact。回退：修实现或版本 Schema。状态：待开发。

## `CQV-TASK-CI-004` 视觉门禁
- Requirement：`CQV-VIS-002..004`。平台：GitHub。目标：Golden、布局、HLSL 离屏和关键帧成为必需检查。
- 前置：VIS-002..006。读取：视觉 Harness。预计修改：workflow、阈值配置。
- 步骤：固定环境；生成结果；比较；上传热图。禁止：失败时自动更新 Golden。
- 测试：负面基线。证据：报告。回退：人工审查新 Golden 后独立提交。状态：待开发。

## `CQV-TASK-CI-005` 文档与敏感信息门禁
- Requirement：`CQV-PRIV-001..003`、`CQV-CI-001`。平台：GitHub。目标：Markdown、链接、敏感模式、禁止端点和 Debug Mock 扫描。
- 前置：基础结构。读取：AGENTS、pitfalls。预计修改：检查脚本/workflow。
- 步骤：`git diff --check`；markdownlint 如适用；链接；密钥模式；consume；DEBUG 标记。禁止：输出匹配到的完整秘密。
- 测试：合成违规 Fixture。证据：负面 CI。回退：移除违规内容。状态：待开发。

## `CQV-TASK-CI-006` EXE/App 烟雾测试
- Requirement：`CQV-REL-004`。平台：双平台。目标：构建产物能启动并建立主进程/窗口入口。
- 前置：MAC/WIN 构建。读取：产物。预计修改：smoke scripts。
- 步骤：隔离配置；启动；等待健康标志；优雅退出；收集脱敏崩溃信息。禁止：把进程存在作为全部功能通过。
- 测试：故意缺 DLL/Framework 应失败。证据：日志。回退：阻止 Artifact 晋升。状态：待开发。

## `CQV-TASK-CI-007` Artifact 身份门禁
- Requirement：`CQV-BRAND-003`、`CQV-REL-001..003`。平台：双平台。目标：文件名、版本、架构、Bundle/Windows Identity 与提交一致。
- 前置：构建 job。读取：产物元数据。预计修改：校验脚本。
- 步骤：解包；验证嵌套组件；输出摘要；拒绝旧 QuotaView 资产名。禁止：只看文件名。
- 测试：错误版本 Fixture。证据：摘要。回退：重新构建。状态：待开发。

## `CQV-TASK-CI-008` 必需检查与分支保护建议
- Requirement：`CQV-CI-001`。平台：GitHub。目标：定义合并前必须通过的 macOS、Windows、Fixture、视觉、文档检查。
- 前置：CI-001..007。读取：GitHub 配置。预计修改：文档，分支保护需权限时外部操作。
- 步骤：稳定 job 名；避免可选 job 冒充必需；记录超时/重跑规则。禁止：绕过失败检查发布。
- 测试：PR 状态汇总。证据：保护规则截图/API 摘要。回退：保留 workflow 门禁。状态：待开发。
