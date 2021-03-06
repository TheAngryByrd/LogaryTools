namespace  Npgsql.Logging

open Npgsql.Logging
open LogaryTools.Infrastructure
open Logary

type LoggaryLogger (logger) =
    inherit NpgsqlLogger() with

        let mapLogLevel level configureMessage=
            match level with
            | NpgsqlLogLevel.Trace -> Logary.LogLevel.Verbose
            | NpgsqlLogLevel.Debug -> Logary.LogLevel.Debug
            | NpgsqlLogLevel.Info ->  Logary.LogLevel.Info
            | NpgsqlLogLevel.Warn ->  Logary.LogLevel.Warn
            | NpgsqlLogLevel.Error -> Logary.LogLevel.Error
            | NpgsqlLogLevel.Fatal -> Logary.LogLevel.Fatal
            | _ -> Logary.LogLevel.Verbose 
            |> (fun level -> logEx' level logger configureMessage )
        override __.IsEnabled(_level : NpgsqlLogLevel) = true
        override __.Log(level, connectorId, msg, ex) =  
            let configureMessage =
                Message.setField "connectorId" connectorId
                >> Message.setField "message" msg
            ex
            |> Option.ofObj
            |> mapLogLevel level configureMessage "connectorId: {connectorId} - {message}"

type LogaryLoggerProvider () =
    interface INpgsqlLoggingProvider with
        member __.CreateLogger(name) = Log.create name |> LoggaryLogger :> NpgsqlLogger
