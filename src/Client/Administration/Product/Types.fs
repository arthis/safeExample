module Livingstone.Administration.Product.Types

open System
open Livingstone.Shared

type Model = { 
    Products: ProductVersionReadModel[] option
}

type AdminProductMsg =
| ChooseProduct of ProductVersionReadModel
| AddVersionnedComponent of VersionnedComponent
| RemoveVersionnedComponent of VersionnedComponent
| CreateProduct of string
| InitialProductsLoaded of Result<ProductVersionReadModel[], exn>