namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>平台角色简要信息（用于下拉选择）</summary>
    public sealed class PlatformRoleSimpleRepDTO
    {
        /// <summary>角色 ID</summary>
        public long Id { get; set; }
        /// <summary>角色编码</summary>
        public string Code { get; set; } = "";
        /// <summary>角色名称</summary>
        public string Name { get; set; } = "";
        /// <summary>状态</summary>
        public string Status { get; set; } = "";
    }
}
