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
using Umlamuli.Benchmarks.Generated;

namespace Umlamuli.Benchmarks;

//[DotTraceDiagnoser]
public class Benchmarks
{
    private IMediator _runtime;
    private IMediator _generatedDi;
    private IMediator _compileTime;
    private readonly Ping _request = new Ping {Message = "Hello World"};
    private readonly Pinged _notification = new Pinged();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _runtime = BuildRuntime();
        _generatedDi = BuildGeneratedDi();
        _compileTime = BuildCompileTime();
    }

    [Benchmark(Baseline = true)]
    public Task Send_Runtime() => _runtime.Send(_request);

    [Benchmark]
    public Task Send_GeneratedDi() => _generatedDi.Send(_request);

    [Benchmark]
    public Task Send_CompileTime() => _compileTime.Send(_request);

    [Benchmark]
    public Task Publish_Runtime() => _runtime.Publish(_notification);

    [Benchmark]
    public Task Publish_GeneratedDi() => _generatedDi.Publish(_notification);

    [Benchmark]
    public Task Publish_CompileTime() => _compileTime.Publish(_notification);

    private static IMediator BuildRuntime()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TextWriter.Null);
        services.AddUmlamuli(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Ping));
            cfg.AddOpenBehavior(typeof(GenericPipelineBehavior<,>));
        });
        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    private static IMediator BuildGeneratedDi()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TextWriter.Null);
        services.AddUmlamuliGenerated();
        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    private static IMediator BuildCompileTime()
    {
        var services = new ServiceCollection();
        services.AddSingleton(TextWriter.Null);
        services.AddUmlamuliCompileTime();
        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
}
