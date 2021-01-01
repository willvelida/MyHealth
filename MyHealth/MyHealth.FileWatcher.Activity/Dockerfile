FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app

COPY *.csproj ./
COPY nuget.config ./
RUN dotnet restore

FROM base AS final
RUN dotnet publish -c Release -o out

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MyHealth.FileWatcher.Activity.dll"]