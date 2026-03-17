using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace YTStdTenantPlatform.Infrastructure.Auth
{
    /// <summary>当前已认证用户的上下文信息</summary>
    public sealed class CurrentUser
    {
        /// <summary>用户 ID</summary>
        public long UserId { get; }

        /// <summary>用户名</summary>
        public string Username { get; }

        /// <summary>显示名称</summary>
        public string DisplayName { get; }

        /// <summary>当前用户拥有的角色编码列表</summary>
        public IReadOnlyList<string> Roles { get; }

        /// <summary>当前用户拥有的权限编码列表</summary>
        public IReadOnlyList<string> Permissions { get; }

        /// <summary>是否为超级管理员</summary>
        public bool IsSuperAdmin { get; }

        /// <summary>请求关联的 TraceId</summary>
        public string TraceId { get; }

        /// <summary>构造当前用户上下文</summary>
        public CurrentUser(
            long userId,
            string username,
            string displayName,
            IReadOnlyList<string> roles,
            IReadOnlyList<string> permissions,
            bool isSuperAdmin,
            string traceId)
        {
            UserId = userId;
            Username = username;
            DisplayName = displayName;
            Roles = roles;
            Permissions = permissions;
            IsSuperAdmin = isSuperAdmin;
            TraceId = traceId;
        }

        /// <summary>匿名用户常量（未认证时使用）</summary>
        public static readonly CurrentUser Anonymous = new CurrentUser(
            0, "anonymous", "匿名用户",
            Array.Empty<string>(), Array.Empty<string>(),
            false, string.Empty);

        /// <summary>HttpContext.Items 中存储当前用户的 Key</summary>
        public const string HttpContextKey = "__CurrentUser__";

        /// <summary>检查当前用户是否拥有指定权限编码</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasPermission(string permissionCode)
        {
            if (IsSuperAdmin) return true;

            for (int i = 0; i < Permissions.Count; i++)
            {
                if (string.Equals(Permissions[i], permissionCode, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>检查当前用户是否拥有指定角色编码</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasRole(string roleCode)
        {
            if (IsSuperAdmin) return true;

            for (int i = 0; i < Roles.Count; i++)
            {
                if (string.Equals(Roles[i], roleCode, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
