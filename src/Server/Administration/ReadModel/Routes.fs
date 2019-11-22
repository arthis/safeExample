module Livingstone.Administration.Readmodel.Routes


open System.Threading.Tasks

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open FSharp.Control.Tasks.V2

open Giraffe
open Saturn
open Livingstone.Shared


open Livingstone.Deployment.Readmodel.Repos



let handleRead<'a> (f:Http.HttpContext->Task<'a option>) next ctx  =
    task { 
        let! result = f ctx
        match result with
        | Some(a)-> 
            printf "success fetch"
            return! Successful.OK a next ctx
        | None -> 
            printf "failure fetch"
            return! RequestErrors.notFound (text "oops") next ctx
    }
let webApp  = router {
    
        get "/api/deployment/getProducts" (handleRead getProducts)
        get "/api/deployment/getComponents" (handleRead getComponents)
    }
    
    
    
        