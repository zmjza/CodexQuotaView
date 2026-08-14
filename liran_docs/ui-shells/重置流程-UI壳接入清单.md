# 重置流程 UI 壳接入清单

## 事实来源

- `Sources/QuotaView/QuotaViewFigmaResetMenu.swift`、`MenuBarView.swift`、`AccountOperations.swift`。
- 固定 `274 × 473 pt`；内容宽 `250 pt`；包含 Header、次数 Hero、票据、说明、按钮、Footer 和内部确认层。

## 状态与事件

- 状态：次数 0/1/多、有效/失效、Demo、确认关闭/打开、提交中模拟、取消。
- 事件：返回、打开确认、Escape 取消、确认 Demo、外部点击策略。
- 次数过多时整体缩放票据；0 次显示 0 张，不伪造固定数量。

## 安全边界

- 只接 `DemoQuotaActionExecutor` 或等价无副作用接口。
- 确认层必须明确消耗一次、立即重置、不可撤销和当前为 Demo；不得调用 `account/rateLimitResetCredit/consume`。
- 确认层位于玻璃内容内部，不使用系统矩形 Alert。

## 视觉门禁

- 静态 ≥95%、几何 ≤2 px；按钮状态和内部阴影逐项比较。
- Windows 使用平台等价实现，不改信息结构或破坏性语义。
