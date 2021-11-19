# Web-api
This repository contains the REST API which the weekplanner uses to connect with the database through, here the web-api, server and database make out the backend of the project.

The web-api is written as a ASP.NET Core project and it is therefore written in C#. The current version of the project is .NET Core 3.1, ASP.NET Core 3.1 and EF Core 3.1, and it is not compatible with newer versions of these frameworks. The project consists of different controllers with endpoints for the requests made by the client, repositories for accessing the database through and models, which translates into the different entities/tables that are in the database.

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

## License

MIT License Copyright (c) 2018-present Aalborg University

## Workflow status

Dev - [![web-api dev Status](https://github.com/aau-giraf/web-api/workflows/Unit%20Test/badge.svg?branch=develop)](https://github.com/aau-giraf/web-api/actions)

Dev - [![web-api dev Status](https://github.com/aau-giraf/web-api/workflows/Integration%20Test/badge.svg?branch=develop)](https://github.com/aau-giraf/web-api/actions)

Dev - [![web-api dev Status](https://github.com/aau-giraf/web-api/workflows/Docker%20push/badge.svg?branch=develop)](https://github.com/aau-giraf/web-api/actions)
