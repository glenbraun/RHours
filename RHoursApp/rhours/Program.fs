
open System

open Json
open RHours.Commands
open RHours.Data
open JsonSerialization

open Microsoft.FSharp.Text.Lexing
open System.Globalization

let ParseJsonFromString (json:string) =
    let lexbuf = LexBuffer<char>.FromString json
    JsonParser.start JsonLexer.json lexbuf

let ParseJsonFromFile (fileName:string) = 
    let fi = System.IO.FileInfo(fileName)
    use textReader = new System.IO.StreamReader(fileName)
    let lexbuf = LexBuffer<char>.FromTextReader textReader
    JsonParser.start JsonLexer.json lexbuf

[<EntryPoint>]
let main argv =
    
    //let x = ParseJsonFromString @"{ ""glen"" : 1 }"

    //let xyz = ParseJsonFromFile @"C:\Projects\RChain\RHours\glenbraun\RHours\RHoursApp\rhours\Sample.json"
    //printfn "%A" xyz

    //RunRHoursMenu()

    let data = 
        {
            Projects = 
                [
                    {
                        Project.Id = "rhours";
                        Name = "RHours";
                    };
                ];
            Contributors = 
                [
                    {
                        Contributor.Id = "glen";
                        Name = "Glen";
                    };
                    {
                        Contributor.Id = "jake";
                        Name = "Jake";
                    };
                    {
                        Contributor.Id = "joshy";
                        Name = "Joshy";
                    };
                ];
            ContributionSpans = 
                [
                    {
                        ProjectId = "rhours";
                        ContributorId = "joshy";
                        StartDate = DateTime(2018, 7, 4);
                        EndDate = DateTime(2018, 7, 5);
                        UtcOffset = 4.0;
                        Contributions = 
                            [
                                {
                                    Contribution.Id = "1";
                                    Term = ContributionTerm.HourlyWithCompoundedInterestAndMaxTerm
                                                (
                                                    {
                                                        Hours = 1.0m;
                                                        HourlyRate = 100.0m
                                                        Token = "USD";
                                                        Interest = 1.0m;
                                                        MaxMultiplier = 10m;
                                                    }
                                                );
                                    Claims = ["Meeting with Jake and Glen."; "Discussed economics, branding, implementation.";];
                                };
                                {
                                    Contribution.Id = "2";
                                    Term = ContributionTerm.HourlyWithCompoundedInterestAndMaxTerm
                                                (
                                                    {
                                                        Hours = 1.0m;
                                                        HourlyRate = 100.0m
                                                        Token = "USD";
                                                        Interest = 1.0m;
                                                        MaxMultiplier = 10m;
                                                    }
                                                );
                                    Claims = ["Coding on solidity contract";];
                                }
                            ];
                    };
                    {
                        ProjectId = "rhours";
                        ContributorId = "joshy";
                        StartDate = DateTime(2018, 7, 11);
                        EndDate = DateTime(2018, 7, 12);
                        UtcOffset = 5.0;
                        Contributions = 
                            [
                                {
                                    Contribution.Id = "1";
                                    Term = ContributionTerm.HourlyWithCompoundedInterestAndMaxTerm
                                                (
                                                    {
                                                        Hours = 1.5m;
                                                        HourlyRate = 100.0m
                                                        Token = "USD";
                                                        Interest = 1.0m;
                                                        MaxMultiplier = 10m;
                                                    }
                                                );
                                    Claims = ["Meeting with Jake and Glen."; "Discussed ideology, law, abstraction.";];
                                }
                            ];
                    };
                    {
                        ProjectId = "rhours";
                        ContributorId = "jake";
                        StartDate = DateTime(2018, 7, 3);
                        EndDate = DateTime(2018, 7, 4);
                        UtcOffset = 7.0;
                        Contributions = 
                            [
                                {
                                    Contribution.Id = "1";
                                    Term = ContributionTerm.CashWithCompoundedInterestAndMaxTerm
                                                (
                                                    {
                                                        Amount = 135.0m;
                                                        Token = "USD";
                                                        Interest = 1.0m;
                                                        MaxMultiplier = 10m;
                                                    }
                                                );
                                    Claims = ["Paid friend for RHours logo.";];
                                }
                            ];
                    };
                ];
        }

    //data.ContributionSpans <- []

    let dataJson = Serialize (data) (fun x y -> None)

    let jsonString = WriteJsonToString dataJson

    let testjson = ParseJsonFromString jsonString
    let testdata = Deserialize<RHoursData> testjson

    //let data2 = Deserialize<RHoursData> dataJson (DeserializeRHoursData<RHoursData>)

    let x = 1
    printfn "%A" testdata
    0 // return an integer exit code

