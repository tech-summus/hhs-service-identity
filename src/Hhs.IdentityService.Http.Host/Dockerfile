FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV DOTNET_EnableDiagnostics=0 \
ASPNETCORE_URLS='http://+;' \
ASPNETCORE_ENVIRONMENT='docker'

USER root

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /build-source

COPY ["./nuget.config", "./"]
COPY ["./common.props", "./"]
COPY ["./common.version.props", "./"]

COPY ["./src/Hhs.IdentityService.Application/Hhs.IdentityService.Application.csproj", "./src/Hhs.IdentityService.Application/"]
COPY ["./src/Hhs.IdentityService.Domain/Hhs.IdentityService.Domain.csproj", "./src/Hhs.IdentityService.Domain/"]
COPY ["./src/Hhs.IdentityService.EntityFrameworkCore/Hhs.IdentityService.EntityFrameworkCore.csproj", "./src/Hhs.IdentityService.EntityFrameworkCore/"]
COPY ["./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj", "./src/Hhs.IdentityService.Http.Host/"]

RUN dotnet restore "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --verbosity minimal

COPY ["./src/Hhs.IdentityService.Application/.", "./src/Hhs.IdentityService.Application/"]
COPY ["./src/Hhs.IdentityService.Domain/.", "./src/Hhs.IdentityService.Domain/"]
COPY ["./src/Hhs.IdentityService.EntityFrameworkCore/.", "./src/Hhs.IdentityService.EntityFrameworkCore/"]
COPY ["./src/Hhs.IdentityService.Http.Host/.", "./src/Hhs.IdentityService.Http.Host/"]

RUN dotnet build "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --no-restore --configuration Release --verbosity minimal

RUN dotnet test "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --no-restore --no-build --configuration Release --verbosity minimal

RUN dotnet publish "./src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj" --no-restore --no-build --configuration Release --output /build-source/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build-stage /build-source/publish .
#RUN apt-get update && apt-get install -y curl
ENTRYPOINT ["dotnet", "Hhs.IdentityService.Http.Host.dll"]
