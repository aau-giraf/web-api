## Synopsis

This repository contains the backend API for the Giraf Project. The API is a .net-core project written in C#.

To run the project locally with a MySQL database first do the following:

0. Prerequisites:
  - Download and install .NET Core 2.1.101 SDK
  - Download and install MySQL Server and MySQL Workbench
  - Create a database named giraf on the MySQL Server

1. Goto: /web-api/GirafRest

2. In a shell:
  - `cp appsettings.example.json appsettings.Development.json`
  - update appsettings.Development.json with connection string to the giraf database
  - `dotnet restore`
  - `dotnet database update`
  - `dotnet run --sample-data`

Once the API is running locally you can navigate to `http://localhost:5000/swagger/` to see and tryout requests to the endpoints

3. (Optional) To login on swagger:
  - Make a Account/Login request with valid login-info (username: `Tobias`, password: `password`)
  - Copy the `data` field containing the token.
  - Click on the green Authorize button (Or the padlocks)
  - Write `Bearer ` (note the space) in the input-field and paste your token. 
  - Click Authorize and close the pop-up. 
  - You are now authorized and can make autorized requests.

## Migrations (Only for developers of the API)
If changes has been made to the model classes then a new migration should be added to be able to update the database without losing data:
  - Goto: /web-api/GirafRest
  - `dotnet ef migrations add <Name-of-migration-describing-the-change>`
  - `dotnet ef database update`
  - If an exception is thrown then adjust the migration Up method in the file Migrations/...<Name-of-migration-describing-the-change>.cs to include the change of the model without triggering any MySQL exceptions
  - When the database is updated confirm that the migration <Name-of-migration-describing-the-change> is added to the table __efmigrationshistory in the giraf database
  - Now check that the Down method in the migration is also working properly by running `dotnet ef database <Name-of-the-previous-migration>`
  - If an exception is thrown then adjust the migration Down method in the file Migrations/...<Name-of-migration-describing-the-change>.cs to include the change of the model without triggering any MySQL exceptions
  - When the database is updated to the previous migration confirm that <Name-of-migration-describing-the-change> is no longer in the table __efmigrationshistory in the giraf database
  - Finally update the database to the newly added migration again `dotnet ef database update`. 
  - If an exception is thrown adjust the Up method of the migration again to fix the issue. 

## Generate Client
Because the REST-API integrates swagger as middle-ware it is possible to generate a client-side API in your prefered language. To do so start up the REST-API navigate to swagger: `http://localhost:5000/swagger/` and copy the url to the swagger json file on the top of the side.

To make a client download swagger-codegen and navigate to the folder:

You can now generate a client side API in for example C# by running the following command:   `java -jar modules/swagger-codegen-cli/target/swagger-codegen-cli.jar generate  -i http://localhost:5000/swagger/v1/swagger.json -l csharp -o Client/Generated/` 

## API Reference

For API reference start the API and navigate to localhost:5000/swagger

## Contributors

SW613f18 & SW615f17

## License

Copyright [2018] [Aalborg University]
