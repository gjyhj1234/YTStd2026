using System;
using YTStdEntity.Attributes;

namespace YTStdTenantPlatform.Entity.TenantPlatform;

/// <summary>数据字典</summary>
[Entity(TableName = "sys_dictionary", NeedAuditTable = true)]
[Index("uq_sys_dictionary_type_code_item_code", "type_code", "item_code", Kind = IndexKind.Unique)]
[Index("idx_sys_dictionary_type_code", "type_code")]
[Index("idx_sys_dictionary_sort_order", "sort_order")]
public class PlatformDictionary
{
    /// <summary>主键</summary>
    [Column(IsPrimaryKey = true)]
    public long Id { get; set; }

    /// <summary>字典类型编码</summary>
    [Column(Length = 64, IsRequired = true)]
    public string TypeCode { get; set; } = "";

    /// <summary>字典类型名称</summary>
    [Column(Length = 128, IsRequired = true)]
    public string TypeName { get; set; } = "";

    /// <summary>字典项编码</summary>
    [Column(Length = 64, IsRequired = true)]
    public string ItemCode { get; set; } = "";

    /// <summary>字典项名称</summary>
    [Column(Length = 128, IsRequired = true)]
    public string ItemName { get; set; } = "";

    /// <summary>字典项值</summary>
    [Column(Length = 255)]
    public string? ItemValue { get; set; }

    /// <summary>是否启用</summary>
    [Column(ColumnName = "is_enabled")]
    public bool IsEnabled { get; set; } = true;

    /// <summary>排序号</summary>
    public int SortOrder { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }

    /// <summary>创建人</summary>
    public long? CreatedBy { get; set; }

    /// <summary>更新人</summary>
    public long? UpdatedBy { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>更新时间</summary>
    public DateTime UpdatedAt { get; set; }
}
