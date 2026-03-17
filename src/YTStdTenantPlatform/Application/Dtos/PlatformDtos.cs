using System;

namespace YTStdTenantPlatform.Application.Dtos
{
    // ──────────────────────────────────────────────────────────
    // 平台用户 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>平台用户列表项</summary>
    public sealed class PlatformUserDto
    {
        /// <summary>用户 ID</summary>
        public long Id { get; set; }
        /// <summary>用户名</summary>
        public string Username { get; set; } = "";
        /// <summary>邮箱</summary>
        public string Email { get; set; } = "";
        /// <summary>手机号</summary>
        public string? Phone { get; set; }
        /// <summary>显示名称</summary>
        public string DisplayName { get; set; } = "";
        /// <summary>状态（active/disabled/locked）</summary>
        public string Status { get; set; } = "";
        /// <summary>是否启用 MFA</summary>
        public bool MfaEnabled { get; set; }
        /// <summary>最后登录时间</summary>
        public DateTime? LastLoginAt { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>创建平台用户请求</summary>
    public sealed class CreatePlatformUserRequest
    {
        /// <summary>用户名</summary>
        public string Username { get; set; } = "";
        /// <summary>邮箱</summary>
        public string Email { get; set; } = "";
        /// <summary>手机号</summary>
        public string? Phone { get; set; }
        /// <summary>显示名称</summary>
        public string DisplayName { get; set; } = "";
        /// <summary>初始密码</summary>
        public string Password { get; set; } = "";
        /// <summary>备注</summary>
        public string? Remark { get; set; }
    }

    /// <summary>更新平台用户请求</summary>
    public sealed class UpdatePlatformUserRequest
    {
        /// <summary>显示名称</summary>
        public string? DisplayName { get; set; }
        /// <summary>手机号</summary>
        public string? Phone { get; set; }
        /// <summary>邮箱</summary>
        public string? Email { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
    }

    // ──────────────────────────────────────────────────────────
    // 平台角色 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>平台角色列表项</summary>
    public sealed class PlatformRoleDto
    {
        /// <summary>角色 ID</summary>
        public long Id { get; set; }
        /// <summary>角色编码</summary>
        public string Code { get; set; } = "";
        /// <summary>角色名称</summary>
        public string Name { get; set; } = "";
        /// <summary>描述</summary>
        public string? Description { get; set; }
        /// <summary>状态</summary>
        public string Status { get; set; } = "";
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>创建平台角色请求</summary>
    public sealed class CreatePlatformRoleRequest
    {
        /// <summary>角色编码</summary>
        public string Code { get; set; } = "";
        /// <summary>角色名称</summary>
        public string Name { get; set; } = "";
        /// <summary>描述</summary>
        public string? Description { get; set; }
    }

    /// <summary>更新平台角色请求</summary>
    public sealed class UpdatePlatformRoleRequest
    {
        /// <summary>角色名称</summary>
        public string? Name { get; set; }
        /// <summary>描述</summary>
        public string? Description { get; set; }
    }

    /// <summary>角色授权请求（批量绑定权限）</summary>
    public sealed class RolePermissionBindRequest
    {
        /// <summary>权限 ID 列表</summary>
        public long[] PermissionIds { get; set; } = Array.Empty<long>();
    }

    /// <summary>角色成员请求（批量绑定用户）</summary>
    public sealed class RoleMemberBindRequest
    {
        /// <summary>用户 ID 列表</summary>
        public long[] UserIds { get; set; } = Array.Empty<long>();
    }

    // ──────────────────────────────────────────────────────────
    // 平台权限 DTO
    // ──────────────────────────────────────────────────────────

    /// <summary>平台权限树节点</summary>
    public sealed class PlatformPermissionDto
    {
        /// <summary>权限 ID</summary>
        public long Id { get; set; }
        /// <summary>权限编码</summary>
        public string Code { get; set; } = "";
        /// <summary>权限名称</summary>
        public string Name { get; set; } = "";
        /// <summary>权限类型（menu/api/action）</summary>
        public string PermissionType { get; set; } = "";
        /// <summary>父级权限 ID</summary>
        public long? ParentId { get; set; }
        /// <summary>路径</summary>
        public string? Path { get; set; }
        /// <summary>HTTP 方法</summary>
        public string? Method { get; set; }
        /// <summary>子节点列表</summary>
        public System.Collections.Generic.List<PlatformPermissionDto>? Children { get; set; }
    }
}
