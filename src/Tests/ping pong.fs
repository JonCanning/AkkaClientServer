module ``ping pong``

open Messages

[<SetUp>]
let setUp() = Server.start()

[<TearDown>]
let tearDown() = 
  Client.dispose()
  Server.dispose()

[<Test>]
let ``register turtle and ping pong``() = 
  let response = 
    Register
    |> RequestMessage
    |> Client.send
  
  let token = 
    match extractResponse response with
    | Registered token -> token
    | msg -> unhandled msg
  
  let response = 
    Ping token
    |> RequestMessage
    |> Client.send
    |> extractResponse
  
  response == Pong token

[<Test>]
let ``register turtle and draw line``() = 
  let response = 
    Register
    |> RequestMessage
    |> Client.send
  
  let token = 
    match extractResponse response with
    | Registered token -> token
    | msg -> unhandled msg
  let response = TurtleCommand(token, Move 100) |> RequestMessage |> Client.send |> extractResponse
  response == TurtleCommandExecuted
  let response = TurtleCommand(token, Color(100uy, 100uy, 100uy)) |> RequestMessage |> Client.send |> extractResponse
  response == TurtleCommandExecuted
  let response = TurtleCommand(token, Polygon(6, 100)) |> RequestMessage |> Client.send |> extractResponse
  response == TurtleCommandExecuted

[<Test>]
let ``unknown token``() =
  let token = System.Guid.NewGuid()
  let response = TurtleCommand(token, Move 100) |> RequestMessage |> Client.send |> extractResponse
  response == UnknownToken token 