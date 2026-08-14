# UI 与平台避坑

## Liquid Glass 视图在隐藏面板中替换后可能没有建立背景采样

**现象：** 清透玻璃视图在 `NSPanel` 隐藏时创建或替换，再显示时可以出现非预期
磨砂表现，直到后续几何变化才重新建立 WindowServer backdrop。

**根因：** `NSGlassEffectView` 的实时背景采样依赖视图已附着到可见且可正确呈现
的窗口；离屏替换不能稳定建立该合成状态。

**正确做法：** 隐藏时只记录待应用的玻璃模式；面板 `orderFront` 后再替换
玻璃 surface，使面板成为 key window，完成 layout/display 并留出一次
WindowServer transaction。

**验证方式：** 在 macOS 26 真机上分别于面板显示和隐藏时切换清透/磨砂，
多次打开面板，确认首帧已有背景折射且不需要额外 resize 才恢复。

**禁止事项：** 不得在面板隐藏时立即拆除并重建可见玻璃层；不得用第二层
模糊材质掩盖 backdrop 未附着问题。

**相关文件或命令：** `Sources/QuotaView/MenuBarPanelController.swift`
中 `requestGlassSurfaceUpdate`、`updateGlassSurface`、`showPanel`和
`QuotaViewLiquidGlassSurface`。

**适用范围：** macOS 26 `NSGlassEffectView`、非激活 `NSPanel`、运行时切换玻璃模式。

## 预览分支不能直接成为稳定生产基线

**现象：** `0.3.2 Preview 1` 多任务灵动岛合并后被整体 revert，后续仅作为
独立 Preview 保留，不进入稳定生产源码。

**根因：** 预览功能包含未完成的响应时延、任务跟随、切换和收展节奏问题，
其交付状态与稳定线不相同。

**正确做法：** 使用独立 Preview tag/Release 和 Prototype 保留实验证据；迁入生产前
建立新迭代规格、稳定 Requirement 追踪和完整验收证据。

**验证方式：** 核对 `docs/specs/README.md`、`HANDOFF.md`、
`VERSION_HISTORY.md` 和生产源码，确认 Preview 状态不被写成稳定已交付。

**禁止事项：** 不得把 Demo/Preview 测试通过解释为生产验收通过；不得直接恢复
已 revert 的生产改动。

**相关文件或命令：** `Prototypes/CodexActivityMultiTaskDemo/`、
`docs/design/quotaview-codex-activity-island-multitask.md`、`VERSION_HISTORY.md`；
证据提交 `8302fa5`。

**适用范围：** Preview/Beta/RC 功能、Prototype 迁移、稳定分支和发布基线管理。
