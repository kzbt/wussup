module Wussup.Parser

open Wussup.Message
open Logary
open Logary.Message
open System
open System.Globalization
open System.IO
open System.Text.RegularExpressions

let dateformat = "d/M/yy, HH:mm:ss"

let logger = Log.create "Wussup.Parser"

let isEmptyOrCtrlChar (s: string) =
  let str = s.Trim()
  String.IsNullOrWhiteSpace str || (str.Length = 1 && (int (str.Chars(0))) = 8206)

/// Extracts the first matching group based on the regex
let (|FirstRegexGroup|_|) pattern input =
  let result = Regex.Match(input, pattern)
  if (result.Success) then Some result.Groups.[1].Value else None

let (|T|A|TA|) input =
  let hasAttachment = Regex.Match(input, "(.*)(<attached:(.*)>)")
  let values =
    if (hasAttachment.Success)
    then List.tail [for m in hasAttachment.Groups -> m.Value.Trim()]
    else [input]

  match values with
    | [t] -> T t
    | t :: _ :: a :: _ when isEmptyOrCtrlChar t -> A a
    | t :: _ :: a :: _ -> TA (t, a)
    | _ -> T ""

let parseItem line regex =
  match line with
    | FirstRegexGroup regex result -> Some (result.Trim())
    | _ -> None

let parseTimestamp line = parseItem line "^\[(.*, .*)\]"

let parseAuthor line = parseItem line "\](.+?):.*"

let parseMessage line = parseItem line "\][^:]*:(.*)"

let rec nextLine (reader: StreamReader, lines: string) =
  let line =
    lines + reader.ReadLine()
    |> fun l -> Regex.Replace(l, @"\p{C}", String.Empty)

  let nextChar = reader.Peek()
  match nextChar with
    | -1 | 91 | 8206 -> line  // 91 - '[', 8206 - Left to right mark
    | _ -> nextLine(reader, line + @"\n")

let inline mkDateTime timestamp =
  let ok, dt = DateTime.TryParseExact(timestamp, dateformat, null, DateTimeStyles.None)
  if ok then Some dt else None

let mkContent text =
  match text with
    | T t -> Text t
    | A a -> Attachment a
    | TA (t, a) -> TextWithAttachment (t, a)

let lineToMessage line =
  let datetime = line |> parseTimestamp |> Option.bind mkDateTime
  let author = parseAuthor line
  let content = line |> parseMessage |> Option.map mkContent

  match datetime, author, content with
    | Some dt, Some author, Some c -> Some { datetime = dt; author = author; content = c}
    | _ -> logger.info (eventX "line missing data"); None
