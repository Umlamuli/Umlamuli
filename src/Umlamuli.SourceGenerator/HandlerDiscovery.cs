//-----------------------------------------------------------------------
// <copyright file="HandlerDiscovery.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Umlamuli.SourceGenerator;

internal static class HandlerDiscovery
{
    private static readonly SymbolDisplayFormat s_fullyQualifiedFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    public static IEnumerable<HandlerRegistration> Inspect(INamedTypeSymbol type, KnownTypes contracts)
    {
        if (type.IsAbstract || type.IsStatic || type.TypeKind != TypeKind.Class)
            yield break;

        if (type.IsUnboundGenericType || type.IsGenericType)
            yield break;

        if (type.DeclaredAccessibility is Accessibility.Private or Accessibility.Protected
            or Accessibility.ProtectedAndInternal)
            yield break;

        var impl = type.ToDisplayString(s_fullyQualifiedFormat);

        foreach (var iface in type.AllInterfaces)
        {
            if (!iface.IsGenericType) continue;

            var def = iface.OriginalDefinition;
            var kind = ClassifyInterface(def, contracts);
            if (kind is null) continue;

            var serviceType = iface.ToDisplayString(s_fullyQualifiedFormat);
            var args = new string?[iface.TypeArguments.Length];
            for (int i = 0; i < iface.TypeArguments.Length; i++)
            {
                var ta = iface.TypeArguments[i];
                args[i] = ta.Kind == SymbolKind.ErrorType
                    ? null
                    : ta.ToDisplayString(s_fullyQualifiedFormat);
            }

            yield return new HandlerRegistration(kind.Value, serviceType, impl, args);
        }
    }

    private static HandlerKind? ClassifyInterface(INamedTypeSymbol def, KnownTypes contracts)
    {
        if (SymbolEqualityComparer.Default.Equals(def, contracts.RequestHandler2))
            return HandlerKind.RequestHandler;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.RequestHandler1))
            return HandlerKind.VoidRequestHandler;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.NotificationHandler))
            return HandlerKind.NotificationHandler;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.StreamRequestHandler))
            return HandlerKind.StreamRequestHandler;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.PipelineBehavior))
            return HandlerKind.PipelineBehavior;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.StreamPipelineBehavior))
            return HandlerKind.StreamPipelineBehavior;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.RequestPreProcessor))
            return HandlerKind.RequestPreProcessor;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.RequestPostProcessor))
            return HandlerKind.RequestPostProcessor;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.RequestExceptionHandler))
            return HandlerKind.RequestExceptionHandler;
        if (SymbolEqualityComparer.Default.Equals(def, contracts.RequestExceptionAction))
            return HandlerKind.RequestExceptionAction;
        return null;
    }
}
