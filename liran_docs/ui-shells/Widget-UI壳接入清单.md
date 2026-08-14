# Widget UI 壳接入清单

## 事实来源

- `Sources/QuotaViewWidget/QuotaViewWidget.swift`、`QuotaViewWidgetSnapshotWriter.swift`、`QuotaViewWidgetContract/WidgetSnapshot.swift`。
- macOS Small/Medium 使用系统容器；内部 `16 pt` 边距，Medium 左右栏间距 `12 pt`。

## 平台壳

- macOS：品牌迁移后的 WidgetKit Small/Medium。
- Windows：原生 Widget Provider 可行时实现等价尺寸；不可行时由 Compact Widget Host 承接，不能取消首版 Widget。

## 数据与状态

- 快照：套餐、主周期、重置倒计时、连接状态、Credits、最近日 Token、累计 Token。
- 状态：available/unavailable/stale；浅/深；中/英；Small/Medium。
- 禁止 Widget 自行调用 Codex 或读取凭据；只读主 App 写入的版本化快照。

## 门禁

- macOS 不自绘外层圆角容器。
- 百分比数字与百分号字号分离；进度条、标签、时钟和四指标行按生产 Token。
- 静态 ≥95%、主要几何 ≤2 px；过期/损坏快照显示不可用。
