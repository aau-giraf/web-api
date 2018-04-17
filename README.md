## Synopsis

This repository contains the backend API for the Giraf Project. The API is a .net-core project written in C#.

To run the project locally with a MySQL database first do the following:

0. Prerequisites:
  - Download and install .NET Core 2.1.101 SDK
  - Download and install MySQL Server and optionally MySQL Workbench or another database manager
  - Create a database named giraf on the MySQL Server. This can be done from the workbench or via cli.

1. open a terminal-emulator and navigate to {project-root}/web-api/GirafRest

2. In a shell:
  - run `cp appsettings.template.json appsettings.Development.json`
  - update appsettings.Development.json with 
	- All fields <> in the"DefaultConnection" string.
    - Jwt.JwtKey field, which must be any (random) string of, at least 40, alpha-numeric charecters
    - Jwt.JwtIssuer field, which is your name or organization. For example "Aalborg University"
  - run `dotnet restore`
  - run `dotnet ef database update`
  - run `dotnet run --sample-data`

Once the API is running locally you can navigate to `http://localhost:5000/swagger/` to see and tryout requests to the endpoints

3. (Optional) To login via swagger:
  - Make a Account/Login request with valid login-info (username: `Tobias`, password: `password`)
  - Copy the `data` field containing the token.
  - Click on the green Authorize button (Or the padlocks)
  - Write `bearer [your-token]` (note the space) in the input-field. 
  - Click Authorize and close the pop-up. 
  - You are now authorized and can make autorized requests.

## Migrations (Only for developers of the API)
If changes has been made to the model classes then a new migration should be added to be able to update the database without losing data:
  - In a shell, navigate to: `.../web-api/GirafRest`
  - `dotnet ef migrations add NameOfMigrationDescribingTheChange`
  - `dotnet ef database update`
  - If an exception is thrown then adjust the migration Up method in the file `Migrations/...NameOfMigrationDescribingTheChange.cs` to include the change of the model without triggering any MySQL exceptions. It may be good to inspect the file in any case, to see that it will function as expected.
  - When the database is updated confirm that the migration `NameOfMigrationDescribingTheChange` is added to the table `__efmigrationshistory` in the giraf database.
  - Now check that the Down method in the migration is also working properly. Determine the name of the last migration before yours, looking at the date and time prefixes in `ls Migrations/MySQL`. If it is `20180409123114_PreviousMigration.cs`, then you must run `dotnet ef database update PreviousMigration`.
  - If an exception is thrown then adjust the migration Down method in the file `Migrations/...NameOfMigrationDescribingTheChange.cs` to include the change of the model without triggering any MySQL exceptions.
  - When the database is updated to the previous migration confirm that `NameOfMigrationDescribingTheChange` is no longer in the table `__efmigrationshistory` in the giraf database.
  - Finally update the database to the newly added migration again using `dotnet ef database update`. 
  - If an exception is thrown adjust the Up method of the migration again to fix the issue. 

## Generate Client
Because the REST-API integrates swagger as middle-ware it is possible to generate a client-side API in your prefered language. To do so start up the REST-API navigate to swagger: `http://localhost:5000/swagger/` and copy the url to the swagger json file on the top of the side.

To make a client download swagger-codegen and navigate to the folder:

You can now generate a client side API in for example C# by running either of the following commands:
  - `java -jar modules/swagger-codegen-cli/target/swagger-codegen-cli.jar generate  -i http://localhost:5000/swagger/v1/swagger.json -l csharp -o Client/Generated/` 
  - `swagger-codegen generate -i http://localhost:5000/swagger/v1/swagger.json -l csharp`

## API Reference

For API reference start the API and navigate to localhost:5000/swagger

## Contributors

SW613f18 & SW615f17

## License

Copyright [2018] [Aalborg University]
