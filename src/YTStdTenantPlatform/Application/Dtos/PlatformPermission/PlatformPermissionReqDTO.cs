using System;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>创建平台权限请求</summary>
    public sealed class CreatePlatformPermissionReqDTO
    {
        /// <summary>权限编码</summary>
        public string Code { get; set; } = "";
        /// <summary>权限名称</summary>
        public string Name { get; set; } = "";
        /// <summary>资源标识</summary>
        public string? Resource { get; set; }
        /// <summary>操作标识</summary>
        public string? Action { get; set; }
        /// <summary>权限类型</summary>
        public string? PermissionType { get; set; }
        /// <summary>父级权限 ID</summary>
        public long? ParentId { get; set; }
        /// <summary>排序号</summary>
        public int SortOrder { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
    }

    /// <summary>更新平台权限请求</summary>
    public sealed class UpdatePlatformPermissionReqDTO
    {
        /// <summary>权限名称</summary>
        public string? Name { get; set; }
        /// <summary>资源标识</summary>
        public string? Resource { get; set; }
        /// <summary>操作标识</summary>
        public string? Action { get; set; }
        /// <summary>排序号</summary>
        public int? SortOrder { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
    }
}
