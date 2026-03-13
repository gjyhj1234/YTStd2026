using System;
using System.Collections.Generic;
using Xunit;
using YTStdEntity;
using YTStdEntity.Attributes;
using YTStdEntity.Audit;

namespace YTStdEntity.Sample;

/// <summary>
/// 实体框架示例测试。
/// 验证源代码生成器生成的 DAL、CRUD、AuditCRUD、Desc 类的所有公共方法与属性是否可访问且行为正确。
/// 注意：CRUD/DAL/AuditCRUD 中需要数据库连接的异步方法无法在单元测试中调用，
/// 但此处通过编译级别验证 API 签名正确、通过类型和常量验证生成代码的结构。
/// </summary>
public class EntitySampleTests
{
    #region SysUserDesc 示例

    /// <summary>
    /// 演示 SysUserDesc 表名常量。
    /// 源代码生成器为每个实体生成 Desc 类，包含表名、字段名常量和字段元数据。
    /// </summary>
    [Fact]
    public void Sample_SysUserDesc_TableName()
    {
        // 验证生成的表名常量
        Assert.Equal("sys_user", SysUserDesc.Name);
    }

    /// <summary>
    /// 演示 SysUserDesc 租户标识。
    /// IsTenant 为 true 表示该表包含 tenant_id 字段，支持多租户隔离。
    /// </summary>
    [Fact]
    public void Sample_SysUserDesc_IsTenant()
    {
        Assert.True(SysUserDesc.IsTenant);
    }

    /// <summary>
    /// 演示 SysUserDesc.Fields 字段名常量。
    /// 这些常量用于构建动态 SQL 时引用字段名，避免硬编码字符串。
    /// </summary>
    [Fact]
    public void Sample_SysUserDesc_FieldConstants()
    {
        Assert.Equal("id", SysUserDesc.Fields.Id);
        Assert.Equal("tenant_id", SysUserDesc.Fields.TenantId);
        Assert.Equal("name", SysUserDesc.Fields.Name);
        Assert.Equal("email", SysUserDesc.Fields.Email);
        Assert.Equal("balance", SysUserDesc.Fields.Balance);
        Assert.Equal("created_at", SysUserDesc.Fields.CreatedAt);
        Assert.Equal("tags", SysUserDesc.Fields.Tags);
    }

    /// <summary>
    /// 演示 SysUserDesc.DictFieldMetas 字段元数据字典。
    /// 提供对字段完整元数据（类型、长度、精度、是否可空、是否主键）的运行时访问。
    /// </summary>
    [Fact]
    public void Sample_SysUserDesc_DictFieldMetas()
    {
        var metas = SysUserDesc.DictFieldMetas;
        Assert.NotNull(metas);
        Assert.True(metas.Count >= 7, "SysUser 至少有 7 个字段");

        // 检查 id 字段元数据
        Assert.True(metas.ContainsKey("id"));
        Assert.Equal("bigint", metas["id"].Type);
        Assert.True(metas["id"].IsPrimaryKey);
        Assert.False(metas["id"].IsNullable);

        // 检查 name 字段元数据
        Assert.True(metas.ContainsKey("name"));
        Assert.False(metas["name"].IsNullable);
        Assert.Equal(50, metas["name"].Length);

        // 检查 email 可空字段
        Assert.True(metas.ContainsKey("email"));
        Assert.True(metas["email"].IsNullable);
    }

    /// <summary>
    /// 演示 SysUserDesc.GetFieldMeta 按名称查询字段元数据。
    /// 返回 null 表示字段不存在。
    /// </summary>
    [Fact]
    public void Sample_SysUserDesc_GetFieldMeta()
    {
        var idMeta = SysUserDesc.GetFieldMeta("id");
        Assert.NotNull(idMeta);
        Assert.Equal("id", idMeta!.Name);
        Assert.True(idMeta.IsPrimaryKey);

        // 不存在的字段返回 null
        var unknown = SysUserDesc.GetFieldMeta("nonexistent");
        Assert.Null(unknown);
    }

    #endregion

    #region OrderDesc 示例

    /// <summary>
    /// 演示 OrderDesc 表名与字段常量。
    /// </summary>
    [Fact]
    public void Sample_OrderDesc_TableName_And_Fields()
    {
        Assert.Equal("order", OrderDesc.Name);
        Assert.True(OrderDesc.IsTenant);

        Assert.Equal("id", OrderDesc.Fields.Id);
        Assert.Equal("tenant_id", OrderDesc.Fields.TenantId);
        Assert.Equal("user_id", OrderDesc.Fields.UserId);
        Assert.Equal("total_amount", OrderDesc.Fields.TotalAmount);
        Assert.Equal("created_at", OrderDesc.Fields.CreatedAt);
    }

    #endregion

    #region OrderItemDesc 示例

    /// <summary>
    /// 演示 OrderItemDesc（从表）的字段常量与元数据。
    /// OrderItem 通过 [DetailOf(typeof(Order))] 标记为 Order 的从表。
    /// </summary>
    [Fact]
    public void Sample_OrderItemDesc_TableName_And_Fields()
    {
        Assert.Equal("order_item", OrderItemDesc.Name);
        Assert.True(OrderItemDesc.IsTenant);

        Assert.Equal("id", OrderItemDesc.Fields.Id);
        Assert.Equal("order_id", OrderItemDesc.Fields.OrderId);
        Assert.Equal("product_name", OrderItemDesc.Fields.ProductName);
        Assert.Equal("quantity", OrderItemDesc.Fields.Quantity);
        Assert.Equal("unit_price", OrderItemDesc.Fields.UnitPrice);
    }

    #endregion

    #region DbNullable 示例

    /// <summary>
    /// 演示 DbNullable 三态结构体的基本用法。
    /// DbNullable 区分"未设置"、"设置为具体值"和"设置为 NULL"。
    /// </summary>
    [Fact]
    public void Sample_DbNullable_Basic()
    {
        // 未设置（默认状态）：Update 时不更新该字段
        DbNullable<string> unset = default;
        Assert.False(unset.IsSet);

        // 显式未设置
        DbNullable<string> explicitUnset = DbNullable<string>.Unset;
        Assert.False(explicitUnset.IsSet);

        // 设置为具体值：Update 时将该字段设为 "hello"
        DbNullable<string> withValue = new DbNullable<string>("hello");
        Assert.True(withValue.IsSet);
        Assert.Equal("hello", withValue.Value);

        // 设置为 NULL：Update 时将该字段设为 NULL
        DbNullable<string> setNull = DbNullable<string>.NullValue;
        Assert.True(setNull.IsSet);
        Assert.Null(setNull.Value);
    }

    /// <summary>
    /// 演示 DbNullable 的隐式转换。
    /// 可直接将值赋给 DbNullable，自动标记为"已设置"。
    /// </summary>
    [Fact]
    public void Sample_DbNullable_ImplicitConversion()
    {
        // 隐式转换：直接赋值
        DbNullable<int> age = 25;
        Assert.True(age.IsSet);
        Assert.Equal(25, age.Value);
    }

    /// <summary>
    /// 演示 DbNullable 的 ToString() 输出。
    /// </summary>
    [Fact]
    public void Sample_DbNullable_ToString()
    {
        Assert.Equal("[Unset]", default(DbNullable<int>).ToString());
        Assert.Equal("[Set: null]", DbNullable<string>.NullValue.ToString());

        DbNullable<int> val = 42;
        Assert.Equal("[Set: 42]", val.ToString());
    }

    #endregion

    #region DBNULL 快捷方式示例

    /// <summary>
    /// 演示 DBNULL 静态类的语义化 NULL 值快捷方式。
    /// 用于在 UpdateFieldsAsync 中显式将字段设为 NULL。
    /// </summary>
    [Fact]
    public void Sample_DBNULL_Shortcuts()
    {
        // 各类型的 NULL 快捷值
        Assert.True(DBNULL.StringValue.IsSet);
        Assert.Null(DBNULL.StringValue.Value);

        Assert.True(DBNULL.IntValue.IsSet);
        Assert.Equal(default, DBNULL.IntValue.Value);

        Assert.True(DBNULL.LongValue.IsSet);
        Assert.True(DBNULL.DecimalValue.IsSet);
        Assert.True(DBNULL.DateTimeValue.IsSet);
        Assert.True(DBNULL.TimeSpanValue.IsSet);
        Assert.True(DBNULL.StringArrayValue.IsSet);
        Assert.True(DBNULL.IntArrayValue.IsSet);
        Assert.True(DBNULL.LongArrayValue.IsSet);
        Assert.True(DBNULL.DecimalArrayValue.IsSet);
        Assert.True(DBNULL.DateTimeArrayValue.IsSet);
        Assert.True(DBNULL.TimeSpanArrayValue.IsSet);
    }

    #endregion

    #region YTFieldMeta 示例

    /// <summary>
    /// 演示 YTFieldMeta 字段元数据结构。
    /// YTFieldMeta 描述单个数据库列的完整信息，由 Desc 类暴露。
    /// </summary>
    [Fact]
    public void Sample_YTFieldMeta_Properties()
    {
        var meta = new YTFieldMeta
        {
            Name = "username",
            Type = "varchar",
            Length = 100,
            Precision = 0,
            IsNullable = false,
            IsPrimaryKey = false,
            IsTenant = false
        };

        Assert.Equal("username", meta.Name);
        Assert.Equal("varchar", meta.Type);
        Assert.Equal(100, meta.Length);
        Assert.False(meta.IsNullable);
        Assert.False(meta.IsPrimaryKey);
        Assert.False(meta.IsTenant);
    }

    #endregion

    #region Attribute 示例

    /// <summary>
    /// 演示 EntityAttribute 的使用方式。
    /// [Entity] 标注在实体类上，声明表名和审计表需求。
    /// </summary>
    [Fact]
    public void Sample_EntityAttribute()
    {
        var attr = new EntityAttribute
        {
            TableName = "my_table",
            NeedAuditTable = true
        };

        Assert.Equal("my_table", attr.TableName);
        Assert.True(attr.NeedAuditTable);
        Assert.Null(attr.ViewSql);
    }

    /// <summary>
    /// 演示 ColumnAttribute 的使用方式。
    /// [Column] 标注在实体属性上，声明列名、长度、精度等元数据。
    /// </summary>
    [Fact]
    public void Sample_ColumnAttribute()
    {
        var attr = new ColumnAttribute
        {
            ColumnName = "user_name",
            Title = "用户名",
            Length = 50,
            IsRequired = true,
            IsPrimaryKey = false
        };

        Assert.Equal("user_name", attr.ColumnName);
        Assert.Equal("用户名", attr.Title);
        Assert.Equal(50, attr.Length);
        Assert.True(attr.IsRequired);
        Assert.Equal(2, attr.Precision); // 默认精度
    }

    /// <summary>
    /// 演示 IndexAttribute 的使用方式。
    /// [Index] 标注在实体类上，声明索引名和列，支持唯一索引。
    /// </summary>
    [Fact]
    public void Sample_IndexAttribute()
    {
        var attr = new IndexAttribute("idx_user_email", "email")
        {
            Kind = IndexKind.Unique
        };

        Assert.Equal("idx_user_email", attr.IndexName);
        Assert.Single(attr.Columns);
        Assert.Equal("email", attr.Columns[0]);
        Assert.Equal(IndexKind.Unique, attr.Kind);
    }

    /// <summary>
    /// 演示 DetailOfAttribute 的使用方式。
    /// [DetailOf] 标注在从表实体类上，声明主表类型和外键。
    /// </summary>
    [Fact]
    public void Sample_DetailOfAttribute()
    {
        var attr = new DetailOfAttribute(typeof(Order))
        {
            ForeignKey = "OrderId"
        };

        Assert.Equal(typeof(Order), attr.MasterType);
        Assert.Equal("OrderId", attr.ForeignKey);
    }

    #endregion

    #region AuditOpt 示例

    /// <summary>
    /// 演示 AuditOpt 审计操作类型枚举。
    /// </summary>
    [Fact]
    public void Sample_AuditOpt_Values()
    {
        Assert.Equal((byte)'I', (byte)AuditOpt.Insert);
        Assert.Equal((byte)'U', (byte)AuditOpt.Update);
        Assert.Equal((byte)'D', (byte)AuditOpt.Delete);
    }

    #endregion

    #region AuditDiffField 示例

    /// <summary>
    /// 演示 AuditDiffField 审计字段差异结构。
    /// 表示两个审计快照之间单个字段的变化。
    /// </summary>
    [Fact]
    public void Sample_AuditDiffField()
    {
        var diff = new AuditDiffField
        {
            FieldName = "name",
            OldValue = "张三",
            NewValue = "李四"
        };

        Assert.Equal("name", diff.FieldName);
        Assert.Equal("张三", diff.OldValue);
        Assert.Equal("李四", diff.NewValue);
    }

    #endregion

    #region AuditRecord 示例

    /// <summary>
    /// 演示 AuditRecord 审计记录结构。
    /// 每个审计记录包含操作类型、时间、操作人和完整的 JSONB 快照。
    /// </summary>
    [Fact]
    public void Sample_AuditRecord()
    {
        var record = new AuditRecord
        {
            AuditId = 1,
            Id = 100,
            Opt = AuditOpt.Insert,
            OperatedAt = DateTime.UtcNow,
            OperatorId = 42,
            TenantId = 1,
            Snapshot = "{\"id\":100,\"name\":\"张三\"}"
        };

        Assert.Equal(1, record.AuditId);
        Assert.Equal(100, record.Id);
        Assert.Equal(AuditOpt.Insert, record.Opt);
        Assert.Equal(42, record.OperatorId);
        Assert.Contains("张三", record.Snapshot);
    }

    #endregion

    #region AuditQueryFilter 示例

    /// <summary>
    /// 演示 AuditQueryFilter 审计查询过滤条件。
    /// 所有条件为 AND 逻辑，null 表示不过滤。
    /// </summary>
    [Fact]
    public void Sample_AuditQueryFilter()
    {
        var filter = new AuditQueryFilter
        {
            Id = 100,
            Opt = AuditOpt.Update,
            OperatorId = 42,
            StartTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            PageIndex = 0,
            PageSize = 20
        };

        Assert.Equal(100, filter.Id);
        Assert.Equal(AuditOpt.Update, filter.Opt);
        Assert.Equal(42, filter.OperatorId);
        Assert.Equal(20, filter.PageSize);
    }

    /// <summary>
    /// 演示 AuditQueryFilter 默认值。
    /// </summary>
    [Fact]
    public void Sample_AuditQueryFilter_Defaults()
    {
        var filter = new AuditQueryFilter();

        Assert.Null(filter.Id);
        Assert.Null(filter.Opt);
        Assert.Null(filter.OperatorId);
        Assert.Null(filter.StartTime);
        Assert.Null(filter.EndTime);
        Assert.Equal(0, filter.PageIndex);
        Assert.Equal(50, filter.PageSize);
    }

    #endregion

    #region MasterDetailAuditResult 示例

    /// <summary>
    /// 演示 MasterDetailAuditResult 基类。
    /// Source Generator 为主从表生成具体子类（如 OrderMasterDetailAuditResult）。
    /// </summary>
    [Fact]
    public void Sample_MasterDetailAuditResult()
    {
        var result = new MasterDetailAuditResult
        {
            TotalCount = 10,
        };
        result.MasterRecords.Add(new AuditRecord
        {
            AuditId = 1,
            Id = 100,
            Opt = AuditOpt.Insert,
            OperatedAt = DateTime.UtcNow,
            OperatorId = 1,
            Snapshot = "{}"
        });

        Assert.Equal(10, result.TotalCount);
        Assert.Single(result.MasterRecords);
    }

    #endregion

    #region SysUserAuditCRUD.DiffAuditSnapshots 示例

    /// <summary>
    /// 演示 DiffAuditSnapshots：比较两个 JSONB 审计快照的字段差异。
    /// 此方法是纯内存操作，不需要数据库连接。
    /// </summary>
    [Fact]
    public void Sample_DiffAuditSnapshots_DetectsNameChange()
    {
        string oldSnapshot = "{\"id\":1,\"tenant_id\":1,\"name\":\"张三\",\"email\":\"zhangsan@example.com\"}";
        string newSnapshot = "{\"id\":1,\"tenant_id\":1,\"name\":\"李四\",\"email\":\"zhangsan@example.com\"}";

        var diffs = SysUserAuditCRUD.DiffAuditSnapshots(1, 100, oldSnapshot, newSnapshot);

        // 只有 name 字段发生了变化
        Assert.Single(diffs);
        Assert.Equal("name", diffs[0].FieldName);
        Assert.Equal("张三", diffs[0].OldValue);
        Assert.Equal("李四", diffs[0].NewValue);
    }

    /// <summary>
    /// 演示 DiffAuditSnapshots：多个字段同时变更。
    /// </summary>
    [Fact]
    public void Sample_DiffAuditSnapshots_MultipleChanges()
    {
        string oldSnapshot = "{\"id\":1,\"tenant_id\":1,\"name\":\"张三\",\"email\":\"old@test.com\",\"balance\":100.00}";
        string newSnapshot = "{\"id\":1,\"tenant_id\":1,\"name\":\"张三\",\"email\":\"new@test.com\",\"balance\":200.00}";

        var diffs = SysUserAuditCRUD.DiffAuditSnapshots(1, 100, oldSnapshot, newSnapshot);

        Assert.Equal(2, diffs.Count);
        Assert.Contains(diffs, d => d.FieldName == "email");
        Assert.Contains(diffs, d => d.FieldName == "balance");
    }

    /// <summary>
    /// 演示 DiffAuditSnapshots：无变更时返回空列表。
    /// </summary>
    [Fact]
    public void Sample_DiffAuditSnapshots_NoChanges()
    {
        string snapshot = "{\"id\":1,\"tenant_id\":1,\"name\":\"张三\"}";

        var diffs = SysUserAuditCRUD.DiffAuditSnapshots(1, 100, snapshot, snapshot);

        Assert.Empty(diffs);
    }

    /// <summary>
    /// 演示 SysUserAuditCRUD 审计表名常量。
    /// </summary>
    [Fact]
    public void Sample_SysUserAuditCRUD_TableName()
    {
        Assert.Equal("sys_user_audit", SysUserAuditCRUD.AuditTableName);
    }

    #endregion

    #region OrderAuditCRUD.DiffAuditSnapshots 示例

    /// <summary>
    /// 演示 OrderAuditCRUD.DiffAuditSnapshots 方法。
    /// </summary>
    [Fact]
    public void Sample_OrderAuditCRUD_DiffSnapshots()
    {
        string oldSnapshot = "{\"id\":1,\"total_amount\":100.00}";
        string newSnapshot = "{\"id\":1,\"total_amount\":200.00}";

        var diffs = OrderAuditCRUD.DiffAuditSnapshots(1, 100, oldSnapshot, newSnapshot);

        Assert.Single(diffs);
        Assert.Equal("total_amount", diffs[0].FieldName);
    }

    /// <summary>
    /// 演示 OrderAuditCRUD 审计表名常量。
    /// </summary>
    [Fact]
    public void Sample_OrderAuditCRUD_TableName()
    {
        Assert.Equal("order_audit", OrderAuditCRUD.AuditTableName);
    }

    #endregion

    #region 生成代码 API 签名编译验证

    /// <summary>
    /// 编译级验证：SysUserCRUD 的所有公共方法签名存在且可引用。
    /// 此测试通过编译即可验证生成代码的 API 完整性。
    /// 注意：实际调用需要数据库连接，此处仅验证方法可访问。
    /// </summary>
    [Fact]
    public void Sample_SysUserCRUD_ApiSurface_CompileCheck()
    {
        // 验证方法引用（不调用）
        // InsertAsync(int tenantId, long userId, SysUser entity)
        Func<int, long, SysUser, System.Threading.Tasks.ValueTask<YTStdAdo.DbInsResult>> insertAsync
            = SysUserCRUD.InsertAsync;
        Assert.NotNull(insertAsync);

        // DeleteAsync(int tenantId, long userId, long id)
        Func<int, long, long, System.Threading.Tasks.ValueTask<YTStdAdo.DbUdqResult>> deleteAsync
            = SysUserCRUD.DeleteAsync;
        Assert.NotNull(deleteAsync);

        // GetAsync(int tenantId, long userId, long id)
        Func<int, long, long, System.Threading.Tasks.ValueTask<SysUser?>> getAsync
            = SysUserCRUD.GetAsync;
        Assert.NotNull(getAsync);
    }

    /// <summary>
    /// 编译级验证：SysUserDAL 的所有公共方法签名存在且可引用。
    /// DAL 类包含 DDL 方法：CreateTableIfNotExists、EnsureColumnLength、CreateIndexIfNotExists。
    /// </summary>
    [Fact]
    public void Sample_SysUserDAL_ApiSurface_CompileCheck()
    {
        // CreateTableIfNotExists(int tenantId, long userId, bool createLogTable)
        Func<int, long, bool, System.Threading.Tasks.ValueTask<bool>> createTable
            = SysUserDAL.CreateTableIfNotExists;
        Assert.NotNull(createTable);

        // EnsureColumnLength(int tenantId, long userId)
        Func<int, long, System.Threading.Tasks.ValueTask<bool>> ensureCol
            = SysUserDAL.EnsureColumnLength;
        Assert.NotNull(ensureCol);

        // CreateIndexIfNotExists(int tenantId, long userId)
        Func<int, long, System.Threading.Tasks.ValueTask<bool>> createIdx
            = SysUserDAL.CreateIndexIfNotExists;
        Assert.NotNull(createIdx);
    }

    /// <summary>
    /// 编译级验证：OrderCRUD 的公共方法签名。
    /// </summary>
    [Fact]
    public void Sample_OrderCRUD_ApiSurface_CompileCheck()
    {
        Func<int, long, Order, System.Threading.Tasks.ValueTask<YTStdAdo.DbInsResult>> insertAsync
            = OrderCRUD.InsertAsync;
        Assert.NotNull(insertAsync);

        Func<int, long, long, System.Threading.Tasks.ValueTask<YTStdAdo.DbUdqResult>> deleteAsync
            = OrderCRUD.DeleteAsync;
        Assert.NotNull(deleteAsync);
    }

    /// <summary>
    /// 编译级验证：OrderItemCRUD 的公共方法签名。
    /// </summary>
    [Fact]
    public void Sample_OrderItemCRUD_ApiSurface_CompileCheck()
    {
        Func<int, long, OrderItem, System.Threading.Tasks.ValueTask<YTStdAdo.DbInsResult>> insertAsync
            = OrderItemCRUD.InsertAsync;
        Assert.NotNull(insertAsync);
    }

    /// <summary>
    /// 编译级验证：SysUserAuditCRUD 异步方法签名。
    /// </summary>
    [Fact]
    public void Sample_SysUserAuditCRUD_ApiSurface_CompileCheck()
    {
        // GetAuditListAsync
        Func<int, long, AuditQueryFilter,
            System.Threading.Tasks.ValueTask<(List<AuditRecord> Records, int TotalCount)>> getList
            = SysUserAuditCRUD.GetAuditListAsync;
        Assert.NotNull(getList);

        // GetAuditByIdAsync
        Func<int, long, long,
            System.Threading.Tasks.ValueTask<AuditRecord?>> getById
            = SysUserAuditCRUD.GetAuditByIdAsync;
        Assert.NotNull(getById);

        // GetAuditHistoryAsync
        Func<int, long, long,
            System.Threading.Tasks.ValueTask<List<AuditRecord>>> getHistory
            = SysUserAuditCRUD.GetAuditHistoryAsync;
        Assert.NotNull(getHistory);

        // DiffAuditSnapshots
        Func<int, long, string, string, List<AuditDiffField>> diff
            = SysUserAuditCRUD.DiffAuditSnapshots;
        Assert.NotNull(diff);
    }

    /// <summary>
    /// 编译级验证：OrderAuditCRUD 主从表审计查询方法签名。
    /// OrderMasterDetailAuditResult 是生成的具体子类，包含 OrderItem 的审计记录。
    /// </summary>
    [Fact]
    public void Sample_OrderAuditCRUD_MasterDetail_CompileCheck()
    {
        // 验证 OrderMasterDetailAuditResult 类继承自 MasterDetailAuditResult
        var result = new OrderAuditCRUD.OrderMasterDetailAuditResult();
        Assert.IsAssignableFrom<MasterDetailAuditResult>(result);
        Assert.NotNull(result.OrderItemRecords);
        Assert.Empty(result.OrderItemRecords);
    }

    #endregion

    #region 实体类示例

    /// <summary>
    /// 演示 SysUser 实体的创建与属性赋值。
    /// </summary>
    [Fact]
    public void Sample_SysUser_Entity()
    {
        var user = new SysUser
        {
            Id = 1,
            TenantId = 100,
            Name = "张三",
            Email = "zhangsan@example.com",
            Balance = 1000.50m,
            CreatedAt = DateTime.UtcNow,
            Tags = new[] { "admin", "vip" }
        };

        Assert.Equal(1, user.Id);
        Assert.Equal("张三", user.Name);
        Assert.Equal(2, user.Tags!.Length);
    }

    /// <summary>
    /// 演示 Order 和 OrderItem 主从表关系。
    /// </summary>
    [Fact]
    public void Sample_Order_MasterDetail()
    {
        var order = new Order
        {
            Id = 1001,
            TenantId = 1,
            UserId = 42,
            TotalAmount = 299.90m,
            CreatedAt = DateTime.UtcNow
        };

        var item1 = new OrderItem
        {
            Id = 1,
            TenantId = 1,
            OrderId = order.Id,
            ProductName = "键盘",
            Quantity = 1,
            UnitPrice = 199.90m
        };

        var item2 = new OrderItem
        {
            Id = 2,
            TenantId = 1,
            OrderId = order.Id,
            ProductName = "鼠标",
            Quantity = 1,
            UnitPrice = 100.00m
        };

        Assert.Equal(order.Id, item1.OrderId);
        Assert.Equal(order.Id, item2.OrderId);
    }

    #endregion

    #region TenantSeparationOptions / IncrementalBackupOptions 示例

    /// <summary>
    /// 演示 TenantSeparationOptions 租户分离配置。
    /// </summary>
    [Fact]
    public void Sample_TenantSeparationOptions()
    {
        var options = new YTStdEntity.Tenant.TenantSeparationOptions
        {
            TargetConnectionString = "Host=target;Database=tenant1",
            TenantIds = new[] { 1, 2, 3 }
        };

        Assert.Equal("Host=target;Database=tenant1", options.TargetConnectionString);
        Assert.Equal(3, options.TenantIds.Length);
    }

    /// <summary>
    /// 演示 IncrementalBackupOptions 增量备份配置。
    /// </summary>
    [Fact]
    public void Sample_IncrementalBackupOptions()
    {
        var options = new YTStdEntity.Backup.IncrementalBackupOptions
        {
            TargetConnectionStrings = new[] { "Host=backup1;Database=db1", "Host=backup2;Database=db2" },
            Interval = TimeSpan.FromMinutes(5)
        };

        Assert.Equal(2, options.TargetConnectionStrings.Length);
        Assert.Equal(TimeSpan.FromMinutes(5), options.Interval);
    }

    /// <summary>
    /// 演示 IncrementalBackupOptions 默认值。
    /// </summary>
    [Fact]
    public void Sample_IncrementalBackupOptions_Defaults()
    {
        var options = new YTStdEntity.Backup.IncrementalBackupOptions();

        Assert.Empty(options.TargetConnectionStrings);
        Assert.Equal(TimeSpan.FromSeconds(30), options.Interval);
    }

    #endregion
}
