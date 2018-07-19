﻿module JsonSerialization

open Json
open System
open System.Collections
open System.Reflection
open Microsoft.FSharp.Reflection

type private ForReflection =
    class end

let rec SerializeEnumerable (data: obj) (callback: (obj -> obj -> obj option)) : obj =
    let enumerable = data :?> IEnumerable

    let seqEnumerable = 
        seq {
            for x in enumerable do
                yield (Serialize x callback)
        }

    (seqEnumerable |> Seq.toArray<obj>) :> obj

and SerializeRecord (data: obj) (callback: (obj -> obj -> obj option)) : obj =
    let dataProperties = data.GetType().GetProperties()
    let fieldList = (FSharpValue.GetRecordFields(data)) |> Array.toList

    let jsonList = 
        fieldList |> List.mapi
            (
                fun i x -> 
                    {
                        JsonPair.Label = (dataProperties.[i]).Name;
                        JsonPair.Value = (Serialize x callback);
                    }
            )
    jsonList :> obj

and SerializeUnion (data: obj) (callback: (obj -> obj -> obj option)) : obj =
    let dataType = data.GetType()
    let (info, caseData) = FSharpValue.GetUnionFields(data, dataType)

    let typePair = 
        {
            JsonPair.Label = "Type";
            JsonPair.Value = info.Name
        }

    let caseJson = SerializeEnumerable caseData callback
    let itemsPair = 
        {
            JsonPair.Label = "Items";
            JsonPair.Value = caseJson;
        }

    ([ typePair; itemsPair; ]) :> obj

and Serialize (data: obj) (callback: (obj -> obj -> obj option)) : obj =
    match callback data callback with
    | Some(x) -> x
    | None -> 
        match data with
        | :? string
        | :? int
        | :? float
        | :? bool -> data
        | :? DateTime as x -> (x.ToString("o")) :> obj
        | :? Decimal as x -> float(x) :> obj
        | _ -> 
            let dataType = data.GetType()
            if FSharpType.IsRecord(dataType) then
                SerializeRecord data callback
            else
                if ((typeof<System.Collections.IEnumerable>).IsAssignableFrom(dataType)) then
                    SerializeEnumerable data callback
                else
                    if FSharpType.IsUnion(dataType) then
                        SerializeUnion data callback
                    else
                        if dataType.IsPrimitive then
                            (data.ToString()) :> obj
                        else
                            ("Unable to serialize data" :> obj)

let private IsList (typeOfT: Type) =
    if typeOfT.IsGenericType then
        let genericTypeOfT = typeOfT.GetGenericTypeDefinition()
        let genericTypeOfList = (typeof<obj list>).GetGenericTypeDefinition()
        if (genericTypeOfT = genericTypeOfList) && (typeOfT.GenericTypeArguments.Length = 1) then
            Some(typeOfT.GenericTypeArguments.[0])
        else
            None
    else
        None

let MakeList<'T> (values: obj array) : 'T list = 
    let typedSequence =
        seq {
            for v in values do
                yield (v :?> 'T)
        }
    typedSequence |> Seq.toList<'T>

let private GetMethod (t: Type) (name: string) =
    let typeOfForReflection = typeof<ForReflection>
    let moduleType = typeOfForReflection.DeclaringType
    let method = moduleType.GetMethod(name);
    method.MakeGenericMethod(t)

let rec Deserialize<'T> (json: obj) : 'T =
    let typeOfT = typeof<'T>   

    match typeOfT with
    | t when t = typeof<string> -> json :?> 'T
    | t when t = typeof<int> -> json :?> 'T
    | t when t = typeof<float> -> json :?> 'T
    | t when t = typeof<bool> -> json :?> 'T
    | t when t = typeof<DateTime> -> (DateTime.Parse(json :?> string) :> obj) :?> 'T
    | t when t = typeof<Decimal> -> (Decimal(json :?> float) :> obj) :?> 'T
    | _ -> 
        if FSharpType.IsRecord(typeOfT) then
            DeserializeRecord<'T> (json :?> JsonObject)
        else
            match IsList(typeOfT) with
            | Some(listType) -> 
                    DeserializeList (json :?> JsonArray) listType
            | None -> 
                if FSharpType.IsUnion(typeOfT) then
                    DeserializeUnion<'T> (json :?> JsonObject)
                else 
                    failwith "Can't deserialize"

and DeserializeUnion<'T> (jsonObj: JsonObject) : 'T =
    match jsonObj with
    | [ { Label= "Type"; Value= unionTypeName; };
        { Label= "Items"; Value= itemsJsonValue; };
      ] -> 
        // 'T is a union
        let typeOfT = typeof<'T>
        let unionCases = FSharpType.GetUnionCases(typeOfT)
        let thisCase = unionCases |> Array.find (fun uc -> uc.Name = (unionTypeName :?> string))
        let fields = thisCase.GetFields()
        let itemsJsonArray = itemsJsonValue :?> JsonArray

        let DeserializeField (json: obj) (field: PropertyInfo) : obj =
            let desMethod = GetMethod (field.PropertyType) "Deserialize"
            desMethod.Invoke(null, [| json |])
            
        let args = Array.map2 DeserializeField itemsJsonArray fields

        let unionObj = FSharpValue.MakeUnion(thisCase, args)
        unionObj :?> 'T

    | _ -> failwith "Bad union format"

and DeserializeList<'T> (jsonArray : JsonArray) (listType: Type) : 'T =    
    let desMethod = GetMethod listType "Deserialize"
    let makeListMethod = GetMethod listType "MakeList"
   
    let desArray = jsonArray |> Array.map (fun x -> desMethod.Invoke(null, [| x |] ))
    let desList = makeListMethod.Invoke(null, [| desArray |])
    (desList :?> 'T)

and DeserializeRecord<'T> (jsonObj : JsonObject) : 'T =
    let typeOfT = typeof<'T>
    let recordFields = FSharpType.GetRecordFields(typeOfT)  // PropertyInfo

    let DeserializeProperty (p:PropertyInfo) (jsonPair: JsonPair) : obj =  // returns deserialized object as obj
        let desMethod = GetMethod (p.PropertyType) "Deserialize"

        if p.Name = jsonPair.Label then
            desMethod.Invoke(null, [| jsonPair.Value |])
        else
            failwith "Bad order of data."

    let recordData = (Seq.map2 DeserializeProperty recordFields jsonObj) |> Seq.toArray
    let recordObj = FSharpValue.MakeRecord(typeOfT, recordData)
    (recordObj :?> 'T)

