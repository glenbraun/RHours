module RHours.Commands

open System
open System.IO
open RHours.Data

type CommandDefinition =
    {
        CommandText : string;
        HelpText: string;
        Execute: (string list -> unit)
    }

type CommandState =
    {
        mutable Name: string;
        mutable Defs: CommandDefinition list;
        mutable Other: obj;
    }

let internal SplitLine (line:string) =
    Array.toList (line.Split(' '))

let rec internal ParseLine (state: CommandState) (parts: string list) =
    match parts with
    | [ ] -> 
        ReadCommand state
    | h :: t -> 
        match ((state.Defs) |> List.tryFind (fun c -> c.CommandText = h)) with
        | Some(cmd) -> 
            (cmd.Execute) t
        | None ->
            printfn "Expected something else"
            ReadCommand state

and ReadCommand (state: CommandState) =
    printfn "%s Menu" (state.Name)
    printf "    "

    let length = (state.Defs) |> List.length
    (state.Defs) |> List.iteri (fun i cmd -> printf "%s%s" (cmd.CommandText) (if i < length - 1 then ", " else ""))
    printfn ""
    printf "> "

    let line = Console.ReadLine()
    let parts = SplitLine line    
    ParseLine state parts
    
let internal data = 
    {
        Config = 
            { 
                PublicFolder = new DirectoryInfo("..\\..\\..\\..\\..\\");
                PrivateFolder = new DirectoryInfo("..\\..\\..\\..\\..\\..\\RHours_private");
            };
        Projects = [];
        Contributors = [];
        ContributionSpans = [];
    }

let internal state = 
    {
        Name = "Main";
        Defs = [];
        Other = null;
    }

let rec internal Project (parts: string list) =
    state.Name <- "Project"
    state.Defs <- 
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
            {
                CommandText = "back";
                HelpText = "Moves back to the main menu";
                Execute = MainMenu;
            };
            {
                CommandText = "exit";
                HelpText = "Exit command";
                Execute = Exit;
            };
        ]
    
    ParseLine state parts

and ProjectAdd (parts: string list) =
    printfn "Project Add %A" parts
    match parts with
    | [ id; name] ->
        match (data.AddProject(id, name)) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id and name"

    ReadCommand state

and ProjectDelete (parts: string list) =
    printfn "Project Delete %A" parts

    match parts with
    | [ id ] ->
        match data.DeleteProject(id) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id"

    ReadCommand state

and ProjectList (parts: string list) =
    printfn "Projects: %A" data.Projects
    ReadCommand state
    
and Contributor (parts: string list) =
    state.Name <- "Contributor"
    state.Defs <- 
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
            {
                CommandText = "back";
                HelpText = "Moves back to the main menu";
                Execute = MainMenu;
            };
            {
                CommandText = "exit";
                HelpText = "Exit command";
                Execute = Exit;
            };
        ]

    ParseLine state parts

and ContributorAdd (parts: string list) =
    printfn "Contributor Add %A" parts
    match parts with
    | [ name; ] ->
        match (data.AddContributor(name)) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected name"

    ReadCommand state

and ContributorDelete (parts: string list) =
    printfn "Contributor Delete %A" parts

    match parts with
    | [ id ] ->
        match data.DeleteContributor(id) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id"

    ReadCommand state

and ContributorList (parts: string list) =
    printfn "Contributors: %A" data.Contributors
    ReadCommand state

and ContributionSpan (parts: string list) =
    state.Name <- "Contribution Span"
    state.Defs <- 
        [
            {
                CommandText = "add";
                HelpText = "Add a contribution span";
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
            {
                CommandText = "back";
                HelpText = "Moves back to the main menu";
                Execute = MainMenu;
            };
            {
                CommandText = "exit";
                HelpText = "Exit command";
                Execute = Exit;
            };
        ]

    ParseLine state parts

and Exit (parts: string list) =
    printfn "%A" data
    printfn "Exit"

and MainMenu (parts: string list) =
    let subcommands = 
        [
            {
                CommandText = "span";
                HelpText = "Contribution Span command";
                Execute = ContributionSpan;
            };
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

    state.Name <- "Main"
    state.Defs <- subcommands
        
    ReadCommand state

let RunRHoursMenu() =
    MainMenu []
