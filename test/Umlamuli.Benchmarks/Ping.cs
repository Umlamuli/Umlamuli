//-----------------------------------------------------------------------
// <copyright file="Ping.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Umlamuli.Benchmarks;

public class Ping : IRequest
{
    public string Message { get; set; }
}

[UsedImplicitly]
public class PingHandler : IRequestHandler<Ping>
{
    public Task Handle(Ping request, CancellationToken cancellationToken) => Task.CompletedTask;
}