# 测试与工具链避坑

## Swift 可编译应用目标不代表当前工具链可运行 XCTest

**现象：** 2026-08-12 在当前 macOS 工作区运行 `swift test` 时，应用、Core、
Probe 和 ActivityHook 目标编译完成，但测试目标在 `import XCTest` 处报
`no such module 'XCTest'`。

**根因：** 信息不全，待人工补充。现有证据只能证明当前选中的 Swift
工具链无法提供 XCTest 模块，不足以确定是 Xcode selection、Command Line Tools
或其他本机配置问题。

**正确做法：** 先记录 `swift --version`、`xcode-select -p` 和 `xcodebuild -version`，
确认使用项目要求的完整 Xcode/Swift 工具链后重试；在问题解决前，将应用
编译成功和单元测试通过分开报告。

**验证方式：** 在工具链修正后重新运行 `swift test`，要求测试目标成功构建、
执行完成且输出明确的通过/失败数量。

**禁止事项：** 不得把应用 target 编译成功声称为“测试通过”；不得在根因未验证
时断言是某个具体 Xcode 或 SDK 故障。

**相关文件或命令：** `Package.swift`、`Tests/QuotaViewCoreTests/`、
`swift test`、`swift --version`、`xcode-select -p`、`xcodebuild -version`。

**适用范围：** macOS 本地 SwiftPM 测试、CI 工具链排障和验证结果报告。

## 本地 Codex app-server 的 usage 接口可能返回上游错误

**现象：** 2026-08-15 在本机运行 CodexQuotaViewProbe（真实 Codex CLI 0.147.0-alpha.6.6，ChatGPT.app 内置），
额度读取失败，错误为 failed to fetch codex rate limits: error sending request for url (https://chatgpt.com/backend-api/wham/usage)。

**根因：** 信息不全，待人工补充。现有证据说明本地 app-server 进程可正常启动并转发请求，
失败发生在上游 ChatGPT 后端（网络、登录态或 CLI/服务兼容性均可能）。

**正确做法：** 把该错误视为“数据不可用”状态处理，应用按不可用/错误分支降级显示，
不崩溃、不显示伪造 0%；验证时区分“本地链路成功但上游失败”与“本地链路不可用”。

**验证方式：** 运行 .build/debug/CodexQuotaViewProbe（或 CI 的 smoke），观察错误分类；
修复上游后应返回真实额度快照。

**禁止事项：** 不得把该上游错误当作产品缺陷静默吞掉；不得为了演示伪造成功。

**相关文件或命令：** Sources/QuotaViewCore/CodexAppServerClient.swift、
Sources/QuotaViewProbe/main.swift、Windows/src/CodexQuotaView.Core/CodexProcessBackend.cs。

**适用范围：** 本地真实 Codex 数据链路验证、CLI 版本升级后的兼容性排查。
