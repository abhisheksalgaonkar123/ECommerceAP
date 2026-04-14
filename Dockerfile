# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Remove any local build artifacts and IDE-generated dirs (including dirs with
# backslash in their name, e.g. Rider's "bin\Debug" Roslyn BuildHost output)
RUN find /src -type d \( -name 'bin' -o -name 'obj' \) -prune -exec rm -rf {} + 2>/dev/null; \
    find /src -mindepth 1 -type d | grep -F '\' | while IFS= read -r d; do rm -rf "$d"; done 2>/dev/null; \
    true

# Restore API project
RUN dotnet restore src/ECommerce.API/ECommerce.API.csproj

# Publish
RUN dotnet publish src/ECommerce.API/ECommerce.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]