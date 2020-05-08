# Giraf Backend

![Build Status](https://github.com/aau-giraf/web-api/workflows/.NET%20Core/badge.svg)

This repository contains the backend API for the Giraf Project. The API is a .net-core project written in C#.

## Build & Run

To run the project locally with a MySQL database first do the following:

0. Prerequisites:
  - Download and install .NET Core 3.1 SDK or a version backwardly compatible with it (https://dotnet.microsoft.com/download/dotnet-core/3.1).
  - Download and install .NET Core ef tools by going to terminal and write "dotnet tool install --global dotnet-ef" after having installed .NET core. 
  - Download MySQL installer (https://dev.mysql.com/downloads/installer/).
    * Install MySQL server 8.0, under the setup, create a root account with password "password", and add a user with username "user" with password "password".
    * (Optional) Install Workbench.
  - For Linux users:
      * Run `apt install libc6-dev` and `apt install libgdiplus` to install dependecies required by System.Drawing.Common.
  - For Mac users:
      * Run `brew install mono-libgdiplus` to install dependecies required by System.Drawing.Common.
    
1. Clone the web_api repository from GitHub.

2. Setup the connection to the local MySQL server.
  - In the `…\web-api\GirafRest` create a copy of the file `appsettings.template.json` and name it `appsettings.Development.json`.
  - Open the created file "appsettings.Development.json" file with a text editor.
  - Change the following (Remember to remove the "<" and ">"): 
    * The DefaultConnection on line 3 making it use the previously setup database name and user, change to: `"DefaultConnection": "server=localhost;port=3306;userid=user;password=password;database=giraf;Allow User Variables=True"`
    * The `Jwt.JwtKey` on line 24 to be any (random) string of, at least 40, alpha-numeric characters.
    * The `Jwt.JwtIssuer` on line 25 to your name or organization. For example "Aalborg University"
    * The `IpRateLimiting.GeneralRules.Limit` to 2000
 
3. Open a terminal and navigate to …\ web-api\GirafRest folder.
  - Run `dotnet restore`  
  - Run `dotnet ef database update`
  - Run `dotnet run --sample-data`

The flags that can be used for the run:

        --prod=[true|false]   | If true then connect to production db, defaults to false.
        --port=integer        | Specify which port to host the server on, defaults to 5000.
        --list                | List options.
        --sample-data         | Tells the rest-api to generate some sample data. This only works on an empty database.
        --pictograms=integer  | Specify how many sample pictograms to generate. Default is 200. Only works when --sample-data is set.
        --logfile=string      | Toggles logging to a file, the string specifies the path to the file relative to the working directory.

Once the API is running locally you can navigate to http://localhost:5000/swagger/ to see and tryout requests to the endpoints. We recommend keeping a text file with often-used DTOs and bearer tokens as part of your workflow.

4. (Optional) To login via swagger:
  - Make an Account/Login request with valid login-info (username: `Tobias`, password: `password`)
  - Copy the `data` field containing the token.
  - Click on the green Authorize button (Or the padlocks).
  - Write `bearer [your-token]` (note the space) in the input-field. 
  - Click Authorize and close the pop-up. 
  - You are now authorized and can make authorized requests.

5. Let weekplanner use the local database and Giraf web_api server.
  - In the weekplanner repository in the …/weekplanner/assets/environments.json file line 2 change the "http://srv.giraf.cs.aau.dk/DEV/API" to:
    * If using an Android emulator: "http://10.0.2.2:5000". 
    * If using a hardware device: Turn on "Use USB Tethering" in the device under networks settings. Next get your computers local ip under network settings, this should be used in the environments.json file followed by ":5000", E.g. "http://192.168.42.130:5000". 
  - If you are having problems in this step and are using either Android Studio or IntelliJ, delete the build-folder created by the IDE and rebuild it. This should make sure that correct environments.json file is read.  

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

Descriptions are generated from method summaries etc. in the code.

## Admin Panel
The Admin panel is currently hosted together with the API and the login page is located at: `host/admin/login.html`
Through the admin panel it is possible to adminstrate users and departments of Giraf and different roles in the system has different functionality and access rights.
Note that it is possible to generate a reset link for users that you have access to. This link is intended to only work if accessed by an unauthorised user.
For more information see the project report of group sw613f18

## Contributors

SW615f20, SW613f18 & SW615f17

## License
MIT License
Copyright (c) 2018-present Aalborg University
