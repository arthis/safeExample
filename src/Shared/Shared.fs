namespace Shared

open System

type Counter = { Value : int }

type Product =
    {
        id: Guid
        version: int
    }