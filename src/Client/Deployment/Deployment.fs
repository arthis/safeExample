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
    | DeployApplicationView
    | DeployProductView
    | DeployAddressingView
    | DeployFullNamespaceView


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { 
    CurrentView : DeploymentTypeView option
    DeploymentApplicationState : DeployApplication.Model option
}

type DeploymentViewMsg =
| ShowDeployApplication
| ShowDeployProduct
| ShowDeployAddressing
| ShowDeployFullNamespace
| DeployProduct of DeployProduct.DeployProductMsg
| DeployApplication of DeployApplication.DeployApplicationMsg

let init () : Model =
    let initialModel = { 
        CurrentView = None
        DeploymentApplicationState = None
    }
    initialModel


let update (msg : DeploymentViewMsg) (currentModel : Model) : Model * Cmd<DeploymentViewMsg> = 
    match msg with 
    | ShowDeployApplication ->
        let state, subCmd = DeployApplication.init()
        printf "ShowDeployApplication"
        let nextModel = { 
            currentModel with CurrentView = Some DeployApplicationView ;  DeploymentApplicationState=Some(state) ;
        }
        nextModel, Cmd.map DeploymentViewMsg.DeployApplication subCmd
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


let showView (model : Model) dispatch =
    match model.CurrentView, model.DeploymentApplicationState with
    | Some ( DeployApplicationView), Some(state) -> 
        DeployApplication.view state ( DeploymentViewMsg.DeployApplication >> dispatch)
    | Some ( DeployProductView),_ -> 
        let state, subCmd = DeployProduct.init()
        DeployProduct.view state ( DeploymentViewMsg.DeployProduct >> dispatch)        
    | _ -> [] 

let view (model : Model) dispatch =
    [
        Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
            [ 
                Heading.h3 [] [ str ("Deployments: ") ] 
            ] 

        Columns.columns []
            [ 
                Column.column [] [ button "deploy application" (fun _ -> dispatch ShowDeployApplication) ]
                Column.column [] [ button "deploy product" (fun _ -> dispatch ShowDeployProduct) ]
                Column.column [] [ button "deploy Addressing" (fun _ -> dispatch ShowDeployAddressing) ]
                Column.column [] [ button "deploy FullNamespace" (fun _ -> dispatch ShowDeployFullNamespace) ]
            ] 

        Content.content [] <| showView model dispatch             



    ]