# 构建与发布避坑

## Developer ID 直接分发的 Widget 必须使用团队前缀 App Group

**现象：** 公开 Build 1 中 Widget 无法读取主 App 写入的共享快照，macOS
系统日志记录 `SystemPolicyAppData` 拒绝访问旧 App Group。

**根因：** 未嵌入 provisioning profile 的 Developer ID 直接分发包使用了
`group.com.quotaview.shared`，不符合该分发形态的共享容器要求。

**正确做法：** 主 App 和 Widget Extension 使用相同的团队前缀 App Group，
并在打包脚本中从 `Info.plist` 读取预期值，校验最终签名 entitlement。

**验证方式：** 检查 App 与 Widget 最终 `codesign -d --entitlements -`
输出一致；在安装后验证快照写入、Widget 时间线归档及系统日志不再拒绝。

**禁止事项：** 不得只改主 App 或只改 Widget；不得以未签名本地运行结果
代替 Developer ID 直接分发验证。

**相关文件或命令：** `Support/QuotaView.entitlements`、
`Support/QuotaViewWidget.entitlements`、`Configs/App.xcconfig`、
`Configs/Widget.xcconfig`、`scripts/build-app.sh`；证据提交 `2323875`。

**适用范围：** macOS Developer ID 直接分发、WidgetKit 共享容器、重命 Bundle ID
或更换签名 Team 的任务。

## Ad-hoc 签名不应带 Hardened Runtime 启动内嵌 Framework

**现象：** `0.2.0 Build 3` 发布包存在 Framework 加载问题并被撤回，Build 4
改变了 ad-hoc 回退签名策略。

**根因：** ad-hoc 签名同时开启 Hardened Runtime 会触发 Library Validation，
使内嵌 Framework 在启动时无法正常加载。

**正确做法：** Developer ID 或 Apple Development 签名保留
`--options runtime --timestamp`；ad-hoc 签名使用 `--timestamp=none` 且不开启
Hardened Runtime。打包脚本必须检查最终签名 flags。

**验证方式：** 运行 `codesign -dv --verbose=4 <App>` 检查 runtime flag，
再对打包产物做真实启动验证和 `codesign --verify --deep --strict --verbose=4 <App>`。

**禁止事项：** 不得将一组 `codesign` 参数无差别用于 ad-hoc 和 Developer ID；
不得只因为构建成功就判定发布包可启动。

**相关文件或命令：** `scripts/build-app.sh`、`VERSION_HISTORY.md`；
证据提交 `8a76b57`、`7e58ffb`，撤回记录见 `VERSION_HISTORY.md`。

**适用范围：** 所有包含内嵌 Framework 的 macOS `.app`/ZIP 打包、签名与发布。

## 发布元数据脚本必须固定解析 locale

**现象：** App Store 元数据检查脚本在不同系统 locale 下可产生不一致的解析结果。

**根因：** `awk` 中的字符类与文本匹配会受进程 locale 影响，脚本未显式固定环境。

**正确做法：** 对机器解析步骤使用 `LC_ALL=C`；测试显式在另一 locale
中调用检查器，证明结果不随环境变化。

**验证方式：** 运行 `Tests/ReleaseScripts/test-appstore-metadata.zsh`，并在不同
`LC_ALL` 值下检查成功与失败 fixture 的结果一致。

**禁止事项：** 不得依赖开发者当前语言环境；不得在未运行非默认 locale
回归测试时声称脚本跨环境稳定。

**相关文件或命令：** `scripts/check-appstore-metadata.sh`、
`Tests/ReleaseScripts/test-appstore-metadata.zsh`；证据提交 `267bce8`。

**适用范围：** Shell/awk 发布门禁、CI 元数据检查、跨语言环境的文本解析。
