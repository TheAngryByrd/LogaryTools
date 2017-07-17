namespace LogaryTools.Infrastructure

[<AutoOpen>]
module Impl =
    open Logary

    let inline addExnIfNeeded (ex : exn option) =
        match ex with
        | Some e -> Message.addExn e
        | _ -> id

    let inline logEx' level logger configureMessage message ex =
        message
        |> Message.event level 
        |> addExnIfNeeded ex
        |> configureMessage
        |> Logger.logSimple logger