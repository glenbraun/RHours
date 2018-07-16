module RHours.Data

open System

type Project =
    {
        Id: string;
        Name: string;
    }

type RHoursData =
    {
        mutable Projects : Project list;
    } with

    member this.AddProject (id: string, name: string) : string option =
        match (this.Projects) |> List.exists (fun p -> p.Id = id) with
        | false -> 
            this.Projects <- { Id = id; Name = name} :: (this.Projects)
            None
        | true ->
            Some(sprintf "A project with the id '%s' already exists." id)

    member this.DeleteProject (id: string) : string option =
        match (this.Projects) |> List.exists (fun p -> p.Id = id) with
        | true -> 
            this.Projects <- (this.Projects) |> List.filter (fun p -> p.Id <> id)
            None
        | false ->
            Some(sprintf "No project with id '%s' exists." id)
     
