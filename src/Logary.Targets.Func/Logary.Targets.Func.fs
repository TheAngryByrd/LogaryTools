namespace Logary.Targets

open System.IO
open System.Runtime.CompilerServices
open Hopac
open Hopac.Infixes
open Logary
open Logary.Internals
open Logary.Target
open Logary.Formatting
open System.Threading.Tasks

module GenericFunc =
    type GenericFuncConf =
        { 
            func : Message -> Job<unit>            
        }

            [<CompiledName "Create">]
            static member create(f : Message -> Job<unit>  ) =
                {
                    /// a generic function for pumping data anywhere
                    func = f
                }
    module internal Impl =
        let loop (conf : GenericFuncConf) // the conf is specific to your target
                (ri : RuntimeInfo) 
                (requests : RingBuffer<_>) 
                (shutdown : Ch<_>) = 

            let rec loop () : Job<unit> =
                Alt.choose [
                    shutdown ^=> fun ack ->
                         ack *<= () :> Job<_>
                    RingBuffer.take requests ^=> function
                        | Log (message, ack) ->
                            job {
                                do! message |> conf.func
                                do! ack *<= ()
                                return! loop ()
                            }
                        | Flush (ackCh, nack) ->
                            job {
                                do! Ch.give ackCh () <|> nack
                                return! loop ()
                            }
                ] :> Job<_>
            loop ()
    
    [<CompiledName "Create">]
    let create conf name = TargetUtils.stdNamedTarget (Impl.loop conf) name