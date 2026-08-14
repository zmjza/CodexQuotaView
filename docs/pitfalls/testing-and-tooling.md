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


## GitHub Actions 日志与 Artifact 在受限网络下可分段下载

**现象：** 2026-08-15 在本机命令行下载 GitHub Actions artifact/日志 ZIP 时，连接在约 1.5MB 处被截断（1.2–5.6MB 不等），校验和与 GitHub 记录不一致。

**根因：** 信息不全，待人工补充。302 重定向到 Azure blob（productionresultssa12）后由本机代理（MITM，198.18.0.115）按连接限流；单次执行窗口约 1.5MB 传输配额。

**正确做法：** 对同一 SAS URL 使用 300KB Range 分块请求，逐次执行窗口累积下载，按精确字节数截取并用 GitHub 记录的 SHA-256 校验；GitHub artifact digest 可直接用于核对。

**验证方式：** 组装后 `shasum -a 256` 与 API 返回的 digest 一致，`unzip -t` 通过。

**禁止事项：** 不得把截断文件当作完整产物验收；不得把命令行下载失败当作 GitHub 产物不存在。

**相关文件或命令：** `actions/artifacts/{id}/zip`、`curl -r`、`git credential fill`。

**适用范围：** 本机所有 GitHub artifact 下载与回下载复核。

## 直接运行 App 二进制会被会话回收，须用 open 启动

**现象：** 2026-08-15 通过 exec 直接执行 `CodexQuotaView.app/Contents/MacOS/CodexQuotaView`（nohup 后台）后，进程在会话结束即消失，状态项窗口未创建。

**根因：** 执行会话进程组随命令结束被回收；LaunchServices（`open -n`）启动的进程独立于会话存活。

**正确做法：** 真机启动一律使用 `open -n <app>`；需要捕获输出时改用 os_log 或日志文件，避免直接运行二进制。

**验证方式：** `open` 启动后 `pgrep` 跨会话存活；CGWindowList 可见 StatusItem。

**禁止事项：** 不得把直接运行二进制的“假启动”当作 App 启动证据。

**适用范围：** 本机 macOS 真机验收。

## 前台应用窗口遮挡导致 UI 自动化点击失效

**现象：** 2026-08-15 验收时设置窗口被 ChatGPT 主窗口（834,46 1591x928）完全覆盖，坐标点击全部被前台应用截获，AX 元素点击对侧边栏无效，页面无法切换。

**根因：** LSUIElement 菜单栏应用的 `activate` 不会使其成为前台应用；CGWindowList 按 z 序确认 ChatGPT 在前。

**正确做法：** 先核对 CGWindowList z 序与前台应用；必要时最小化遮挡窗口（AXMinimized）或把目标窗口移到未被覆盖区域，再执行坐标点击；元素点击用 `AXUIElementCopyElementAtPosition` + `AXPress` 绕过遮挡。

**验证方式：** 移窗/置前后再点击，AX 状态与截图双重确认。

**禁止事项：** 不得在被遮挡状态下把“点了没反应”误判为 App 缺陷。

**适用范围：** 本机 macOS UI 自动化验收。

## 无签名构建的发布相关占位符

**现象：** 1.0.0 Build 1 无签名构建中 `SUPublicEDKey=CHANGE_ME_SPARKLE_EDDSA_PUBLIC_KEY`，App Group 标识为 `TEAMID.com.zmjza.codexquotaview.shared`。

**根因：** 未签名/无 entitlements 构建无法携带真实公钥与 App Group；Sparkle 门禁（untrustedSignature）与 Widget 快照 fail-soft 均按设计降级。

**正确做法：** 正式签名发布前必须替换 EdDSA 公钥与 Team ID，并在签名/公证后回测更新与 Widget；内部候选交付如实标注占位符。

**验证方式：** PlistBuddy 检查 Info.plist；`ls ~/Library/Group Containers` 确认容器。

**禁止事项：** 不得把占位符版本描述为“可自动更新”或“Widget 可用”。

**适用范围：** CodexQuotaView 候选构建与正式发布前检查。
