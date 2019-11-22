namespace Server.Tests

open System
open Expecto
open FSharp.Control.Tasks.V2




module  Build =


    open Livingstone.Shared
    open Livingstone.Application
    open Livingstone.Application.PublicTypes

    let tests =
      testList "onBuildSucceded" [
        // testCaseAsync  "the version" <| async {

            // let application = {

            // }

            // let applicationName = "xx-yy"
            // let cmd:BuildInfosDto =
            //   {
            //       Version = "1.2.3"
            //       HelmValues = "https://myBlobHelm"
            //       Data = "https://myBlobData"
            //       ApplicationName = applicationName
            //   }

            // let! result = Api.addBuildInformation cmd

            // match result with
            // | Ok events ->
            //   let expected = [ BuildAddedToApplication({name=applicationName; newBuild="1.2.3"}) ] |> List.toSeq
            //   Expect.sequenceEqual events expected "BuildAddedToApplication event has been produced"
            // | Error err -> ()



            // Expect.isSome result "onBuildSucceed should gives the id of the build deployed"
        //   }



        test "another test that fails" {
          Expect.equal (3+3) 6 "3+3"
        }

        testAsync "this is an async test" {
          let! x = async { return 4 }
          Expect.equal x (2+2) "2+2"
        }
      ]
