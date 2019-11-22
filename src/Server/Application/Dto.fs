namespace Livingstone.Application

open System

open Livingstone.Shared
open Livingstone.Application.PublicTypes

[<AutoOpen>]
module ApplicationDto =

    //COMMANDS
    [<CLIMutable>]
    type BuildInfosDto =
        {
            ApplicationId : string
            CommitId : string
            Tag :string
            Branch : string
            Version : string
        }


    let toUnvalidatedBuildInfos (dto:BuildInfosDto) : UnvalidatedBuildInfos =
        let domainObj : UnvalidatedBuildInfos = {
            // this is a simple 1:1 copy which always succeeds
                applicationId = ApplicationId.create dto.ApplicationId
                commitId = CommitId.create dto.CommitId
                tag = Tag.createOption dto.Tag
                branch = Branch.create dto.Branch
                version = Version.create dto.Version
            }
        domainObj

