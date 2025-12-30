//-----------------------------------------------------------------------
// <copyright file="ExceptionsHandlers.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using Umlamuli.Pipeline;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Umlamuli.Examples.ExceptionHandler;

public class CommonExceptionHandler : IRequestExceptionHandler<PingResource, Pong, Exception>
{
    private readonly TextWriter _writer;

    public CommonExceptionHandler(TextWriter writer) => _writer = writer;

    public async Task Handle(PingResource request,
        Exception exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name must be written in messages by LogExceptionAction before
        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(CommonExceptionHandler).FullName}'").ConfigureAwait(false);
        
        state.SetHandled(new Pong());
    }
}

public class ConnectionExceptionHandler : IRequestExceptionHandler<PingResource, Pong, ConnectionException>
{
    private readonly TextWriter _writer;

    public ConnectionExceptionHandler(TextWriter writer) => _writer = writer;

    public async Task Handle(PingResource request,
        ConnectionException exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name must be written in messages by LogExceptionAction before
        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(ConnectionExceptionHandler).FullName}'").ConfigureAwait(false);
        
        state.SetHandled(new Pong());
    }
}

public class AccessDeniedExceptionHandler : IRequestExceptionHandler<PingResource, Pong, ForbiddenException>
{
    private readonly TextWriter _writer;

    public AccessDeniedExceptionHandler(TextWriter writer) => _writer = writer;

    public async Task Handle(PingResource request,
        ForbiddenException exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name must be written in messages by LogExceptionAction before
        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(AccessDeniedExceptionHandler).FullName}'").ConfigureAwait(false);
        
        state.SetHandled(new Pong());
    }
}

public class ServerExceptionHandler : IRequestExceptionHandler<PingNewResource, Pong, ServerException>
{
    private readonly TextWriter _writer;

    public ServerExceptionHandler(TextWriter writer) => _writer = writer;

    public virtual async Task Handle(PingNewResource request,
        ServerException exception,
        RequestExceptionHandlerState<Pong> state,
        CancellationToken cancellationToken)
    {
        // Exception type name must be written in messages by LogExceptionAction before
        // Exception handler type name required because it is checked later in messages
        await _writer.WriteLineAsync($"---- Exception Handler: '{typeof(ServerExceptionHandler).FullName}'").ConfigureAwait(false);
        
        state.SetHandled(new Pong());
    }
}
