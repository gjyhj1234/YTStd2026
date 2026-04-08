using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace YTStdI18n.Generator;

/// <summary>
/// i18n Source Generator（重构版）。
/// 扫描标注了 [I18nResource] 的语言资源类（const int 成员），自动生成：
/// 1. K 常量索引类（平坦结构，数组索引从 0 递增）
/// 2. I18n 包装类（包含 T() 翻译方法）
/// 3. I18n.Register() 注册方法（填充 string[][] 翻译数组）
/// 4. 前端 locales/generated/{locale}.json（增量策略）
/// 5. 编译期校验所有语言包一致性（YTSI001-YTSI006）
/// </summary>
[Generator]
public sealed class I18nGenerator : IIncrementalGenerator
{
    private const string AttributeName = "YTStdI18n.I18nResourceAttribute";

    /// <summary>
    /// 初始化增量生成器管线。
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 扫描所有 [I18nResource] 标注的类
        var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName,
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) => GetResourceClassInfo(ctx))
            .Where(static x => x != null)
            .Collect();

        // 获取全局配置（前端输出路径）
        var globalOptions = context.AnalyzerConfigOptionsProvider;

        // 合并并生成
        var combined = classDeclarations.Combine(globalOptions);
        context.RegisterSourceOutput(combined, static (spc, pair) =>
        {
            Execute(spc, pair.Left!, pair.Right);
        });
    }

    /// <summary>
    /// 从语法上下文中提取资源类信息。
    /// </summary>
    private static ResourceClassInfo? GetResourceClassInfo(GeneratorAttributeSyntaxContext ctx)
    {
        if (ctx.TargetNode is not ClassDeclarationSyntax classDecl)
            return null;

        var classSymbol = ctx.TargetSymbol as INamedTypeSymbol;
        if (classSymbol == null)
            return null;

        // 读取 IsBase 属性
        bool isBase = false;
        foreach (var attrData in classSymbol.GetAttributes())
        {
            if (attrData.AttributeClass?.ToDisplayString() == AttributeName)
            {
                foreach (var named in attrData.NamedArguments)
                {
                    if (named.Key == "IsBase" && named.Value.Value is bool b)
                    {
                        isBase = b;
                    }
                }
            }
        }

        string className = classSymbol.Name;
        string? ns = classSymbol.ContainingNamespace?.IsGlobalNamespace == true
            ? null
            : classSymbol.ContainingNamespace?.ToDisplayString();

        // 提取所有 const int 字段及其 XML 注释或 [Description] 属性
        var fields = new List<ResourceFieldInfo>();

        foreach (var member in classDecl.Members)
        {
            if (member is not FieldDeclarationSyntax fieldDecl) continue;

            // 检查是否是 const
            bool isConst = false;
            foreach (var modifier in fieldDecl.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.ConstKeyword))
                {
                    isConst = true;
                    break;
                }
            }
            if (!isConst) continue;

            // 检查类型是否是 int
            var semanticModel = ctx.SemanticModel;
            var typeInfo = semanticModel.GetTypeInfo(fieldDecl.Declaration.Type);
            if (typeInfo.Type == null || typeInfo.Type.SpecialType != SpecialType.System_Int32)
                continue;

            // 提取每个变量声明
            foreach (var variable in fieldDecl.Declaration.Variables)
            {
                string fieldName = variable.Identifier.Text;

                // 提取 const 值
                int fieldValue = 0;
                if (variable.Initializer?.Value != null)
                {
                    var constantValue = semanticModel.GetConstantValue(variable.Initializer.Value);
                    if (constantValue.HasValue && constantValue.Value is int intVal)
                    {
                        fieldValue = intVal;
                    }
                }

                // 提取翻译文本：优先 IFieldSymbol 的 XML 注释，其次 trivia，最后 [Description] 属性
                var fieldSymbol = semanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                string? translation = ExtractSummaryFromSymbol(fieldSymbol);
                if (translation == null)
                {
                    // 尝试从语法树 trivia 提取
                    translation = ExtractXmlSummary(fieldDecl);
                }
                if (translation == null)
                {
                    // 尝试从 [Description] 属性提取
                    translation = ExtractDescriptionAttribute(fieldSymbol);
                }
                if (translation == null)
                {
                    // 最后尝试从字段行内注释提取 (/// text 或 // text)
                    translation = ExtractFromLineComment(fieldDecl);
                }

                fields.Add(new ResourceFieldInfo
                {
                    Name = fieldName,
                    Value = fieldValue,
                    Translation = translation,
                    Location = variable.GetLocation()
                });
            }
        }

        return new ResourceClassInfo
        {
            ClassName = className,
            Namespace = ns,
            IsBase = isBase,
            Fields = fields,
            Location = classDecl.GetLocation()
        };
    }

    /// <summary>
    /// 从字段符号的 XML 文档注释中提取 summary 文本。
    /// </summary>
    private static string? ExtractSummaryFromSymbol(IFieldSymbol? fieldSymbol)
    {
        if (fieldSymbol == null) return null;

        string? xml = fieldSymbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(xml)) return null;

        // 手动解析 <summary>...</summary>
        int startTag = xml.IndexOf("<summary>");
        if (startTag < 0) return null;
        int contentStart = startTag + 9; // "<summary>".Length

        int endTag = xml.IndexOf("</summary>", contentStart);
        if (endTag < 0) return null;

        string content = xml.Substring(contentStart, endTag - contentStart).Trim();
        if (content.Length == 0) return null;

        return content;
    }

    /// <summary>
    /// 从字段声明的 XML 文档注释中提取 summary 文本。
    /// </summary>
    private static string? ExtractXmlSummary(FieldDeclarationSyntax fieldDecl)
    {
        foreach (var trivia in fieldDecl.GetLeadingTrivia())
        {
            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                var structure = trivia.GetStructure();
                if (structure is DocumentationCommentTriviaSyntax docComment)
                {
                    foreach (var node in docComment.Content)
                    {
                        if (node is XmlElementSyntax xmlElement)
                        {
                            // 检查是否是 <summary> 标签
                            string? tagName = null;
                            var startTag = xmlElement.StartTag;
                            if (startTag?.Name != null)
                            {
                                tagName = startTag.Name.ToString();
                            }

                            if (tagName == "summary")
                            {
                                var sb = new StringBuilder();
                                foreach (var contentNode in xmlElement.Content)
                                {
                                    if (contentNode is XmlTextSyntax textSyntax)
                                    {
                                        foreach (var token in textSyntax.TextTokens)
                                        {
                                            if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                                            {
                                                sb.Append(token.Text);
                                            }
                                        }
                                    }
                                }
                                string result = sb.ToString().Trim();
                                if (result.Length > 0)
                                    return result;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 从字段的 [Description] 属性中提取文本。
    /// </summary>
    private static string? ExtractDescriptionAttribute(IFieldSymbol? fieldSymbol)
    {
        if (fieldSymbol == null) return null;

        foreach (var attr in fieldSymbol.GetAttributes())
        {
            string? attrName = attr.AttributeClass?.ToDisplayString();
            if (attrName == "System.ComponentModel.DescriptionAttribute"
                && attr.ConstructorArguments.Length > 0
                && attr.ConstructorArguments[0].Value is string desc)
            {
                return desc;
            }
        }
        return null;
    }

    /// <summary>
    /// 从字段声明的所有 trivia 中尝试提取文本。
    /// 处理 DocumentationMode.None 时 /// 被解析为 SingleLineCommentTrivia 的情况。
    /// </summary>
    private static string? ExtractFromLineComment(FieldDeclarationSyntax fieldDecl)
    {
        // 收集所有 leading trivia 的原始文本，查找 <summary>...</summary>
        foreach (var trivia in fieldDecl.GetLeadingTrivia())
        {
            string raw = trivia.ToFullString();

            // 在任意类型的 trivia 中搜索 <summary>...</summary>
            int startIdx = raw.IndexOf("<summary>");
            if (startIdx >= 0)
            {
                int contentStart = startIdx + 9; // "<summary>".Length
                int endIdx = raw.IndexOf("</summary>", contentStart);
                if (endIdx >= 0)
                {
                    string text = raw.Substring(contentStart, endIdx - contentStart).Trim();
                    if (text.Length > 0) return text;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 执行代码生成和校验。
    /// </summary>
    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<ResourceClassInfo?> rawPacks,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        // 收集有效资源类
        var allPacks = new List<ResourceClassInfo>();
        foreach (var p in rawPacks)
        {
            if (p != null) allPacks.Add(p);
        }

        if (allPacks.Count == 0) return;

        // 查找基准语言
        ResourceClassInfo? baseLang = null;
        var otherLangs = new List<ResourceClassInfo>();
        foreach (var pack in allPacks)
        {
            if (pack.IsBase)
            {
                if (baseLang != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.MultipleBaseLangs,
                        pack.Location,
                        baseLang.ClassName, pack.ClassName));
                    return;
                }
                baseLang = pack;
            }
            else
            {
                otherLangs.Add(pack);
            }
        }

        if (baseLang == null)
        {
            // YTSI001: 未找到基准语言
            Location? firstLoc = allPacks.Count > 0 ? allPacks[0].Location : null;
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.NoBaseLang,
                firstLoc));
            return;
        }

        // 检查基准语言内部是否有重复 const int 值（YTSI005）
        if (!ValidateDuplicateValues(context, baseLang)) return;

        // 检查非基准语言
        foreach (var other in otherLangs)
        {
            ValidateDuplicateValues(context, other);
            ValidateKeyConsistency(context, baseLang, other);
        }

        // 构建基准字段的排序列表（按声明顺序作为数组索引）
        var baseFields = baseLang.Fields;

        // 生成 K 常量索引类
        GenerateKClass(context, baseLang, baseFields);

        // 生成 I18n 包装类 + Register 方法
        GenerateI18nClass(context, baseLang, otherLangs, baseFields);

        // 生成前端 JSON（如果路径已配置）
        GenerateFrontendJson(context, baseLang, otherLangs, baseFields, optionsProvider);
    }

    /// <summary>
    /// 检查语言包内部是否有重复的 const int 值。
    /// </summary>
    /// <returns>无重复返回 true。</returns>
    private static bool ValidateDuplicateValues(
        SourceProductionContext context,
        ResourceClassInfo pack)
    {
        bool valid = true;
        for (int i = 0; i < pack.Fields.Count; i++)
        {
            for (int j = i + 1; j < pack.Fields.Count; j++)
            {
                if (pack.Fields[i].Value == pack.Fields[j].Value)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.DuplicateKey,
                        pack.Fields[j].Location ?? pack.Location,
                        pack.ClassName, pack.Fields[i].Value,
                        pack.Fields[i].Name, pack.Fields[j].Name));
                    valid = false;
                }
            }
        }
        return valid;
    }

    /// <summary>
    /// 校验非基准语言与基准语言的 key 一致性。
    /// </summary>
    private static void ValidateKeyConsistency(
        SourceProductionContext context,
        ResourceClassInfo baseLang,
        ResourceClassInfo other)
    {
        // YTSI003: 非基准缺少基准的 key
        foreach (var baseField in baseLang.Fields)
        {
            bool found = false;
            foreach (var otherField in other.Fields)
            {
                if (otherField.Name == baseField.Name)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.MissingKey,
                    other.Location,
                    other.ClassName, baseField.Name, baseLang.ClassName));
            }
        }

        // YTSI004: 非基准包含基准中不存在的 key
        foreach (var otherField in other.Fields)
        {
            bool found = false;
            foreach (var baseField in baseLang.Fields)
            {
                if (baseField.Name == otherField.Name)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.ExtraKey,
                    otherField.Location ?? other.Location,
                    other.ClassName, otherField.Name, baseLang.ClassName));
            }
        }
    }

    /// <summary>
    /// 生成 K 常量索引类（平坦结构，数组索引从 0 递增）。
    /// </summary>
    private static void GenerateKClass(
        SourceProductionContext context,
        ResourceClassInfo baseLang,
        List<ResourceFieldInfo> baseFields)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("namespace YTStdI18n;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// 国际化键常量索引类。");
        sb.AppendLine("/// 常量值为翻译数组中的索引位置（从 0 递增）。");
        sb.Append("/// 由 Source Generator 根据基准语言 ");
        sb.Append(baseLang.ClassName);
        sb.AppendLine(" 自动生成。");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class K");
        sb.AppendLine("{");

        for (int i = 0; i < baseFields.Count; i++)
        {
            var field = baseFields[i];
            if (field.Translation != null)
            {
                sb.Append("    /// <summary>");
                sb.Append(EscapeXml(field.Translation));
                sb.Append(" (Code=");
                sb.Append(field.Value);
                sb.AppendLine(")</summary>");
            }
            sb.Append("    public const int ");
            sb.Append(field.Name);
            sb.Append(" = ");
            sb.Append(i);
            sb.AppendLine(";");
        }

        sb.AppendLine("}");

        context.AddSource("K.g.cs", sb.ToString());
    }

    /// <summary>
    /// 生成 I18n 包装类和 Register() 方法。
    /// </summary>
    private static void GenerateI18nClass(
        SourceProductionContext context,
        ResourceClassInfo baseLang,
        List<ResourceClassInfo> otherLangs,
        List<ResourceFieldInfo> baseFields)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine("using YTStdLogger.Core;");
        sb.AppendLine();
        sb.AppendLine("namespace YTStdI18n;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// 国际化全局入口（由 Source Generator 自动生成）。");
        sb.AppendLine("/// 包装 I18nCore 并添加 T() 翻译方法和 Register() 注册方法。");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class I18n");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>获取或设置全局默认语言。线程安全（volatile 读写）。</summary>");
        sb.AppendLine("    public static Lang DefaultLang { get => I18nCore.DefaultLang; set => I18nCore.DefaultLang = value; }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>初始化国际化系统。应用启动时调用一次。</summary>");
        sb.AppendLine("    public static void Init(Lang defaultLang = Lang.ZhCn) => I18nCore.Init(defaultLang);");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>设置指定租户的语言偏好。</summary>");
        sb.AppendLine("    public static void SetTenantLang(int tenantId, Lang lang) => I18nCore.SetTenantLang(tenantId, lang);");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>获取指定租户的语言偏好，未设置则返回全局默认语言。</summary>");
        sb.AppendLine("    public static Lang GetTenantLang(int tenantId) => I18nCore.GetTenantLang(tenantId);");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 根据租户语言偏好翻译指定键。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <param name=\"tenantId\">租户 ID。</param>");
        sb.AppendLine("    /// <param name=\"key\">翻译键索引（K 常量值）。</param>");
        sb.AppendLine("    /// <returns>翻译后的文本。</returns>");
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public static string T(int tenantId, int key) => I18nCore.T(tenantId, key);");
        sb.AppendLine();

        // Register() 方法
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// 注册所有翻译数据到 I18nCore。");
        sb.AppendLine("    /// 由 Source Generator 自动生成。应用启动时调用一次。");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static void Register()");
        sb.AppendLine("    {");
        sb.Append("        var translations = new string[");
        sb.Append(I18nCoreLangCount);
        sb.AppendLine("][];");
        sb.AppendLine();

        // 为每种语言生成翻译数组
        // 基准语言（ZhCn = 0）
        GenerateTranslationArray(sb, baseLang, baseFields, 0, "ZhCn");

        // 其他语言
        foreach (var other in otherLangs)
        {
            string langEnumName = ClassNameToLangEnum(other.ClassName);
            int langIndex = LangEnumToIndex(langEnumName);
            if (langIndex >= 0)
            {
                GenerateTranslationArray(sb, other, baseFields, langIndex, langEnumName);
            }
        }

        sb.AppendLine("        I18nCore.RegisterTranslations(translations);");
        sb.AppendLine("        Logger.Info(0, 0L, \"[I18n] 语言包注册完成\");");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("I18n.g.cs", sb.ToString());
    }

    /// <summary>
    /// 生成单个语言的翻译数组初始化代码。
    /// </summary>
    private static void GenerateTranslationArray(
        StringBuilder sb,
        ResourceClassInfo langPack,
        List<ResourceFieldInfo> baseFields,
        int langIndex,
        string langName)
    {
        sb.Append("        // ");
        sb.Append(langName);
        sb.Append(" (index ");
        sb.Append(langIndex);
        sb.AppendLine(")");
        sb.Append("        translations[");
        sb.Append(langIndex);
        sb.AppendLine("] = new string[]");
        sb.AppendLine("        {");

        // 按基准字段顺序输出翻译
        for (int i = 0; i < baseFields.Count; i++)
        {
            var baseField = baseFields[i];

            // 在当前语言包中查找同名字段
            string? translation = null;
            foreach (var field in langPack.Fields)
            {
                if (field.Name == baseField.Name)
                {
                    translation = field.Translation;
                    break;
                }
            }

            sb.Append("            ");
            if (translation != null)
            {
                sb.Append('"');
                sb.Append(EscapeCSharpString(translation));
                sb.Append('"');
            }
            else
            {
                sb.Append("null");
            }
            sb.Append(", // ");
            sb.AppendLine(baseField.Name);
        }

        sb.AppendLine("        };");
        sb.AppendLine();
    }

    /// <summary>
    /// 生成前端 locales/generated/{locale}.json 文件（增量策略）。
    /// 注意：此方法执行文件 IO，需要抑制 RS1035 分析器警告。
    /// </summary>
#pragma warning disable RS1035 // 前端 JSON 生成需要文件 IO
    private static void GenerateFrontendJson(
        SourceProductionContext context,
        ResourceClassInfo baseLang,
        List<ResourceClassInfo> otherLangs,
        List<ResourceFieldInfo> baseFields,
        AnalyzerConfigOptionsProvider optionsProvider)
    {
        // 读取前端输出路径
        string? frontendPath = null;
        if (optionsProvider.GlobalOptions.TryGetValue("build_property.I18nFrontendOutputPath", out string? path))
        {
            frontendPath = path;
        }

        if (string.IsNullOrEmpty(frontendPath))
        {
            // 未配置路径，跳过
            return;
        }

        if (!Directory.Exists(frontendPath))
        {
            // YTSI006: 目录不存在
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.FrontendPathNotFound,
                null,
                frontendPath));
            return;
        }

        // 为基准语言生成 JSON
        string baseLangLocale = LangEnumToLocale("ZhCn");
        GenerateSingleLocaleJson(frontendPath!, baseLangLocale, baseLang, baseFields);

        // 为其他语言生成 JSON
        foreach (var other in otherLangs)
        {
            string langEnumName = ClassNameToLangEnum(other.ClassName);
            string locale = LangEnumToLocale(langEnumName);
            if (locale.Length > 0)
            {
                GenerateSingleLocaleJson(frontendPath!, locale, other, baseFields);
            }
        }
    }

    /// <summary>
    /// 生成单个 locale 的 JSON 文件（增量策略：只增不覆盖、删除已废弃）。
    /// </summary>
    private static void GenerateSingleLocaleJson(
        string frontendPath,
        string locale,
        ResourceClassInfo langPack,
        List<ResourceFieldInfo> baseFields)
    {
        string filePath = Path.Combine(frontendPath, locale + ".json");

        // 读取现有 JSON 内容
        var existingEntries = new Dictionary<string, string>();
        if (File.Exists(filePath))
        {
            ReadJsonEntries(filePath, existingEntries);
        }

        // 构建最终 entries：保留已有 key 的值，新增不存在的，删除已废弃的
        var finalEntries = new SortedDictionary<int, KeyValuePair<string, string>>();
        foreach (var baseField in baseFields)
        {
            string codeStr = baseField.Value.ToString();

            // 在当前语言包中查找翻译
            string? translation = null;
            foreach (var field in langPack.Fields)
            {
                if (field.Name == baseField.Name)
                {
                    translation = field.Translation;
                    break;
                }
            }

            // 增量策略：已存在 → 不修改，不存在 → 新增
            string finalValue;
            if (existingEntries.ContainsKey(codeStr))
            {
                finalValue = existingEntries[codeStr];
            }
            else
            {
                finalValue = translation ?? "";
            }

            finalEntries[baseField.Value] = new KeyValuePair<string, string>(codeStr, finalValue);
        }

        // 写出 JSON（UTF-8 无 BOM，按数字排序）
        var sb = new StringBuilder();
        sb.AppendLine("{");
        bool first = true;
        foreach (var entry in finalEntries)
        {
            if (!first) sb.AppendLine(",");
            first = false;
            sb.Append("  \"");
            sb.Append(entry.Value.Key);
            sb.Append("\": \"");
            sb.Append(EscapeJsonString(entry.Value.Value));
            sb.Append('"');
        }
        sb.AppendLine();
        sb.AppendLine("}");

        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(false));
    }

    /// <summary>
    /// 读取 JSON 文件中的键值对（简单解析器，不依赖 System.Text.Json）。
    /// </summary>
    private static void ReadJsonEntries(string filePath, Dictionary<string, string> entries)
    {
        string content;
        try
        {
            content = File.ReadAllText(filePath, Encoding.UTF8);
        }
        catch (IOException)
        {
            // 文件无法读取时视为空文件，使用默认空 entries
            return;
        }

        // 简单 JSON 解析：只处理 { "key": "value", ... } 格式
        int pos = 0;
        int len = content.Length;

        // 跳到第一个 {
        while (pos < len && content[pos] != '{') pos++;
        if (pos >= len) return;
        pos++; // skip {

        while (pos < len)
        {
            // 跳过空白和逗号
            while (pos < len && (content[pos] == ' ' || content[pos] == '\t'
                || content[pos] == '\r' || content[pos] == '\n' || content[pos] == ','))
                pos++;

            if (pos >= len || content[pos] == '}') break;

            // 解析 key
            string? key = ReadJsonString(content, ref pos);
            if (key == null) break;

            // 跳过空白和冒号
            while (pos < len && (content[pos] == ' ' || content[pos] == '\t'
                || content[pos] == '\r' || content[pos] == '\n'))
                pos++;

            if (pos >= len || content[pos] != ':') break;
            pos++; // skip :

            // 跳过空白
            while (pos < len && (content[pos] == ' ' || content[pos] == '\t'
                || content[pos] == '\r' || content[pos] == '\n'))
                pos++;

            // 解析 value
            string? value = ReadJsonString(content, ref pos);
            if (value == null) break;

            entries[key] = value;
        }
    }

    /// <summary>
    /// 从 JSON 内容中读取一个带引号的字符串。
    /// </summary>
    private static string? ReadJsonString(string content, ref int pos)
    {
        int len = content.Length;
        if (pos >= len || content[pos] != '"') return null;
        pos++; // skip opening "

        var sb = new StringBuilder();
        while (pos < len && content[pos] != '"')
        {
            if (content[pos] == '\\' && pos + 1 < len)
            {
                pos++;
                char escaped = content[pos];
                switch (escaped)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case '/': sb.Append('/'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    default: sb.Append('\\'); sb.Append(escaped); break;
                }
            }
            else
            {
                sb.Append(content[pos]);
            }
            pos++;
        }

        if (pos < len) pos++; // skip closing "
        return sb.ToString();
    }
#pragma warning restore RS1035

    // ========== 辅助方法 ==========

    /// <summary>
    /// I18nCore.LangCount 值（与 Lang 枚举数量一致）。
    /// </summary>
    private const int I18nCoreLangCount = 5;

    /// <summary>
    /// 将类名转换为 Lang 枚举名。例如 "LangEn" → "En"。
    /// </summary>
    private static string ClassNameToLangEnum(string className)
    {
        if (className.Length > 4 && className[0] == 'L' && className[1] == 'a'
            && className[2] == 'n' && className[3] == 'g')
        {
            return className.Substring(4);
        }
        return className;
    }

    /// <summary>
    /// 将 Lang 枚举名转换为数组索引。
    /// </summary>
    private static int LangEnumToIndex(string langEnumName)
    {
        if (langEnumName == "ZhCn") return 0;
        if (langEnumName == "En") return 1;
        if (langEnumName == "Ja") return 2;
        if (langEnumName == "ZhTw") return 3;
        if (langEnumName == "MsMy") return 4;
        return -1;
    }

    /// <summary>
    /// 将 Lang 枚举名转换为前端 locale 字符串。
    /// </summary>
    private static string LangEnumToLocale(string langEnumName)
    {
        if (langEnumName == "ZhCn") return "zh-CN";
        if (langEnumName == "En") return "en-US";
        if (langEnumName == "Ja") return "ja-JP";
        if (langEnumName == "ZhTw") return "zh-TW";
        if (langEnumName == "MsMy") return "ms-MY";
        return "";
    }

    /// <summary>
    /// 转义 C# 字符串字面量中的特殊字符。
    /// </summary>
    private static string EscapeCSharpString(string s)
    {
        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 转义 JSON 字符串中的特殊字符。
    /// </summary>
    private static string EscapeJsonString(string s)
    {
        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 转义 XML 文本中的特殊字符。
    /// </summary>
    private static string EscapeXml(string s)
    {
        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            switch (c)
            {
                case '<': sb.Append("&lt;"); break;
                case '>': sb.Append("&gt;"); break;
                case '&': sb.Append("&amp;"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    // ========== 内部数据模型 ==========

    /// <summary>
    /// [I18nResource] 标注的资源类信息。
    /// </summary>
    private sealed class ResourceClassInfo
    {
        /// <summary>类名。</summary>
        public string ClassName { get; set; } = "";
        /// <summary>命名空间。</summary>
        public string? Namespace { get; set; }
        /// <summary>是否为基准语言。</summary>
        public bool IsBase { get; set; }
        /// <summary>所有 const int 字段信息。</summary>
        public List<ResourceFieldInfo> Fields { get; set; } = new List<ResourceFieldInfo>();
        /// <summary>源代码位置。</summary>
        public Location? Location { get; set; }
    }

    /// <summary>
    /// 资源类中的 const int 字段信息。
    /// </summary>
    private sealed class ResourceFieldInfo
    {
        /// <summary>字段名。</summary>
        public string Name { get; set; } = "";
        /// <summary>const int 值（错误码）。</summary>
        public int Value { get; set; }
        /// <summary>翻译文本（来自 XML 注释或 Description 属性）。</summary>
        public string? Translation { get; set; }
        /// <summary>源代码位置。</summary>
        public Location? Location { get; set; }
    }
}
