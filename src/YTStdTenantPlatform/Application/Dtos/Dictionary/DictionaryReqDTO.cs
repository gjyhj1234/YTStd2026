namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>创建字典请求</summary>
    public sealed class CreateDictionaryReqDTO
    {
        /// <summary>类型编码</summary>
        public string TypeCode { get; set; } = "";
        /// <summary>类型名称</summary>
        public string TypeName { get; set; } = "";
        /// <summary>项编码</summary>
        public string ItemCode { get; set; } = "";
        /// <summary>项名称</summary>
        public string ItemName { get; set; } = "";
        /// <summary>项值</summary>
        public string? ItemValue { get; set; }
        /// <summary>排序号</summary>
        public int SortOrder { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
    }

    /// <summary>更新字典请求</summary>
    public sealed class UpdateDictionaryReqDTO
    {
        /// <summary>项名称</summary>
        public string? ItemName { get; set; }
        /// <summary>项值</summary>
        public string? ItemValue { get; set; }
        /// <summary>排序号</summary>
        public int? SortOrder { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
    }
}
