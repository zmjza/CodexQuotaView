# 设置窗口 UI 壳接入清单

## 事实来源与尺寸

- `Sources/QuotaView/SettingsView.swift`、`AppPreferences.swift`、`MenuBarPanelController.swift`。
- macOS 默认内容 `872 × 637 pt`，最小 `780 × 560 pt`；Sidebar `200 pt`；设置使用系统字体和系统表面，不使用主面板玻璃视觉。

## 页面结构

- Sidebar：通用、主面板、活动/Hook、Widget、更新/关于等，以真实生产信息架构为准。
- 详情：标题、说明、分组卡片、设置行和原生控件。
- Windows：使用 WinUI NavigationView 或经验证的等价原生结构，保持信息密度、对齐和控件语义，不照搬 macOS 交通灯。

## 状态与预留

- 外观/语言跟随系统时禁用手动项。
- Hook：未安装、连接中、已连接、需处理、错误。
- 更新：不可用、检查中、最新、发现更新、失败。
- 事件：切换、选择、安装/修复 Hook、检查更新、打开链接。
- 后续锚点：`TODO(codex-state)`、`TODO(codex-connect)`、`TODO(codex-validate)`。

## 门禁

- 禁止品牌渐变、环境光晕、固定强调色和自绘 Switch。
- 最小窗口下使用滚动，不压缩控件或截断英文。
- 键盘、VoiceOver/Narrator、Increase Contrast 和 Reduce Motion 必须进入接收检查。
