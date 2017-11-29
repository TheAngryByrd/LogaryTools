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
        member x.Log(
                        loglevel : Microsoft.Extensions.Logging.LogLevel,
                        eventId : EventId,
                        state : 'a,
                        ex : exn,
                        formatter : Func<'a, Exception, string>
                    ) =
            let configureMessage =
                    Message.setFieldFromObject "eventId" eventId
            
            let logger = mapLogLevel loglevel configureMessage
            let formattedMessage = formatter.Invoke(state,ex)
            ex
            |> Option.ofObj
            |> logger formattedMessage
         member x.IsEnabled _ = true
         member x.BeginScope _ = {
             new IDisposable with
                member x.Dispose () = ()
         }

type LogaryLoggerProvider () =
    interface ILoggerProvider with
        member x.CreateLogger(name) = 
            Logary.Logging.getLoggerByName(name) |> LoggaryLogger :> ILogger
        member x.Dispose () = ()
