# SAFE Ricky and Morty

This repository demos the use of [GraphQL](https://graphql.org/) in F# application - using [Rick and Morty Api](https://rickandmortyapi.com/documentation/) as an example. The site is built with [SAFE Template](https://safe-stack.github.io/docs/template-overview/) and [Snowflaqe](https://github.com/Zaid-Ajaj/Snowflaqe), following the tutorial by @CompositionalIT: https://www.compositional-it.com/news-blog/client-side-graphql-with-f/

## Prerequisites

* [.NET Core SDK](https://www.microsoft.com/net/download) 5.0 or higher
* [Node LTS](https://nodejs.org/en/download/)

## Development

```bash
# for the first time only
dotnet tool restore

# development - http://localhost:8080
dotnet run

# production
dotnet run bundle
```
