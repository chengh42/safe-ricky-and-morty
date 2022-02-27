open Fake.Core
open Fake.IO

open Helpers

initializeContext()

let appPath = Path.getFullName "src/App"
let publishPath = Path.getFullName "publish"

Target.create "Clean" (fun _ ->
    Shell.cleanDir publishPath
    run dotnet "fable clean --yes" appPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run npm "install" ".")

Target.create "Bundle" (fun _ ->
    run dotnet "fable -o output -s --run webpack -p" appPath
)

Target.create "Run" (fun _ ->
    run dotnet "fable watch -o output -s --run webpack-dev-server" appPath
)

Target.create "Format" (fun _ ->
    run dotnet "fantomas . -r" "src"
)

open Fake.Core.TargetOperators

let dependencies = [
    "Clean"
        ==> "InstallClient"
        ==> "Bundle"

    "Clean"
        ==> "InstallClient"
        ==> "Run"
]

[<EntryPoint>]
let main args = runOrDefault args