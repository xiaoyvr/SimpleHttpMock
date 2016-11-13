// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing

let xunitrunner = "packages/xunit.runners/tools/xunit.console.clr4.exe"

Target "Clean" (fun _ ->
    !! "./src/**/bin/"
        ++ "./src/**/obj/"
        ++ "./test/**/bin/"
        ++ "./test/**/obj/"
        |> CleanDirs
)

// Default Target
Target "Default" (fun _ -> 
    trace "Hello World from FAKE"
)

Target "Build" (fun _ -> 
    !! "./src/**/*.csproj"
    ++ "./test/**/*.csproj"
        |> MSBuildRelease "" "Build"
        |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! "./test/**/bin/**/test.dll"
        |> xUnit (fun p -> 
        {p with
            HtmlOutputPath = Some("./tmp/TestOutput/" @@ "html");
            ToolPath = xunitrunner })
)

"Clean"
    ==> "Build"
    ==> "Test"
    ==> "Default"

// start build
RunTargetOrDefault "Default"