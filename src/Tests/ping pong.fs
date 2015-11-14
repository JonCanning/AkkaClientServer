module ``ping pong``

open Messages

[<SetUp>]
let setUp() = Server.start()

[<TearDown>]
let tearDown() = 
  Client.dispose()
  Server.dispose()

[<Test>]
let ``ping returns pong``() = Client.send Ping == Pong

[<Test>]
let ``pong returns ping``() = Client.send Pong == Ping
