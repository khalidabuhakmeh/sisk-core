﻿// The Sisk Framework source code
// Copyright (c) 2023 PROJECT PRINCIPIUM
//
// The code below is licensed under the MIT license as
// of the date of its publication, available at
//
// File name:   HttpServerHandlerRepository.cs
// Repository:  https://github.com/sisk-http/core

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sisk.Core.Entity;
using Sisk.Core.Routing;

namespace Sisk.Core.Http.Handlers;

enum HttpServerHandlerActionEvent
{
    ServerStarting,
    ServerStarted,
    SetupRouter,
    ContextBagCreated,
    HttpRequestOpen,
    HttpRequestClose,
    Exception,
    Stopping,
    Stopped,
}

internal class HttpServerHandlerRepository
{
    private readonly HttpServer parent;
    private readonly List<HttpServerHandler> handlers = new List<HttpServerHandler>(20);
    internal readonly DefaultHttpServerHandler _default = new DefaultHttpServerHandler();

    public HttpServerHandlerRepository(HttpServer parent)
    {
        this.parent = parent;
        RegisterHandler(_default);
    }

    public void RegisterHandler(HttpServerHandler handler)
    {
        handlers.Add(handler);
    }

    private bool IsEventBreakable(HttpServerHandlerActionEvent eventName)
        => eventName == HttpServerHandlerActionEvent.ServerStarting
        || eventName == HttpServerHandlerActionEvent.ServerStarted
        || eventName == HttpServerHandlerActionEvent.SetupRouter;

    private void CallEvery(Action<HttpServerHandler> action, HttpServerHandlerActionEvent eventName)
    {
        Span<HttpServerHandler> hspan = CollectionsMarshal.AsSpan(handlers);
        ref HttpServerHandler hpointer = ref MemoryMarshal.GetReference(hspan);
        for (int i = 0; i < hspan.Length; i++)
        {
            HttpServerHandler handler = Unsafe.Add(ref hpointer, i);

            try
            {
                action(handler);
            }
            catch (Exception ex)
            {
                if (parent.ServerConfiguration.ThrowExceptions == false && IsEventBreakable(eventName) == false)
                {
                    parent.ServerConfiguration.ErrorsLogsStream?.WriteException(ex);
                }
                else throw;
            }
        }
    }

    internal void ServerStarting(HttpServer val) => CallEvery(handler => handler.InvokeOnServerStarting(val), HttpServerHandlerActionEvent.ServerStarting);
    internal void ServerStarted(HttpServer val) => CallEvery(handler => handler.InvokeOnServerStarted(val), HttpServerHandlerActionEvent.ServerStarted);
    internal void SetupRouter(Router val) => CallEvery(handler => handler.InvokeOnSetupRouter(val), HttpServerHandlerActionEvent.SetupRouter);
    internal void ContextBagCreated(TypedValueDictionary val) => CallEvery(handler => handler.InvokeOnContextBagCreated(val), HttpServerHandlerActionEvent.ContextBagCreated);
    internal void HttpRequestOpen(HttpRequest val) => CallEvery(handler => handler.InvokeOnHttpRequestOpen(val), HttpServerHandlerActionEvent.HttpRequestOpen);
    internal void HttpRequestClose(HttpServerExecutionResult val) => CallEvery(handler => handler.InvokeOnHttpRequestClose(val), HttpServerHandlerActionEvent.HttpRequestClose);
    internal void Exception(Exception val) => CallEvery(handler => handler.InvokeOnException(val), HttpServerHandlerActionEvent.Exception);
    internal void Stopping(HttpServer val) => CallEvery(handler => handler.InvokeOnServerStopping(val), HttpServerHandlerActionEvent.Stopping);
    internal void Stopped(HttpServer val) => CallEvery(handler => handler.InvokeOnServerStopped(val), HttpServerHandlerActionEvent.Stopped);
}
