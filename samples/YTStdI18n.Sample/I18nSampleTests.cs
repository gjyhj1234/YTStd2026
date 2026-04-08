using Xunit;
using YTStdI18n;
using YTStdI18n.Sample;

namespace YTStdI18n.Sample;

public class I18nSampleTests
{
    public I18nSampleTests()
    {
        // 每次测试前初始化
        I18n.Init(Lang.ZhCn);
        I18n.Register();
    }

    [Fact]
    public void KConstants_HaveCorrectValues()
    {
        // 验证 K 常量索引从 0 递增
        Assert.Equal(0, K.Success);
        Assert.Equal(1, K.Failed);
        Assert.Equal(2, K.NotFound);
        Assert.Equal(3, K.Unauthorized);
        Assert.Equal(4, K.Forbidden);
        Assert.Equal(5, K.ValidationError);
        Assert.Equal(6, K.LoginSuccess);
        Assert.Equal(7, K.LoginFailed);
        Assert.Equal(8, K.AccountLocked);
        Assert.Equal(9, K.PasswordTooShort);
        Assert.Equal(10, K.ConnectionFailed);
        Assert.Equal(11, K.QueryTimeout);
        Assert.Equal(12, K.InsertFailed);
        Assert.Equal(13, K.UpdateFailed);
        Assert.Equal(14, K.DeleteFailed);
    }

    [Fact]
    public void T_ZhCn_ReturnsChineseTranslation()
    {
        I18n.DefaultLang = Lang.ZhCn;
        Assert.Equal("操作成功", I18n.T(0, K.Success));
        Assert.Equal("操作失败", I18n.T(0, K.Failed));
        Assert.Equal("未找到数据", I18n.T(0, K.NotFound));
    }

    [Fact]
    public void T_En_ReturnsEnglishTranslation()
    {
        I18n.DefaultLang = Lang.En;
        Assert.Equal("Operation successful", I18n.T(0, K.Success));
        Assert.Equal("Operation failed", I18n.T(0, K.Failed));
        Assert.Equal("Data not found", I18n.T(0, K.NotFound));
    }

    [Fact]
    public void T_UserMessages_ZhCn()
    {
        I18n.DefaultLang = Lang.ZhCn;
        Assert.Equal("登录成功", I18n.T(0, K.LoginSuccess));
        Assert.Equal("用户名或密码错误", I18n.T(0, K.LoginFailed));
        Assert.Equal("账户已被锁定", I18n.T(0, K.AccountLocked));
        Assert.Equal("密码长度不能少于 8 位", I18n.T(0, K.PasswordTooShort));
    }

    [Fact]
    public void T_UserMessages_En()
    {
        I18n.DefaultLang = Lang.En;
        Assert.Equal("Login successful", I18n.T(0, K.LoginSuccess));
        Assert.Equal("Invalid username or password", I18n.T(0, K.LoginFailed));
        Assert.Equal("Account has been locked", I18n.T(0, K.AccountLocked));
        Assert.Equal("Password must be at least 8 characters", I18n.T(0, K.PasswordTooShort));
    }

    [Fact]
    public void T_DbMessages_ZhCn()
    {
        I18n.DefaultLang = Lang.ZhCn;
        Assert.Equal("数据库连接失败", I18n.T(0, K.ConnectionFailed));
        Assert.Equal("新增数据失败", I18n.T(0, K.InsertFailed));
    }

    [Fact]
    public void T_DbMessages_En()
    {
        I18n.DefaultLang = Lang.En;
        Assert.Equal("Database connection failed", I18n.T(0, K.ConnectionFailed));
        Assert.Equal("Insert data failed", I18n.T(0, K.InsertFailed));
    }

    [Fact]
    public void T_WithTenantId_UsesTenantLang()
    {
        I18n.DefaultLang = Lang.ZhCn;
        I18n.SetTenantLang(1001, Lang.En);
        Assert.Equal("Operation successful", I18n.T(1001, K.Success));
    }

    [Fact]
    public void LanguageSwitching_WorksAtRuntime()
    {
        I18n.DefaultLang = Lang.ZhCn;
        Assert.Equal("操作成功", I18n.T(0, K.Success));

        I18n.DefaultLang = Lang.En;
        Assert.Equal("Operation successful", I18n.T(0, K.Success));

        // 切回
        I18n.DefaultLang = Lang.ZhCn;
        Assert.Equal("操作成功", I18n.T(0, K.Success));
    }

    [Fact]
    public void TenantLang_IndependentFromGlobal()
    {
        I18n.DefaultLang = Lang.ZhCn;
        I18n.SetTenantLang(2001, Lang.En);

        // 全局使用中文
        Assert.Equal("操作成功", I18n.T(0, K.Success));
        // 租户 2001 使用英文
        Assert.Equal("Operation successful", I18n.T(2001, K.Success));
    }

    [Fact]
    public void T_FallbackToZhCn_WhenLangNotRegistered()
    {
        // 日语未注册，应回退到简体中文
        I18n.SetTenantLang(3001, Lang.Ja);
        Assert.Equal("操作成功", I18n.T(3001, K.Success));
    }

    [Fact]
    public void T_ReturnsEmpty_WhenNotInitialized()
    {
        I18nCore.Init(Lang.ZhCn);
        // 未调用 Register()，translations 为 null
        Assert.Equal(string.Empty, I18nCore.T(0, 0));

        // 恢复
        I18n.Register();
    }
}
