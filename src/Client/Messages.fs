module Messages

open System

type Token = Guid
type Angle = float
type Length = int
type Sides = int

type TurtleCommand = 
  | Turn of Angle
  | Move of Length
  | Polygon of Sides * Length
  | Color of byte * byte * byte

type Request = 
  | Register
  | TurtleCommand of Token * TurtleCommand
  | Ping of Token

type Response = 
  | Registered of Token
  | TurtleCommandExecuted
  | Pong of Token
  | UnknownToken of Token

type Message = 
  | RequestMessage of Request
  | ResponseMessage of Response

let extractResponse = 
  function 
  | ResponseMessage response -> response
  | _ -> failwith "Not a response"

let extractRequest = 
  function 
  | RequestMessage request -> request
  | _ -> failwith "Not a request"

exception UnhandledMessageException of obj

let unhandled o = UnhandledMessageException o |> raise
