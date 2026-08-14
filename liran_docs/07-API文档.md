# 本地 API 与协议文档

## Codex App Server 只读调用

| 方法 | 当前证据 | 用途 | 失败语义 |
|---|---|---|---|
| `account/rateLimits/read` | `CodexAppServerClient.swift` | 套餐、额度窗口、Credits、重置机会 | 无旧快照则不可用；有旧快照时保留并标记最新请求失败 |
| `account/usage/read` | `CodexAppServerClient.swift` | Token 汇总和日桶 | 可独立降级，不应让额度读取一并失败 |

禁止调用：`account/rateLimitResetCredit/consume`。重置流程只使用 `DemoQuotaActionExecutor` 或等价无副作用实现。

## 后端接口

平台内部统一暴露以下语义接口，具体语言签名由实现决定：

- `locateCodex()`：定位合法本地 Codex 可执行程序。
- `fetchRateLimits()`：读取并解码额度。
- `fetchUsage()`：读取并解码 Token 用量。
- `refresh(consumers)`：协调菜单/托盘、面板和 Widget 刷新。
- `cancel()`：在关闭或超时时停止子进程与读取任务。
- `sanitizedFailure()`：只返回有界错误分类。

Native 与 WSL Backend 必须实现同一 Fixture 合同。不得通过读取 `~/.codex` 凭据替代官方本地调用。

## Hook 传输

- macOS：沿用现有本地桥接语义，迁移 Helper 名称、路径和认证身份。
- Windows：首选当前用户范围 Named Pipe；断连时可写入有界队列。
- Envelope：版本、事件白名单、时间戳和一次性本地认证信息。
- 接收端必须验证版本、载荷大小、字段白名单、时间窗口和认证；失败只记录脱敏原因。

## Widget 快照

- 快照采用版本化 JSON/二进制 Codec，原子写入。
- macOS 主 App 与 Widget 使用相同团队前缀 App Group。
- Windows Widget Provider 或 Compact Host 使用同一只读 ViewModel，但不得开放跨用户全局 Pipe。
- 过期快照必须显示更新时间和不可用状态，不能伪装实时。

## 更新协议

- Sparkle：签名 appcast、不可变 GitHub Release ZIP、EdDSA 验证；是否进入公开 appcast 由用户对精确版本另行显式授权。
- Velopack：GitHub Release 资产和稳定通道；Setup 与 Portable 行为分别记录。
- 本阶段未知：最终 Feed URL、Windows Release Channel URL、证书身份。均标记待确认，不得硬编码。
