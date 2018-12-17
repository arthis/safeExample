namespace Shared

open System
open System.Reflection.Emit

type Counter = { Value : int }

[<AutoOpen>]
module ContainerVersion =

    [<CLIMutable>]
    type ContainerVersion =
        {
            major: int
            minor:int
            patch:int
            label: string option
        }

    let Initial = { major=0; minor=0; patch=0; label =None}

    let Semver cv= 
        match cv.label with 
        | Some(l) -> sprintf "%i.%i.%i.%s" cv.major cv.minor cv.patch l
        | None -> sprintf "%i.%i.%i" cv.major cv.minor cv.patch 

[<AutoOpen>]
module Container =

    [<CLIMutable>]
    type Container =
        {
            id: Guid
            name: string
            version: ContainerVersion
        }

    
    let Initial = {  id=Guid.NewGuid(); name= "";version = ContainerVersion.Initial }


// open Container

// module ContainerList =

//     [<CLIMutable>]
//     type ContainerList =
//         {
//             containers : Container[]
//         }

//     let create c =
//         {
//             containers =c
//         }

type Product =
    {
        id: Guid
        version: int
    }