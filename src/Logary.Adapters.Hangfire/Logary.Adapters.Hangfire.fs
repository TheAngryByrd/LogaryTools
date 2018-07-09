namespace Hangfire.Logging

open System
open Hangfire.Logging
open Logary
open LogaryTools.Infrastructure

module Util =
    let mapLogLevel level =
        match level with
        | Hangfire.Logging.LogLevel.Trace -> Logary.LogLevel.Verbose 
        | Hangfire.Logging.LogLevel.Debug -> Logary.LogLevel.Debug
        | Hangfire.Logging.LogLevel.Info ->  Logary.LogLevel.Info
        | Hangfire.Logging.LogLevel.Warn ->  Logary.LogLevel.Warn
        | Hangfire.Logging.LogLevel.Error -> Logary.LogLevel.Error
        | Hangfire.Logging.LogLevel.Fatal -> Logary.LogLevel.Fatal
        | _ -> Logary.LogLevel.Warn

type LogaryLogger (logger) =
    let log level = logEx' level logger

    interface ILogProvider with
        member __.GetLogger(name : string) = Log.create name |> LogaryLogger :> ILog

    interface ILog with
        member __.Log(logLevel: Hangfire.Logging.LogLevel, messageFunc: Func<string>, ``exception``: exn): bool = 
            match (messageFunc,``exception``) with
            | null, null -> true
            | _ ->
                let msg = messageFunc.Invoke()
                let level = Util.mapLogLevel logLevel
                let configurer = Message.setField "message" msg
                let exn = Option.ofObj ``exception``
                log level configurer "{message}" exn
                true