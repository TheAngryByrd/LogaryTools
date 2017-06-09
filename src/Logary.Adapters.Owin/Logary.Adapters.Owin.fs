namespace  Microsoft.Owin.Logging

open System
open Microsoft.Owin.Logging
open System.Diagnostics
open LogaryTools.Infrastructure
open Logary

type LoggaryLogger (logger) =

    let mapLogLevel level =
        match level with
        | TraceEventType.Verbose -> logEx' Logary.LogLevel.Verbose logger
        | TraceEventType.Start -> logEx' Logary.LogLevel.Debug logger
        | TraceEventType.Stop -> logEx' Logary.LogLevel.Debug logger
        | TraceEventType.Start -> logEx' Logary.LogLevel.Debug logger
        | TraceEventType.Suspend -> logEx' Logary.LogLevel.Debug logger
        | TraceEventType.Resume -> logEx' Logary.LogLevel.Debug logger
        | TraceEventType.Transfer -> logEx' Logary.LogLevel.Debug logger
        | TraceEventType.Information -> logEx' Logary.LogLevel.Info logger
        | TraceEventType.Warning -> logEx' Logary.LogLevel.Warn logger
        | TraceEventType.Error -> logEx' Logary.LogLevel.Error logger
        | TraceEventType.Critical-> logEx' Logary.LogLevel.Fatal logger
        | _ -> logEx' Logary.LogLevel.Verbose logger

    interface ILogger with
        override x.WriteCore( 
            eventType : TraceEventType, 
            eventId : int,  
            state : obj, 
            ex : exn, 
            formatter : Func<obj, Exception, string>) =  
            
                let logger = mapLogLevel eventType
                let format = formatter.Invoke(state,ex)
                ex
                |> Option.ofObj
                |> logger format

                true

type LogaryLoggerProvider () =
    interface ILoggerFactory with
        member x.Create(name) = Logary.Logging.getLoggerByName(name) |> LoggaryLogger :> ILogger
