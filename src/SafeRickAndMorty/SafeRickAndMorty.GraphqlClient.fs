namespace SafeRickAndMorty

open Fable.SimpleHttp
open Fable.SimpleJson

type GraphqlInput<'T> = { query: string; variables: Option<'T> }
type GraphqlSuccessResponse<'T> = { data: 'T }
type GraphqlErrorResponse = { errors: ErrorType list }

type SafeRickAndMortyGraphqlClient(url: string, headers: Header list) =
    /// <summary>Creates SafeRickAndMortyGraphqlClient specifying list of headers</summary>
    /// <remarks>
    /// In order to enable all F# types serialization and deserialization, this client uses Fable.SimpleJson from <a href="https://github.com/Zaid-Ajaj/Fable.SimpleJson">Fable.SimpleJson</a>
    /// </remarks>
    /// <param name="url">GraphQL endpoint URL</param>
    new(url: string) = SafeRickAndMortyGraphqlClient(url, [ ])
    member _.GetCharacters(input: GetCharacters.InputVariables) =
        async {
            let query = """
                query GetCharacters($name: String!) {
                    characters(filter: { name: $name }) {
                        results {
                            name
                            image
                            id
                            species
                            status
                        }
                    }
                }
            """
            let! response =
                Http.request url
                |> Http.method POST
                |> Http.headers [ Headers.contentType "application/json"; yield! headers ]
                |> Http.content (BodyContent.Text (Json.serialize { query = query; variables = Some input }))
                |> Http.send

            match response.statusCode with
            | 200 ->
                let response = Json.parseNativeAs<GraphqlSuccessResponse<GetCharacters.Query>> response.responseText
                return Ok response.data

            | errorStatus ->
                let response = Json.parseNativeAs<GraphqlErrorResponse> response.responseText
                return Error response.errors
        }
