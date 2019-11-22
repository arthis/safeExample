module Livingstone.Administration.Product.State

open Elmish
open Livingstone.Administration.Product.Rest
open Livingstone.Administration.Product.Types
open Livingstone.Shared

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<AdminProductMsg> =
    printf "products init"
    let initialModel = { 
        Products = None
    }
    let loadCountCmd =
        Cmd.ofPromise
            initialProducts
            []
            (Ok >> InitialProductsLoaded)
            (Error >> InitialProductsLoaded)
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : AdminProductMsg) (model : Model) : Model * Cmd<AdminProductMsg> =
    match msg with
    | InitialProductsLoaded (Ok products)->
        printf "InitialProductsLoaded"
        let nextModel = { 
            Products = Some products
        }
        nextModel, Cmd.none
    | InitialProductsLoaded (Error ex)->        
        printf "InitialProductsLoaded error %A" ex
        model, Cmd.none
    | ChooseProduct x->
        model, Cmd.none
    | AddVersionnedComponent x->
        model, Cmd.none
    | RemoveVersionnedComponent x->
        model, Cmd.none    
    | CreateProduct x->
        model, Cmd.none    

 