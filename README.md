## Synopsis

This repository contains the backend API for the Giraf Project. The API is a .net-core project.

To run the project locally with a sqlite database first do the following:

1. Goto: /web-api/GirafRest

2. In a shell:
  - `cp appsettings.example.json appsettings.Development.json`
  - update appsettings.Development.json as necessary
  - `dotnet restore`
  - `dotnet run --sample-data`

Once the API is running locally you can navigate to `http://localhost:5000/swagger/` to see and tryout requests to the endpoints

## Migrations Sqlite
  Add migration:
  -  `dotnet ef migrations add <MigrationName> -o Migrations --context GirafSqliteDbContext`

  Update local Sqlite db:
  - Delete /GirafRest/bin and /GirafRest/Giraf.db
  - `dotnet run`

## Generate Client
Because the REST-API integrates swagger as middle-ware it is possible to generate a client-side API in your prefered language. To do so start up the REST-API navigate to swagger: `http://localhost:5000/swagger/` and copy the url to the swagger json file on the top of the side.

To make a client download swagger-codegen and navigate to the folder:

You can now generate a client side API in for example C# by running the following command:   `java -jar modules/swagger-codegen-cli/target/swagger-codegen-cli.jar generate  -i http://localhost:5000/swagger/v1/swagger.json -l csharp -o Client/Generated/
` 

## Installation

Make sure you have .netcore v2 and that you restore nugets: `dotnet restore`

## API Reference

For API reference start the API and navigate to localhost:5000/swagger

## Contributors

SW613f18 & SW615f17

## License

Copyright [2018] [Aalborg University]
