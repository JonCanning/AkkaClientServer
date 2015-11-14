module Client

open Akka.FSharp
open System

let config = """
  akka {
    actor {
      provider = "Akka.Remote.RemoteActorRefProvider, Akka.Remote"
      serialization-bindings { "System.Object" = wire }
      }
    remote {
      helios.tcp {
        port = 8000
        hostname = localhost
      }
    }
  }
  """
let system = Configuration.parse config |> System.create "client"
let server = system.ActorSelection "akka.tcp://server@localhost:8001/user/server"
let send msg = server.Ask(msg, TimeSpan.FromSeconds 5.) |> Async.RunSynchronously
let dispose = system.Dispose >> system.AwaitTermination
