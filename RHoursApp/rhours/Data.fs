module RHours.Data

open System
open System.IO
open RHours.Crypto
open Json
open JsonSerialization

type Project =
    {
        Id: string;
        Name: string;
    }

type ContributorInfoPublic =
    {
        Name: string;
        PublicKey: string;
        PrivateInfoHash: byte array;
    }

type ContributorInfoAttribute =
    {
        Name: string;
        Value: string;
    }

type ContributorInfoPrivate = 
    {
        PublicKey: string;
        PrivateKey: string;
        Attributes: ContributorInfoAttribute list;
    }

type HourlyWithCompoundedInterestAndMaxTerm =
    {
        Hours: decimal;
        HourlyRate: decimal;
        Token: string;
        Interest: decimal;
        MaxMultiplier: decimal;
    }

type CashWithCompoundedInterestAndMaxTerm = 
    {
        Amount: decimal;
        Token: string;
        Interest: decimal;
        MaxMultiplier: decimal;
    }

type CompensationTerm =
    | HourlyWithCompoundedInterestAndMaxTerm of HourlyWithCompoundedInterestAndMaxTerm
    | CashWithCompoundedInterestAndMaxTerm of CashWithCompoundedInterestAndMaxTerm

type Contribution =
    {
        Id: string;
        Terms: CompensationTerm;
        Claims: string list;
    }

type ContributionSpan =
    {
        ProjectId: string;
        ContributorId: string;
        StartDate: DateTime;
        EndDate: DateTime;
        UtcOffset: float;
        Contributions: Contribution list;
    }

type CompensationInvoice =
    {
        Project: Project;
        Contributor: ContributorInfoPublic;
        StartDate: DateTime;
        EndDate: DateTime;
        UtcOffset: float;
        Contributions: Contribution list;
    }

type CompensationProposal =
    {
        Invoice: CompensationInvoice;
        InvoiceHash: byte[];
        ContributorSignature: byte[];       // Signature of InvoiceHash
        ContributorPublicKey: string;
    }

type CompensationAgreement = 
    {
        Proposal: CompensationProposal;
        ProposalHash: byte[];
        AcceptorSignature: byte[];      // Signature of ProposalHash using Acceptor Key
        AcceptorPublicKey: string;
    }

type RHoursConfig = 
    {
        PublicFolder: DirectoryInfo;
        PrivateFolder: DirectoryInfo;
    }

type RHoursData =
    {
        mutable Config: RHoursConfig;
        mutable Projects : Project list;
        mutable Contributors : ContributorInfoPublic list;
        mutable ContributionSpans : ContributionSpan list;
    } with

    member this.ProjectExists(id: string) = 
        (this.Projects) |> List.exists (fun x -> x.Id = id)

    member this.ContributorExists(name: string) = 
        (this.Contributors) |> List.exists (fun x -> x.Name = name)

    member this.AddProject (id: string, name: string) : string option =
        match this.ProjectExists(id) with
        | false -> 
            this.Projects <- { Project.Id = id; Name = name} :: (this.Projects)
            None
        | true ->
            Some(sprintf "A project with the id '%s' already exists." id)

    member this.GetProject (id: string) : Project =
        (this.Projects) |> List.find (fun x -> x.Id = id)

    member this.DeleteProject (id: string) : string option =
        match this.ProjectExists(id) with
        | true -> 
            this.Projects <- (this.Projects) |> List.filter (fun x -> x.Id <> id)
            None
        | false ->
            Some(sprintf "No project with id '%s' exists." id)

    member this.ContributorPrivateFileExists(name: string) =
        let filename = name + ".json"
        let files = this.Config.PrivateFolder.GetFiles(filename)
        files.Length > 0

    member this.CreateContributorPrivateFile(name: string) =
        if this.ContributorPrivateFileExists(name) then
            failwith "Contributor file exists."
        else
            let filename = name + ".json"
            File.CreateText(Path.Combine(this.Config.PrivateFolder.FullName, filename))
     
    member this.AddContributor (name: string) : string option =
        match (this.ContributorExists(name), this.ContributorPrivateFileExists(name)) with
        | (false, false) -> 
            // generate key pair
            // create private key file
            // create ContributorInfoPrivate and serialize it to the file
            // need json with indent

            let (publicKey, privateKey) = CryptoProvider.CreateKeyPair()
            use privateInfoFile = this.CreateContributorPrivateFile(name)
            let privateInfo = 
                {
                    PublicKey = publicKey;
                    PrivateKey = privateKey;
                    Attributes = [ { ContributorInfoAttribute.Name = "Example Attribute Name"; Value = "Example Attribute Value"; }; ];
                }
            let json = Serialize privateInfo
            WriteJson privateInfoFile json

            let jsonBytes = GetJsonBytes json
            let privateInfoHash = CryptoProvider.Hash(jsonBytes)

            this.Contributors <- { ContributorInfoPublic.Name = name; PublicKey = publicKey; PrivateInfoHash = privateInfoHash; } :: (this.Contributors)
            None
        | (true, _) ->
            Some(sprintf "A contributor with the name '%s' already exists." name)
        | (_, true) ->
            Some("A contributor private file already exists.")

    member this.GetContributor (name: string) : ContributorInfoPublic =
        (this.Contributors) |> List.find (fun x -> x.Name = name)

    member this.DeleteContributor (name: string) : string option =
        match this.ContributorExists(name) with
        | true -> 
            this.Contributors <- (this.Contributors) |> List.filter (fun x -> x.Name <> name)
            None
        | false ->
            Some(sprintf "No contributor with name '%s' exists." name)

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
                            UtcOffset = 0.0;
                            Contributions = [];
                        }

                    this.ContributionSpans <- span :: (this.ContributionSpans)
                    None
                else
                    Some(sprintf "No contributor with id '%s' exists." contributorId)
            else
                Some(sprintf "No project with id '%s' exists." projectId)

    member this.GetContribution (projectId: string, contributorId: string, contributionId: string) : ContributionSpan * Contribution =
        let resultOption = 
            (this.ContributionSpans) |> 
            List.tryPick (
                fun x -> 
                    match (x.Contributions) |> List.tryFind (fun y -> y.Id = contributionId) with
                    | Some(c) -> Some(x, c)
                    | None -> None
                )
        match resultOption with
        | Some(result) -> result
        | None -> failwith "Unable to find contribution"

    member this.CreateCompensationInvoice (projectId: string, contributorId: string, contributionId: string) : CompensationInvoice = 
        let project = this.GetProject(projectId)
        let contributor = this.GetContributor(contributorId)
        let (span, contribution) = this.GetContribution(projectId, contributorId, contributionId)
        {
            Project = project;
            Contributor = contributor;
            StartDate = span.StartDate;
            EndDate = span.EndDate;
            UtcOffset = span.UtcOffset;
            Contributions = [ contribution; ]
        }
