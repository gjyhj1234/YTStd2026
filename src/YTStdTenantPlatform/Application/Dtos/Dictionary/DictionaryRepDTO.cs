using System;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>字典项</summary>
    public sealed class DictionaryRepDTO
    {
        /// <summary>字典 ID</summary>
        public long Id { get; set; }
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
        /// <summary>是否启用</summary>
        public bool IsEnabled { get; set; }
        /// <summary>排序号</summary>
        public int SortOrder { get; set; }
        /// <summary>备注</summary>
        public string? Remark { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>字典类型摘要</summary>
    public sealed class DictionaryTypeRepDTO
    {
        /// <summary>类型编码</summary>
        public string TypeCode { get; set; } = "";
        /// <summary>类型名称</summary>
        public string TypeName { get; set; } = "";
        /// <summary>项数量</summary>
        public int ItemCount { get; set; }
    }
}
