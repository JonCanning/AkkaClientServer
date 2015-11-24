module Server

open Akka.FSharp
open Messages
open System

let config = """
    akka {
      actor {
        provider = "Akka.Remote.RemoteActorRefProvider, Akka.Remote"
        serialization-bindings { "System.Object" = wire }
        }
      remote {
        helios.tcp {
          port = 8001 
          hostname = localhost
        }
      }
    }
    """
let (<!!) (actor : Actor<_>) msg = actor.Sender() <! ResponseMessage msg

let turtleHandler (actor : Actor<_>) msg = 
  match extractRequest msg with
  | Ping token -> actor <!! Pong token
  | msg -> unhandled msg

let handler (actor : Actor<_>) msg = 
  match extractRequest msg with
  | RegisterTurtle -> 
    let token = Guid.NewGuid()
    actorOf2 turtleHandler
    |> spawn actor (token |> string)
    |> ignore
    actor <!! TurtleRegistered token
  | Ping token -> 
    let handler = 
      token
      |> string
      |> actor.ActorSelection
    handler.Tell(msg, actor.Sender())

let system = Configuration.parse config |> System.create "server"

let start() = 
  actorOf2 handler
  |> spawn system "server"
  |> ignore

let dispose = system.Dispose >> system.AwaitTermination
