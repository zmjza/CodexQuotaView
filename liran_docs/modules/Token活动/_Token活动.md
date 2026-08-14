# Token 活动模块

## `CQV-TASK-TOKEN-001` Token 汇总投影
- Requirement：`CQV-DATA-005`。平台：双平台。目标：最近报告日、30 日、累计 Token 一致。
- 前置：PROTO-002、DATA-006。读取：Presentation、Provider Adapter。预计修改：双端聚合。
- 步骤：使用 UTC 日桶；防溢出；缺失不当零。禁止：按本地日期重分桶。
- 测试：跨月、时区、溢出、空数据。证据：Fixture。回退：隐藏不可用指标。状态：待开发。

## `CQV-TASK-TOKEN-002` 30 日成本估算
- Requirement：`CQV-DATA-006`。平台：双平台。目标：迁移现有估算公式、精度和单位。
- 前置：TOKEN-001。读取：`EstimatedCostChartModel` 与现有规格。预计修改：共享公式说明和双端实现。
- 步骤：查证生产公式；Decimal 运算；标记估算；缺失 Token 不估算。禁止：编造实时价格或写成账单。
- 测试：已知输入、舍入、极值。证据：跨端结果。回退：显示不可用。状态：待开发。

## `CQV-TASK-TOKEN-003` 活动范围与网格模型
- Requirement：`CQV-DATA-007`。平台：双平台。目标：周/月/三月/半年范围和 16 列网格一致。
- 前置：TOKEN-001、PROTO-004。读取：TokenActivityGridModel、AGENTS。预计修改：双端 GridModel。
- 步骤：真实日期右下对齐；补齐格置前；缺失日标不可用；半年按最早有效桶。禁止：把补齐格变真实 0。
- 测试：全部范围和边界日期。证据：布局模型快照。回退：默认月范围。状态：待开发。

## `CQV-TASK-TOKEN-004` 活动格交互与辅助功能
- Requirement：`CQV-DATA-007`、`CQV-UX-005`。平台：双平台。目标：Hover、紧凑数值、取消延迟和辅助功能一致。
- 前置：TOKEN-003、UI 壳。读取：TokenActivityHoverController。预计修改：UI 交互。
- 步骤：0.5s Hover；K/M/B；切换/离开取消；补齐格隐藏。禁止：Tooltip 残留或完整长数字撑布局。
- 测试：计时器和 UI 自动化。证据：交互录制/快照。回退：即时静态 Tooltip。状态：待开发。

## `CQV-TASK-TOKEN-005` 动态高度与 Reduce Motion
- Requirement：`CQV-DATA-007`、`CQV-UX-005`。平台：双平台。目标：周期切换只移动底边，减少动态时直接布局。
- 前置：TOKEN-003、VIS-001。读取：面板高度逻辑。预计修改：窗口尺寸协调。
- 步骤：先准备目标内容；固定顶部；0.14s；Reduce Motion 关闭动画。禁止：内容先进入旧高度。
- 测试：各范围高度快照。证据：关键帧。回退：无动画直接更新。状态：待开发。
