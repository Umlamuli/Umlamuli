//-----------------------------------------------------------------------
// <copyright file="Pinged.cs" company="Umlamuli">
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

public class Pinged : INotification;

[UsedImplicitly]
public class PingedHandler : INotificationHandler<Pinged>
{
    public Task Handle(Pinged notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}