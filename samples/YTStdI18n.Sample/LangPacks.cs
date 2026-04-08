using YTStdI18n;

namespace YTStdI18n.Sample;

/// <summary>
/// 简体中文语言包（基准语言）。
/// 所有 const int 字段的声明顺序即为 K 常量索引的顺序。
/// 翻译文本通过 XML summary 注释定义。
/// </summary>
[I18nResource(IsBase = true)]
public static class LangZhCn
{
    /// <summary>操作成功</summary>
    public const int Success = 0;

    /// <summary>操作失败</summary>
    public const int Failed = 1;

    /// <summary>未找到数据</summary>
    public const int NotFound = 10001;

    /// <summary>未授权访问</summary>
    public const int Unauthorized = 10002;

    /// <summary>禁止访问</summary>
    public const int Forbidden = 10003;

    /// <summary>数据验证失败</summary>
    public const int ValidationError = 10004;

    /// <summary>登录成功</summary>
    public const int LoginSuccess = 20001;

    /// <summary>用户名或密码错误</summary>
    public const int LoginFailed = 20002;

    /// <summary>账户已被锁定</summary>
    public const int AccountLocked = 20003;

    /// <summary>密码长度不能少于 8 位</summary>
    public const int PasswordTooShort = 20004;

    /// <summary>数据库连接失败</summary>
    public const int ConnectionFailed = 30001;

    /// <summary>查询超时</summary>
    public const int QueryTimeout = 30002;

    /// <summary>新增数据失败</summary>
    public const int InsertFailed = 30003;

    /// <summary>更新数据失败</summary>
    public const int UpdateFailed = 30004;

    /// <summary>删除数据失败</summary>
    public const int DeleteFailed = 30005;
}

/// <summary>
/// 英语语言包。
/// const int 字段名必须与基准语言 LangZhCn 严格一致。
/// </summary>
[I18nResource]
public static class LangEn
{
    /// <summary>Operation successful</summary>
    public const int Success = 0;

    /// <summary>Operation failed</summary>
    public const int Failed = 1;

    /// <summary>Data not found</summary>
    public const int NotFound = 10001;

    /// <summary>Unauthorized access</summary>
    public const int Unauthorized = 10002;

    /// <summary>Access forbidden</summary>
    public const int Forbidden = 10003;

    /// <summary>Validation error</summary>
    public const int ValidationError = 10004;

    /// <summary>Login successful</summary>
    public const int LoginSuccess = 20001;

    /// <summary>Invalid username or password</summary>
    public const int LoginFailed = 20002;

    /// <summary>Account has been locked</summary>
    public const int AccountLocked = 20003;

    /// <summary>Password must be at least 8 characters</summary>
    public const int PasswordTooShort = 20004;

    /// <summary>Database connection failed</summary>
    public const int ConnectionFailed = 30001;

    /// <summary>Query timeout</summary>
    public const int QueryTimeout = 30002;

    /// <summary>Insert data failed</summary>
    public const int InsertFailed = 30003;

    /// <summary>Update data failed</summary>
    public const int UpdateFailed = 30004;

    /// <summary>Delete data failed</summary>
    public const int DeleteFailed = 30005;
}
