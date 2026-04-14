FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/ECommerce.Architecture.Tests/ECommerce.Architecture.Tests.csproj", "tests/ECommerce.Architecture.Tests/"]
COPY ["src/ECommerce.Domain/ECommerce.Domain.csproj", "src/ECommerce.Domain/"]
COPY ["src/ECommerce.Application/ECommerce.Application.csproj", "src/ECommerce.Application/"]
COPY ["src/ECommerce.Contracts/ECommerce.Contracts.csproj", "src/ECommerce.Contracts/"]
COPY ["src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj", "src/ECommerce.Infrastructure/"]
COPY ["src/ECommerce.API/ECommerce.API.csproj", "src/ECommerce.API/"]
RUN dotnet restore "tests/ECommerce.Architecture.Tests/ECommerce.Architecture.Tests.csproj"
COPY . .
WORKDIR "/src/tests/ECommerce.Architecture.Tests"
RUN dotnet build "./ECommerce.Architecture.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerce.Architecture.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Architecture.Tests.dll"]
