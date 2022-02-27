namespace rec SafeRickAndMorty

[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type CacheControlScope =
    | [<CompiledName "PUBLIC">] Public
    | [<CompiledName "PRIVATE">] Private

type FilterCharacter =
    { name: Option<string>
      status: Option<string>
      species: Option<string>
      ``type``: Option<string>
      gender: Option<string> }

type FilterLocation =
    { name: Option<string>
      ``type``: Option<string>
      dimension: Option<string> }

type FilterEpisode =
    { name: Option<string>
      episode: Option<string> }

/// The error returned by the GraphQL backend
type ErrorType = { message: string }
