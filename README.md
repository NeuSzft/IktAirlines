# :airplane: Airlines Management Project

[<img src="https://trello.com/favicon.ico" width="12"/> Trello](https://trello.com/b/dmFaW9eT/iktairlines)\
[<img src="https://static.figma.com/app/icon/1/favicon.svg" width="12"/> Figma](https://www.figma.com/design/m6DMnSU3LX5Ax3IFF2Gs4B/Airlines?node-id=0-1&t=AlGC7EdEvBNXLnfR-0)

### Table of Contents
- [Overview](#overview)
- [Database API](#database-api)
- [Web Interface](#web-interface)
- [Desktop Application](#desktop-application)
- [Getting started](#getting-started)
- [Testing](#testing)

## Overview

This project contains an API for managing a PostgreSQL database, a web interface for seeing available flights and their prices, and a desktop application that provides a graphical interface for managing the database trough the API.

## Database API

The API is made using [APS.NET Core](https://github.com/dotnet/aspnetcore) and it's [Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-8.0) approach.

List of available [endpoints](./AirportManagement/docs/endpoints.md).

## Web Interface

[User manual](./AirportWeb/user-manual.md)

## Desktop Application

The desktop application's purpose is to make adding, updating and removing entries from the database a simple task.

It is made using the [Windows Presentation Foundation](https://github.com/dotnet/wpf) framework and is only available for windows machines.

Check the [user manual](./AirportManagement/docs/desktop-app-manual.md) for help.

## Getting started

The database, API and web server can be run using [docker compose](https://github.com/docker/compose) and the [compose.yml](./compose.yml) compose file.
Run `docker compose up` command to launch the containers.
> It can also be done by running the [run.sh](./run.sh) shell script.

Local ports used by the services:
- 5432 - PostgreSQL database
- 5000 - Database API
- 8080 - Web server

## Testing

The tests can be run using [docker compose](https://github.com/docker/compose) and the appropriate compose file. Hoverver it is advised to use the shell scripts instead.

| Test | Compose File | Shell Script | Latest Results |
| --- | --- | --- | --- |
| API tests | [compose.test-api.yml](./compose.test-api.yml) | [test-api.sh](./test-api.sh) | [view](./AirportManagement/api-test-results.md) |
| Selenium tests | [compose.test-web.yml](./compose.test-web.yml) | [test-web.sh](./test-web.sh) | [view](./AirportWebTest/web-selenium-test-results.md) |
