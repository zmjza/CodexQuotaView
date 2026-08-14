# Compact Widget Host UI 壳接入清单

## 启用条件

仅当 `CQV-TASK-WIDGET-004` 以实际 SDK、注册和 GitHub 直分发证据证明 Windows 原生 Widget Provider 不可行或不稳定时启用。启用本路线不降低 Widget 的首版优先级。

## 形态

- Small 与 Medium 两种固定内容模式，以 macOS Widget 为视觉母版。
- 无边框、无任务栏按钮可选、可固定桌面、支持多屏位置持久化。
- 提供锁定位置、始终在桌面层/普通窗口层的可验证选项；不得强行始终置顶遮挡工作。
- 从托盘和设置可打开、切换尺寸、重置位置和关闭。

## 数据与状态

- 使用 Widget 共享 ViewModel 和版本化本地快照。
- 支持浅/深、中/英、available/unavailable/stale、Small/Medium。
- 不自行启动 Codex 后端，不读取凭据。

## 接收门禁

- 固定尺寸、文字不溢出、主要几何 ≤2 px、静态相似度 ≥95%。
- 多屏、DPI、Explorer 重启、锁定/解锁、系统主题和重启持久化必须有内部测试。
- 实现路径例外可以记录，但不得借此重新设计 Widget。
