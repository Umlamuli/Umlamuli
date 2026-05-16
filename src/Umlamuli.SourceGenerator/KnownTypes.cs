//-----------------------------------------------------------------------
// <copyright file="KnownTypes.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;

namespace Umlamuli.SourceGenerator;

internal sealed class KnownTypes
{
    private KnownTypes(
        INamedTypeSymbol requestHandler2,
        INamedTypeSymbol requestHandler1,
        INamedTypeSymbol notificationHandler,
        INamedTypeSymbol streamRequestHandler,
        INamedTypeSymbol pipelineBehavior,
        INamedTypeSymbol streamPipelineBehavior,
        INamedTypeSymbol requestPreProcessor,
        INamedTypeSymbol requestPostProcessor,
        INamedTypeSymbol requestExceptionHandler,
        INamedTypeSymbol requestExceptionAction,
        INamedTypeSymbol requestMarker,
        INamedTypeSymbol requestOfTMarker,
        INamedTypeSymbol notificationMarker,
        INamedTypeSymbol streamRequestMarker)
    {
        RequestHandler2 = requestHandler2;
        RequestHandler1 = requestHandler1;
        NotificationHandler = notificationHandler;
        StreamRequestHandler = streamRequestHandler;
        PipelineBehavior = pipelineBehavior;
        StreamPipelineBehavior = streamPipelineBehavior;
        RequestPreProcessor = requestPreProcessor;
        RequestPostProcessor = requestPostProcessor;
        RequestExceptionHandler = requestExceptionHandler;
        RequestExceptionAction = requestExceptionAction;
        RequestMarker = requestMarker;
        RequestOfTMarker = requestOfTMarker;
        NotificationMarker = notificationMarker;
        StreamRequestMarker = streamRequestMarker;
    }

    public INamedTypeSymbol RequestHandler2 { get; }
    public INamedTypeSymbol RequestHandler1 { get; }
    public INamedTypeSymbol NotificationHandler { get; }
    public INamedTypeSymbol StreamRequestHandler { get; }
    public INamedTypeSymbol PipelineBehavior { get; }
    public INamedTypeSymbol StreamPipelineBehavior { get; }
    public INamedTypeSymbol RequestPreProcessor { get; }
    public INamedTypeSymbol RequestPostProcessor { get; }
    public INamedTypeSymbol RequestExceptionHandler { get; }
    public INamedTypeSymbol RequestExceptionAction { get; }
    public INamedTypeSymbol RequestMarker { get; }
    public INamedTypeSymbol RequestOfTMarker { get; }
    public INamedTypeSymbol NotificationMarker { get; }
    public INamedTypeSymbol StreamRequestMarker { get; }

    public static KnownTypes? Resolve(Compilation compilation)
    {
        var rh2 = compilation.GetTypeByMetadataName("Umlamuli.IRequestHandler`2");
        var rh1 = compilation.GetTypeByMetadataName("Umlamuli.IRequestHandler`1");
        var nh = compilation.GetTypeByMetadataName("Umlamuli.INotificationHandler`1");
        var srh = compilation.GetTypeByMetadataName("Umlamuli.IStreamRequestHandler`2");
        var pb = compilation.GetTypeByMetadataName("Umlamuli.IPipelineBehavior`2");
        var spb = compilation.GetTypeByMetadataName("Umlamuli.IStreamPipelineBehavior`2");
        var pre = compilation.GetTypeByMetadataName("Umlamuli.Pipeline.IRequestPreProcessor`1");
        var post = compilation.GetTypeByMetadataName("Umlamuli.Pipeline.IRequestPostProcessor`2");
        var eh = compilation.GetTypeByMetadataName("Umlamuli.Pipeline.IRequestExceptionHandler`3");
        var ea = compilation.GetTypeByMetadataName("Umlamuli.Pipeline.IRequestExceptionAction`2");
        var requestMarker = compilation.GetTypeByMetadataName("Umlamuli.IRequest");
        var requestOfT = compilation.GetTypeByMetadataName("Umlamuli.IRequest`1");
        var notif = compilation.GetTypeByMetadataName("Umlamuli.INotification");
        var stream = compilation.GetTypeByMetadataName("Umlamuli.IStreamRequest`1");

        if (rh2 is null || rh1 is null || nh is null || srh is null || pb is null || spb is null
            || pre is null || post is null || eh is null || ea is null
            || requestMarker is null || requestOfT is null || notif is null || stream is null)
        {
            return null;
        }

        return new KnownTypes(
            rh2, rh1, nh, srh, pb, spb, pre, post, eh, ea,
            requestMarker, requestOfT, notif, stream);
    }
}
