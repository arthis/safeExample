namespace Server.Tests

open System
open Expecto
open FSharp.Control.Tasks.V2

module  VersionRange =

    open Livingstone.DependencyGraph


    let parseRange text = DependenciesFileParser.parseVersionRequirement(text).Range

    let tests =
        testList "version range rules" [
            test "can detect minimum version" {
                Expect.equal <| parseRange ">= 2.2" <| (VersionRange.AtLeast "2.2") <| ">= 2.2"
                Expect.equal <| parseRange ">= 1.2" <| (VersionRange.AtLeast "1.2") <| ">= 1.2"
            }

            test "can detect specific version" {
                Expect.equal <| parseRange "2.2" <| (VersionRange.Exactly "2.2") <| "2.2"
                Expect.equal <| parseRange "1.2" <| (VersionRange.Exactly "1.2") <| "1.2"

                Expect.equal <| parseRange "= 2.2" <| (VersionRange.Exactly "2.2") <| "= 2.2"
                Expect.equal <| parseRange "= 1.2" <| (VersionRange.Exactly "1.2") <| "= 1.2"
            }


            test "can detect ordinary Between" {
                Expect.equal <| parseRange "~> 2.2" <| (VersionRange.Between("2.2","3.0")) <| "~> 2.2"
                Expect.equal <| parseRange "~> 1.2" <| (VersionRange.Between("1.2","2.0")) <| "~> 1.2"

                Expect.isTrue <| (VersionRequirement(parseRange "~> 1.0",PreReleaseStatus.All)).IsInRange(SemVer.Parse("1.0.071.9556")) <|  "parsing 1.0.071.9556"
                Expect.isTrue <| (VersionRequirement(parseRange "~> 1.0",PreReleaseStatus.All)).IsInRange(SemVer.Parse("1.0.071.9556")) <|  "parsing 1.0.071.9432"

            }

            test "can detect lower versions for ~>" {
                Expect.equal <| parseRange "~> 3.2.0.0" <| (VersionRange.Between("3.2.0.0","3.2.1.0")) <| "~> 3.2.0.0"

                Expect.equal <| parseRange "~> 1.2.3.4" <| (VersionRange.Between("1.2.3.4","1.2.4.0")) <| "~> 1.2.3.4"
                Expect.equal <| parseRange "~> 1.2.3" <| (VersionRange.Between("1.2.3","1.3.0")) <| "~> 1.2.3"
                Expect.equal <| parseRange "~> 1.2" <| (VersionRange.Between("1.2","2.0")) <| "~> 1.2"
                Expect.equal <| parseRange "~> 1.0" <| (VersionRange.Between("1.0","2.0")) <| "~> 1.0"
                Expect.equal <| parseRange "~> 1" <| (VersionRange.Between("1","2")) <| "~> 1"
            }

            test "can detect greater-than" {
                Expect.equal <| parseRange "> 3.2" <| (VersionRange.GreaterThan(SemVer.Parse "3.2")) <| "> 3.2"
            }

            test "can detect less-than" {
                Expect.equal <| parseRange "< 3.1" <| (VersionRange.LessThan(SemVer.Parse "3.1")) <| "< 3.1"
            }

            test "can detect less-than-or-equal" {
                Expect.equal <| parseRange "<= 3.1" <| (VersionRange.Maximum(SemVer.Parse "3.1")) <| "<= 3.1"
            }

            test "can detect range" {
                Expect.equal <| parseRange ">= 1.2.3 < 1.5" <| (VersionRange.Range(VersionRangeBound.Including,SemVer.Parse "1.2.3",SemVer.Parse("1.5"), VersionRangeBound.Excluding)) <| ">= 1.2.3 < 1.5"
                Expect.equal <| parseRange "> 1.2.3 < 1.5" <| (VersionRange.Range(VersionRangeBound.Excluding,SemVer.Parse "1.2.3",SemVer.Parse("1.5"), VersionRangeBound.Excluding)) <| "> 1.2.3 < 1.5"
                Expect.equal <| parseRange "> 1.2.3 <= 2.5" <| (VersionRange.Range(VersionRangeBound.Excluding,SemVer.Parse "1.2.3",SemVer.Parse("2.5"), VersionRangeBound.Including)) <| "> 1.2.3 <= 2.5"
                Expect.equal <| parseRange ">= 1.2 <= 2.5" <| (VersionRange.Range(VersionRangeBound.Including,SemVer.Parse "1.2",SemVer.Parse("2.5"), VersionRangeBound.Including)) <| ">= 1.2 <= 2.5"
                Expect.equal <| parseRange "~> 1.2 >= 1.2.3" <| (VersionRange.Range(VersionRangeBound.Including,SemVer.Parse "1.2.3",SemVer.Parse("2.0"), VersionRangeBound.Excluding)) <| "~> 1.2 >= 1.2.3"
                Expect.equal <| parseRange "~> 1.2 > 1.2.3" <| (VersionRange.Range(VersionRangeBound.Excluding,SemVer.Parse "1.2.3",SemVer.Parse("2.0"), VersionRangeBound.Excluding)) <| "~> 1.2 > 1.2.3"
            }

            test "can detect minimum empty version" {
                Expect.equal <| parseRange "" <| parseRange ">= 0" <| "string.empty"
                Expect.equal <| parseRange null <| parseRange ">= 0" <| "null"
            }

            test "can detect prereleases" {
                Expect.equal
                <| DependenciesFileParser.parseVersionRequirement "<= 3.1"
                <| (VersionRequirement(VersionRange.Maximum(SemVer.Parse "3.1"),PreReleaseStatus.No))
                <| "<= 3.1"

                Expect.equal
                <| DependenciesFileParser.parseVersionRequirement "<= 3.1 prerelease"
                <| (VersionRequirement(VersionRange.Maximum(SemVer.Parse "3.1"),PreReleaseStatus.All))
                <| "<= 3.1 prerelease"

                Expect.equal
                <| DependenciesFileParser.parseVersionRequirement "> 3.1 alpha beta"
                <| (VersionRequirement(VersionRange.GreaterThan(SemVer.Parse "3.1"),(PreReleaseStatus.Concrete ["alpha"; "beta"])))
                <| "> 3.1 alpha beta"

            }

            test "can detect override operator" {
                Expect.equal  <| parseRange "== 3.2.0.0" <| (VersionRange.OverrideAll(SemVer.Parse "3.2.0.0")) <| "== 3.2.0.0"
            }

            test "can detect override operator for beta" {
                Expect.equal  <| parseRange "== 0.0.5-beta" <| (VersionRange.OverrideAll(SemVer.Parse "0.0.5-beta")) <| "== 0.0.5-beta"
            }
        ]