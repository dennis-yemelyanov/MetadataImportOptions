using System.Linq;
using Microsoft.CodeAnalysis;

namespace MyGenerator
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var memberNamesProvider = context.CompilationProvider
                .Select(static (c, _) =>
                {
                    return c
                        .WithOptions(c.Options.WithMetadataImportOptions(MetadataImportOptions.All))
                        .GetTypeByMetadataName("MyLibrary.MyLibraryClass")!.GetMembers().Select(x => x.Name);
                });

            context.RegisterSourceOutput(memberNamesProvider, (productionContext, memberNames) =>
            {
                var text = 
$@"public class GeneratedResult
{{
    public static void Print()
    {{
        System.Console.WriteLine(""{string.Join(" ", memberNames)}"");
    }}
}}";

                productionContext.AddSource("GeneratedResult.g.cs", text);
            });
        }
    }
}
