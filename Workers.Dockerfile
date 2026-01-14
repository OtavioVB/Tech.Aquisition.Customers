FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ARG APP_UID=0
USER ${APP_UID}

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Tech.Aquisitions.Customers.Workers/Tech.Aquisitions.Customers.Workers.csproj", "src/Tech.Aquisitions.Customers.Workers/"]
COPY ["src/Tech.Aquisitions.Customers.Application/Tech.Aquisitions.Customers.Application.csproj", "src/Tech.Aquisitions.Customers.Application/"]
COPY ["src/Tech.Aquisitions.Customers.Domain/Tech.Aquisitions.Customers.Domain.csproj", "src/Tech.Aquisitions.Customers.Domain/"]
COPY ["src/Tech.Aquisitions.Customers.Infrascructure/Tech.Aquisitions.Customers.Infrascructure.csproj", "src/Tech.Aquisitions.Customers.Infrascructure/"]

RUN dotnet restore "src/Tech.Aquisitions.Customers.Workers/Tech.Aquisitions.Customers.Workers.csproj"

COPY . .

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/src/Tech.Aquisitions.Customers.Workers

RUN dotnet publish "Tech.Aquisitions.Customers.Workers.csproj" \
  -c ${BUILD_CONFIGURATION} \
  -o /app/publish \
  /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Tech.Aquisitions.Customers.Workers.dll"]
