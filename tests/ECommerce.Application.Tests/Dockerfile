FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/ECommerce.Application.Tests/ECommerce.Application.Tests.csproj", "tests/ECommerce.Application.Tests/"]
COPY ["src/ECommerce.Application/ECommerce.Application.csproj", "src/ECommerce.Application/"]
COPY ["src/ECommerce.Domain/ECommerce.Domain.csproj", "src/ECommerce.Domain/"]
COPY ["src/ECommerce.Contracts/ECommerce.Contracts.csproj", "src/ECommerce.Contracts/"]
RUN dotnet restore "tests/ECommerce.Application.Tests/ECommerce.Application.Tests.csproj"
COPY . .
WORKDIR "/src/tests/ECommerce.Application.Tests"
RUN dotnet build "./ECommerce.Application.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerce.Application.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Application.Tests.dll"]
