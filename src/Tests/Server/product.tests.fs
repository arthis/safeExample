namespace Server.Tests

open System
open Expecto
open FSharp.Control.Tasks.V2




module  DependencyGraph =

    open Livingstone.DependencyGraph
    open Livingstone.DependencyGraph.Product

    let parseRange text = DependenciesFileParser.parseVersionRequirement(text).Range

    let tests =
      testList "product" [

        test "create product" {
            let newState = Product.create "bar"
            Expect.sequenceEqual newState.Applications [] "no applications at the creation of the product"
            Expect.equal newState.Name "bar" "name of the product is defined at the creation of the product"
        }

        test "add first product" {
            let newApplication = ProductApplication.create <| ProductApplication.FromNameAndVersion("foo" ,"1.0.0")
            let newState =
                Product.create "bar"
                |> Product.addApplicationVersion (FromProductApplication newApplication)
            Expect.sequenceEqual newState.Applications [newApplication] "no applications at the creation of the product"
        }

        test "add second product" {
            let newApplication = ProductApplication.create <| ProductApplication.FromNameAndVersion("foo" ,"1.0.0")
            let newApplication2 = ProductApplication.create <| ProductApplication.FromNameAndVersion("foobar" ,"1.54.0")
            let newState =
                Product.create "bar"
                |> Product.addApplicationVersion (FromProductApplication newApplication)
                |> Product.addApplicationVersion (FromProductApplication newApplication2)
            Expect.sequenceEqual newState.Applications [newApplication2;newApplication] "no applications at the creation of the product"
        }

      ]