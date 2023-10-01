using System;
using System.IO;

namespace Dgmjr.CodeGeneration;

public record struct CodeTemplate
{
    public string StringTemplate { get; set; }
    public string? ManifestResourceName { get; set; }
    public Scriban.Template Template { get; set; }

    public CodeTemplate(string codeTemplate)
    {
        this.StringTemplate = codeTemplate;
        this.ManifestResourceName = null;
        this.Template = Scriban.Template.Parse(StringTemplate);
    }

    public static CodeTemplate FromResource(string manifestResourceName)
    {
        try
        {
            var codeTemplate = new CodeTemplate(
                new StreamReader(
                    typeof(CodeTemplate).Assembly.GetManifestResourceStream(manifestResourceName)
                ).ReadToEnd()
            );
            return codeTemplate;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Could not load resource {manifestResourceName}", e);
        }
    }
}

public record struct CodeTemplate<T>
{
    public string StringTemplate { get; set; }
    public string ManifestResourceName { get; set; }
    public Scriban.Template Template { get; set; }

    public static CodeTemplate FromResource(string manifestResourceName)
    {
        var codeTemplate = new CodeTemplate(
            new StreamReader(
                typeof(T).Assembly.GetManifestResourceStream(manifestResourceName)
            ).ReadToEnd()
        );
        return codeTemplate;
    }
}
