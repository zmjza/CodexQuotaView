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
