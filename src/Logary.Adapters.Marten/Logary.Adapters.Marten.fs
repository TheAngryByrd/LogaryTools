namespace  Marten


open System
open Marten
open Marten.Services
open Npgsql
open LogaryTools.Infrastructure
open System.Linq
open Logary

type LoggaryLogger (logger) =
    interface IMartenLogger with
        member this.StartSession(session : IQuerySession) : IMartenSessionLogger = this :> IMartenSessionLogger
        member  this.SchemaChange(sql : string) = 
            logEx' Logary.LogLevel.Debug logger (Message.setField "sql" sql) sql None

    interface IMartenSessionLogger with
        member this.LogSuccess(command:NpgsqlCommand) =
            logEx' Logary.LogLevel.Debug logger (Message.setField "commandText" command.CommandText) command.CommandText None

        member this.LogFailure(command:NpgsqlCommand, ex : exn) =
            logEx' Logary.LogLevel.Error logger (Message.setField "commandText" command.CommandText) command.CommandText (Some ex)
        member this.RecordSavedChanges(session: IDocumentSession, commit : IChangeSet) =
            
            let msg =
                sprintf "Persisted %d updates, %d inserts, and %d deletions"
                    (commit.Updated.Count())
                    (commit.Inserted.Count())
                    (commit.Deleted.Count())

            let configureMessage =
                Message.setField "Updated" (commit.Updated.Count())
                >> Message.setField "Inserted" (commit.Inserted.Count())
                >> Message.setField "Deleted" (commit.Deleted.Count())
                >> Message.setField "Patches" (commit.Patches.Count())

            logEx' Logary.LogLevel.Debug logger configureMessage msg None 
    