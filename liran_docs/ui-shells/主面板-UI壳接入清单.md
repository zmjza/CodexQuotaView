# 主面板 UI 壳接入清单

## 事实来源

- 第一事实来源：`Sources/QuotaView/QuotaViewFigmaMenu.swift`、`MenuBarPanelController.swift`、`CodexTheme.swift`。
- 约束：固定宽 `274 pt`，内容宽 `250 pt`；高度按可见指标和 Token 活动范围动态计算；macOS 是 Windows 唯一母版。

## 后续允许范围

- macOS：品牌/资源迁移涉及的现有主面板文件，不在壳阶段改业务投影。
- Windows：由 WIN-001 最终创建的主面板 Page、局部 Style、ViewModel 接口、Fixture 文件。
- 禁止：新增 WebView、改全局主题、第二层整面玻璃、接真实消费接口。

## 必需状态与事件

- 状态：available、refreshing、unavailable、error；套餐；主周期；Spark；Credits；Token；成本；周/月/三月/半年；有/无重置机会；浅/深；清透/磨砂；中/英；Reduce Motion。
- 事件：手动刷新、周期切换、打开重置、打开设置、打开 Codex、退出、Tooltip、外部点击关闭。
- 数据入口：版本化 Presentation Fixture；后续接 `CodexStatusStore`/Windows 等价 Store。

## 视觉与接收门禁

- 静态 ≥95%，主要几何 ≤2 px；Header、概览、指标行、图表、入口和 Footer 分区逐项比较。
- 文字、图标、按钮位于玻璃采样树之外；连接状态圆点不解释额度风险。
- 壳接收时只允许合成 Fixture；必须能从正式或测试入口打开，不能把壳完成写成业务完成。
