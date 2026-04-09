using System;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>创建平台用户请求</summary>
    public sealed class CreatePlatformUserReqDTO
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
    public sealed class UpdatePlatformUserReqDTO
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

    /// <summary>重置密码请求</summary>
    public sealed class ResetPasswordReqDTO
    {
        /// <summary>新密码（为空则自动生成）</summary>
        public string? NewPassword { get; set; }
    }

    /// <summary>批量用户 ID 请求</summary>
    public sealed class BatchUserIdsReqDTO
    {
        /// <summary>用户 ID 列表</summary>
        public long[] Ids { get; set; } = [];
    }
}
