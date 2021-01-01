# MyHealth

MyHealth is my personal health application that uses event-driven microservices architecture. I currently use a FitBit to capture health metrics (Activities, Sleep Activity).

## High-Level Architecture

Every month, I extract a csv file from fitbit.com containing my daily activities and sleep data. These csv files are then dropped into local directories that are picked up by .NET Core applications and uploaded to Blob Storage.

This triggers an event to EventGrid, which is then picked up by an Azure Function that processes the data and stores it in Azure Cosmos DB.

Diagram coming soon.

## Service List

### Docker Applications

| Application Name | Link |
| ---------------- | ---- |
| MyHealth.FileWatcher.Activity | [Code Link](https://github.com/willvelida/MyHealth/tree/main/MyHealth/MyHealth.FileWatcher.Activity) |

### Azure Functions

| Application Name | Link |
| ---------------- | ---- |
| MyHealth.DBSink.Activity | [Code Link](https://github.com/willvelida/MyHealth/tree/main/MyHealth/MyHealth.DBSink.Activity) |

### Custom NuGet Packages

| Application Name | Link | Build Status |
| ---------------- | ---- | ------------ |
| MyHealth.Helpers | [Code Link](https://github.com/willvelida/MyHealth/tree/main/MyHealth/MyHealth.Helpers) | [![Build Status](https://dev.azure.com/williamvelida/MyHealth/_apis/build/status/willvelida.MyHealth?branchName=main)](https://dev.azure.com/williamvelida/MyHealth/_build/latest?definitionId=27&branchName=main) |