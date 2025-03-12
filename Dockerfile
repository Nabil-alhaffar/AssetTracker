# Use the official .NET 7 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.sln ./
COPY AssetTracker/*.csproj ./AssetTracker/
RUN dotnet restore AssetTracker/AssetTracker.csproj

# Copy everything else and build the application
COPY AssetTracker/. ./AssetTracker/
WORKDIR /app/AssetTracker
RUN dotnet publish -c Release -o /app/publish

# Use the .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
CMD ["dotnet", "AssetTracker.dll"]
