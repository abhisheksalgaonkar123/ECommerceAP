FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/ECommerce.Domain.Tests/ECommerce.Domain.Tests.csproj", "tests/ECommerce.Domain.Tests/"]
COPY ["src/ECommerce.Domain/ECommerce.Domain.csproj", "src/ECommerce.Domain/"]
RUN dotnet restore "tests/ECommerce.Domain.Tests/ECommerce.Domain.Tests.csproj"
COPY . .
WORKDIR "/src/tests/ECommerce.Domain.Tests"
RUN dotnet build "./ECommerce.Domain.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerce.Domain.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.Domain.Tests.dll"]
