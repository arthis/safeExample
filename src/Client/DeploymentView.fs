module DeploymentView


open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Thoth.Json

open Shared


open Fulma
open System.Collections.Generic
open ClientHelpers


type DeploymentTypeView =
    | DeployContainerView
    | DeployProductView
    | DeployAddressingView
    | DeployFullNamespaceView


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { 
 CurrentView : DeploymentTypeView option
}

type Msg =
| ShowDeployContainer
| ShowDeployProduct
| ShowDeployAddressing
| ShowDeployFullNamespace

let init () : Model =
    let initialModel = { 
        CurrentView = None
    }
    initialModel


let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> = 
    match msg with 
    | ShowDeployContainer ->
        let nextModel = { 
            currentModel with CurrentView = Some DeployContainerView 
        }
        nextModel, Cmd.none
    | ShowDeployProduct ->
        let nextModel = { 
            currentModel with CurrentView = Some DeployProductView
        }
        nextModel, Cmd.none    
    | ShowDeployAddressing ->
        let nextModel = { 
            currentModel with CurrentView = Some DeployAddressingView
        }
        nextModel, Cmd.none        
    | ShowDeployFullNamespace ->
        let nextModel = { 
            currentModel with CurrentView = Some DeployFullNamespaceView
        }
        nextModel, Cmd.none            
    | _ -> currentModel, Cmd.none


let view (model : Model) (dispatch : Msg -> unit) =
    [
        Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
            [ 
                Heading.h3 [] [ str ("Deployments: ") ] 
            ] 

        Columns.columns []
            [ 
                Column.column [] [ button "deploy container" (fun _ -> dispatch ShowDeployContainer) ]
                Column.column [] [ button "deploy product" (fun _ -> dispatch ShowDeployProduct) ]
                Column.column [] [ button "deploy Addressing" (fun _ -> dispatch ShowDeployAddressing) ]
                Column.column [] [ button "deploy FullNamespace" (fun _ -> dispatch ShowDeployFullNamespace) ]
            ] 
    ]