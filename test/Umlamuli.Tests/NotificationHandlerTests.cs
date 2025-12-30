//-----------------------------------------------------------------------
// <copyright file="NotificationHandlerTests.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Umlamuli.Tests;

public class NotificationHandlerTests
{
    public class Ping : INotification
    {
        public string? Message { get; set; }
    }

    public class PongChildHandler : NotificationHandler<Ping>
    {
        private readonly TextWriter _writer;

        public PongChildHandler(TextWriter writer)
        {
            _writer = writer;
        }

        protected override void Handle(Ping notification)
        {
            _writer.WriteLine(notification.Message + " Pong");
        }
    }

    [Fact]
    public async Task Should_call_abstract_handle_method()
    {
        var builder = new StringBuilder();
        var writer = new StringWriter(builder);

        INotificationHandler<Ping> handler = new PongChildHandler(writer);

        await handler.Handle(
            new Ping() { Message = "Ping" }, 
            TestContext.Current.CancellationToken
        );

        var result = builder.ToString();
        result.ShouldContain("Ping Pong");
    }
}