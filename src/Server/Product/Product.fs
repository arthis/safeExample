namespace Livingstone.DependencyGraph

type AppName = private | AppName of string
type ProductApplication = private | ProductApplication of AppName*SemVerInfo

type Node = private | Node of ProductApplication*ProductApplication list

module AppName =
    let value (AppName(n)) = n
    let create n = AppName(n)
    let equals a b =
        value a = value b


module ProductApplication =
    type From =
        | FromAppNameAndSemver of AppName*SemVerInfo
        | FromNameAndVersion of string*string

    let value (ProductApplication(n,s)) = (n,s)

    let create = function
        | FromAppNameAndSemver(n,s) -> ProductApplication(n,s)
        | FromNameAndVersion(n,v) ->
            let appName = AppName.create n
            let semVer = SemVer.Parse v
            ProductApplication(appName,semVer)

    let isProductApplication a b =
        let nA,sA = value a
        let nB,sB = value b
        AppName.equals nA nB

module Node =
    let value (Node(pa,pas)) = (pa,pas)
    let create  str = Node(str)


module Product =

    type State =
        {
            Name : string
            Applications : ProductApplication list
        }

    let checkDependency state name version =
        state.Applications
        |> List.exists (fun pa ->
            let (n,s)= ProductApplication.value pa
            n= name && s=version
        )

    let create name=
        {
            Name= name
            Applications= []
        }

    let private createProductApplication  n v =
        ProductApplication.create <| ProductApplication.FromNameAndVersion(n,v)

    type From =
        | FromProductApplication of ProductApplication
        | FromNameAndVersion of string*string

    let private actOnProduct f from state =
        match from with
        | FromProductApplication(pa) -> f state pa
        | FromNameAndVersion(n,v) ->
            createProductApplication n v
            |> f state

    let addApplicationVersion  =
        actOnProduct
        <| fun s p-> { s with Applications = p :: s.Applications  }

    let updateApplicationVersion =
        actOnProduct
        <| fun s pa -> { s with Applications = s.Applications |> List.map ( fun app -> if  ProductApplication.isProductApplication app pa then  pa else app) }

    let removeApplication  =
        let remove s pa=
            let newList =
                s.Applications
                |> List.map ( fun app ->  if  ProductApplication.isProductApplication app pa then  None  else Some(app))
                |> List.choose id
            { s with Applications =  newList}

        actOnProduct  remove




