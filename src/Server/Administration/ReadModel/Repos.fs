module Livingstone.Administration.Readmodel.Repos

open System
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Livingstone.Shared


let getApplications ctx =
    let c = [|
        [|
            { Version.Initial with minor = 1 }
            { Version.Initial with minor = 2 }
            { Version.Initial with minor = 3 }
        |]
        |> ComponentReadModel.create "iv-da" 
        [|
            { Version.Initial with minor = 1 }
            { Version.Initial with minor = 2 }
        |]
        |> ComponentReadModel.create "fb-iv-bff" 
        [|
            { Version.Initial with minor = 1; patch=1 }
        |]
        |> ComponentReadModel.create "fb-iv-spa" 
        [|
            { Version.Initial with minor = 1 }
        |]
        |> ComponentReadModel.create "svc-pdfexport" 
    |]

    Task.FromResult(Some(c))



let getProducts ctx =
    
    let productAll =
        {
            id= Guid.Parse("6152cfc0-2c01-422b-9fc1-499fabddcae3")
            name= "productAll"
        }        
    let versionProduct = 
        { 
            product= productAll
            version = { Version.Initial with minor=1; patch=1}
        }

    let readModel =
        {
            versionnedProduct = versionProduct
            Components = 
            [|
                Component.create "iv-da" { Version.Initial with minor = 1 }
                Component.create "fb-iv-bff" { Version.Initial with minor = 1; patch=1 }
                Component.create "fb-iv-spa" { Version.Initial with minor = 1 }
                Component.create "svc-pdfexport" { Version.Initial with minor = 1 }
            |]
        }        

    let products = [|
        readModel
    |]

    Task.FromResult(Some(products))