//-----------------------------------------------------------------------
// <copyright file="ServiceFactoryTests.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Umlamuli.Tests;

public class ServiceFactoryTests
{
    public class Ping : IRequest<Pong>;

    public class Pong
    {
        public string? Message { get; set; }
    }

    [Fact]
    public async Task Should_throw_given_no_handler()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var mediator = new Mediator(serviceProvider);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => mediator.Send(new Ping(), TestContext.Current.CancellationToken)
        );
    }
}