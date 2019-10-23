module Wussup.Store

open FSharp.Data.Sql

let [<Literal>] Vendor = Common.DatabaseProviderTypes.POSTGRESQL

let [<Literal>] ConnString = "Host=localhost;Port=5432;Database=wussup;Username=kiransebastian;Password="

type Sql = SqlDataProvider<Vendor, ConnString, UseOptionTypes = true>

let ctx = Sql.GetDataContext()

