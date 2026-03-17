using System;
using System.Collections.Generic;

namespace YTStdTenantPlatform.Application.Dtos
{
    /// <summary>统一 API 响应包装</summary>
    public sealed class ApiResult
    {
        /// <summary>是否成功</summary>
        public bool Success { get; set; }

        /// <summary>消息</summary>
        public string Message { get; set; } = "";

        /// <summary>TraceId</summary>
        public string TraceId { get; set; } = "";

        /// <summary>成功结果</summary>
        public static ApiResult Ok(string message = "操作成功")
            => new ApiResult { Success = true, Message = message };

        /// <summary>失败结果</summary>
        public static ApiResult Fail(string message = "操作失败")
            => new ApiResult { Success = false, Message = message };
    }

    /// <summary>带数据的统一 API 响应包装</summary>
    public sealed class ApiResult<T>
    {
        /// <summary>是否成功</summary>
        public bool Success { get; set; }

        /// <summary>消息</summary>
        public string Message { get; set; } = "";

        /// <summary>数据</summary>
        public T? Data { get; set; }

        /// <summary>TraceId</summary>
        public string TraceId { get; set; } = "";

        /// <summary>成功结果</summary>
        public static ApiResult<T> Ok(T data, string message = "操作成功")
            => new ApiResult<T> { Success = true, Message = message, Data = data };

        /// <summary>失败结果</summary>
        public static ApiResult<T> Fail(string message = "操作失败")
            => new ApiResult<T> { Success = false, Message = message };
    }

    /// <summary>分页请求</summary>
    public sealed class PagedRequest
    {
        /// <summary>页码（从 1 开始）</summary>
        public int Page { get; set; } = 1;

        /// <summary>每页条数</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>关键字搜索</summary>
        public string? Keyword { get; set; }

        /// <summary>状态过滤</summary>
        public string? Status { get; set; }

        /// <summary>规范化页码（最小为 1）</summary>
        public int NormalizedPage => Page < 1 ? 1 : Page;

        /// <summary>规范化每页条数（1~200）</summary>
        public int NormalizedPageSize => PageSize < 1 ? 20 : (PageSize > 200 ? 200 : PageSize);

        /// <summary>偏移量</summary>
        public int Offset => (NormalizedPage - 1) * NormalizedPageSize;
    }

    /// <summary>分页响应</summary>
    public sealed class PagedResult<T>
    {
        /// <summary>数据列表</summary>
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

        /// <summary>总条数</summary>
        public long Total { get; set; }

        /// <summary>当前页码</summary>
        public int Page { get; set; }

        /// <summary>每页条数</summary>
        public int PageSize { get; set; }

        /// <summary>总页数</summary>
        public int TotalPages => PageSize > 0 ? (int)((Total + PageSize - 1) / PageSize) : 0;
    }
}
