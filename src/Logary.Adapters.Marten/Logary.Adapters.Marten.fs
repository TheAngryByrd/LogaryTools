namespace  Marten

open Marten
open Marten.Services
open Npgsql
open LogaryTools.Infrastructure
open System.Linq
open Logary

type LoggaryLogger (logger) =
    interface IMartenLogger with
        member this.StartSession(_session : IQuerySession) : IMartenSessionLogger = this :> IMartenSessionLogger
        member __.SchemaChange(sql : string) = 
            logEx' Logary.LogLevel.Debug logger (Message.setField "sql" sql) sql None

    interface IMartenSessionLogger with
        member __.LogSuccess(command:NpgsqlCommand) =
            logEx' Logary.LogLevel.Debug logger (Message.setField "commandText" command.CommandText) command.CommandText None

        member __.LogFailure(command:NpgsqlCommand, ex : exn) =
            logEx' Logary.LogLevel.Error logger (Message.setField "commandText" command.CommandText) command.CommandText (Some ex)
        member __.RecordSavedChanges(_session: IDocumentSession, commit : IChangeSet) =
            let msg = sprintf "Persisted {Updated} updates, {Inserted} inserts, {Deleted} deletions, {Patches} patched"
            let configureMessage =
                Message.setField "Updated" (commit.Updated.Count())
                >> Message.setField "Inserted" (commit.Inserted.Count())
                >> Message.setField "Deleted" (commit.Deleted.Count())
                >> Message.setField "Patches" (commit.Patches.Count())

            logEx' Logary.LogLevel.Debug logger configureMessage msg None 
    