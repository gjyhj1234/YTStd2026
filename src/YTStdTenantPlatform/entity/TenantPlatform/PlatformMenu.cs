using System;
using YTStdEntity.Attributes;

namespace YTStdTenantPlatform.Entity.TenantPlatform;

/// <summary>平台菜单</summary>
[Entity(TableName = "sys_menu", NeedAuditTable = true)]
[Index("uq_sys_menu_code", "code", Kind = IndexKind.Unique)]
[Index("idx_sys_menu_parent_id", "parent_id")]
[Index("idx_sys_menu_sort_order", "sort_order")]
public class PlatformMenu
{
    /// <summary>主键</summary>
    [Column(IsPrimaryKey = true)]
    public long Id { get; set; }

    /// <summary>父菜单 ID（顶级为 0）</summary>
    public long ParentId { get; set; }

    /// <summary>菜单编码</summary>
    [Column(Length = 128, IsRequired = true)]
    public string Code { get; set; } = "";

    /// <summary>菜单名称</summary>
    [Column(Length = 128, IsRequired = true)]
    public string Name { get; set; } = "";

    /// <summary>菜单图标</summary>
    [Column(Length = 128)]
    public string? Icon { get; set; }

    /// <summary>路由路径</summary>
    [Column(Length = 255)]
    public string? RoutePath { get; set; }

    /// <summary>组件路径</summary>
    [Column(Length = 255)]
    public string? ComponentPath { get; set; }

    /// <summary>关联权限码</summary>
    [Column(Length = 128)]
    public string? PermissionCode { get; set; }

    /// <summary>菜单类型（1=目录 2=菜单 3=按钮）</summary>
    [Column(DbType = "smallint")]
    public int MenuType { get; set; }

    /// <summary>是否启用</summary>
    [Column(ColumnName = "is_enabled")]
    public bool IsEnabled { get; set; }

    /// <summary>是否外链</summary>
    [Column(ColumnName = "is_external")]
    public bool IsExternal { get; set; }

    /// <summary>是否可见</summary>
    [Column(ColumnName = "is_visible")]
    public bool IsVisible { get; set; } = true;

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
