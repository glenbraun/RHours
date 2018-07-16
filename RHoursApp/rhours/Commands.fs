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
 
let rec ProjectAdd (parts: string list) =
    printfn "Project Add %A" parts
    ReadCommands RHoursCommands

and ProjectDelete (parts: string list) =
    printfn "Project Delete %A" parts
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
        ]
    ParseLine subcommands parts
    
and Exit (parts: string list) =
    printfn "Exit"

and RHoursCommands = 
    [
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

