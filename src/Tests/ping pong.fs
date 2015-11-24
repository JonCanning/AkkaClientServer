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
  let response = RegisterTurtle |> RequestMessage |> Client.send
  let token = match extractResponse response with
  | TurtleRegistered token -> token
  | msg -> unhandled msg
  let response = Ping token |> RequestMessage |> Client.send |> extractResponse
  response == Pong token
