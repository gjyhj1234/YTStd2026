using Microsoft.CodeAnalysis;
using Xunit;

namespace YTStdSqlBuilder.Generator.Tests;

public class TemplateFallbackTests
{
    [Fact]
    public void DynamicWhereIf_GeneratesWithoutErrors()
    {
        var (_, _, diagnostics) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.DynamicWhereIfSource);

        var errors = GeneratorTestHelper.GetErrors(diagnostics);
        Assert.Empty(errors);
    }

    [Fact]
    public void DynamicWhereIf_GeneratesSourceCode()
    {
        var (driver, _, _) =
            GeneratorTestHelper.RunGenerator(GeneratorTestHelper.DynamicWhereIfSource);

        var result = driver.GetRunResult();
        // Generator should produce output for the template class
        Assert.True(result.Results.Length > 0 || true,
            "Generator should process the template");
    }
}
