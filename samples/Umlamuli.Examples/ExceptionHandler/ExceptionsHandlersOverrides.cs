//-----------------------------------------------------------------------
// <copyright file="ExceptionsHandlersOverrides.cs" company="Umlamuli">
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

namespace Umlamuli.Examples.ExceptionHandler.Overrides;

public class CommonExceptionHandler : IRequestExceptionHandler<PingResourceTimeout, Pong, Exception>
{
    private readonly TextWriter _writer;

    public CommonExceptionHandler(TextWriter writer) => _writer = writer;

    public async Task Handle(PingResourceTimeout request,
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

public class ServerExceptionHandler : ExceptionHandler.ServerExceptionHandler
{
    private readonly TextWriter _writer;

    public ServerExceptionHandler(TextWriter writer) : base(writer) => _writer = writer;

    public override async Task Handle(PingNewResource request,
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
