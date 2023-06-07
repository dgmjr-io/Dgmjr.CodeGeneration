namespace Dgmjr.CodeGeneration.Extensibility;

public class InterfaceImplementationCodeGenerator : CodeGenerator
{
    public override string Name => "InterfaceImplementationCodeGenerator";

    public override Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
    {
        var interfaceSymbol = context.ProcessingNode.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().First().GetDeclaredSymbol(context.SemanticModel);

        return Task.FromResult(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                interfaceSymbol.ToClassDeclarationSyntax()));
    }
}

[ExportCodeGeneratorFactory(Name, LanguageNames.CSharp)]
[Shared]
public class InterfaceImplementationCodeGeneratorFactory : CodeGeneratorFactory
{
    public const string Name = "InterfaceImplementationCodeGenerator";

    public override string Name => Name;

    public override IEnumerable<string> FileExtensions => new[] { ".cs" };

    public override Task<IEnumerable<CodeGenerator>> CreateGeneratorsAsync(Project project, CodeGeneratorRegistrationContext context)
    {
        if (project.Language != LanguageNames.CSharp)
        {
            return Task.FromResult(Enumerable.Empty<CodeGenerator>());
        }

        return Task.FromResult(
            context.CodeGeneratorDescriptors.Where(
                codeGeneratorDescriptor => codeGeneratorDescriptor.Name == Name).Select(
                    codeGeneratorDescriptor => new InterfaceImplementationCodeGenerator()));
    }
}

[Export(typeof(CodeGeneratorRegistrationService))]
[Shared]
public class InterfaceImplementationCodeGeneratorRegistrationService : CodeGeneratorRegistrationService
{
    public override Task RegisterCodeGeneratorsAsync(CodeGeneratorRegistrationContext context)
    {
        context.RegisterCodeGenerator(
            InterfaceImplementationCodeGeneratorFactory.Name,
            (provider, project, language) => new InterfaceImplementationCodeGenerator(),
            (provider, project, language) => new[] { new CommandId(CommandSet, CommandId) });

        return Task.CompletedTask;
    }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.SubjectBuffer.CurrentSnapshot.TextBuffer.GetWorkspace();

        var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Single();

        var solution = workspace.CurrentSolution.WithDocumentSyntaxRoot(documentId, codeGeneratorResult.GetRoot(cancellationToken));

        workspace.TryApplyChanges(solution);
    }
}

[ExportCommandArgs(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandArgs : ICommandArgs
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandArgs(ITextBuffer subjectBuffer)
    {
        SubjectBuffer = subjectBuffer;
    }

    public ITextBuffer SubjectBuffer { get; }
}

[ExportCommandHandler(CommandSet, CommandId)]
[Shared]
public class InterfaceImplementationCommandHandler : ICommandHandler<InterfaceImplementationCommandArgs>
{
    public const string CommandSet = "InterfaceImplementationCommandSet";
    public const int CommandId = 0x0100;

    [ImportingConstructor]
    public InterfaceImplementationCommandHandler(
        IAsyncServiceProvider serviceProvider,
        [ImportMany] IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> codeGenerators)
    {
        ServiceProvider = serviceProvider;
        CodeGenerators = codeGenerators;
    }

    private IAsyncServiceProvider ServiceProvider { get; }
    private IEnumerable<Lazy<CodeGenerator, CodeGeneratorMetadata>> CodeGenerators { get; }

    public async Task ExecuteCommandAsync(InterfaceImplementationCommandArgs args, CancellationToken cancellationToken)
    {
        var document = args.SubjectBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
        if (document == null)
        {
            return;
        }

        var codeGenerator = CodeGenerators.Single(codeGenerator => codeGenerator.Metadata.Name == InterfaceImplementationCodeGeneratorFactory.Name);

        var codeGeneratorDriver = new CodeGeneratorDriver(
            ServiceProvider,
            document.Project,
            ImmutableArray.Create(codeGenerator.Value),
            cancellationToken);

        var codeGeneratorResults = await codeGeneratorDriver.AddFileAsync(
            document.Project.Solution.GetProject(document.Project.Id),
            document.FilePath,
            document.GetSyntaxRootAsync().WaitAndGetResult(cancellationToken),
            ImmutableArray<TextSpan>.Empty,
            cancellationToken);

        var codeGeneratorResult = codeGeneratorResults.Single();

        var workspace = args.
