[<EntryPoint>]
let main _ = 
  Server.start()
  System.Console.ReadLine() |> ignore
  0 
