# Copilot Instructions

## Project Guidelines
- 在该仓库中，主键 ID 不能由 CRUD 生成器自动分配；事务插入常依赖主表 ID，业务代码必须显式获取并设置 ID。
- 在该仓库中，优先考虑正确性。代码必须保持 AOT 友好，适用于 .NET 10 NativeAOT，且不使用反射、动态或 LINQ；优化以实现高性能和低 GC；仅针对 PostgreSQL/Npgsql；生成与现有实体和逻辑一致的生产级可编译代码。
- 在该仓库中，`Logger.Debug` 必须仅使用委托重载，绝不使用直接字符串参数或字符串插值/连接。优先传递一个 lambda，并尽量减少字符串连接，以避免在 Debug 被禁用时的内存分配。
- 在该仓库中，手动 JSON 字符串连接应替换为基于 `Utf8JsonWriter` 的构造，以实现 AOT 友好的低分配 JSON 生成。对于 AOT 友好的 JSON，优先使用 `JsonSerializerContext` 和 `JsonSerializable` 源生成，而不是手动元数据注册，以保持代码简洁和可读。