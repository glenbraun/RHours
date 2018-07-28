module RHours.Data

open System
open System.IO
open Microsoft.FSharp.Text.Lexing

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
        PublicName: string;
        PublicKey: string;
        mutable PrivateInfoHash: byte array;
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


let ParseJsonFromString(json:string) =
    let lexbuf = LexBuffer<char>.FromString json
    JsonParser.start JsonLexer.json lexbuf
    
let ParseJsonFromFile (fileName:string) = 
    use textReader = new System.IO.StreamReader(fileName)
    let lexbuf = LexBuffer<char>.FromTextReader textReader
    JsonParser.start JsonLexer.json lexbuf

type RHoursData =
    {
        [<SkipSerialization>]
        mutable Config: RHoursConfig;
        mutable Projects : Project list;
        mutable Contributors : ContributorInfoPublic list;
        mutable ContributionSpans : ContributionSpan list;
    } with

    member this.Initialize(config: RHoursConfig) =
        // Create the public folder if it doesn't exist
        // Create the private folder if it doesn't exist
        // Parse and desrialize the data in the rhours.json file if it exists
        // Save the current data to an rhours.json file
        //      This will create it if it didn't exist before
        //      Also saves it in the standard format if it wasn't before

        this.Config <- config
        if not(config.PublicFolder.Exists) then
            config.PublicFolder.Create()        

        if not(config.PrivateFolder.Exists) then
            config.PrivateFolder.Create()
        
        let files = config.PublicFolder.GetFiles("rhours.json")
        if files.Length = 1 then
            let fileJson = ParseJsonFromFile (files.[0].FullName)
            let fileData = JsonSerialization.Deserialize<RHoursData> fileJson
            this.Projects <- fileData.Projects
            this.Contributors <- fileData.Contributors
            this.ContributionSpans <- fileData.ContributionSpans
        
        let json = Serialize this
        use rhoursFile = File.CreateText(Path.Combine(config.PublicFolder.FullName, "rhours.json"))
        WriteJson rhoursFile json

    member this.ProjectExists(id: string) = 
        (this.Projects) |> List.exists (fun x -> x.Id = id)

    member this.ContributorExists(publicName: string) = 
        (this.Contributors) |> List.exists (fun x -> x.PublicName = publicName)

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

    member this.GetContributorPrivateInfo(name: string) =
        let filename = name + ".json"
        let files = this.Config.PrivateFolder.GetFiles(filename)
        if files.Length = 1 then
            // load the private info from the file, makes a json object
            // deserialize the json to a typed object
            // calculate the hash
            // update the public information with the hash
            let json = ParseJsonFromFile (files.[0].FullName)
            let privateInfo = JsonSerialization.Deserialize<ContributorInfoPrivate> json
            privateInfo
        else
            failwith "Contributor private file does not exist."

    member this.CreateContributorPrivateFile(name: string) =
        if this.ContributorPrivateFileExists(name) then
            failwith "Contributor file exists."
        else
            let filename = name + ".json"
            File.CreateText(Path.Combine(this.Config.PrivateFolder.FullName, filename))
     
    member this.AddContributor (publicName: string) : string option =
        match (this.ContributorExists(publicName), this.ContributorPrivateFileExists(publicName)) with
        | (false, false) -> 
            // generate key pair
            // create private key file
            // create ContributorInfoPrivate and serialize it to the file
            // need json with indent

            let (publicKey, privateKey) = CryptoProvider.CreateKeyPair()
            use privateInfoFile = this.CreateContributorPrivateFile(publicName)
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

            this.Contributors <- { ContributorInfoPublic.PublicName = publicName; PublicKey = publicKey; PrivateInfoHash = privateInfoHash; } :: (this.Contributors)
            None
        | (true, _) ->
            Some(sprintf "A contributor with the name '%s' already exists." publicName)
        | (_, true) ->
            Some("A contributor private file already exists.")

    member this.GetContributor (publicName: string) : ContributorInfoPublic =
        (this.Contributors) |> List.find (fun x -> x.PublicName = publicName)

    member this.HashContributor (name: string) : string option =
        match this.ContributorExists(name) with
        | true -> 
            let privateInfo = this.GetContributorPrivateInfo(name)
            let serializedJson = JsonSerialization.Serialize privateInfo
            let jsonBytes = GetJsonBytes serializedJson
            let privateInfoHash = CryptoProvider.Hash(jsonBytes)
            let publicInfo = this.GetContributor(name)
            publicInfo.PrivateInfoHash <- privateInfoHash
            None
        | false ->
            Some(sprintf "No contributor with name '%s' exists." name)

    member this.DeleteContributor (publicName: string) : string option =
        match this.ContributorExists(publicName) with
        | true -> 
            this.Contributors <- (this.Contributors) |> List.filter (fun x -> x.PublicName <> publicName)
            None
        | false ->
            Some(sprintf "No contributor with name '%s' exists." publicName)

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
