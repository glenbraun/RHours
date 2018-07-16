﻿module Json

open System
open System.IO

type JsonPair = {
        Label : string;
        mutable Value : obj
    }

type JsonObject = JsonPair list

type JsonArray = obj array

let GetMemberValue<'T> (json:JsonObject) (label:string) =
    let {Value = value} = json |> List.find ( fun {Label=l} -> l = label)
    value :?> 'T

let TryGetMemberValue<'T> (json:JsonObject) (label:string) = 
    match json |> List.tryFind ( fun {Label=l} -> l = label) with
    | Some({Value = value}) -> Some(value :?> 'T)
    | None -> None

let SetMemberValue (json:JsonObject) (label:string) (value:obj) =
    match json |> List.tryFind ( fun {Label=l} -> l = label) with
    | Some(m) -> 
        m.Value <- value        
    | None -> 
        failwith "Member not found"

type JsonWriter (tw:TextWriter, depthOpt:int option) =
    let mutable depth = 0

    let IncDepth() = depth <- depth + 1
    let DecDepth() = depth <- depth - 1
    
    new (tw:TextWriter) = JsonWriter(tw, None)

    member this.WriteNull() =
        tw.Write("null")

    member this.WriteBool(value:bool) =
        tw.Write(if value then "true" else "false")

    member this.WriteInt(value:int) =
        tw.Write(value)

    member this.WriteFloat(value:float) = 
        tw.Write(value)

    member this.WriteString(s:string) =
        // todo: better json encoding of string
        tw.Write("\"")
        tw.Write(s.Replace("\"", "\\\""))
        tw.Write("\"")

    member this.Write(json:JsonObject) =
        match depthOpt with
        | Some(maxdepth) when maxdepth <= depth ->
            tw.Write("{ ... }")
        | _ -> 
            IncDepth()
            tw.Write("{")
            let length = json.Length
            json |>
                List.iteri (fun i m -> 
                                this.Write(m)
                                if i < length - 1 then 
                                    tw.Write(",")
                           )
            tw.Write("}")
            DecDepth()
        
    member this.Write({Label=label; Value=value;}:JsonPair) =
        this.WriteString(label)
        tw.Write(":")
        this.Write(value)
    
    member this.Write(json:JsonArray) =
        match depthOpt with
        | Some(maxdepth) when maxdepth <= depth ->
            tw.Write("[ ... ]")
        | _ -> 
            IncDepth()
            tw.Write("[")
            match json.Length with
            | 0 -> ()
            | length ->
                for i = 0 to length - 2 do
                    this.Write(json.[i])
                    tw.Write(",")
                this.Write(json.[length-1])
            tw.Write("]")
            DecDepth()

    member this.Write(data:obj) =
        match data with
        | null -> this.WriteNull()
        | :? string as json -> this.WriteString(json)
        | :? int as json -> this.WriteInt(json)
        | :? float as json -> this.WriteFloat(json)
        | :? bool as json -> this.WriteBool(json)
        | :? JsonObject as json -> this.Write(json)
        | :? JsonArray as json -> this.Write(json)
        | _ -> failwith "unexpected json data type"

let WriteJson (tw:TextWriter) (json:obj) =
    let ptw = JsonWriter(tw)
    ptw.Write(json)

let WriteJsonToString (json:obj) =
    use sw = new StringWriter()
    let ptw = JsonWriter(sw)
    ptw.Write(json)
    sw.Flush()
    sw.ToString()
    
