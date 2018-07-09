namespace Logary.Targets

open Hopac
open Hopac.Infixes
open Logary
open Logary.Internals

module GenericFunc =
    type GenericFuncConf =
        { func : Message -> Job<unit> }

        [<CompiledName "Create">]
        static member create(f : Message -> Job<unit>  ) = { func = f }
        
    module internal Impl =
        let loop conf (api: TargetAPI) = 

            let rec loop () : Job<unit> =
                Alt.choose [
                    api.shutdownCh ^=> fun ack ->
                         ack *<= () :> Job<_>
                    RingBuffer.take api.requests ^=> function
                        | Log (message, ack) ->
                            job {
                                do! message |> conf.func
                                do! ack *<= ()
                                return! loop ()
                            }
                        | Flush (ack, nack) ->
                            job {
                                do! ack <|> nack
                                return! loop ()
                            }
                ] :> Job<_>
            loop ()
    
    [<CompiledName "Create">]
    let create conf name = TargetConf.createSimple (Impl.loop conf) name