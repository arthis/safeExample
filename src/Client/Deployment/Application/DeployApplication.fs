module DeployApplication

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch


open Thoth.Json

open Shared



open Fulma
open System.Collections.Generic

open System.Collections.Generic


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { 
    Containers: Container[] option
    ContainerToDeploy: Container option 
}

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type DeployApplicationMsg =
| DeployApplication
| CancelDeployAppication
| InitialContainersLoaded of Result<Container[], exn>

let initialContainers = fetchAs<Container[]> "/api/getContainers" (Decode.Auto.generateDecoder())

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<DeployApplicationMsg> =
    let initialModel = { 
        Containers = None
        ContainerToDeploy = None
    }
    let loadCountCmd =
        Cmd.ofPromise
            initialContainers
            []
            (Ok >> InitialContainersLoaded)
            (Error >> InitialContainersLoaded)
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : DeployApplicationMsg) (currentModel : Model) : Model * Cmd<DeployApplicationMsg> =
    match msg with
    | InitialContainersLoaded (Ok containers)->
        let nextModel = { 
            Containers = Some containers ;
            ContainerToDeploy = None;
        }
        nextModel, Cmd.none
    | DeployApplication ->
        currentModel, Cmd.none
    | CancelDeployAppication ->
        currentModel, Cmd.none
    | _ -> currentModel, Cmd.none

let createOption (container:Container) dispatch =
    Dropdown.Item.a [ ] [ str container.name ]

let createOptions (model : Model) dispatch =
    match model.Containers with
    | Some(containers) -> Seq.map (fun c -> createOption c dispatch) containers
    | None -> [] |> List.toSeq<Fable.Import.React.ReactElement>

let CreateDropDownList (model : Model) dispatch =
    Dropdown.dropdown [ Dropdown.IsHoverable ]
        [ div [ ]
            [ Button.button [ ]
                [ span [ ]
                    [ str "Dropdown" ]
                  Icon.icon [ Icon.Size IsSmall ]
                    [ ] ] ]
          Dropdown.menu [ ]
            [ Dropdown.content [ ]  
                (createOptions model dispatch )
            ] 
        ]    

let view (model : Model) dispatch =
    [
        Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
            [ 
                Heading.h3 [] [ str ("Deploy Application: ") ] 
            ] 

        Columns.columns []
            [ 
                CreateDropDownList model dispatch
            ] 
                
    ]
          

