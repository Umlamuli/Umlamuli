//-----------------------------------------------------------------------
// <copyright file="GenericHandler.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Umlamuli.Examples;

public class GenericHandler : INotificationHandler<INotification>
{
    private readonly TextWriter _writer;

    public GenericHandler(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Handle(INotification notification, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("Got notified.");
    }
}