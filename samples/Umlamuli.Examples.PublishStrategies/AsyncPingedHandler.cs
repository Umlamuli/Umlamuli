//-----------------------------------------------------------------------
// <copyright file="AsyncPingedHandler.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Umlamuli.Examples.PublishStrategies;

public class AsyncPingedHandler : INotificationHandler<Pinged>
{
    public AsyncPingedHandler(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public async Task Handle(Pinged notification, CancellationToken cancellationToken)
    {
        if (Name == "2")
        {
            throw new ArgumentException("Name cannot be '2'");
        }

        Console.WriteLine($"[AsyncPingedHandler {Name}] {DateTime.Now:HH:mm:ss.fff} : Pinged");
        await Task.Delay(100).ConfigureAwait(false);
        Console.WriteLine($"[AsyncPingedHandler {Name}] {DateTime.Now:HH:mm:ss.fff} : After pinged");
    }
}