namespace Livingstone.DependencyGraph

module DependenciesFileParser =

    open System.Numerics
    open System
    open System.IO
    open Livingstone.Utils

    let twiddle (minimum:SemVerInfo) =
        let inline isNumeric (item: string) =
            match BigInteger.TryParse item with
            | true, number -> Some(number)
            | false, _ -> None

        let mutable fragments =
            ((minimum.AsString.Split '-').[0].Split '.')
            |> Array.map isNumeric
            |> Array.takeWhile (fun i -> i.IsSome)
            |> Array.choose id

        let proIndex = Math.Max(fragments.Length - 2, 0)
        fragments.[proIndex] <- fragments.[proIndex] + BigInteger.One

        let promoted = fragments |> Array.take (proIndex + 1)
        String.Join(".", promoted |> Array.map (fun i -> i.ToString()))


    let parseVersionRequirement (text : string) : VersionRequirement =
        try
            let inline parsePrerelease (versions:SemVerInfo list) (texts : string list) =
                let items = texts |> List.filter ((<>) "") |> List.distinct
                match items with
                | [] ->
                    versions
                    |> List.collect (function { PreRelease = Some x } -> [x.Name] | _ -> [])
                    |> List.distinct
                    |> function [] -> PreReleaseStatus.No | xs -> PreReleaseStatus.Concrete xs
                | [x] when String.equalsIgnoreCase x "prerelease" -> PreReleaseStatus.All
                | _ -> PreReleaseStatus.Concrete items

            if String.IsNullOrWhiteSpace text then VersionRequirement(VersionRange.AtLeast "0",PreReleaseStatus.No) else

            match text.Split([|' '|],StringSplitOptions.RemoveEmptyEntries) |> Array.toList with
            |  ">=" :: v1 :: "<" :: v2 :: rest ->
                let v1 = SemVer.Parse v1
                let v2 = SemVer.Parse v2
                VersionRequirement(VersionRange.Range(VersionRangeBound.Including,v1,v2,VersionRangeBound.Excluding),parsePrerelease [v1; v2] rest)
            |  ">=" :: v1 :: "<=" :: v2 :: rest ->
                let v1 = SemVer.Parse v1
                let v2 = SemVer.Parse v2
                VersionRequirement(VersionRange.Range(VersionRangeBound.Including,v1,v2,VersionRangeBound.Including),parsePrerelease [v1; v2] rest)
            |  "~>" :: v1 :: ">=" :: v2 :: rest ->
                let v1 = SemVer.Parse(twiddle (SemVer.Parse v1))
                let v2 = SemVer.Parse v2
                VersionRequirement(VersionRange.Range(VersionRangeBound.Including,v2,v1,VersionRangeBound.Excluding),parsePrerelease [v1; v2] rest)
            |  "~>" :: v1 :: ">" :: v2 :: rest ->
                let v1 = SemVer.Parse(twiddle (SemVer.Parse v1))
                let v2 = SemVer.Parse v2
                VersionRequirement(VersionRange.Range(VersionRangeBound.Excluding,v2,v1,VersionRangeBound.Excluding),parsePrerelease [v1; v2] rest)
            |  "~>" :: v1 :: "<=" :: v2 :: rest ->
                let v1 = SemVer.Parse v1
                let v2 = List.min [SemVer.Parse (twiddle v1); SemVer.Parse v2]
                VersionRequirement(VersionRange.Range(VersionRangeBound.Including,v1,v2,VersionRangeBound.Including),parsePrerelease [v1; v2] rest)
            |  "~>" :: v1 :: "<" :: v2 :: rest ->
                let v1 = SemVer.Parse v1
                let v2 = List.min [SemVer.Parse (twiddle v1); SemVer.Parse v2]
                VersionRequirement(VersionRange.Range(VersionRangeBound.Including,v1,v2,VersionRangeBound.Excluding),parsePrerelease [v1; v2] rest)
            |  ">" :: v1 :: "<" :: v2 :: rest ->
                let v1 = SemVer.Parse v1
                let v2 = SemVer.Parse v2
                VersionRequirement(VersionRange.Range(VersionRangeBound.Excluding,v1,v2,VersionRangeBound.Excluding),parsePrerelease [v1; v2] rest)
            |  ">" :: v1 :: "<=" :: v2 :: rest ->
                let v1 = SemVer.Parse v1
                let v2 = SemVer.Parse v2
                VersionRequirement(VersionRange.Range(VersionRangeBound.Excluding,v1,v2,VersionRangeBound.Including),parsePrerelease [v1; v2] rest)
            | _ ->
                let splitVersion (text:string) =
                    match VersionRange.BasicOperators |> List.tryFind(text.StartsWith) with
                    | Some token -> token, text.Replace(token + " ", "").Split ' ' |> Array.toList
                    | None -> "=", text.Split ' ' |> Array.toList

                match splitVersion text with
                | "==", version :: rest ->
                    let v = SemVer.Parse version
                    VersionRequirement(VersionRange.OverrideAll v,parsePrerelease [v] rest)
                | ">=", version :: rest ->
                    let v = SemVer.Parse version
                    VersionRequirement(VersionRange.Minimum v,parsePrerelease [v] rest)
                | ">", version :: rest ->
                    let v = SemVer.Parse version
                    VersionRequirement(VersionRange.GreaterThan v,parsePrerelease [v] rest)
                | "<", version :: rest ->
                    let v = SemVer.Parse version
                    VersionRequirement(VersionRange.LessThan v,parsePrerelease [v] rest)
                | "<=", version :: rest ->
                    let v = SemVer.Parse version
                    VersionRequirement(VersionRange.Maximum v,parsePrerelease [v] rest)
                | "~>", minimum :: rest ->
                    let v1 = SemVer.Parse minimum
                    VersionRequirement(VersionRange.Between(minimum,twiddle v1),parsePrerelease [v1] rest)
                | _, version :: rest ->
                    let v = SemVer.Parse version
                    VersionRequirement(VersionRange.Specific v,parsePrerelease [v] rest)
                | _ -> failwithf "could not parse version range \"%s\"" text
        with
        | _ -> failwithf "could not parse version range \"%s\"" text