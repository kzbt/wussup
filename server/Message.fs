module Wussup.Message

open FSharp.Data.Sql
open System
open Wussup

type Content =
  | Text of string
  | Attachment of string
  | TextWithAttachment of string * string

type Message = {
  datetime: DateTime
  author: string
  content: Content
}

let save (msg: Message): Unit =
  let row = Store.ctx.Public.Message.Create()
  row.Datetime <- msg.datetime
  row.Author <- msg.author
  match msg.content with
    | Text t ->
      row.TextContent <- Some t

    | Attachment a ->
      row.Attachment <- Some a

    | TextWithAttachment (t, a) ->
      row.TextContent <- Some t;
      row.Attachment <- Some a
   
  Store.ctx.SubmitUpdates()

let private _toMessage (row: Store.Sql.dataContext.``public.messageEntity``) =
  let textContent = row.TextContent
  let attachment = row.Attachment
  let content =
    match (textContent, attachment) with
      | (Some t, Some a) -> TextWithAttachment (t, a)
      | (Some t, None) -> Text t
      | (None, Some a) -> Attachment a
      | _ -> Text ""

  { datetime = row.Datetime; author = row.Author; content = content }


let findAll = Store.ctx.Public.Message |> Seq.map _toMessage
