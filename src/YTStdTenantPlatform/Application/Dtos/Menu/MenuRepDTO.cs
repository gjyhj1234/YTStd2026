using System;
using System.Collections.Generic;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>菜单列表项</summary>
    public sealed class MenuRepDTO
    {
        /// <summary>菜单 ID</summary>
        public long Id { get; set; }
        /// <summary>父菜单 ID</summary>
        public long ParentId { get; set; }
        /// <summary>菜单编码</summary>
        public string Code { get; set; } = "";
        /// <summary>菜单名称</summary>
        public string Name { get; set; } = "";
        /// <summary>图标</summary>
        public string? Icon { get; set; }
        /// <summary>路由路径</summary>
        public string? RoutePath { get; set; }
        /// <summary>组件路径</summary>
        public string? ComponentPath { get; set; }
        /// <summary>权限编码</summary>
        public string? PermissionCode { get; set; }
        /// <summary>菜单类型</summary>
        public int MenuType { get; set; }
        /// <summary>是否启用</summary>
        public bool IsEnabled { get; set; }
        /// <summary>是否外链</summary>
        public bool IsExternal { get; set; }
        /// <summary>是否可见</summary>
        public bool IsVisible { get; set; }
        /// <summary>排序号</summary>
        public int SortOrder { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>子菜单</summary>
        public List<MenuRepDTO>? Children { get; set; }
    }
}
