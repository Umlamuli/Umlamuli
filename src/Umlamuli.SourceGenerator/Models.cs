//-----------------------------------------------------------------------
// <copyright file="Models.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Umlamuli.SourceGenerator;

internal enum HandlerKind
{
    RequestHandler,
    VoidRequestHandler,
    NotificationHandler,
    StreamRequestHandler,
    PipelineBehavior,
    StreamPipelineBehavior,
    RequestPreProcessor,
    RequestPostProcessor,
    RequestExceptionHandler,
    RequestExceptionAction,
}

internal sealed class HandlerRegistration : IEquatable<HandlerRegistration>
{
    public HandlerRegistration(HandlerKind kind, string serviceType, string implementationType, string?[] genericArguments)
    {
        Kind = kind;
        ServiceType = serviceType;
        ImplementationType = implementationType;
        GenericArguments = genericArguments;
    }

    public HandlerKind Kind { get; }
    public string ServiceType { get; }
    public string ImplementationType { get; }
    public string?[] GenericArguments { get; }

    public bool Equals(HandlerRegistration? other)
        => other is not null
           && Kind == other.Kind
           && ServiceType == other.ServiceType
           && ImplementationType == other.ImplementationType;

    public override bool Equals(object? obj) => obj is HandlerRegistration r && Equals(r);

    public override int GetHashCode()
    {
        unchecked
        {
            int h = (int)Kind;
            h = (h * 397) ^ ServiceType.GetHashCode();
            h = (h * 397) ^ ImplementationType.GetHashCode();
            return h;
        }
    }
}

internal sealed class HandlerCollection : IEquatable<HandlerCollection>
{
    public HandlerCollection(string assemblyName, string generatedNamespace, IReadOnlyList<HandlerRegistration> registrations)
    {
        AssemblyName = assemblyName;
        GeneratedNamespace = generatedNamespace;
        Registrations = registrations;
    }

    public string AssemblyName { get; }
    public string GeneratedNamespace { get; }
    public IReadOnlyList<HandlerRegistration> Registrations { get; }

    public bool Equals(HandlerCollection? other)
        => other is not null
           && AssemblyName == other.AssemblyName
           && GeneratedNamespace == other.GeneratedNamespace
           && Registrations.SequenceEqual(other.Registrations);

    public override bool Equals(object? obj) => obj is HandlerCollection c && Equals(c);

    public override int GetHashCode()
    {
        unchecked
        {
            int h = AssemblyName.GetHashCode();
            h = (h * 397) ^ GeneratedNamespace.GetHashCode();
            foreach (var r in Registrations)
                h = (h * 397) ^ r.GetHashCode();
            return h;
        }
    }
}
