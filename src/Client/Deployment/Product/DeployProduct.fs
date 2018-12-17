module DeployProduct

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch


open Thoth.Json

open Shared


open Fulma
open System.Collections.Generic


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { 
    Products: Product list option
    Product: Product option 
}

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type DeployProductMsg =
| DeployProduct
| CancelDeployProduct
| InitialProductLoaded of Result<Product list, exn>

let initialProduct = fetchAs<Product list> "/api/getProducts" (Decode.Auto.generateDecoder())

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<DeployProductMsg> =
    let initialModel = { 
        Products = None 
        Product = None
    }
    let loadCountCmd =
        Cmd.ofPromise
            initialProduct
            []
            (Ok >> InitialProductLoaded)
            (Error >> InitialProductLoaded)
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : DeployProductMsg) (currentModel : Model) : Model * Cmd<DeployProductMsg> =
    match currentModel.Products, msg with
    | _, InitialProductLoaded (Ok products)->
        let nextModel = { 
            Products = Some products ;
            Product = None;
        }
        nextModel, Cmd.none
    | _, DeployProduct ->
        currentModel, Cmd.none
    | _, CancelDeployProduct ->
        currentModel, Cmd.none
    | _ -> currentModel, Cmd.none

let basic () =
    Dropdown.dropdown [ Dropdown.IsHoverable ]
        [ div [ ]
            [ Button.button [ ]
                [ span [ ]
                    [ str "Dropdown" ]
                  Icon.icon [ Icon.Size IsSmall ]
                    [ ] ] ]
          Dropdown.menu [ ]
            [ Dropdown.content [ ]
                [ Dropdown.Item.a [ ]
                    [ str "Item n°1" ]
                  Dropdown.Item.a [ ]
                    [ str "Item n°2" ]
                  Dropdown.Item.a [ Dropdown.Item.IsActive true ]
                    [ str "Item n°3" ]
                  Dropdown.Item.a [ ]
                    [ str "Item n°4" ]
                //   Dropdown.divider [ ]
                  Dropdown.Item.a [ ]
                    [ str "Item n°5" ] ] ] ]    

let view (model : Model) dispatch =
    [
        Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
            [ 
                Heading.h3 [] [ str ("Deploy Product: ") ] 
            ] 

        Columns.columns []
            [ 
                basic()
            ] 
                
    ]
          

