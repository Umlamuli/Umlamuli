//-----------------------------------------------------------------------
// <copyright file="UmlamuliIncrementalGenerator.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Umlamuli.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class UmlamuliIncrementalGenerator : IIncrementalGenerator
{
    private const string DefaultGeneratedNamespace = "Umlamuli.Generated";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax c && c.BaseList is not null,
                static (ctx, _) =>
                {
                    var decl = (ClassDeclarationSyntax)ctx.Node;
                    return ctx.SemanticModel.GetDeclaredSymbol(decl) as INamedTypeSymbol;
                })
            .Where(static s => s is not null)
            .Collect();

        var compilationAndClasses = context.CompilationProvider.Combine(classSymbols);

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) =>
        {
            var (compilation, classes) = source;
            var contracts = KnownTypes.Resolve(compilation);
            if (contracts is null)
                return;

            var generatedNamespace = ResolveGeneratedNamespace(compilation);

            var registrations = new List<HandlerRegistration>();
            var seen = new HashSet<HandlerRegistration>();
            foreach (var symbol in classes)
            {
                if (symbol is null) continue;
                if (!SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, compilation.Assembly))
                    continue;

                foreach (var reg in HandlerDiscovery.Inspect(symbol, contracts))
                {
                    if (seen.Add(reg))
                        registrations.Add(reg);
                }
            }

            registrations.Sort(static (a, b) =>
            {
                int byKind = ((int)a.Kind).CompareTo((int)b.Kind);
                if (byKind != 0) return byKind;
                int byService = string.CompareOrdinal(a.ServiceType, b.ServiceType);
                if (byService != 0) return byService;
                return string.CompareOrdinal(a.ImplementationType, b.ImplementationType);
            });

            var collection = new HandlerCollection(
                compilation.AssemblyName ?? "Generated",
                generatedNamespace,
                registrations);

            spc.AddSource(
                $"{Emitter.GeneratedClassName}.g.cs",
                SourceText.From(Emitter.EmitRegistrations(collection), Encoding.UTF8));
            spc.AddSource(
                $"{Emitter.GeneratedMediatorTypeName}.g.cs",
                SourceText.From(Emitter.EmitMediator(collection), Encoding.UTF8));
        });
    }

    private static string ResolveGeneratedNamespace(Compilation compilation)
    {
        var rootNamespace = compilation.AssemblyName;
        if (string.IsNullOrWhiteSpace(rootNamespace))
            return DefaultGeneratedNamespace;
        return $"{rootNamespace}.Generated";
    }
}
