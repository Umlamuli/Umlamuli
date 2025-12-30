//-----------------------------------------------------------------------
// <copyright file="Benchmarks.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Umlamuli.Benchmarks;

//[DotTraceDiagnoser]
public class Benchmarks
{
    private IMediator _mediator;
    private readonly Ping _request = new Ping {Message = "Hello World"};
    private readonly Pinged _notification = new Pinged();

    [GlobalSetup]
    public void GlobalSetup()
    {
        var services = new ServiceCollection();

        services.AddSingleton(TextWriter.Null);

        services.AddUmlamuli(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Ping));
            cfg.AddOpenBehavior(typeof(GenericPipelineBehavior<,>));
        });

        var provider = services.BuildServiceProvider();

        _mediator = provider.GetRequiredService<IMediator>();
    }

    [Benchmark]
    public Task SendingRequests()
    {
        return _mediator.Send(_request);
    }

    [Benchmark]
    public Task PublishingNotifications()
    {
        return _mediator.Publish(_notification);
    }
}