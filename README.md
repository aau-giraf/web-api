## Synopsis

This repository contains the backend API for the Giraf Project. The API is a .net-core project.

To run the project locally with a sqlite database first do the following:

1. Goto: /web-api/GirafRest

2. In a shell:
  - `export ASPNETCORE_ENVIRONMENT=Development`
  - `cp appsettings.example.json appsettings.Development.json`
  - update appsettings.Development.json as necessary
  - `dotnet restore`
  - `dotnet run --sample-data`

Once the API is running locally you can navigate to `http://localhost:5000/swagger/` to see and tryout requests to the endpoints

## Migrations Sqlite
  Add migration:
  - `dotnet ef migrations add InitialMigration -o Migrations/Sqlite -e Development -c GirafSqliteDbContext`

  Update:
  - `dotnet ef database update InitialMigration -c GirafSqliteDbContext -e Development`

## Generate Client
Because the REST-API integrates swagger as middle-ware it is possible to generate a client-side API in your prefered language. To do so start up the REST-API navigate to swagger: `http://localhost:5000/swagger/` and copy the url to the swagger json file on the top of the side.

To make a client download swagger-codegen and navigate to the folder:

You can now generate a client side API in for example C# by running the following command:   `java -jar modules/swagger-codegen-cli/target/swagger-codegen-cli.jar generate  -i http://localhost:5000/swagger/v1/swagger.json -l csharp -o Client/Generated/
` 

## Motivation

## Installation

Make sure you have .netcore v2 and that you restore nugets: `dotnet restore`

## API Reference

For API reference start the API and navigate to localhost:5000/swagger

## Contributors

Auhtor's of the project:

## License

Copyright [yyyy] [name of copyright owner]

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
