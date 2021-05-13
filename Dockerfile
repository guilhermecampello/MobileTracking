# https://hub.docker.com/_/microsoft-dotnet
# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet publish WebApplication -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/WebApplication/out ./
ENTRYPOINT ["dotnet", "WebApplication.dll"]
