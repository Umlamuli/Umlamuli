//-----------------------------------------------------------------------
// <copyright file="RequestExceptionActionProcessorStrategy.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Specifies how request exception actions should be processed within the pipeline.
/// </summary>
/// <remarks>
///     Use this strategy to control whether exception actions execute only for exceptions
///     that are not handled by other behaviors/handlers, or for all exceptions as they occur.
/// </remarks>
public enum RequestExceptionActionProcessorStrategy
{
    /// <summary>
    ///     Executes exception actions only for exceptions that bubble up unhandled by
    ///     other pipeline behaviors or handlers.
    /// </summary>
    ApplyForUnhandledExceptions,

    /// <summary>
    ///     Executes exception actions for all exceptions, regardless of whether they are
    ///     subsequently handled by other behaviors or handlers.
    /// </summary>
    ApplyForAllExceptions
}