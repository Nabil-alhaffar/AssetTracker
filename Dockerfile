# Use the official .NET 7 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy solution and projects first to leverage Docker caching
COPY *.sln ./
COPY AssetTracker/*.csproj ./AssetTracker/
RUN dotnet restore AssetTracker/AssetTracker.csproj

# Copy the rest of the app files and build the application
COPY AssetTracker/. ./AssetTracker/
WORKDIR /app/AssetTracker

# Generate the certificate before publishing
ARG PASSWORD_ENV_SEEDED
RUN dotnet dev-certs https -ep /app/aspnetapp.pfx -p ${PASSWORD_ENV_SEEDED}


RUN dotnet publish -c Release -o /app/publish

# Use the .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy the certificate from the build stage
COPY --chmod=0755 --from=build /app/aspnetapp.pfx /https/aspnetapp.pfx

# Copy the published application files from the build stage
COPY --from=build /app/publish /app

# Expose necessary ports
EXPOSE 80 443 5001

# Start the application
CMD ["dotnet", "AssetTracker.dll"]
