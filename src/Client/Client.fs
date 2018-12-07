module Client

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

type PortalView =
| DeploymentView of DeploymentView.Model
| ProductAdministrationView



// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { 
    CurrentPortal : PortalView option
}

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| ShowDeployment
| ShowAdministration
| DeployContainerView of DeploymentView.Msg
| DeployProductView of DeploymentView.Msg
| DeployAddressingView of DeploymentView.Msg
| DeployFullNamespaceView of DeploymentView.Msg

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { 
        CurrentPortal = None 
    }
    initialModel, Cmd.none


// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match  msg with
    | ShowDeployment ->
        let nextModel = { 
            currentModel with CurrentPortal = Some <| DeploymentView(DeploymentView.init())
        }
        nextModel, Cmd.none
    | ShowAdministration ->
        let nextModel = { 
            currentModel with CurrentPortal = Some ProductAdministrationView
        }
        nextModel, Cmd.none
    | DeployContainerView msg' ->
        let res, cmd = 
            match currentModel.CurrentPortal with
            | Some(DeploymentView(m)) -> DeploymentView.update msg' m
            | _ -> DeploymentView.init(), Cmd.none
        { currentModel with CurrentPortal = Some(DeploymentView(res)) }, Cmd.map DeployContainerView cmd
    | DeployProductView msg' ->
        let res, cmd = 
            match currentModel.CurrentPortal with
            | Some(DeploymentView(m)) -> DeploymentView.update msg' m
            | _ -> DeploymentView.init(), Cmd.none
        { currentModel with CurrentPortal = Some(DeploymentView(res)) }, Cmd.map DeployProductView cmd    
    | DeployAddressingView msg' ->
        let res, cmd = 
            match currentModel.CurrentPortal with
            | Some(DeploymentView(m)) -> DeploymentView.update msg' m
            | _ -> DeploymentView.init(), Cmd.none
        { currentModel with CurrentPortal = Some(DeploymentView(res)) }, Cmd.map DeployAddressingView cmd        
    | DeployFullNamespaceView msg' ->
        let res, cmd = 
            match currentModel.CurrentPortal with
            | Some(DeploymentView(m)) -> DeploymentView.update msg' m
            | _ -> DeploymentView.init(), Cmd.none
        { currentModel with CurrentPortal = Some(DeploymentView(res)) }, Cmd.map DeployFullNamespaceView cmd            
    | _ -> currentModel, Cmd.none


let safeComponents =
    let components =
        span [ ]
           [
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io/elmish/" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://mangelmaxime.github.io/Fulma" ] [ str "Fulma" ]
           ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]

// let showPortal (model : Model) (dispatch : Msg -> unit) =
//     match model.CurrentPortal with
//     | Some ( DeploymentView( m)) -> 
//         DeploymentView.view m dispatch
        
//     | Some ProductAdministrationView -> []
//     | _ -> []  


let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]

        

          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
