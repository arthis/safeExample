module Livingstone.Administration.Product.View


open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

open Livingstone.Administration.Product.Types
open Livingstone.ClientHelpers
open Livingstone.Shared


let showProduct (product:ProductVersionReadModel) =
    Content.content []
        [
            str <| sprintf "%s, version %s" product.versionnedProduct.product.name product.versionnedProduct.version.SemVer
        ]


let showProducts (products: ProductVersionReadModel[]) =
    Content.content [] 
        [ 
            span [] <| (products |> Seq.map showProduct |> Seq.toArray)
        ]       

let showProductsOption products =
    match products with 
    | Some(p) -> showProducts p
    | None -> Content.content [] [ span [] [ str "no products..." ] ]       


let view (model : Model) (dispatch:AdminProductMsg->unit) =

    [
        Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Left) ] ]
            [ 
                Heading.h3 [] [ str ("  Products: ") ] 
                
            ] 
        
        showProductsOption model.Products
                
    ]
          

