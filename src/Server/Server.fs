module Server

open Akka.FSharp
open Messages
open System
open Plotting
open System.Drawing

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

let turtleHandler (mbx : Actor<_>) =
  let rec loop plotter =
    actor {
      let! msg = mbx.Receive()
      let plotter =
        match extractRequest msg with
        | TurtleCommand(token, command) ->
          match command with
          | Move x ->
            let plotter = Plotting.move x plotter
            mbx <!! TurtleCommandExecuted
            plotter
        | _ ->  plotter
      return! loop plotter
     } 
  let plotter = {Plotter.bitmap = new Bitmap(800, 800)
                 position = 400, 400
                 color = Color.Black
                 direction = 0.}
  loop plotter

let handler (actor : Actor<_>) msg = 
  match extractRequest msg with
  | Register -> 
    let token = Guid.NewGuid()
    turtleHandler |> spawn actor (token |> string) |> ignore
    actor <!! Registered token
  | Ping token -> actor <!! Pong token
  | TurtleCommand(token, _) | Ping token -> 
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
