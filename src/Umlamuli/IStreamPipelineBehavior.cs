//-----------------------------------------------------------------------
// <copyright file="IStreamPipelineBehavior.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
namespace Umlamuli;

/// <summary>
///     Represents an async enumerable continuation for the next task to execute in the pipeline
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
/// <returns>Async Enumerable returning a <typeparamref name="TResponse" /></returns>
public delegate IAsyncEnumerable<TResponse> StreamHandlerDelegate<out TResponse>();

/// <summary>
///     Stream Pipeline behavior to surround the inner handler.
///     Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IStreamPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    ///     Stream Pipeline handler. Perform any additional behavior and iterate the <paramref name="next" /> delegate as
    ///     necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">
    ///     Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the
    ///     handler.
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse" /></returns>
    IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}