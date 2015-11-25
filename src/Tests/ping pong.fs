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