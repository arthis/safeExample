module Livingstone.AdministrationSection


open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch
open Thoth.Json
open Livingstone.Shared

open Fulma
open System.Collections.Generic
open ClientHelpers
open Fable.Import

type AdministrationTypeView =
    | AdminProductView

type Model = { 
    CurrentView : AdministrationTypeView option
    AdminProductState : Administration.Product.Types.Model option
}

type AdministrationViewMsg =
| ShowAdminProduct
| AdminProduct of Administration.Product.Types.AdminProductMsg


let init () =
    let initialModel = { 
        CurrentView = None
        AdminProductState= None
    }

    initialModel,Cmd.none

let update (msg : AdministrationViewMsg) (currentModel : Model) : Model * Cmd<AdministrationViewMsg> = 
    match msg with 
    | ShowAdminProduct ->
        let state, subCmd = Administration.Product.State.init()
        let nextModel = { 
            currentModel with 
                CurrentView = Some AdminProductView
                AdminProductState = Some(state);
        }
        nextModel, Cmd.map AdminProduct subCmd    
    | AdminProduct cmd ->
        let newModel,subCmd =
            match currentModel.AdminProductState with 
            | Some (m) -> Administration.Product.State.update cmd m
            | None -> Administration.Product.State.init() 
        { currentModel with  AdminProductState= Some newModel }, Cmd.map AdminProduct subCmd
    | _ ->
        currentModel, Cmd.none    


let showView (model : Model) dispatch =
    match model.CurrentView, model.AdminProductState with
    | Some ( AdminProductView),Some(state) -> 
        Administration.Product.View.view state ( AdministrationViewMsg.AdminProduct >> dispatch)        
    | _ -> []

let view (model : Model) dispatch =
    [
        Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
            [ 
                Heading.h3 [] [ str ("Administration: ") ] 
            ] 

        Columns.columns []
            [ 
                Column.column [] [ button "products" (fun _ -> dispatch ShowAdminProduct) ]
            ] 

        Content.content [] <| showView model dispatch             



    ]