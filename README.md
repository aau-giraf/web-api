# Web-api
This repository contains the REST API which the weekplanner uses to connect with the database through, here the web-api, server and database make out the backend of the project.

The web-api is written as a .NET project and it is therefore written in C#. The current version of the project is .NET 8 and EF6, and it is not compatible with newer versions of these frameworks. The project consists of different controllers with endpoints for the requests made by the client, repositories for accessing the database through and models, which translates into the different entities/tables that are in the database.

An explination of the structure of the web-api can be found on the [wiki](https://aau-giraf.github.io/wiki/Development/Web_API/), and a setup guide can be found [here](https://aau-giraf.github.io/wiki/Getting_Started/setup/#web-api).

# Branches
This repository uses the scaled trunkbased branching strategy, as explained here: [Github setup](https://github.com/aau-giraf/.github/blob/main/wiki/about/github.md). In this repository the "trunk" is named develop, and this is the branch that all developers should branch from when solving an issue. The naming convention for these branches are:

| Issue type | Name                   | Example     |
| :--------: | :--------------------- | :---------: |
| User Story | feature/\<issue-number\> | feature/697 |
| Task       | task/\<issue-number\>    | task/918    |
| Bug fix    | bug-fix/\<issue-number\> | bug-fix/299 |

Other than the branches being used for development and the trunk, there exists some release branches, where the newest release branch is running on the PROD-environment. The release branches can only be created by members of the release group in the organization, and they should adhere to the following naming convention:
- Naming is release-\<release-version\> fx release-1.0
- A hot-fix on a release will increment the number after the dot (.)
- A new release will increment the number before the dot (.)

# Running with Docker
To run the wep-api with Docker, clone the repository and navigate into the web-api folder.

Ensure to have Docker Desktop or Docker and Docker compose installed and running.

In the terminal write the command: `docker compose up -d`

The Docker compose will setup the database and seed it with sample data before starting the web-api.

**The database can be accessed from port 5100**

You can confirm the web-api is running by going to the [swagger endpoint](http://localhost:5000/swagger).

## License

MIT License Copyright (c) 2018-present Aalborg University

## Workflow status

Dev - [![web-api dev Status](https://github.com/aau-giraf/web-api/workflows/Unit%20Test/badge.svg?branch=develop)](https://github.com/aau-giraf/web-api/actions)

Dev - [![web-api dev Status](https://github.com/aau-giraf/web-api/workflows/Integration%20Test/badge.svg?branch=develop)](https://github.com/aau-giraf/web-api/actions)

Dev - [![web-api dev Status](https://github.com/aau-giraf/web-api/workflows/Docker%20push/badge.svg?branch=develop)](https://github.com/aau-giraf/web-api/actions)
