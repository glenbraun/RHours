
open System
open System.IO

open SignContribution

open RHours.Commands
open RHours.Data



[<EntryPoint>]
let main argv =

    //let x = ParseJsonFromString @"{ ""glen"" : 1 }"

    //let xyz = ParseJsonFromFile @"C:\Projects\RChain\RHours\glenbraun\RHours\RHoursApp\rhours\Sample.json"
    //printfn "%A" xyz

    //let (publicKey, priateKey) = RHours.Crypto.CryptoProvider.CreateKeyPair()

    //let x  = 1
    //printfn "%A" (publicKey, priateKey)

    RunRHoursMenu()

    let data = 
        {
            Config = 
                { 
                    PublicFolder = new DirectoryInfo("..\\..\\..\\..\\..\\");
                    PrivateFolder = new DirectoryInfo("..\\..\\..\\..\\..\\..\\RHours_private");
                };

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
                        ContributorInfoPublic.Name = "glen";
                        PublicKey = "MCowBQYDK2VwAyEACOnA0dtn/SPVrl/OVYE1//xZP0xGV7x2vxjkgFH0cW0=";
                        PrivateInfoHash = [| |];
                    };
                    {
                        ContributorInfoPublic.Name = "jake";
                        PublicKey = "Jake Public";
                        PrivateInfoHash = [| |];
                    };
                    {
                        ContributorInfoPublic.Name = "joshy";
                        PublicKey = "MCowBQYDK2VwAyEAedqYOtnLSAkDXDg4+ovGow+HA1KZmM5SsuaKJJD6Xf8=";
                        PrivateInfoHash = [| |];
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
                                    Terms = CompensationTerm.HourlyWithCompoundedInterestAndMaxTerm
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
                                    Terms = CompensationTerm.HourlyWithCompoundedInterestAndMaxTerm
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
                                    Terms = CompensationTerm.HourlyWithCompoundedInterestAndMaxTerm
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
                                    Terms = CompensationTerm.CashWithCompoundedInterestAndMaxTerm
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

    //let dataJson = Serialize (data) (fun x y -> None)

    //let jsonString = WriteJsonToString dataJson

    //let testjson = ParseJsonFromString jsonString
    //let testdata = Deserialize<RHoursData> testjson

    //let data2 = Deserialize<RHoursData> dataJson (DeserializeRHoursData<RHoursData>)
    let joshyPublicKey = "MCowBQYDK2VwAyEAedqYOtnLSAkDXDg4+ovGow+HA1KZmM5SsuaKJJD6Xf8="
    let joshyPrivateKey = "MC4CAQAwBQYDK2VwBCIEIMjBC0tPSJgEqDOWdGOoA3CDFuTi5xoEeZas6/jY3Hj4"

    let glenPublicKey = "MCowBQYDK2VwAyEACOnA0dtn/SPVrl/OVYE1//xZP0xGV7x2vxjkgFH0cW0="
    let glenPrivateKey = "MC4CAQAwBQYDK2VwBCIEIBJ7kFZXvA/XjCQc+7c0NBRMWRvCzMLgXIcEZYv36ISN"

    let invoice = data.CreateCompensationInvoice("rhours", "joshy", "1")
    let proposal = SignCompensationInvoice invoice joshyPublicKey joshyPrivateKey
    let agreement = SignCompensationProposal proposal glenPublicKey glenPrivateKey

    let pver = VerifyCompensationProposal proposal
    let pagree = VerifyCompensationAgreement agreement

    printfn "%A" (proposal, agreement, pver, pagree)
    0 // return an integer exit code

