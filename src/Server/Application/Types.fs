namespace Livingstone.Application

open System



module PublicTypes =

    type ApplicationName = private ApplicationName of string
    type ApplicationId = private ApplicationId of string
    type CommitId = private CommitId of string
    type Tag = private Tag of string
    type Branch = private Branch of string
    type Version = private Version of string


    module ApplicationName =
        let value (ApplicationName str) = str

        let create  str = ApplicationName(str)

    module ApplicationId =
        let value (ApplicationId str) = str

        let create  str = ApplicationId(str)

    module CommitId =

        let value (CommitId str) = str

        let create  str = CommitId(str)

    module Tag =

        let value (Tag str) = str

        let create  str = Tag(str)

        let createOption  str =
            if String.IsNullOrEmpty str then  Some(Tag(str))
            else None

    module Branch =

        let value (Branch str) = str

        let create  str = Branch(str)

    module Version =

        let value (Version str) = str
        let create  str = Version(str)


    type UnvalidatedBuildInfos =
        {
            applicationId : ApplicationId
            commitId : CommitId
            tag : Tag option
            branch : Branch
            version : Version
        }

    type ValidatedBuildInfos = {
        applicationId : ApplicationId
        commitId : CommitId
        tag : Tag option
        branch : Branch
        version : Version
    }

    type NewApplicationCreated = {
        name: string
    }

    type BuildArtifact = {
        applicationId : ApplicationId
        commitId : CommitId
        tag : Tag option
        branch : Branch
        version : Version
    }

    type BuildAddedToApplication = {
        name: string
        newBuild : string
    }

    type ApplicationEvent =
    | BuildAddedToApplication of BuildAddedToApplication

    type VersionError = {
        BadVersion : string
    }

    type ApplicationNotFoundError = {
        ApplicationNotfound : string
    }

    type ApplicationEnded = {
        endDate: DateTime
    }

    type SaveError = {
        ex : Exception
    }

    type PublishError = {
        eventsPublished : ApplicationEvent list
        eventsNotPublished : ApplicationEvent list
        ex : Exception
    }

    type ValidationError = ValidationError of string


    type ApplicationError =
    | Validation of ValidationError
    | ApplicationNotFound of ApplicationNotFoundError
    | ApplicationEnded of ApplicationEnded
    | PublishError of PublishError
    | SaveError of SaveError


    type NewApplication =
        {
            name : string
            port: int
        }

    type KnownApplication =
        {
            name : string
            port: int
            builds : BuildArtifact list
        }

    type Application =
    | NewApplication of NewApplication
    | KnownApplication of KnownApplication



    type AddBuild =
        UnvalidatedBuildInfos -> AsyncResult<Application*ApplicationEvent list, ApplicationError>


module internal InternalTypes =

    open PublicTypes


    type GetApplication = ApplicationId -> AsyncResult<KnownApplication, ApplicationNotFoundError>

    type CheckApplicationExists = string -> bool

    type PublishEvents =
        ApplicationEvent list -> AsyncResult<unit, PublishError>


    type SaveApplication =
        Application -> AsyncResult<unit, SaveError>


    type ValidateBuildInfos =
          CheckApplicationExists -> UnvalidatedBuildInfos -> AsyncResult<ValidatedBuildInfos, ValidationError>

    type AddBuildInfos =
        GetApplication -> ValidatedBuildInfos ->  AsyncResult<Application, ApplicationNotFoundError>



    type CreateEvents =
        Application                           // input
         -> ApplicationEvent list              // output
