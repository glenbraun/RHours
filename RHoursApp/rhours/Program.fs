

open RHours.Commands

open Microsoft.FSharp.Text.Lexing

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

    //let x = ParseJsonFromFile @"C:\Projects\RChain\RHours\glenbraun\RHours\RHoursApp\rhours\Sample.json"

    ReadCommands RHoursCommands

    0 // return an integer exit code
