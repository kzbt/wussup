#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.create "Clean" (fun _ -> !!"server/**/bin" ++ "server/**/obj" |> Shell.cleanDirs)

Target.create "Build" (fun _ -> !!"server/**/*.*proj" |> Seq.iter (DotNet.build id))

Target.create "Run" (fun _ -> DotNet.exec (DotNet.Options.withWorkingDirectory "./server") "watch run" "" |> ignore)

Target.create "All" ignore

"Clean" ==> "Build" ==> "Run" ==> "All"

Target.runOrDefault "All"
