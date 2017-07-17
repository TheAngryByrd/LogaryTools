namespace  Microsoft.Owin.Logging

open System
open Microsoft.Owin.Logging
open System.Diagnostics
open LogaryTools.Infrastructure
open Logary

type LoggaryLogger (logger) =

    let mapLogLevel level =
        match level with
        | TraceEventType.Verbose ->     Logary.LogLevel.Verbose
        | TraceEventType.Start ->       Logary.LogLevel.Debug 
        | TraceEventType.Stop ->        Logary.LogLevel.Debug
        | TraceEventType.Suspend ->     Logary.LogLevel.Debug
        | TraceEventType.Resume ->      Logary.LogLevel.Debug
        | TraceEventType.Transfer ->    Logary.LogLevel.Debug
        | TraceEventType.Information -> Logary.LogLevel.Info
        | TraceEventType.Warning ->     Logary.LogLevel.Warn
        | TraceEventType.Error ->       Logary.LogLevel.Error
        | TraceEventType.Critical->     Logary.LogLevel.Fatal
        | _ ->                          Logary.LogLevel.Verbose         
        |> (fun level -> logEx' level logger)

    interface ILogger with
        override x.WriteCore(eventType : TraceEventType, 
                             eventId : int,  
                             state : obj, 
                             ex : exn, 
                             formatter : Func<obj, Exception, string>) =  
                let configureMessage =
                    Message.setField "eventId" eventId
            
                let logger = mapLogLevel eventType configureMessage
                let formattedMessage = formatter.Invoke(state,ex)
                ex
                |> Option.ofObj
                |> logger formattedMessage

                true

type LogaryLoggerProvider () =
    interface ILoggerFactory with
        member x.Create(name) = Logary.Logging.getLoggerByName(name) |> LoggaryLogger :> ILogger
