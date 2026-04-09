namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>修改密码请求参数</summary>
    public sealed class ChangePasswordReqDTO
    {
        /// <summary>旧密码</summary>
        public string OldPassword { get; set; } = "";
        /// <summary>新密码</summary>
        public string NewPassword { get; set; } = "";
    }
}
