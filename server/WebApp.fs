module Wussup.WebApp

open Wussup
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

let private webapp =
  choose [
    route "/messages" >=> json Message.findAll
  ]

let private configureApp (app: IApplicationBuilder) =
  app.UseGiraffe webapp

let private configureServices (services: IServiceCollection) =
  services.AddGiraffe() |> ignore

let start: Unit =
  WebHostBuilder()
    .UseKestrel()
    .Configure(System.Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices)
    .Build()
    .Run()
