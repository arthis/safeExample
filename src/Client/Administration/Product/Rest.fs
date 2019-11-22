module Livingstone.Administration.Product.Rest

open System
open Fable.PowerPack.Fetch

open Thoth.Json

open Livingstone.Administration.Product.Types
open Livingstone.Shared
open Livingstone.RestHelpers

let initialProducts = fetchAs<ProductVersionReadModel[]> "/api/deployment/getProducts" (Decode.Auto.generateDecoder())

// let deployApplication = post<DeployApplicationCmd,Guid> "/api/administration/deployApplication" (Decode.Auto.generateDecoder())