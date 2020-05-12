# Giraf Backend

![Build Status](https://github.com/aau-giraf/web-api/workflows/.NET%20Core/badge.svg)
[![codecov](https://codecov.io/gh/aau-giraf/web-api/branch/develop/graph/badge.svg)](https://codecov.io/gh/aau-giraf/web-api)


This repository contains the backend API for the Giraf Project. The API is a .net-core project written in C#.

## Build & Run Locally

To run the project locally with a MySQL database follow the steps provided in the Wiki: 
[Build and Run Locally](https://aau-giraf.github.io/wiki/development/rest_api_development/BuildAndRunLocally/)

## Migrations (Only for developers of the API)
If changes has been made to the database, new migrations should be added. Follow the guide in the Wiki on how to do migrations: 
[Migrations](https://aau-giraf.github.io/wiki/development/rest_api_development/Database/)

## Generate Client
In order to generate a client-side API follow the guide in the Wiki on how to do so: 
[Swagger Guide](https://aau-giraf.github.io/wiki/development/rest_api_development/Swagger/)

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
