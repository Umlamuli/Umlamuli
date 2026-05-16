//-----------------------------------------------------------------------
// <copyright file="Emitter.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umlamuli.SourceGenerator;

internal static class Emitter
{
    public const string GeneratedClassName = "UmlamuliGeneratedExtensions";
    public const string GeneratedMediatorTypeName = "UmlamuliGeneratedMediator";

    public static string EmitRegistrations(HandlerCollection collection)
    {
        var sb = new StringBuilder();
        AppendHeader(sb);
        sb.AppendLine("using global::Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using global::Microsoft.Extensions.DependencyInjection.Extensions;");
        sb.AppendLine();
        sb.Append("namespace ").Append(collection.GeneratedNamespace).AppendLine(";");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("///     Source-generated entry points to register Umlamuli handlers without runtime assembly scanning.");
        sb.AppendLine("/// </summary>");
        sb.Append("public static class ").AppendLine(GeneratedClassName);
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    ///     Registers core Umlamuli services and every handler/behavior discovered at compile time.");
        sb.AppendLine("    ///     Uses the reflection-free wrapper-based <see cref=\"global::Umlamuli.Mediator\"/> implementation.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddUmlamuliGenerated(");
        sb.AppendLine("        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (services is null) throw new global::System.ArgumentNullException(nameof(services));");
        sb.AppendLine();
        sb.AppendLine("        services.TryAddTransient<global::Umlamuli.IMediator, global::Umlamuli.Mediator>();");
        AppendCoreServices(sb);
        AppendHandlerRegistrations(sb, collection.Registrations);
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    ///     Registers core Umlamuli services using the compile-time <see cref=\"" + GeneratedMediatorTypeName + "\"/>");
        sb.AppendLine("    ///     for switch-based dispatch, plus every handler/behavior discovered at compile time.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddUmlamuliCompileTime(");
        sb.AppendLine("        this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (services is null) throw new global::System.ArgumentNullException(nameof(services));");
        sb.AppendLine();
        sb.Append("        services.TryAddTransient<global::Umlamuli.IMediator, ").Append(GeneratedMediatorTypeName).AppendLine(">();");
        AppendCoreServices(sb);
        AppendHandlerRegistrations(sb, collection.Registrations);
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string EmitMediator(HandlerCollection collection)
    {
        var requestHandlers = collection.Registrations
            .Where(r => r.Kind == HandlerKind.RequestHandler && r.GenericArguments.Length == 2
                        && r.GenericArguments[0] is not null && r.GenericArguments[1] is not null)
            .GroupBy(r => r.GenericArguments[0]!)
            .Select(g => g.First())
            .ToList();

        var voidRequestHandlers = collection.Registrations
            .Where(r => r.Kind == HandlerKind.VoidRequestHandler && r.GenericArguments.Length == 1
                        && r.GenericArguments[0] is not null)
            .GroupBy(r => r.GenericArguments[0]!)
            .Select(g => g.First())
            .ToList();

        var streamHandlers = collection.Registrations
            .Where(r => r.Kind == HandlerKind.StreamRequestHandler && r.GenericArguments.Length == 2
                        && r.GenericArguments[0] is not null && r.GenericArguments[1] is not null)
            .GroupBy(r => r.GenericArguments[0]!)
            .Select(g => g.First())
            .ToList();

        var notificationHandlers = collection.Registrations
            .Where(r => r.Kind == HandlerKind.NotificationHandler && r.GenericArguments.Length == 1
                        && r.GenericArguments[0] is not null)
            .GroupBy(r => r.GenericArguments[0]!)
            .ToList();

        var sb = new StringBuilder();
        AppendHeader(sb);
        sb.AppendLine("using global::System;");
        sb.AppendLine("using global::System.Collections.Generic;");
        sb.AppendLine("using global::System.Linq;");
        sb.AppendLine("using global::System.Threading;");
        sb.AppendLine("using global::System.Threading.Tasks;");
        sb.AppendLine("using global::Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using global::Umlamuli;");
        sb.AppendLine("using global::Umlamuli.NotificationPublishers;");
        sb.AppendLine();
        sb.Append("namespace ").Append(collection.GeneratedNamespace).AppendLine(";");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("///     Compile-time mediator implementation that switch-dispatches over discovered request types.");
        sb.AppendLine("///     Resolves pipeline behaviors from the service provider at dispatch time so behaviors remain pluggable.");
        sb.AppendLine("/// </summary>");
        sb.Append("public sealed class ").Append(GeneratedMediatorTypeName).AppendLine(" : global::Umlamuli.IMediator");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly global::System.IServiceProvider _serviceProvider;");
        sb.AppendLine("    private readonly global::Umlamuli.INotificationPublisher _publisher;");
        sb.AppendLine();
        sb.Append("    public ").Append(GeneratedMediatorTypeName).AppendLine("(");
        sb.AppendLine("        global::System.IServiceProvider serviceProvider,");
        sb.AppendLine("        global::Umlamuli.INotificationPublisher publisher)");
        sb.AppendLine("    {");
        sb.AppendLine("        _serviceProvider = serviceProvider ?? throw new global::System.ArgumentNullException(nameof(serviceProvider));");
        sb.AppendLine("        _publisher = publisher ?? throw new global::System.ArgumentNullException(nameof(publisher));");
        sb.AppendLine("    }");
        sb.AppendLine();

        EmitSendWithResponse(sb, requestHandlers);
        EmitSendVoid(sb, voidRequestHandlers);
        EmitSendObject(sb, requestHandlers, voidRequestHandlers);
        EmitPublishGeneric(sb, notificationHandlers);
        EmitPublishObject(sb);
        EmitCreateStreamGeneric(sb, streamHandlers);
        EmitCreateStreamObject(sb, streamHandlers);
        EmitHelpers(sb);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void EmitSendWithResponse(StringBuilder sb, IReadOnlyList<HandlerRegistration> handlers)
    {
        sb.AppendLine("    public global::System.Threading.Tasks.Task<TResponse> Send<TResponse>(");
        sb.AppendLine("        global::Umlamuli.IRequest<TResponse> request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (request is null) throw new global::System.ArgumentNullException(nameof(request));");
        sb.AppendLine("        switch (request)");
        sb.AppendLine("        {");
        foreach (var h in handlers)
        {
            var req = h.GenericArguments[0]!;
            var resp = h.GenericArguments[1]!;
            sb.Append("            case ").Append(req).AppendLine(" r:");
            sb.Append("                return (global::System.Threading.Tasks.Task<TResponse>)(object)InvokeRequestHandler<")
                .Append(req).Append(", ").Append(resp).AppendLine(">(r, cancellationToken);");
        }
        sb.AppendLine("            default:");
        sb.AppendLine("                throw new global::System.InvalidOperationException(");
        sb.AppendLine("                    $\"No source-generated handler registered for request type '{request.GetType()}'.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitSendVoid(StringBuilder sb, IReadOnlyList<HandlerRegistration> handlers)
    {
        sb.AppendLine("    public global::System.Threading.Tasks.Task Send<TRequest>(");
        sb.AppendLine("        TRequest request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("        where TRequest : global::Umlamuli.IRequest");
        sb.AppendLine("    {");
        sb.AppendLine("        if (request is null) throw new global::System.ArgumentNullException(nameof(request));");
        sb.AppendLine("        switch (request)");
        sb.AppendLine("        {");
        foreach (var h in handlers)
        {
            var req = h.GenericArguments[0]!;
            sb.Append("            case ").Append(req).AppendLine(" r:");
            sb.Append("                return InvokeVoidRequestHandler<").Append(req).AppendLine(">(r, cancellationToken);");
        }
        sb.AppendLine("            default:");
        sb.AppendLine("                throw new global::System.InvalidOperationException(");
        sb.AppendLine("                    $\"No source-generated handler registered for request type '{request.GetType()}'.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitSendObject(
        StringBuilder sb,
        IReadOnlyList<HandlerRegistration> withResponse,
        IReadOnlyList<HandlerRegistration> voidHandlers)
    {
        sb.AppendLine("    public async global::System.Threading.Tasks.Task<object?> Send(");
        sb.AppendLine("        object request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (request is null) throw new global::System.ArgumentNullException(nameof(request));");
        sb.AppendLine("        switch (request)");
        sb.AppendLine("        {");
        foreach (var h in withResponse)
        {
            var req = h.GenericArguments[0]!;
            var resp = h.GenericArguments[1]!;
            sb.Append("            case ").Append(req).AppendLine(" r:");
            sb.Append("                return await InvokeRequestHandler<").Append(req).Append(", ").Append(resp).AppendLine(">(r, cancellationToken).ConfigureAwait(false);");
        }
        foreach (var h in voidHandlers)
        {
            var req = h.GenericArguments[0]!;
            sb.Append("            case ").Append(req).AppendLine(" r:");
            sb.Append("                await InvokeVoidRequestHandler<").Append(req).AppendLine(">(r, cancellationToken).ConfigureAwait(false);");
            sb.AppendLine("                return global::Umlamuli.Unit.Value;");
        }
        sb.AppendLine("            default:");
        sb.AppendLine("                throw new global::System.ArgumentException(");
        sb.AppendLine("                    $\"{request.GetType().Name} does not have a source-generated handler\", nameof(request));");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitPublishGeneric(StringBuilder sb, IReadOnlyList<IGrouping<string, HandlerRegistration>> groups)
    {
        sb.AppendLine("    public global::System.Threading.Tasks.Task Publish<TNotification>(");
        sb.AppendLine("        TNotification notification,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("        where TNotification : global::Umlamuli.INotification");
        sb.AppendLine("    {");
        sb.AppendLine("        if (notification is null) throw new global::System.ArgumentNullException(nameof(notification));");
        sb.AppendLine("        return PublishCore(notification, cancellationToken);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    private global::System.Threading.Tasks.Task PublishCore(");
        sb.AppendLine("        global::Umlamuli.INotification notification,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken)");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (notification)");
        sb.AppendLine("        {");
        foreach (var group in groups)
        {
            var notif = group.Key;
            sb.Append("            case ").Append(notif).AppendLine(" n:");
            sb.AppendLine("            {");
            sb.Append("                var handlers = _serviceProvider.GetServices<global::Umlamuli.INotificationHandler<").Append(notif).AppendLine(">>();");
            sb.AppendLine("                var executors = handlers.Select(h => new global::Umlamuli.NotificationHandlerExecutor(h,");
            sb.Append("                    (theNotification, theToken) => h.Handle((").Append(notif).AppendLine(")theNotification, theToken)));");
            sb.AppendLine("                return _publisher.Publish(executors, n, cancellationToken);");
            sb.AppendLine("            }");
        }
        sb.AppendLine("            default:");
        sb.AppendLine("                return global::System.Threading.Tasks.Task.CompletedTask;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitPublishObject(StringBuilder sb)
    {
        sb.AppendLine("    public global::System.Threading.Tasks.Task Publish(");
        sb.AppendLine("        object notification,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (notification is null) throw new global::System.ArgumentNullException(nameof(notification));");
        sb.AppendLine("        if (notification is global::Umlamuli.INotification typed)");
        sb.AppendLine("            return PublishCore(typed, cancellationToken);");
        sb.AppendLine("        throw new global::System.ArgumentException(");
        sb.AppendLine("            $\"{nameof(notification)} does not implement {nameof(global::Umlamuli.INotification)}\");");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitCreateStreamGeneric(StringBuilder sb, IReadOnlyList<HandlerRegistration> handlers)
    {
        sb.AppendLine("    public global::System.Collections.Generic.IAsyncEnumerable<TResponse> CreateStream<TResponse>(");
        sb.AppendLine("        global::Umlamuli.IStreamRequest<TResponse> request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (request is null) throw new global::System.ArgumentNullException(nameof(request));");
        sb.AppendLine("        switch (request)");
        sb.AppendLine("        {");
        foreach (var h in handlers)
        {
            var req = h.GenericArguments[0]!;
            var resp = h.GenericArguments[1]!;
            sb.Append("            case ").Append(req).AppendLine(" r:");
            sb.Append("                return (global::System.Collections.Generic.IAsyncEnumerable<TResponse>)InvokeStreamHandler<")
                .Append(req).Append(", ").Append(resp).AppendLine(">(r, cancellationToken);");
        }
        sb.AppendLine("            default:");
        sb.AppendLine("                throw new global::System.InvalidOperationException(");
        sb.AppendLine("                    $\"No source-generated stream handler registered for request type '{request.GetType()}'.\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitCreateStreamObject(StringBuilder sb, IReadOnlyList<HandlerRegistration> handlers)
    {
        if (handlers.Count == 0)
        {
            sb.AppendLine("    public global::System.Collections.Generic.IAsyncEnumerable<object?> CreateStream(");
            sb.AppendLine("        object request,");
            sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken = default)");
            sb.AppendLine("    {");
            sb.AppendLine("        if (request is null) throw new global::System.ArgumentNullException(nameof(request));");
            sb.AppendLine("        throw new global::System.ArgumentException(");
            sb.AppendLine("            $\"{request.GetType().Name} does not have a source-generated stream handler\", nameof(request));");
            sb.AppendLine("    }");
            sb.AppendLine();
            return;
        }

        sb.AppendLine("    public async global::System.Collections.Generic.IAsyncEnumerable<object?> CreateStream(");
        sb.AppendLine("        object request,");
        sb.AppendLine("        [global::System.Runtime.CompilerServices.EnumeratorCancellation] global::System.Threading.CancellationToken cancellationToken = default)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (request is null) throw new global::System.ArgumentNullException(nameof(request));");
        sb.AppendLine("        switch (request)");
        sb.AppendLine("        {");
        foreach (var h in handlers)
        {
            var req = h.GenericArguments[0]!;
            var resp = h.GenericArguments[1]!;
            sb.Append("            case ").Append(req).AppendLine(" r:");
            sb.AppendLine("            {");
            sb.Append("                await foreach (var item in InvokeStreamHandler<").Append(req).Append(", ").Append(resp).AppendLine(">(r, cancellationToken).ConfigureAwait(false))");
            sb.AppendLine("                    yield return item;");
            sb.AppendLine("                yield break;");
            sb.AppendLine("            }");
        }
        sb.AppendLine("            default:");
        sb.AppendLine("                throw new global::System.ArgumentException(");
        sb.AppendLine("                    $\"{request.GetType().Name} does not have a source-generated stream handler\", nameof(request));");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void EmitHelpers(StringBuilder sb)
    {
        sb.AppendLine("    private global::System.Threading.Tasks.Task<TResponse> InvokeRequestHandler<TRequest, TResponse>(");
        sb.AppendLine("        TRequest request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken)");
        sb.AppendLine("        where TRequest : global::Umlamuli.IRequest<TResponse>");
        sb.AppendLine("    {");
        sb.AppendLine("        global::System.Threading.Tasks.Task<TResponse> Handler(global::System.Threading.CancellationToken t = default)");
        sb.AppendLine("            => _serviceProvider.GetRequiredService<global::Umlamuli.IRequestHandler<TRequest, TResponse>>()");
        sb.AppendLine("                .Handle(request, t == global::System.Threading.CancellationToken.None ? cancellationToken : t);");
        sb.AppendLine();
        sb.AppendLine("        return _serviceProvider");
        sb.AppendLine("            .GetServices<global::Umlamuli.IPipelineBehavior<TRequest, TResponse>>()");
        sb.AppendLine("            .Reverse()");
        sb.AppendLine("            .Aggregate(");
        sb.AppendLine("                (global::Umlamuli.RequestHandlerDelegate<TResponse>)Handler,");
        sb.AppendLine("                (next, pipeline) => t => pipeline.Handle(request, next,");
        sb.AppendLine("                    t == global::System.Threading.CancellationToken.None ? cancellationToken : t))();");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    private async global::System.Threading.Tasks.Task InvokeVoidRequestHandler<TRequest>(");
        sb.AppendLine("        TRequest request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken)");
        sb.AppendLine("        where TRequest : global::Umlamuli.IRequest");
        sb.AppendLine("    {");
        sb.AppendLine("        async global::System.Threading.Tasks.Task<global::Umlamuli.Unit> Handler(global::System.Threading.CancellationToken t = default)");
        sb.AppendLine("        {");
        sb.AppendLine("            await _serviceProvider.GetRequiredService<global::Umlamuli.IRequestHandler<TRequest>>()");
        sb.AppendLine("                .Handle(request, t == global::System.Threading.CancellationToken.None ? cancellationToken : t)");
        sb.AppendLine("                .ConfigureAwait(false);");
        sb.AppendLine("            return global::Umlamuli.Unit.Value;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        await _serviceProvider");
        sb.AppendLine("            .GetServices<global::Umlamuli.IPipelineBehavior<TRequest, global::Umlamuli.Unit>>()");
        sb.AppendLine("            .Reverse()");
        sb.AppendLine("            .Aggregate(");
        sb.AppendLine("                (global::Umlamuli.RequestHandlerDelegate<global::Umlamuli.Unit>)Handler,");
        sb.AppendLine("                (next, pipeline) => t => pipeline.Handle(request, next,");
        sb.AppendLine("                    t == global::System.Threading.CancellationToken.None ? cancellationToken : t))()");
        sb.AppendLine("            .ConfigureAwait(false);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    private global::System.Collections.Generic.IAsyncEnumerable<TResponse> InvokeStreamHandler<TRequest, TResponse>(");
        sb.AppendLine("        TRequest request,");
        sb.AppendLine("        global::System.Threading.CancellationToken cancellationToken)");
        sb.AppendLine("        where TRequest : global::Umlamuli.IStreamRequest<TResponse>");
        sb.AppendLine("    {");
        sb.AppendLine("        global::System.Collections.Generic.IAsyncEnumerable<TResponse> Handler()");
        sb.AppendLine("            => _serviceProvider.GetRequiredService<global::Umlamuli.IStreamRequestHandler<TRequest, TResponse>>()");
        sb.AppendLine("                .Handle(request, cancellationToken);");
        sb.AppendLine();
        sb.AppendLine("        return _serviceProvider");
        sb.AppendLine("            .GetServices<global::Umlamuli.IStreamPipelineBehavior<TRequest, TResponse>>()");
        sb.AppendLine("            .Reverse()");
        sb.AppendLine("            .Aggregate(");
        sb.AppendLine("                (global::Umlamuli.StreamHandlerDelegate<TResponse>)Handler,");
        sb.AppendLine("                (next, pipeline) => () => pipeline.Handle(request, next, cancellationToken))();");
        sb.AppendLine("    }");
    }

    private static void AppendCoreServices(StringBuilder sb)
    {
        sb.AppendLine("        services.TryAddTransient<global::Umlamuli.ISender>(sp => sp.GetRequiredService<global::Umlamuli.IMediator>());");
        sb.AppendLine("        services.TryAddTransient<global::Umlamuli.IPublisher>(sp => sp.GetRequiredService<global::Umlamuli.IMediator>());");
        sb.AppendLine("        services.TryAddTransient<global::Umlamuli.INotificationPublisher, global::Umlamuli.NotificationPublishers.ForeachAwaitPublisher>();");
        sb.AppendLine();
    }

    private static void AppendHandlerRegistrations(StringBuilder sb, IReadOnlyList<HandlerRegistration> registrations)
    {
        if (registrations.Count == 0) return;

        foreach (var r in registrations)
        {
            switch (r.Kind)
            {
                case HandlerKind.RequestHandler:
                case HandlerKind.VoidRequestHandler:
                case HandlerKind.StreamRequestHandler:
                    sb.Append("        services.AddTransient<").Append(r.ServiceType).Append(", ").Append(r.ImplementationType).AppendLine(">();");
                    break;
                case HandlerKind.NotificationHandler:
                case HandlerKind.PipelineBehavior:
                case HandlerKind.StreamPipelineBehavior:
                case HandlerKind.RequestPreProcessor:
                case HandlerKind.RequestPostProcessor:
                case HandlerKind.RequestExceptionHandler:
                case HandlerKind.RequestExceptionAction:
                    sb.Append("        services.AddTransient<").Append(r.ServiceType).Append(", ").Append(r.ImplementationType).AppendLine(">();");
                    break;
            }
        }

        sb.AppendLine();
    }

    private static void AppendHeader(StringBuilder sb)
    {
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#pragma warning disable CS1591");
        sb.AppendLine();
    }
}
