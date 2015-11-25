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
let writeImage plotter token = sprintf "c:/tmp/%O.bmp" token |> plotter.Bitmap.Save

let turtleHandler (mbx : Actor<_>) = 
  let rec loop plotter = 
    actor { 
      let! msg = mbx.Receive()
      let token, plotter = 
        match extractRequest msg with
        | TurtleCommand(token, command) -> 
          match command with
          | Move x -> Some token, Plotting.move x plotter
          | Turn x -> Some token, Plotting.turn x plotter
          | Polygon(x, y) -> Some token, Plotting.polygon x y plotter
        | _ -> None, plotter
      
      let plotter = 
        match token, plotter with
        | Some token, plotter -> 
          writeImage plotter token
          plotter
        | _ -> plotter
      
      mbx <!! TurtleCommandExecuted
      return! loop plotter
    }
  
  let plotter = 
    { Plotter.Bitmap = new Bitmap(800, 800)
      Position = 400, 400
      Color = Color.White
      Direction = 0. }
  
  loop plotter

let handler (actor : Actor<_>) msg = 
  match extractRequest msg with
  | Register -> 
    let token = Guid.NewGuid()
    turtleHandler
    |> spawn actor (token |> string)
    |> ignore
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
