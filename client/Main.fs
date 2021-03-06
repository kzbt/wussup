module Main

open Fable.Core
open Fable.React
open Fable.React.Props
open Fable.React.Helpers

open Elmish
open Elmish.React

type Model = { Count : int }

type Messages = Incr | Decr

let update msg model =
    match msg with
    | Incr -> { Count = model.Count + 1 }
    | Decr -> { Count = model.Count - 1 }

// Helper function, a string is a valid ReactElement
let text (content: string) : ReactElement = unbox content
// Helper function, for the initial model
let init() = { Count = 0 }

let view model dispatch =
    div [] [
      button [ OnClick (fun _ -> dispatch Incr) ] [ text "Increment" ]
      div [] [ text (string model.Count) ]
      button [ OnClick (fun _ -> dispatch Decr) ] [ text "Decrement"]
    ]

Program.mkSimple init update view
|> Program.withReactBatched "app"
|> Program.run
