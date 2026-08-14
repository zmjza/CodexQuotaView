# Shared Protocol Assets

CodexQuotaView 双平台共享的协议资产。macOS Swift 与 Windows C# 分别实现编解码，但必须对同一 Fixture 产生一致的规范化语义输出。

## 内容

- `schemas/quota-snapshot.schema.json`：快照 JSON Schema，版本固定为 1。
- `fixtures/*.json`：脱敏 Fixture，覆盖 available、warning、exhausted、unavailable、error。
- `fixture_runner`：双端校验脚本；当前实现见 `scripts/validate-fixtures.py`，Windows 侧测试引用同一目录。

## 规则

- 禁止写入凭据、Cookie、账号标识、提示词、命令参数、工具输出、完整路径、完整 App Server 响应。
- 未知值必须保留为 null 或已知枚举的 unknown，不得编造。
- 修改 Schema 必须同时提升 `schemaVersion` 并更新全部 Fixture 与双端测试。
