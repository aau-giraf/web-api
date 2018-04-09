# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/).

[//]: # ( ## [x.y.z] - yyyy-mm-dd)
[//]: # (Describe each version with the following sections: Added, Changed, Removed, Deprecated, Fixed, Security)

## [Unreleased]

### Added
- 

### Changed
- All pictograms in the sample database are now public

### Removed

## [1.002.01] - Unreleased
### Added
- Pictogram search and pagination, improving efficiency of pictogram retrieval
- Endpoint to retrieve user icons seperately from the main user request
- Add version number prefix to API endpoints
- Add templates for weeks and corresponding controller

### Changed
- Citizens can now have more than one guardian
- Pictogram images are no longer returned in the index/search request. Instead the image data can be retrieved by visiting the `pictogram/{id}/image` endpoint.
- Change development database provider from SQLite to MySQL
- Logout requests now won't throw an error if you are not logged in
- Authentication tokens are now the developers responsibility instead of the browser/cookie-manager
- `/v1/Departmnt` now returns list of name and ids on all departments
- `/v1/Week` now returns list of name and ids on all weeks
- `GirafUserDTO` has been changed s.t. it only contains meta data on the user and the usericon

### Fixed
- Public pictograms no longer require authorization to read
- An exception is no longer raised when trying to add a weekday that is not valid instead an errorcode is returned
- Anyone can no longer add a user to a department

## [1.001.01] - 2018-03-12
### Added
- Images on pictograms in the sample databse
- Erronius requests now get HTTP code 200 (OK) and errors are instead indicated via JSON objects.
  All responses follow the same format of `{"success":..., "errorProperties":..., "errorKey":... }`
- [Swagger](https://swagger.io/) - Makes it easier to discover and test the REST API through a webinterface as well as being 
  able to automatically generate client implementations.
- Endpoint to access png format of pictograms as opposed to a base64 encoded string

### Changed
- **Migrates to dotnet core version 2**
- Implements the [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/) git workflow instead of simply working on the `master` branch.
- Makes it possible to send login request even if already logged in. *(makes the API less pedantic)*

## [0.001.01] - 2017-05-22
2017 version of the API.
Changes will be tracked from this point onwards.


[//]: # ( List of link specifications, wont be rendered )
[Unreleased]: http://git.giraf.cs.aau.dk/Giraf-Rest/web-api/compare/v1.001.01...develop
[1.001.01]: http://git.giraf.cs.aau.dk/Giraf-Rest/web-api/compare/v0.001.01...v1.001.01
[0.001.01]: http://git.giraf.cs.aau.dk/Giraf-Rest/web-api/compare/v0...v0.001.01
