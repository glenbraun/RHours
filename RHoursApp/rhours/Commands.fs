module RHours.Commands

open System
open System.IO
open RHours.Data
open JsonParser

let mutable Data = 
    {
        Version = "1.0";
        Config = 
            { 
                PublicFolder = new DirectoryInfo(Directory.GetCurrentDirectory())
                PrivateFolder = new DirectoryInfo(Directory.GetCurrentDirectory())
            };
        Projects = [];
        Contributors = [];
        CompensationAgreements = [];
        InvoiceEvents = [];
    }

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
        mutable PreviousName: string;
        mutable PreviousDefs: CommandDefinition list;
        mutable ActiveProject: Project option;
        mutable ActiveContributor: ContributorInfoPublic option;
        mutable ActiveAgreement: CompensationAgreement option;
        mutable ActiveSpan: ContributionSpan option;
    } with

    member this.SetToPrevious() =
        this.Name <- this.PreviousName
        this.Defs <- this.PreviousDefs

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

    state.PreviousName <- state.Name
    state.PreviousDefs <- state.Defs

    let line = Console.ReadLine()
    let parts = SplitLine line    
    ParseLine state parts
    //Data.Save()
    
let internal state = 
    {
        Name = "Main";
        Defs = [];
        PreviousName = "Main";
        PreviousDefs = [];
        ActiveProject = None;
        ActiveContributor = None;
        ActiveAgreement = None;
        ActiveSpan = None;
    }

let rec internal ProjectMenu (parts: string list) =
    state.Name <- "Project"
    state.Defs <- 
        [
            {
                CommandText = "select";
                HelpText = "Select a project";
                Execute = ProjectSelectMenu;
            };
            {
                CommandText = "add";
                HelpText = "Add a project";
                Execute = ProjectAddMenu;
            };
            {
                CommandText = "delete";
                HelpText = "Delete a project";
                Execute = ProjectDeleteMenu;
            };
            {
                CommandText = "list";
                HelpText = "Shows the project list";
                Execute = ProjectListMenu;
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

and ProjectSelectMenu (parts: string list) =
    printfn "Project Select %A" parts

    match parts with
    | [ id ] ->
        if Data.ProjectExists(id) then
            state.ActiveProject <- Some(Data.GetProject(id))
        else
            printfn "Project with id '%s' does not exist." id
    | _ ->
        printfn "Expected id"
    
    state.SetToPrevious()
    ReadCommand state

and ProjectAddMenu (parts: string list) =
    printfn "Project Add %A" parts
    match parts with
    | [ id; name] ->
        match (Data.AddProject(id, name)) with
        | Choice1Of2(project) -> state.ActiveProject <- Some(project)
        | Choice2Of2(err) -> printfn "%s" err
    | _ ->
        printfn "Expected id and name"

    state.SetToPrevious()
    ReadCommand state

and ProjectDeleteMenu (parts: string list) =
    printfn "Project Delete %A" parts

    match parts with
    | [ id ] ->
        match Data.DeleteProject(id) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected id"

    state.SetToPrevious()
    ReadCommand state

and ProjectListMenu (parts: string list) =
    printfn "Projects: %A" Data.Projects
    state.SetToPrevious()
    ReadCommand state
    
and ContributorMenu (parts: string list) =
    state.Name <- "Contributor"
    state.Defs <- 
        [
            {
                CommandText = "add";
                HelpText = "Add a contributor";
                Execute = ContributorAddMenu;
            };
            {
                CommandText = "select";
                HelpText = "Select a contributor";
                Execute = ContributorSelectMenu;
            };
            {
                CommandText = "hash";
                HelpText = "Update hash of private info for a contributor."
                Execute = ContributorHashMenu;
            };
            {
                CommandText = "delete";
                HelpText = "Delete a contributor";
                Execute = ContributorDeleteMenu;
            };
            {
                CommandText = "list";
                HelpText = "Shows the contributor list";
                Execute = ContributorListMenu;
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

and ContributorSelectMenu (parts: string list) =
    printfn "Contributor Select %A" parts

    match parts with
    | [ publicName ] ->
        if Data.ContributorExists(publicName) then
            state.ActiveContributor <- Some(Data.GetContributor(publicName))
        else
            printfn "Contributor with name '%s' does not exist." publicName
    | _ ->
        printfn "Expected public name"

    state.SetToPrevious()
    ReadCommand state

and ContributorAddMenu (parts: string list) =
    printfn "Contributor Add %A" parts
    match parts with
    | [ name; ] ->
        match (Data.AddContributor(name)) with
        | Choice1Of2(contributor) -> state.ActiveContributor <- Some(contributor)
        | Choice2Of2(err) -> printfn "%s" err
    | _ ->
        printfn "Expected name"

    state.SetToPrevious()
    ReadCommand state

and ContributorHashMenu (parts: string list) =
    printfn "Contributor Hash %A" parts

    match parts with
    | [ name ] ->
        match Data.HashContributor(name) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected name"

    state.SetToPrevious()
    ReadCommand state

and ContributorDeleteMenu (parts: string list) =
    printfn "Contributor Delete %A" parts

    match parts with
    | [ name ] ->
        match Data.DeleteContributor(name) with
        | Some(err) -> printfn "%s" err
        | None -> ()
    | _ ->
        printfn "Expected name"

    state.SetToPrevious()
    ReadCommand state

and ContributorListMenu (parts: string list) =
    printfn "Contributors: %A" Data.Contributors
    state.SetToPrevious()
    ReadCommand state

and InvoiceMenu (parts: string list) =
    state.Name <- "Invoice Menu"
    state.Defs <- 
        [
            {
                CommandText = "add";
                HelpText = "Add a invoice";
                Execute = InvoiceAddMenu;
            };
            {
                CommandText = "span";
                HelpText = "Manage contribution spans.";
                Execute = InvoiceSpanMenu;
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

and InvoiceAddMenu (parts: string list) =
    printfn "Invoice Add %A" parts
    match (state.ActiveProject, state.ActiveContributor) with
    | (Some(project), Some(contributor)) ->
        match parts with
        | [ ] ->
            let agreement = Data.AddAgreement(project, contributor)
            state.ActiveAgreement <- Some(agreement)
        | _ ->
            printfn "Expected nothing"
    | (None, _) ->
        printfn "No active project selected"
    | (_, None) ->
        printfn "No active conributor selected"

    state.SetToPrevious()
    ReadCommand state

and InvoiceSpanMenu (parts: string list) =
    state.Name <- "Invoice Span Menu"
    state.Defs <- 
        [
            {
                CommandText = "add";
                HelpText = "Add a span to the current invoice";
                Execute = InvoiceSpanAddMenu;
            };
            {
                CommandText = "back";
                HelpText = "Moves back to the invoice menu";
                Execute = InvoiceMenu;
            };
            {
                CommandText = "exit";
                HelpText = "Exit command";
                Execute = Exit;
            };
        ]

    ParseLine state parts

and InvoiceSpanAddMenu (parts: string list) =
    printfn "Invoice Span Add %A" parts
    match state.ActiveAgreement with
    | Some(agreement) ->
        match parts with
        | [ spanStart; spanEnd; spanOffset; ] ->
            let (startOK, startDate) = DateTime.TryParse(spanStart)
            let (endOK, endDate) = DateTime.TryParse(spanEnd)
            let (offsetOK, utcOffset) = Double.TryParse(spanOffset)

            match (startOK, endOK, offsetOK) with
            | (true, true, true) -> 
                match (Data.AddSpan(agreement, startDate, endDate, utcOffset)) with
                | Choice1Of2(span) -> 
                    state.ActiveSpan <- Some(span)
                | Choice2Of2(err) ->
                    printfn "%s" err
            | (false, _, _) ->
                printfn "Unable to parse StartDate"
            | (_, false, _) ->
                printfn "Unable to parse EndDate"
            | (_, _, false) ->
                printfn "Unable to parse UtcOffset"
        | _ ->
            printfn "Expected {StartDate} {EndDate} {UtcOffset}"
    | None ->
        printfn "No active agreement"

    state.SetToPrevious()
    ReadCommand state

and Exit (parts: string list) =
    printfn "%A" Data
    printfn "Exit"

and MainMenu (parts: string list) =
    let subcommands = 
        [
            {
                CommandText = "invoice";
                HelpText = "Used to manage the invoices.";
                Execute = InvoiceMenu;
            };
            {
                CommandText = "contributor";
                HelpText = "Contributor command";
                Execute = ContributorMenu;
            };
            {
                CommandText = "project";
                HelpText = "Project command";
                Execute = ProjectMenu;
            };
            {
                CommandText = "exit";
                HelpText = "Exit command";
                Execute = Exit;
            };
        ]

    state.Name <- "Main"
    state.Defs <- subcommands
    state.PreviousName <- "Main"
    state.PreviousDefs <- subcommands
    
    ReadCommand state

let RunRHoursMenu() =
    MainMenu []
