module FSharp.Json.Decode.Tests

open FSharp.Json.Decode
open NUnit.Framework

let (|Result|) res =
    match res with
    | Ok v -> v
    | Err err -> failwithf "unexpected: %s" err

[<Test>]
let ``returns "hello world"`` () =
    let (Result result) = decodeString dstring "\"hello world\""
    Assert.AreEqual("hello world", result)

[<Test>]
let ``returns 42`` () =
  let (Result result) = decodeString dint "42"
  Assert.AreEqual(42, result)

[<Test>]
let ``returns 1.23123`` () =
    let (Result result) = decodeString dfloat "1.23123"
    Assert.AreEqual(1.23123, result)

[<Test>]
let ``returns true`` () =
    let (Result result) = decodeString dbool "true"
    Assert.AreEqual(true, result)

[<Test>]
let ``returns false`` () =
    let (Result result) = decodeString dbool "false"
    Assert.AreEqual(false, result)

[<Test>]
let ``returns 42 if null`` () =
    let (Result result) = decodeString (dnull 42) ""
    Assert.AreEqual(42, result)

[<Test>]
let ``returns maybe 42`` () =
    let (Result result) = decodeString (maybe dint) "42"
    Assert.AreEqual(Some 42, result)
    let (Result result) = decodeString (maybe dint) ""
    Assert.AreEqual(None, result)

[<Test>]
let ``returns object1`` () =
    let (Result result) =
        decodeString
            (object1 id
                     ("name" := dstring))
            "{ name: \"foo\" }"
    Assert.AreEqual("foo", result)

[<Test>]
let ``returns object2`` () =
    let (Result result) =
        decodeString
            (object2 (fun name age -> name, age)
                     ("name" := dstring)
                     ("age" := dint))
            "{ name: \"foo\", age: 42 }"
    Assert.AreEqual(("foo", 42), result)

[<Test>]
let ``returns integer list`` () =
    let (Result result) =
        decodeString
            (list dint)
            "[ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ]"
    Assert.AreEqual([1..10], result)

[<Test>]
let ``returns either foo or bar if empty`` () =
    let test input =
        let (Result result) =
            decodeString
                (oneOf [dstring; dnull "bar"])
                input
        result
    Assert.AreEqual("foo", test "\"foo\"")
    Assert.AreEqual("bar", test "\"bar\"")
    Assert.AreEqual("bar", test "")

[<Test>]
let ``returns keyvaluepairs`` () =
    let (Result result) =
        decodeString
            (keyValuePairs dstring)
            "{ name: \"foo\", name2: \"bar\" }"
    Assert.AreEqual([ ("name", "foo"); ("name2", "bar") ], result)

[<Test>]
let ``returns map`` () =
    let (Result result) =
        decodeString
            (dmap dstring)
            "{ name: \"foo\", name2: \"bar\" }"
    Assert.AreEqual(Map.ofList [ ("name", "foo"); ("name2", "bar") ], result)

[<Test>]
let ``returns email and age at position`` () =
    let json = "{ person: { contact: { email: \"email@example.com\" }, age: 42 } }"
    let (Result email) =
        decodeString
            (at [ "person"; "contact"; "email" ] dstring)
            json
    Assert.AreEqual("email@example.com", email)
    let (Result age) =
        decodeString
            (at [ "person"; "age" ] dint)
            json
    Assert.AreEqual(42, age)