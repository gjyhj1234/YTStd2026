using Microsoft.CodeAnalysis;

namespace YTStdI18n.Generator;

/// <summary>
/// i18n Source Generator 诊断描述器集合。
/// 诊断 ID 使用 YTSI 前缀。
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>YTSI001: 未找到 IsBase=true 的基准语言包。</summary>
    public static readonly DiagnosticDescriptor NoBaseLang = new DiagnosticDescriptor(
        id: "YTSI001",
        title: "Missing base language pack",
        messageFormat: "No class marked with [I18nResource(IsBase = true)] was found",
        category: "I18nGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>YTSI002: 存在多个 IsBase=true 的语言包。</summary>
    public static readonly DiagnosticDescriptor MultipleBaseLangs = new DiagnosticDescriptor(
        id: "YTSI002",
        title: "Multiple base languages",
        messageFormat: "Multiple classes marked with [I18nResource(IsBase = true)]: '{0}' and '{1}'",
        category: "I18nGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>YTSI003: 非基准语言包缺少基准语言中的 key。</summary>
    public static readonly DiagnosticDescriptor MissingKey = new DiagnosticDescriptor(
        id: "YTSI003",
        title: "Language pack missing key",
        messageFormat: "Language pack '{0}' is missing key '{1}' which exists in base language '{2}'",
        category: "I18nGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>YTSI004: 非基准语言包包含基准语言中不存在的 key。</summary>
    public static readonly DiagnosticDescriptor ExtraKey = new DiagnosticDescriptor(
        id: "YTSI004",
        title: "Language pack has extra key",
        messageFormat: "Language pack '{0}' contains key '{1}' which does not exist in base language '{2}'",
        category: "I18nGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>YTSI005: 同一语言包中存在重复 key（相同 const int 值）。</summary>
    public static readonly DiagnosticDescriptor DuplicateKey = new DiagnosticDescriptor(
        id: "YTSI005",
        title: "Duplicate key value in language pack",
        messageFormat: "Language pack '{0}' has duplicate const int value {1} in fields '{2}' and '{3}'",
        category: "I18nGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <summary>YTSI006: 前端 generated 目录不存在（跳过前端生成）。</summary>
    public static readonly DiagnosticDescriptor FrontendPathNotFound = new DiagnosticDescriptor(
        id: "YTSI006",
        title: "Frontend generated directory not found",
        messageFormat: "Frontend output path '{0}' does not exist, skipping frontend JSON generation",
        category: "I18nGenerator",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
