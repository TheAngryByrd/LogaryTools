namespace LogaryTools.Infrastructure

[<AutoOpen>]
module Impl =
    open Logary

    let inline addExnIfNeeded (ex : exn option) =
        match ex with
        | Some e -> Message.addExn e
        | _ -> id


    let inline logEx' level logger message ex =
        message
        |> Message.event level 
        |> addExnIfNeeded ex
        |> Logger.logSimple logger