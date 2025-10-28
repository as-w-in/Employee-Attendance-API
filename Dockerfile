# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy all files and restore dependencies
COPY . .
RUN dotnet restore

# Build the release version
RUN dotnet publish -c Release -o out

# Use a lightweight runtime image for final deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy built files from the previous stage
COPY --from=build /app/out .

# Expose port 8080 for Render
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "api-demo.dll"]
