module Wussup.Main

open Wussup
open System.IO

let readLines (filePath: string) = seq {
  use reader = new StreamReader(filePath)
  while not reader.EndOfStream do
    yield Parser.nextLine(reader, "")
}

let loadFromFile (path: string): Unit =
  let messages = Seq.map Parser.lineToMessage (readLines path)
  messages
  |> Seq.filter Option.isSome
  |> Seq.iter (Option.iter Message.save)

[<EntryPoint>]
let main _argv =
  WebApp.start
  0
