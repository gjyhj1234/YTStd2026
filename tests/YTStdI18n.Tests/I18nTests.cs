using Xunit;
using YTStdI18n;

namespace YTStdI18n.Tests;

public class I18nCoreTests
{
    [Fact]
    public void Init_SetsDefaultLang()
    {
        I18nCore.Init(Lang.En);
        Assert.Equal(Lang.En, I18nCore.DefaultLang);

        // Reset to default
        I18nCore.Init(Lang.ZhCn);
    }

    [Fact]
    public void DefaultLang_CanBeSetAndGet()
    {
        I18nCore.Init(Lang.ZhCn);
        Assert.Equal(Lang.ZhCn, I18nCore.DefaultLang);

        I18nCore.DefaultLang = Lang.En;
        Assert.Equal(Lang.En, I18nCore.DefaultLang);

        // Reset
        I18nCore.DefaultLang = Lang.ZhCn;
    }

    [Fact]
    public void SetTenantLang_SetsAndGetsTenantLanguage()
    {
        I18nCore.Init(Lang.ZhCn);
        I18nCore.SetTenantLang(1001, Lang.En);

        Assert.Equal(Lang.En, I18nCore.GetTenantLang(1001));
    }

    [Fact]
    public void GetTenantLang_UnsetTenant_ReturnsDefaultLang()
    {
        I18nCore.Init(Lang.ZhCn);

        // Use a unique tenant ID that won't conflict
        Assert.Equal(Lang.ZhCn, I18nCore.GetTenantLang(99999));
    }

    [Fact]
    public void LangCount_IsFive()
    {
        Assert.Equal(5, I18nCore.LangCount);
    }

    [Fact]
    public void RegisterTranslations_StoresData()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "测试文本" };
        I18nCore.RegisterTranslations(translations);

        Assert.Equal("测试文本", I18nCore.T(0, 0));
    }

    [Fact]
    public void T_ReturnsEmptyString_WhenNotRegistered()
    {
        I18nCore.Init(Lang.ZhCn);
        // Init clears _translations
        Assert.Equal(string.Empty, I18nCore.T(0, 0));
    }

    [Fact]
    public void T_ReturnsCorrectTranslation_ForDefaultLang()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "中文", "中文2" };
        translations[1] = new[] { "English", "English2" };
        I18nCore.RegisterTranslations(translations);

        Assert.Equal("中文", I18nCore.T(0, 0));
        Assert.Equal("中文2", I18nCore.T(0, 1));
    }

    [Fact]
    public void T_ReturnsCorrectTranslation_ForTenantLang()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "中文" };
        translations[1] = new[] { "English" };
        I18nCore.RegisterTranslations(translations);

        I18nCore.SetTenantLang(5001, Lang.En);
        Assert.Equal("English", I18nCore.T(5001, 0));
    }

    [Fact]
    public void T_FallbackToZhCn_WhenTargetLangNull()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "中文回退" };
        translations[1] = new string[] { null! };
        I18nCore.RegisterTranslations(translations);

        I18nCore.SetTenantLang(6001, Lang.En);
        Assert.Equal("中文回退", I18nCore.T(6001, 0));
    }

    [Fact]
    public void T_FallbackToZhCn_WhenTargetLangArrayNull()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "中文回退" };
        // translations[2] (Ja) is null
        I18nCore.RegisterTranslations(translations);

        I18nCore.SetTenantLang(7001, Lang.Ja);
        Assert.Equal("中文回退", I18nCore.T(7001, 0));
    }

    [Fact]
    public void T_ReturnsEmpty_WhenKeyOutOfRange()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "只有一项" };
        I18nCore.RegisterTranslations(translations);

        Assert.Equal(string.Empty, I18nCore.T(0, 999));
    }

    [Fact]
    public void Init_ClearsTranslations()
    {
        I18nCore.Init(Lang.ZhCn);
        var translations = new string[5][];
        translations[0] = new[] { "数据" };
        I18nCore.RegisterTranslations(translations);

        Assert.Equal("数据", I18nCore.T(0, 0));

        // Re-init should clear
        I18nCore.Init(Lang.ZhCn);
        Assert.Equal(string.Empty, I18nCore.T(0, 0));
    }
}

