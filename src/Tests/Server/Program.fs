module ServerTests

open System
open Expecto
open FSharp.Control.Tasks.V2

open Livingstone.DataProvider

let dataProvider = DataProvider.create DataProvider.applicationSample DataProvider.productSample


let allTests =
  [
    Server.Tests.DependencyGraph.tests
    Server.Tests.VersionRange.tests
  ]
  |> testList "All tests"

[<EntryPoint>]
let main args =
  runTestsWithArgs defaultConfig args allTests