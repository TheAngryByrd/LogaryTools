namespace Hangfire.Logging

open System
open Hangfire.Logging
open LogaryTools.Infrastructure
type LogaryLogger (logger) =

    let mapLogLevel level =
        match level with
        | Hangfire.Logging.LogLevel.Trace -> logEx' Logary.LogLevel.Verbose logger
        | Hangfire.Logging.LogLevel.Debug -> logEx' Logary.LogLevel.Debug logger
        | Hangfire.Logging.LogLevel.Info -> logEx' Logary.LogLevel.Info logger
        | Hangfire.Logging.LogLevel.Warn -> logEx' Logary.LogLevel.Warn logger
        | Hangfire.Logging.LogLevel.Error -> logEx' Logary.LogLevel.Error logger
        | Hangfire.Logging.LogLevel.Fatal -> logEx' Logary.LogLevel.Fatal logger
        | _ -> logEx' Logary.LogLevel.Warn logger

    interface ILogProvider with
        member x.GetLogger(name : string) =
           Logary.Logging.getLoggerByName(name) |> LogaryLogger :> ILog
    interface ILog with
        member x.Log(logLevel: Hangfire.Logging.LogLevel, messageFunc: Func<string>, ``exception``: exn): bool = 
            match (messageFunc,``exception``) with
            | null, null -> true
            | _ ->
                let msg = messageFunc.Invoke()
                ``exception``
                |> Option.ofObj
                |> mapLogLevel logLevel msg 
                true