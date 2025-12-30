//-----------------------------------------------------------------------
// <copyright file="ContravariantFilter.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
namespace Umlamuli.Examples.Windsor;

using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;

public class ContravariantFilter : IHandlersFilter
{
    public bool HasOpinionAbout(Type service)
    {
        if (!service.IsGenericType)
            return false;

        var genericType = service.GetGenericTypeDefinition();
        var genericArguments = genericType.GetGenericArguments();
        return genericArguments.Count() == 1
               && genericArguments.Single().GenericParameterAttributes.HasFlag(GenericParameterAttributes.Contravariant);
    }

    public IHandler[] SelectHandlers(Type service, IHandler[] handlers)
    {
        return handlers;
    }
}