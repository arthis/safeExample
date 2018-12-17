module Portal

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
type PortalMsg =
| ShowDeployment
| ShowAdministration
| DeployView of DeploymentView.DeploymentViewMsg

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<PortalMsg> =
    let initialModel = { 
        CurrentPortal = None 
    }
    initialModel, Cmd.none


// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : PortalMsg) (currentModel : Model) : Model * Cmd<PortalMsg> =
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
    | DeployView msg' ->
        let res, cmd = 
            match currentModel.CurrentPortal with
            | Some(DeploymentView(m)) -> DeploymentView.update msg' m
            | _ -> DeploymentView.init(), Cmd.none
        { currentModel with CurrentPortal = Some(DeploymentView(res)) }, Cmd.map DeployView cmd
    // | _ -> currentModel, Cmd.none


let safeComponents =
    [
        p [] [img [ Src "/Images/livingstone.png"; Style [ Width(100) ] ]]
        p [ ] [  strong [] [ str "Living Stone" ]]
    ]
    
        
   


let showPortal (model : Model) dispatch =
    match model.CurrentPortal with
    | Some ( DeploymentView( m)) -> 
        DeploymentView.view m ( PortalMsg.DeployView >> dispatch)
    | Some ProductAdministrationView -> []
    | _ -> []  


// let view (model : Model) dispatch =
//     div []
//         [ Navbar.navbar [ Navbar.Color IsPrimary ]
//             [ Navbar.Item.div [ ]
//                 [ Heading.h2 [ ]
//                     [ str "SAFE Template" ] ] ]
          
//           Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
//             [ 
//                 Heading.h3 [] [ str ("Portal: ") ] 
//             ] 

//           Columns.columns []
//             [ 
//                 Column.column [] [ button "deployment" (fun _ -> dispatch ShowDeployment) ]
//             ] 
          
//           Content.content [] <| showPortal model dispatch 

//           Footer.footer [ ]
//                 [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
//                     [ Content.content []safeComponents ] ] ]


let view (model : Model) dispatch =
        div [] [
            Navbar.navbar [ Navbar.Color IsPrimary ]
                [ Navbar.Item.div [ ]
                    [ Heading.h2 [ ] [ str "Living Stone" ] 
                      span [] [ str "deployment tooling" ] ]  
                ]

            Columns.columns [ ]
                [ 
                    Column.column [ Column.Option.Width(Screen.Desktop, Column.Is1) ]  [ 
                        button "deployment" (fun _ -> dispatch ShowDeployment) 
                        button "administration" (fun _ -> dispatch ShowAdministration) 
                    ]
                    Column.column [] [ 
                        Content.content [] <| showPortal model dispatch                     
                    ]
                ]

            Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ Content.content [] safeComponents ] 
                ] 
        ]

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
