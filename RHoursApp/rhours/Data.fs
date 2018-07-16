module RHours.Data

open System

type Project =
    {
        Id: string;
        Name: string;
    }

type Contributor =
    {
        Id: string;
        Name: string;
    }

type HourlyWithCompoundedInterestAndMaxTerm =
    {
        Hours: decimal;
        HourlyRate: decimal;
        Interest: decimal;
        MaxMultiplier: decimal;
    }

type CashWithCompoundedInterestAndMaxTerm = 
    {
        Amount: decimal;
        Interest: decimal;
        MaxMultiplier: decimal;
    }

type ContributionTerm =
    | HourlyWithCompoundedInterestAndMaxTerm of HourlyWithCompoundedInterestAndMaxTerm
    | CashWithCompoundedInterestAndMaxTerm of CashWithCompoundedInterestAndMaxTerm

type Contribution =
    {
        Id: string;
        Terms: ContributionTerm;
    }

type ContributionSpan =
    {
        ProjectId: string;
        ContributorId: string;
        StartDate: DateTime;
        EndDate: DateTime;
        Contributions: Contribution list;
    }

type RHoursData =
    {
        mutable Projects : Project list;
        mutable Contributors : Contributor list;
        mutable ContributionSpans : ContributionSpan list;
    } with

    member this.ProjectExists(id: string) = 
        (this.Projects) |> List.exists (fun x -> x.Id = id)

    member this.ContributorExists(id: string) = 
        (this.Contributors) |> List.exists (fun x -> x.Id = id)

    member this.AddProject (id: string, name: string) : string option =
        match this.ProjectExists(id) with
        | false -> 
            this.Projects <- { Project.Id = id; Name = name} :: (this.Projects)
            None
        | true ->
            Some(sprintf "A project with the id '%s' already exists." id)

    member this.DeleteProject (id: string) : string option =
        match this.ProjectExists(id) with
        | true -> 
            this.Projects <- (this.Projects) |> List.filter (fun x -> x.Id <> id)
            None
        | false ->
            Some(sprintf "No project with id '%s' exists." id)
     
    member this.AddContributor (id: string, name: string) : string option =
        match this.ContributorExists(id) with
        | false -> 
            this.Contributors <- { Contributor.Id = id; Name = name} :: (this.Contributors)
            None
        | true ->
            Some(sprintf "A contributor with the id '%s' already exists." id)

    member this.DeleteContributor (id: string) : string option =
        match this.ContributorExists(id) with
        | true -> 
            this.Contributors <- (this.Contributors) |> List.filter (fun x -> x.Id <> id)
            None
        | false ->
            Some(sprintf "No contributor with id '%s' exists." id)

    member this.AddContributionSpan (projectId: string, contributorId: string, startDate: DateTime, endDate: DateTime) : string option =
        if startDate > endDate then
            Some("The start date of a contribution be before its end date.")
        else
            if this.ProjectExists(projectId) then
                if this.ContributorExists(contributorId) then
                    let span = 
                        {
                            ProjectId = projectId;
                            ContributorId = contributorId;
                            StartDate = startDate;
                            EndDate = endDate;
                            Contributions = [];
                        }

                    this.ContributionSpans <- span :: (this.ContributionSpans)
                    None
                else
                    Some(sprintf "No contributor with id '%s' exists." contributorId)
            else
                Some(sprintf "No project with id '%s' exists." projectId)
