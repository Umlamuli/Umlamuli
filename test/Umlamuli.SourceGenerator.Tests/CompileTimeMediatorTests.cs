//-----------------------------------------------------------------------
// <copyright file="CompileTimeMediatorTests.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Umlamuli.SourceGenerator.Tests.Generated;
using Xunit;

namespace Umlamuli.SourceGenerator.Tests;

[Collection(SharedHandlerStateCollection.Name)]
public class CompileTimeMediatorTests
{
    [Fact]
    public void AddUmlamuliCompileTime_registers_generated_mediator()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        mediator.ShouldBeOfType<UmlamuliGeneratedMediator>();
    }

    [Fact]
    public async global::System.Threading.Tasks.Task GeneratedMediator_dispatches_request_with_response()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        var response = await mediator.Send(new Ping("compile"));
        response.Reply.ShouldBe("pong: compile");
    }

    [Fact]
    public async global::System.Threading.Tasks.Task GeneratedMediator_dispatches_void_request()
    {
        AckHandler.LastSeen = 0;

        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new Ack(99));
        AckHandler.LastSeen.ShouldBe(99);
    }

    [Fact]
    public async global::System.Threading.Tasks.Task GeneratedMediator_publishes_notification()
    {
        TickHandlerA.Sum = 0;
        TickHandlerB.Count = 0;

        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Publish(new Tick(7));

        TickHandlerA.Sum.ShouldBe(7);
        TickHandlerB.Count.ShouldBe(1);
    }

    [Fact]
    public async global::System.Threading.Tasks.Task GeneratedMediator_creates_stream()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        var items = new System.Collections.Generic.List<int>();
        await foreach (var i in mediator.CreateStream(new StreamPing(4), CancellationToken.None))
            items.Add(i);

        items.ShouldBe(new[] { 0, 1, 2, 3 });
    }

    [Fact]
    public async global::System.Threading.Tasks.Task GeneratedMediator_applies_pipeline_behaviors()
    {
        PingLoggingBehavior.Invocations = 0;

        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new Ping("behaviors"));

        PingLoggingBehavior.Invocations.ShouldBe(1);
    }

    [Fact]
    public async global::System.Threading.Tasks.Task GeneratedMediator_dispatches_via_object_send()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliCompileTime();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        var response = await mediator.Send((object)new Ping("obj"));
        response.ShouldBeOfType<Pong>().Reply.ShouldBe("pong: obj");
    }
}
