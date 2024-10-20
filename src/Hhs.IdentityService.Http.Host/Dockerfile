FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && apt-get install -y curl
RUN apt-get clean && rm -rf /var/lib/apt/lists/* && rm -rf /var/cache/apt/lists/*

WORKDIR /app
EXPOSE 80

ENV DOTNET_EnableDiagnostics=0 \
ASPNETCORE_URLS='http://+;' \
ASPNETCORE_ENVIRONMENT='docker'

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /build-source

RUN groupadd -r -g 1000 servicegroup
RUN useradd -r -u 1000 serviceuser -g servicegroup

COPY --chown=1000:1000 ["./nuget.config", "./"]
COPY --chown=1000:1000 ["./common.props", "./"]
COPY --chown=1000:1000 ["./common.version.props", "./"]

COPY --chown=1000:1000 ["./src/Hhs.IdentityService.Application/Hhs.IdentityService.Application.csproj", "./src/Hhs.IdentityService.Application/"]
COPY --chown=1000:1000 ["./src/Hhs.IdentityService.Domain/Hhs.IdentityService.Domain.csproj", "./src/Hhs.IdentityService.Domain/"]
COPY --chown=1000:1000 ["./src/Hhs.IdentityService.EntityFrameworkCore/Hhs.IdentityService.EntityFrameworkCore.csproj", "./src/Hhs.IdentityService.EntityFrameworkCore/"]
COPY --chown=1000:1000 ["./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj", "./src/Hhs.IdentityService.Http.Host/"]

ENV NUGET_CERT_REVOCATION_MODE=offline
RUN dotnet restore "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --verbosity minimal --configfile ./nuget.config

COPY --chown=1000:1000 ["./src/Hhs.IdentityService.Application/.", "./src/Hhs.IdentityService.Application/"]
COPY --chown=1000:1000 ["./src/Hhs.IdentityService.Domain/.", "./src/Hhs.IdentityService.Domain/"]
COPY --chown=1000:1000 ["./src/Hhs.IdentityService.EntityFrameworkCore/.", "./src/Hhs.IdentityService.EntityFrameworkCore/"]
COPY --chown=1000:1000 ["./src/Hhs.IdentityService.Http.Host/.", "./src/Hhs.IdentityService.Http.Host/"]

RUN dotnet build "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --no-restore --configuration Release --verbosity minimal

RUN dotnet test "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --no-restore --no-build --configuration Release --verbosity minimal

RUN dotnet publish "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --no-restore --no-build --configuration Release --output /build-source/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --chown=1000:1000 --from=build-stage /build-source/publish .

RUN chown -R 1000:1000 /app
RUN chmod 755 /app
USER 1000

ENTRYPOINT ["dotnet", "Hhs.IdentityService.Http.Host.dll"]
