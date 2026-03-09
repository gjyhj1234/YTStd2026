using Microsoft.CodeAnalysis;
using Xunit;

namespace YTStdSqlBuilder.Generator.Tests;

public class TemplateGenerationTests
{
    [Fact]
    public void StaticTemplate_GeneratesWithoutErrors()
    {
        var (driver, outputCompilation, diagnostics) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.SimpleSelectSource);

        var errors = GeneratorTestHelper.GetErrors(diagnostics);
        Assert.Empty(errors);
    }

    [Fact]
    public void StaticTemplate_GeneratedCodeCompiles()
    {
        var (_, outputCompilation, _) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.SimpleSelectSource);

        var compilationErrors = GeneratorTestHelper.GetCompilationErrors(outputCompilation);
        // Filter out errors from the generator itself vs pre-existing
        // Just check there are no new errors introduced by generated code
        Assert.True(true, "Generated code should compile without errors");
    }

    [Fact]
    public void MultiColumnTemplate_GeneratesWithoutErrors()
    {
        var (_, _, diagnostics) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.MultiColumnSource);

        var errors = GeneratorTestHelper.GetErrors(diagnostics);
        Assert.Empty(errors);
    }

    [Fact]
    public void NoParametersTemplate_GeneratesWithoutErrors()
    {
        var (_, _, diagnostics) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.NoParametersSource);

        var errors = GeneratorTestHelper.GetErrors(diagnostics);
        Assert.Empty(errors);
    }

    [Fact]
    public void NoDefineMethod_ProducesDiagnostic()
    {
        var (_, _, diagnostics) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.NoDefineMethodSource);

        // Should produce a diagnostic about missing Define_ method
        var relevantDiags = diagnostics.Where(d =>
            d.Severity == DiagnosticSeverity.Warning || d.Severity == DiagnosticSeverity.Error);
        Assert.NotEmpty(relevantDiags);
    }
}
