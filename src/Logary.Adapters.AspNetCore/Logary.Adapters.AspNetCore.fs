namespace  Microsoft.Extensions.Logging

open System
open Microsoft.Extensions.Logging
open LogaryTools.Infrastructure
open Logary

type LoggaryLogger (logger) =

    let mapLogLevel level =
        match level with
        | Microsoft.Extensions.Logging.LogLevel.Trace ->     Logary.LogLevel.Verbose
        | Microsoft.Extensions.Logging.LogLevel.None ->       Logary.LogLevel.Verbose 
        | Microsoft.Extensions.Logging.LogLevel.Debug ->        Logary.LogLevel.Debug
        | Microsoft.Extensions.Logging.LogLevel.Information -> Logary.LogLevel.Info
        | Microsoft.Extensions.Logging.LogLevel.Warning ->     Logary.LogLevel.Warn
        | Microsoft.Extensions.Logging.LogLevel.Error ->       Logary.LogLevel.Error
        | Microsoft.Extensions.Logging.LogLevel.Critical->     Logary.LogLevel.Fatal
        | _ ->                                                 Logary.LogLevel.Verbose         
        |> (fun level -> logEx' level logger)

    interface ILogger with
        member __.Log(
                        loglevel : Microsoft.Extensions.Logging.LogLevel,
                        eventId : EventId,
                        state : 'a,
                        ex : exn,
                        formatter : Func<'a, Exception, string>
                    ) =
            let formattedMessage = formatter.Invoke(state,ex)
            let configureMessage =
                    Message.setField "eventId" eventId
                    >> Message.setField "message" formattedMessage
                    >> Message.setField "state" state
            
            let logger = mapLogLevel loglevel configureMessage

            ex
            |> Option.ofObj
            |> logger "{eventId} - {message}"
         member __.IsEnabled _ = true
         member __.BeginScope _ = {
             new IDisposable with
                member __.Dispose () = ()
         }

type LogaryLoggerProvider () =
    interface ILoggerProvider with
        member __.CreateLogger(name) = Log.create name |> LoggaryLogger :> ILogger
        member __.Dispose () = ()
