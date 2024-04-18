FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj", "src/Hhs.IdentityService.Http.Host/"]
RUN dotnet restore "src/Hhs.IdentityService.Http.Host/Hhs.IdentityService.Http.Host.csproj"
COPY . .
WORKDIR "/src/src/Hhs.IdentityService.Http.Host"
RUN dotnet build "Hhs.IdentityService.Http.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hhs.IdentityService.Http.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hhs.IdentityService.Http.Host.dll"]
