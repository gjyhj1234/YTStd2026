using Microsoft.CodeAnalysis;

namespace YTStdEntity.Generator;

/// <summary>
/// 实体 Source Generator 入口。
/// 扫描标注了 [Entity] 的实体类，自动生成 DAL、CRUD、审计查询、描述类代码。
/// </summary>
[Generator]
public sealed class EntityGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TODO: 实现 Source Generator
        // 1. 使用 ForAttributeWithMetadataName 发现 [Entity] 标注的类
        // 2. 解析实体结构（字段、主键、索引、审计配置、主从关系）
        // 3. 生成 {Entity}DAL.g.cs — 建表/视图/索引/审计表/触发器
        // 4. 生成 {Entity}CRUD.g.cs — Insert/Update/Delete/Get/GetList
        // 5. 生成 {Entity}AuditCRUD.g.cs — 审计查询/历史/比较/主从联合查询
        // 6. 生成 {Entity}Desc.g.cs — 字段常量/元数据/索引器
    }
}
