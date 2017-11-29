namespace Hangfire.Logging

open System
open Hangfire.Logging
open Logary
open LogaryTools.Infrastructure
type LogaryLogger (logger) =

    let mapLogLevel level =
        match level with
        | Hangfire.Logging.LogLevel.Trace -> Logary.LogLevel.Verbose 
        | Hangfire.Logging.LogLevel.Debug -> Logary.LogLevel.Debug
        | Hangfire.Logging.LogLevel.Info ->  Logary.LogLevel.Info
        | Hangfire.Logging.LogLevel.Warn ->  Logary.LogLevel.Warn
        | Hangfire.Logging.LogLevel.Error -> Logary.LogLevel.Error
        | Hangfire.Logging.LogLevel.Fatal -> Logary.LogLevel.Fatal
        | _ -> Logary.LogLevel.Warn
        |> (fun level -> logEx' level logger)

    interface ILogProvider with
        member x.GetLogger(name : string) =
           Logary.Logging.getLoggerByName(name) |> LogaryLogger :> ILog
    interface ILog with
        member x.Log(logLevel: Hangfire.Logging.LogLevel, messageFunc: Func<string>, ``exception``: exn): bool = 
            match (messageFunc,``exception``) with
            | null, null -> true
            | _ ->

                let msg = messageFunc.Invoke()
                let configurer =
                    Message.setField "message" msg
                ``exception``
                |> Option.ofObj
                |> mapLogLevel logLevel configurer "{message}" 
                true