module Index

open Elmish
open Fable.Remoting.Client
open SafeRickAndMorty

type AsyncOperationStatus<'t> =
    | Started
    | Finished of 't

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't

type Model = { Characters: Deferred<GetCharacters.Character list>; SearchValue: string }

type Msg =
    | GetRickAndMortyCharacters of AsyncOperationStatus<Result<GetCharacters.Query,ErrorType list>>
    | SetSearchValue of string
    | SearchCharacter

let client = SafeRickAndMortyGraphqlClient("https://rickandmortyapi.com/graphql")

let init () : Model * Cmd<Msg> =
    let model = { Characters = HasNotStartedYet; SearchValue = "" }
    let input: GetCharacters.InputVariables = { name = model.SearchValue }
    let cmd = Cmd.OfAsync.perform client.GetCharacters input (Finished >> GetRickAndMortyCharacters)
    model, cmd

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | GetRickAndMortyCharacters Started -> model, Cmd.none
    | GetRickAndMortyCharacters (Finished (Error e)) -> model, Cmd.none
    | GetRickAndMortyCharacters (Finished (Ok queryResult)) ->
        let characters =
            queryResult.characters
            |> Option.toList
            |> List.collect (fun characters ->
                characters.results
                |> Option.toList
                |> List.concat
                |> List.choose id)
        { model with Characters = Resolved characters }, Cmd.none
    | SetSearchValue searchVal -> { model with SearchValue = searchVal }, Cmd.none
    | SearchCharacter ->
        let cmd =
            Cmd.OfAsync.perform client.GetCharacters { name = model.SearchValue } (Finished >> GetRickAndMortyCharacters)
        model, cmd


open Feliz
open Feliz.Bulma

let displayOption<'t> (f: 't -> IReactProperty) (opt) = opt |> Option.toList |> (List.map f)
let displayOptionString = displayOption<string> prop.text
let displayOptionImage = displayOption prop.src

let navBar =
    Bulma.navbar [
        prop.style [
            style.backgroundColor "#FF3CAC"
            style.backgroundImage "linear-gradient(225deg, #FF3CAC 0%, #784BA0 50%, #2B86C5 100%)"
        ]
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    prop.style [ style.color "white"; style.fontSize 20 ]
                    prop.text "Client side GraphQL"
                ]
            ]
        ]
    ]

let characterView (character: GetCharacters.Character) =
    Bulma.box [
    prop.style [ style.height 300; style.width 200 ]
    prop.children [
        Html.img (displayOptionImage character.image)
        Html.div (displayOptionString character.name)
        Html.div (displayOptionString character.species)
        Html.div (displayOptionString character.status)
        ]
    ]

let charactersView (characters: GetCharacters.Character list) (dispatch: Msg -> unit) =
    Bulma.columns [
        columns.isMultiline
        prop.children [
        for character in characters do
        Bulma.column [
            characterView character
        ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    Html.div [
        prop.children [
            navBar
            Bulma.container [
                prop.style [
                    style.marginTop 50
                    style.display.flex
                    style.justifyContent.center
                    style.alignContent.center
                    style.flexDirection.column ]
                prop.children [
                    Bulma.box [
                        Bulma.columns [
                            Bulma.column [
                                column.is10
                                prop.children [
                                    Bulma.input.search [
                                        prop.value model.SearchValue
                                        prop.onChange (SetSearchValue >> dispatch)
                                    ]
                                ]
                            ]
                            Bulma.column [
                                column.is2
                                prop.children [
                                    Bulma.button.button [
                                        prop.style [ style.width (length.percent 100) ]
                                        prop.text "Search"
                                        prop.onClick (fun _ -> SearchCharacter |> dispatch)
                                    ]
                                ]
                            ]
                        ]
                    ]
                    match model.Characters with
                    | HasNotStartedYet -> Html.div "Has not started"
                    | InProgress -> Html.div "Loading"
                    | Resolved characters ->
                        match characters with
                        | [] -> Html.div "No characters with that name"
                        | _ -> charactersView characters dispatch
                    ]
            ]
        ]
    ]