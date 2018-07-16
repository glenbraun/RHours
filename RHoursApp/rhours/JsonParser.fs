// Implementation file for parser generated by fsyacc
module internal JsonParser
#nowarn "64";; // turn off warnings that type variables used in production annotations are instantiated to concrete type
open Microsoft.FSharp.Text.Lexing
open Microsoft.FSharp.Text.Parsing.ParseHelpers
# 1 "JsonGrammar.fsy"

open System
open Json


# 12 "JsonParser.fs"
// This type is the type of tokens accepted by the parser
type token = 
  | EOF
  | JSON_FLOAT of (string)
  | JSON_INT of (string)
  | JSON_STRING of (string)
  | JSON_NULL of (string)
  | JSON_BOOL of (string)
  | ID of (string)
  | SYM_COMMA
  | SYM_LBRACKET
  | SYM_RBRACKET
  | SYM_LCURLY
  | SYM_RCURLY
  | SYM_COLON
// This type is used to give symbolic names to token indexes, useful for error messages
type tokenId = 
    | TOKEN_EOF
    | TOKEN_JSON_FLOAT
    | TOKEN_JSON_INT
    | TOKEN_JSON_STRING
    | TOKEN_JSON_NULL
    | TOKEN_JSON_BOOL
    | TOKEN_ID
    | TOKEN_SYM_COMMA
    | TOKEN_SYM_LBRACKET
    | TOKEN_SYM_RBRACKET
    | TOKEN_SYM_LCURLY
    | TOKEN_SYM_RCURLY
    | TOKEN_SYM_COLON
    | TOKEN_end_of_input
    | TOKEN_error
// This type is used to give symbolic names to token indexes, useful for error messages
type nonTerminalId = 
    | NONTERM__startstart
    | NONTERM_start
    | NONTERM_JsonValue
    | NONTERM_JsonString
    | NONTERM_JsonNumber
    | NONTERM_JsonObject
    | NONTERM_JsonMembers
    | NONTERM_JsonPair
    | NONTERM_JsonArray
    | NONTERM_JsonElements
    | NONTERM_JsonBool
    | NONTERM_JsonNull

// This function maps tokens to integer indexes
let tagOfToken (t:token) = 
  match t with
  | EOF  -> 0 
  | JSON_FLOAT _ -> 1 
  | JSON_INT _ -> 2 
  | JSON_STRING _ -> 3 
  | JSON_NULL _ -> 4 
  | JSON_BOOL _ -> 5 
  | ID _ -> 6 
  | SYM_COMMA  -> 7 
  | SYM_LBRACKET  -> 8 
  | SYM_RBRACKET  -> 9 
  | SYM_LCURLY  -> 10 
  | SYM_RCURLY  -> 11 
  | SYM_COLON  -> 12 

// This function maps integer indexes to symbolic token ids
let tokenTagToTokenId (tokenIdx:int) = 
  match tokenIdx with
  | 0 -> TOKEN_EOF 
  | 1 -> TOKEN_JSON_FLOAT 
  | 2 -> TOKEN_JSON_INT 
  | 3 -> TOKEN_JSON_STRING 
  | 4 -> TOKEN_JSON_NULL 
  | 5 -> TOKEN_JSON_BOOL 
  | 6 -> TOKEN_ID 
  | 7 -> TOKEN_SYM_COMMA 
  | 8 -> TOKEN_SYM_LBRACKET 
  | 9 -> TOKEN_SYM_RBRACKET 
  | 10 -> TOKEN_SYM_LCURLY 
  | 11 -> TOKEN_SYM_RCURLY 
  | 12 -> TOKEN_SYM_COLON 
  | 15 -> TOKEN_end_of_input
  | 13 -> TOKEN_error
  | _ -> failwith "tokenTagToTokenId: bad token"

/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production
let prodIdxToNonTerminal (prodIdx:int) = 
  match prodIdx with
    | 0 -> NONTERM__startstart 
    | 1 -> NONTERM_start 
    | 2 -> NONTERM_JsonValue 
    | 3 -> NONTERM_JsonValue 
    | 4 -> NONTERM_JsonValue 
    | 5 -> NONTERM_JsonValue 
    | 6 -> NONTERM_JsonValue 
    | 7 -> NONTERM_JsonValue 
    | 8 -> NONTERM_JsonString 
    | 9 -> NONTERM_JsonNumber 
    | 10 -> NONTERM_JsonNumber 
    | 11 -> NONTERM_JsonObject 
    | 12 -> NONTERM_JsonMembers 
    | 13 -> NONTERM_JsonMembers 
    | 14 -> NONTERM_JsonMembers 
    | 15 -> NONTERM_JsonPair 
    | 16 -> NONTERM_JsonArray 
    | 17 -> NONTERM_JsonElements 
    | 18 -> NONTERM_JsonElements 
    | 19 -> NONTERM_JsonBool 
    | 20 -> NONTERM_JsonNull 
    | _ -> failwith "prodIdxToNonTerminal: bad production index"

let _fsyacc_endOfInputTag = 15 
let _fsyacc_tagOfErrorTerminal = 13

// This function gets the name of a token as a string
let token_to_string (t:token) = 
  match t with 
  | EOF  -> "EOF" 
  | JSON_FLOAT _ -> "JSON_FLOAT" 
  | JSON_INT _ -> "JSON_INT" 
  | JSON_STRING _ -> "JSON_STRING" 
  | JSON_NULL _ -> "JSON_NULL" 
  | JSON_BOOL _ -> "JSON_BOOL" 
  | ID _ -> "ID" 
  | SYM_COMMA  -> "SYM_COMMA" 
  | SYM_LBRACKET  -> "SYM_LBRACKET" 
  | SYM_RBRACKET  -> "SYM_RBRACKET" 
  | SYM_LCURLY  -> "SYM_LCURLY" 
  | SYM_RCURLY  -> "SYM_RCURLY" 
  | SYM_COLON  -> "SYM_COLON" 

// This function gets the data carried by a token as an object
let _fsyacc_dataOfToken (t:token) = 
  match t with 
  | EOF  -> (null : System.Object) 
  | JSON_FLOAT _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | JSON_INT _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | JSON_STRING _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | JSON_NULL _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | JSON_BOOL _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | ID _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x 
  | SYM_COMMA  -> (null : System.Object) 
  | SYM_LBRACKET  -> (null : System.Object) 
  | SYM_RBRACKET  -> (null : System.Object) 
  | SYM_LCURLY  -> (null : System.Object) 
  | SYM_RCURLY  -> (null : System.Object) 
  | SYM_COLON  -> (null : System.Object) 
let _fsyacc_gotos = [| 0us; 65535us; 1us; 65535us; 0us; 1us; 4us; 65535us; 0us; 2us; 20us; 21us; 22us; 25us; 26us; 25us; 4us; 65535us; 0us; 4us; 20us; 4us; 22us; 4us; 26us; 4us; 4us; 65535us; 0us; 5us; 20us; 5us; 22us; 5us; 26us; 5us; 4us; 65535us; 0us; 6us; 20us; 6us; 22us; 6us; 26us; 6us; 2us; 65535us; 13us; 14us; 17us; 18us; 2us; 65535us; 13us; 16us; 17us; 16us; 4us; 65535us; 0us; 7us; 20us; 7us; 22us; 7us; 26us; 7us; 2us; 65535us; 22us; 23us; 26us; 27us; 4us; 65535us; 0us; 8us; 20us; 8us; 22us; 8us; 26us; 8us; 4us; 65535us; 0us; 9us; 20us; 9us; 22us; 9us; 26us; 9us; |]
let _fsyacc_sparseGotoTableRowOffsets = [|0us; 1us; 3us; 8us; 13us; 18us; 23us; 26us; 29us; 34us; 37us; 42us; |]
let _fsyacc_stateToProdIdxsTableElements = [| 1us; 0us; 1us; 0us; 1us; 1us; 1us; 1us; 1us; 2us; 1us; 3us; 1us; 4us; 1us; 5us; 1us; 6us; 1us; 7us; 1us; 8us; 1us; 9us; 1us; 10us; 1us; 11us; 1us; 11us; 1us; 11us; 2us; 13us; 14us; 1us; 14us; 1us; 14us; 1us; 15us; 1us; 15us; 1us; 15us; 1us; 16us; 1us; 16us; 1us; 16us; 2us; 17us; 18us; 1us; 18us; 1us; 18us; 1us; 19us; 1us; 20us; |]
let _fsyacc_stateToProdIdxsTableRowOffsets = [|0us; 2us; 4us; 6us; 8us; 10us; 12us; 14us; 16us; 18us; 20us; 22us; 24us; 26us; 28us; 30us; 32us; 35us; 37us; 39us; 41us; 43us; 45us; 47us; 49us; 51us; 54us; 56us; 58us; 60us; |]
let _fsyacc_action_rows = 30
let _fsyacc_actionTableElements = [|7us; 32768us; 1us; 12us; 2us; 11us; 3us; 10us; 4us; 29us; 5us; 28us; 8us; 22us; 10us; 13us; 0us; 49152us; 1us; 32768us; 0us; 3us; 0us; 16385us; 0us; 16386us; 0us; 16387us; 0us; 16388us; 0us; 16389us; 0us; 16390us; 0us; 16391us; 0us; 16392us; 0us; 16393us; 0us; 16394us; 1us; 16396us; 3us; 19us; 1us; 32768us; 11us; 15us; 0us; 16395us; 1us; 16397us; 7us; 17us; 1us; 16396us; 3us; 19us; 0us; 16398us; 1us; 32768us; 12us; 20us; 7us; 32768us; 1us; 12us; 2us; 11us; 3us; 10us; 4us; 29us; 5us; 28us; 8us; 22us; 10us; 13us; 0us; 16399us; 7us; 32768us; 1us; 12us; 2us; 11us; 3us; 10us; 4us; 29us; 5us; 28us; 8us; 22us; 10us; 13us; 1us; 32768us; 9us; 24us; 0us; 16400us; 1us; 16401us; 7us; 26us; 7us; 32768us; 1us; 12us; 2us; 11us; 3us; 10us; 4us; 29us; 5us; 28us; 8us; 22us; 10us; 13us; 0us; 16402us; 0us; 16403us; 0us; 16404us; |]
let _fsyacc_actionTableRowOffsets = [|0us; 8us; 9us; 11us; 12us; 13us; 14us; 15us; 16us; 17us; 18us; 19us; 20us; 21us; 23us; 25us; 26us; 28us; 30us; 31us; 33us; 41us; 42us; 50us; 52us; 53us; 55us; 63us; 64us; 65us; |]
let _fsyacc_reductionSymbolCounts = [|1us; 2us; 1us; 1us; 1us; 1us; 1us; 1us; 1us; 1us; 1us; 3us; 0us; 1us; 3us; 3us; 3us; 1us; 3us; 1us; 1us; |]
let _fsyacc_productionToNonTerminalTable = [|0us; 1us; 2us; 2us; 2us; 2us; 2us; 2us; 3us; 4us; 4us; 5us; 6us; 6us; 6us; 7us; 8us; 9us; 9us; 10us; 11us; |]
let _fsyacc_immediateActions = [|65535us; 49152us; 65535us; 16385us; 16386us; 16387us; 16388us; 16389us; 16390us; 16391us; 16392us; 16393us; 16394us; 65535us; 65535us; 16395us; 65535us; 65535us; 16398us; 65535us; 65535us; 16399us; 65535us; 65535us; 16400us; 65535us; 65535us; 16402us; 16403us; 16404us; |]
let _fsyacc_reductions ()  =    [| 
# 170 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data :  obj )) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
                      raise (Microsoft.FSharp.Text.Parsing.Accept(Microsoft.FSharp.Core.Operators.box _1))
                   )
                 : '_startstart));
# 179 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonValue)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 31 "JsonGrammar.fsy"
                                                                        _1 
                   )
# 31 "JsonGrammar.fsy"
                 :  obj ));
# 190 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonString)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 34 "JsonGrammar.fsy"
                                                                       _1 :> obj 
                   )
# 34 "JsonGrammar.fsy"
                 : 'JsonValue));
# 201 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonNumber)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 35 "JsonGrammar.fsy"
                                                                       _1 :> obj 
                   )
# 35 "JsonGrammar.fsy"
                 : 'JsonValue));
# 212 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonObject)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 36 "JsonGrammar.fsy"
                                                                       _1 :> obj 
                   )
# 36 "JsonGrammar.fsy"
                 : 'JsonValue));
# 223 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonArray)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 37 "JsonGrammar.fsy"
                                                                       _1 :> obj 
                   )
# 37 "JsonGrammar.fsy"
                 : 'JsonValue));
# 234 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonBool)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 38 "JsonGrammar.fsy"
                                                                       _1 :> obj 
                   )
# 38 "JsonGrammar.fsy"
                 : 'JsonValue));
# 245 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonNull)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 39 "JsonGrammar.fsy"
                                                                       _1 :> obj 
                   )
# 39 "JsonGrammar.fsy"
                 : 'JsonValue));
# 256 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 42 "JsonGrammar.fsy"
                                                                       _1 
                   )
# 42 "JsonGrammar.fsy"
                 : 'JsonString));
# 267 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 45 "JsonGrammar.fsy"
                                                                       int(_1) :> obj 
                   )
# 45 "JsonGrammar.fsy"
                 : 'JsonNumber));
# 278 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 46 "JsonGrammar.fsy"
                                                                       float(_1) :> obj 
                   )
# 46 "JsonGrammar.fsy"
                 : 'JsonNumber));
# 289 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonMembers)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 49 "JsonGrammar.fsy"
                                                                       _2; 
                   )
# 49 "JsonGrammar.fsy"
                 : 'JsonObject));
# 300 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 52 "JsonGrammar.fsy"
                                                                       [ ] 
                   )
# 52 "JsonGrammar.fsy"
                 : 'JsonMembers));
# 310 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonPair)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 53 "JsonGrammar.fsy"
                                                                       [ _1 ] 
                   )
# 53 "JsonGrammar.fsy"
                 : 'JsonMembers));
# 321 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonPair)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonMembers)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 54 "JsonGrammar.fsy"
                                                                       _1 :: _3 
                   )
# 54 "JsonGrammar.fsy"
                 : 'JsonMembers));
# 333 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonValue)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 57 "JsonGrammar.fsy"
                                                                       { Label = _1; Value = _3; } 
                   )
# 57 "JsonGrammar.fsy"
                 : 'JsonPair));
# 345 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _2 = (let data = parseState.GetInput(2) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonElements)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 60 "JsonGrammar.fsy"
                                                                       Array.ofList _2 
                   )
# 60 "JsonGrammar.fsy"
                 : 'JsonArray));
# 356 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonValue)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 63 "JsonGrammar.fsy"
                                                                       [ _1 ] 
                   )
# 63 "JsonGrammar.fsy"
                 : 'JsonElements));
# 367 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonValue)) in
            let _3 = (let data = parseState.GetInput(3) in (Microsoft.FSharp.Core.Operators.unbox data : 'JsonElements)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 64 "JsonGrammar.fsy"
                                                                       _1 :: _3 
                   )
# 64 "JsonGrammar.fsy"
                 : 'JsonElements));
# 379 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 67 "JsonGrammar.fsy"
                                                                       bool.Parse(_1) 
                   )
# 67 "JsonGrammar.fsy"
                 : 'JsonBool));
# 390 "JsonParser.fs"
        (fun (parseState : Microsoft.FSharp.Text.Parsing.IParseState) ->
            let _1 = (let data = parseState.GetInput(1) in (Microsoft.FSharp.Core.Operators.unbox data : string)) in
            Microsoft.FSharp.Core.Operators.box
                (
                   (
# 70 "JsonGrammar.fsy"
                                                                       null 
                   )
# 70 "JsonGrammar.fsy"
                 : 'JsonNull));
|]
# 402 "JsonParser.fs"
let tables () : Microsoft.FSharp.Text.Parsing.Tables<_> = 
  { reductions= _fsyacc_reductions ();
    endOfInputTag = _fsyacc_endOfInputTag;
    tagOfToken = tagOfToken;
    dataOfToken = _fsyacc_dataOfToken; 
    actionTableElements = _fsyacc_actionTableElements;
    actionTableRowOffsets = _fsyacc_actionTableRowOffsets;
    stateToProdIdxsTableElements = _fsyacc_stateToProdIdxsTableElements;
    stateToProdIdxsTableRowOffsets = _fsyacc_stateToProdIdxsTableRowOffsets;
    reductionSymbolCounts = _fsyacc_reductionSymbolCounts;
    immediateActions = _fsyacc_immediateActions;
    gotos = _fsyacc_gotos;
    sparseGotoTableRowOffsets = _fsyacc_sparseGotoTableRowOffsets;
    tagOfErrorTerminal = _fsyacc_tagOfErrorTerminal;
    parseError = (fun (ctxt:Microsoft.FSharp.Text.Parsing.ParseErrorContext<_>) -> 
                              match parse_error_rich with 
                              | Some f -> f ctxt
                              | None -> parse_error ctxt.Message);
    numTerminals = 16;
    productionToNonTerminalTable = _fsyacc_productionToNonTerminalTable  }
let engine lexer lexbuf startState = (tables ()).Interpret(lexer, lexbuf, startState)
let start lexer lexbuf :  obj  =
    Microsoft.FSharp.Core.Operators.unbox ((tables ()).Interpret(lexer, lexbuf, 0))
