namespace  Marten


open System
open Marten
open Marten.Services
open Npgsql
open LogaryTools.Infrastructure
open System.Linq

type LoggaryLogger (logger) as this=
    interface IMartenLogger with
        member this.StartSession(session : IQuerySession) : IMartenSessionLogger = this :> IMartenSessionLogger
        member  this.SchemaChange(sql : string) = 
            logEx' Logary.LogLevel.Debug logger sql None

    interface IMartenSessionLogger with
        member this.LogSuccess(command:NpgsqlCommand) =
            logEx' Logary.LogLevel.Debug logger command.CommandText None

        member this.LogFailure(command:NpgsqlCommand, ex : exn) =
            logEx' Logary.LogLevel.Error logger command.CommandText (Some ex)
        member this.RecordSavedChanges(session: IDocumentSession, commit : IChangeSet) =
            let msg =
                sprintf "Persisted %d updates, %d inserts, and %d deletions"
                    (commit.Updated.Count())
                    (commit.Inserted.Count())
                    (commit.Deleted.Count())

            logEx' Logary.LogLevel.Debug logger msg None
    