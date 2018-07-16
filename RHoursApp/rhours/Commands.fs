module RHours.Commands

open System
open RHours.Data

type CommandDefinition =
    {
        CommandText : string;
        HelpText: string;
        Execute: (string list -> unit)
    }

let internal SplitLine (line:string) =
    Array.toList (line.Split(' '))

let rec internal ParseLine (defs: CommandDefinition list) (parts: string list) =
    match parts with
    | [ ] -> 
        ReadCommand defs
    | h :: t -> 
        match (defs |> List.tryFind (fun c -> c.CommandText = h)) with
        | Some(cmd) -> 
            (cmd.Execute) t
        | None ->
            printfn "Expected something else"
            ReadCommand defs

and ReadCommand (defs: CommandDefinition list) =
    let line = Console.ReadLine()
    let parts = SplitLine line    
    ParseLine defs parts
    
let ReadCommands (defs: CommandDefinition list) =
    ReadCommand defs




let internal data = 
    {
        Projects = [];
        Contributors = [];
        ContributionSpans = [];
    }

let rec ProjectAdd (parts: string list) =
    printfn "Project Add %A" parts
    match parts with
    | [ id; name] ->
        match (data.AddProject(id, name)) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id and name"

    ReadCommands RHoursCommands

and ProjectDelete (parts: string list) =
    printfn "Project Delete %A" parts

    match parts with
    | [ id ] ->
        match data.DeleteProject(id) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id"

    ReadCommands RHoursCommands

and ProjectList (parts: string list) =
    printfn "Projects: %A" data.Projects
    ReadCommands RHoursCommands

and Project (parts: string list) =
    let subcommands = 
        [
            {
                CommandText = "add";
                HelpText = "Add a project";
                Execute = ProjectAdd;
            };
            {
                CommandText = "delete";
                HelpText = "Delete a project";
                Execute = ProjectDelete;
            };
            {
                CommandText = "list";
                HelpText = "Shows the project list";
                Execute = ProjectList;
            };
        ]
    ParseLine subcommands parts
    
and ContributorAdd (parts: string list) =
    printfn "Contributor Add %A" parts
    match parts with
    | [ id; name] ->
        match (data.AddContributor(id, name)) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id and name"

    ReadCommands RHoursCommands

and ContributorDelete (parts: string list) =
    printfn "Contributor Delete %A" parts

    match parts with
    | [ id ] ->
        match data.DeleteContributor(id) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id"

    ReadCommands RHoursCommands

and ContributorList (parts: string list) =
    printfn "Contributors: %A" data.Contributors
    ReadCommands RHoursCommands

and Contributor (parts: string list) =
    let subcommands = 
        [
            {
                CommandText = "add";
                HelpText = "Add a contributor";
                Execute = ContributorAdd;
            };
            {
                CommandText = "delete";
                HelpText = "Delete a contributor";
                Execute = ContributorDelete;
            };
            {
                CommandText = "list";
                HelpText = "Shows the contributor list";
                Execute = ContributorList;
            };
        ]
    ParseLine subcommands parts

and Exit (parts: string list) =
    printfn "%A" data

    printfn "Exit"

and RHoursCommands = 
    [
        {
            CommandText = "contributor";
            HelpText = "Contributor command";
            Execute = Contributor;
        };
        {
            CommandText = "project";
            HelpText = "Project command";
            Execute = Project;
        };
        {
            CommandText = "exit";
            HelpText = "Exit command";
            Execute = Exit;
        };
    ]

