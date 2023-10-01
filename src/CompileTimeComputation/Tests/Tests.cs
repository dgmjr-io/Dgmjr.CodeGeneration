namespace Dgmjr.CodeGeneration.CompileTimeComputation.Tests;

using Dgmjr.CodeGeneration.CompileTimeComputation.Tests;
using Dgmjr.CodeGeneration.Testing;

public class Tests(ITestOutputHelper output) : BaseTest<CompileTimeComputationGenerator>(output)
{
    public override CompileTimeComputationGenerator UnitUnderTest { get; } =
        new CompileTimeComputationGenerator();

    [Fact]
    public void Can_Produce_Compile_Time_Computation()
    {
        Logger.LogSourceToBeCompiled(Constants.Source);
        var (inputCompilation, outputCompilation, _) = UnitUnderTest.RunGenerators(
            Constants.Source
        );
        var inputErrors = inputCompilation.GetDiagnostics().Errors();
        var outputErrors = outputCompilation.GetDiagnostics().Errors();
        // errors.Any().Should().BeFalse("there were errors during compilation");
        Logger.LogInformation("Input Errors:");
        if (inputErrors.Any())
        {
            foreach (var error in inputErrors)
            {
                Logger.LogDiagnosticError(error.Location, error.Id, error.Descriptor);
            }
        }
        Logger.LogInformation("Output Errors:");
        if (outputErrors.Any())
        {
            foreach (var error in outputErrors)
            {
                Logger.LogDiagnosticError(error);
            }
        }
        var compiledType = outputCompilation.GetTypeByMetadataName(
            $"{Constants.Foo}.{Constants.Bar}"
        );
        compiledType.Should().NotBeNull();
        var guidStringField =
            compiledType.GetMembers(Constants.GuidString).FirstOrDefault() as IFieldSymbol;
        guidStringField.Should().NotBeNull();
        Logger.LogInformation($"guidStringField.ConstantValue: {guidStringField.ConstantValue}");
    }
}
