using System;
using System.Threading.Tasks;

namespace YTStdAdo;

/// <summary>
/// 全局静态数据库入口类。
/// 所有数据库操作通过此类进行，内部管理连接池。
/// </summary>
public static partial class DB
{
    /// <summary>初始化连接池，应用启动时调用一次</summary>
    public static void Init(DbOptions options)
    {
        // TODO: 实现连接池初始化
        throw new NotImplementedException();
    }

    /// <summary>优雅关闭连接池</summary>
    public static ValueTask ShutdownAsync()
    {
        // TODO: 实现连接池关闭
        throw new NotImplementedException();
    }
}
