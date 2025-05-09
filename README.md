# HackerNews Best Stories API

This project is an ASP.NET Core Web API that exposes a single endpoint to fetch the top N best stories from the Hacker News API, sorted by score.

# Features

- Fetches top N stories from Hacker News /v0/beststories.json

- Retrieves details via /v0/item/{id}.json

- Sorts stories by score (descending)

- Includes caching to reduce load on Hacker News API

- Handles concurrency via SemaphoreSlim

- Swagger (OpenAPI) enabled

- Unit tests using xUnit and Moq

# How to Run

## Requirements

[.NET 8 or higher](https://dotnet.microsoft.com/en-us/download)

Git

## Run the app

```
git clone https://github.com/sergey-sokolov-by/DeveloperCodingTask.git
cd DeveloperCodingTask/HackerNewsBestStoriesApi

dotnet restore

dotnet build HackerNewsBestStoriesApi.sln

dotnet run --project HackerNewsBestStoriesApi/HackerNewsBestStoriesApi.csproj
```

Then open Swagger UI at:
```
https://localhost:50372/swagger
```
Or you can use any other API client

Request example:
```
GET /api/beststories/top/20
```

# Assumptions Made

- The project is expected to remain relatively small in scope, so a lightweight architecture was chosen.

- The Hacker News /beststories.json endpoint returns stories in descending order of score (checked manually).

- Cache timeouts of 10 seconds for the beststories list and 1 minute for individual story details are sufficient for responsiveness while reducing API load.

# Possible Enhancements

- Refactor toward Clean Architecture if the project grows in complexity or requires long-term maintainability.

- Introduce retry policies (e.g. Polly) on HTTP client

- Paginate response or support offset-based query

- Improve exception handling & fallback behaviors

- Add structured logging (e.g. Serilog)

# Test Coverage

Run tests:
```
dotnet test HackerNewsBestStoriesApi.Tests/HackerNewsBestStoriesApi.Tests.csproj
```