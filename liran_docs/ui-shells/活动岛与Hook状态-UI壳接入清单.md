# 活动岛与 Hook 状态 UI 壳接入清单

## 事实来源

- `Sources/QuotaView/CodexActivityIsland.swift`、`CodexActivityRuntime.swift`、`QuotaViewCore/CodexActivityModels.swift`。
- 只以当前稳定单任务生产状态为范围；`0.3.2 Preview 1` 多任务状态明确排除。

## 壳范围

- macOS：迁移品牌、Helper 状态文案和图标，不改变稳定状态机。
- Windows：原生顶部浮层/紧凑状态窗，保持状态、流体球、展开/收起和错误语义；具体系统位置需按多屏和任务栏验证。

## 状态与事件

- 状态必须从稳定 Reducer 自动提取并形成 Fixture，不在本清单凭空枚举。
- Hook UI：未检测、可安装、连接中、已连接、需重启、需处理、错误。
- 事件：安装/修复、展开/收起、关闭、超时、工作区变化、工具类别变化。

## 隐私与视觉门禁

- 只显示工作区末级名称和允许状态；不显示提示词、命令参数、输出或完整路径。
- 流体球关键帧 ≥90%，静态壳 ≥95%；Reduce Motion 有明确静态/低动态形态。
