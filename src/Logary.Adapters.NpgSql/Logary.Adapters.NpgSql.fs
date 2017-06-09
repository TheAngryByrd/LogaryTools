namespace  Npgsql.Logging


open System
open Npgsql.Logging
open Infrastructure

type LoggaryLogger (logger) =
    inherit NpgsqlLogger() with

        let mapLogLevel level =
            match level with
            | NpgsqlLogLevel.Trace -> logEx' Logary.LogLevel.Verbose logger
            | NpgsqlLogLevel.Debug -> logEx' Logary.LogLevel.Debug logger
            | NpgsqlLogLevel.Info -> logEx' Logary.LogLevel.Info logger
            | NpgsqlLogLevel.Warn -> logEx' Logary.LogLevel.Warn logger
            | NpgsqlLogLevel.Error -> logEx' Logary.LogLevel.Error logger
            | NpgsqlLogLevel.Fatal -> logEx' Logary.LogLevel.Fatal logger
            | _ -> logEx' Logary.LogLevel.Verbose logger
        override x.IsEnabled(level : NpgsqlLogLevel) =
            true
        override x.Log(level, connectorId, msg, ex) =  
            ex
            |> Option.ofObj
            |> mapLogLevel level msg 

type LogaryLoggerProvider () =
    interface INpgsqlLoggingProvider with
        member x.CreateLogger(name) = Logary.Logging.getLoggerByName(name) |> LoggaryLogger :> NpgsqlLogger
