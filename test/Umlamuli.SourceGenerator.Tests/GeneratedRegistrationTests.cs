//-----------------------------------------------------------------------
// <copyright file="GeneratedRegistrationTests.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Umlamuli.SourceGenerator.Tests.Generated;
using Xunit;

namespace Umlamuli.SourceGenerator.Tests;

[Collection(SharedHandlerStateCollection.Name)]
public class GeneratedRegistrationTests
{
    [Fact]
    public void AddUmlamuliGenerated_registers_mediator_and_handlers()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliGenerated();

        var provider = services.BuildServiceProvider();
        provider.GetService<IMediator>().ShouldNotBeNull();
        provider.GetService<ISender>().ShouldNotBeNull();
        provider.GetService<IPublisher>().ShouldNotBeNull();

        provider.GetService<IRequestHandler<Ping, Pong>>().ShouldBeOfType<PingHandler>();
        provider.GetService<IRequestHandler<Ack>>().ShouldBeOfType<AckHandler>();
        provider.GetService<IStreamRequestHandler<StreamPing, int>>().ShouldBeOfType<StreamPingHandler>();
    }

    [Fact]
    public async global::System.Threading.Tasks.Task AddUmlamuliGenerated_dispatches_request_through_existing_mediator()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliGenerated();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        var response = await mediator.Send(new Ping("hi"));
        response.Reply.ShouldBe("pong: hi");
    }

    [Fact]
    public async global::System.Threading.Tasks.Task AddUmlamuliGenerated_pipeline_behaviors_are_registered()
    {
        PingLoggingBehavior.Invocations = 0;

        var services = new ServiceCollection();
        services.AddUmlamuliGenerated();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new Ping("hello"));

        PingLoggingBehavior.Invocations.ShouldBe(1);
    }

    [Fact]
    public async global::System.Threading.Tasks.Task AddUmlamuliGenerated_publishes_notification_to_all_handlers()
    {
        TickHandlerA.Sum = 0;
        TickHandlerB.Count = 0;

        var services = new ServiceCollection();
        services.AddUmlamuliGenerated();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Publish(new Tick(5));

        TickHandlerA.Sum.ShouldBe(5);
        TickHandlerB.Count.ShouldBe(1);
    }

    [Fact]
    public async global::System.Threading.Tasks.Task AddUmlamuliGenerated_void_request_handler_runs()
    {
        AckHandler.LastSeen = 0;

        var services = new ServiceCollection();
        services.AddUmlamuliGenerated();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        await mediator.Send(new Ack(42));

        AckHandler.LastSeen.ShouldBe(42);
    }

    [Fact]
    public async global::System.Threading.Tasks.Task AddUmlamuliGenerated_stream_request_works()
    {
        var services = new ServiceCollection();
        services.AddUmlamuliGenerated();
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        var items = new System.Collections.Generic.List<int>();
        await foreach (var i in mediator.CreateStream(new StreamPing(3), CancellationToken.None))
            items.Add(i);

        items.ShouldBe(new[] { 0, 1, 2 });
    }

    [Fact]
    public void AddUmlamuli_runtime_registrar_still_works_side_by_side()
    {
        var services = new ServiceCollection();
        services.AddUmlamuli(cfg => cfg.RegisterServicesFromAssembly(typeof(Ping).Assembly));
        var provider = services.BuildServiceProvider();
        provider.GetService<IMediator>().ShouldNotBeNull();
        provider.GetService<IRequestHandler<Ping, Pong>>().ShouldBeOfType<PingHandler>();
    }
}
