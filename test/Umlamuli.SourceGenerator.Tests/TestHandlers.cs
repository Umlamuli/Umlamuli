//-----------------------------------------------------------------------
// <copyright file="TestHandlers.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Umlamuli;

namespace Umlamuli.SourceGenerator.Tests;

public sealed record Ping(string Message) : IRequest<Pong>;

public sealed record Pong(string Reply);

public sealed class PingHandler : IRequestHandler<Ping, Pong>
{
    public Task<Pong> Handle(Ping request, CancellationToken cancellationToken)
        => Task.FromResult(new Pong($"pong: {request.Message}"));
}

public sealed record Ack(int Id) : IRequest;

public sealed class AckHandler : IRequestHandler<Ack>
{
    public static int LastSeen;

    public Task Handle(Ack request, CancellationToken cancellationToken)
    {
        LastSeen = request.Id;
        return Task.CompletedTask;
    }
}

public sealed record Tick(int Count) : INotification;

public sealed class TickHandlerA : INotificationHandler<Tick>
{
    public static int Sum;

    public Task Handle(Tick notification, CancellationToken cancellationToken)
    {
        Sum += notification.Count;
        return Task.CompletedTask;
    }
}

public sealed class TickHandlerB : INotificationHandler<Tick>
{
    public static int Count;

    public Task Handle(Tick notification, CancellationToken cancellationToken)
    {
        Count++;
        return Task.CompletedTask;
    }
}

public sealed record StreamPing(int Take) : IStreamRequest<int>;

public sealed class StreamPingHandler : IStreamRequestHandler<StreamPing, int>
{
    public async IAsyncEnumerable<int> Handle(StreamPing request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 0; i < request.Take; i++)
        {
            yield return i;
            await Task.Yield();
        }
    }
}

public sealed class PingLoggingBehavior : IPipelineBehavior<Ping, Pong>
{
    public static int Invocations;

    public Task<Pong> Handle(Ping request, RequestHandlerDelegate<Pong> next,
        CancellationToken cancellationToken)
    {
        Invocations++;
        return next(cancellationToken);
    }
}
