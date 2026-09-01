# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY common.props ./
COPY NuGet.Config ./
COPY src/MyDocsProject.Domain.Shared/MyDocsProject.Domain.Shared.csproj src/MyDocsProject.Domain.Shared/
COPY src/MyDocsProject.Domain/MyDocsProject.Domain.csproj src/MyDocsProject.Domain/
COPY src/MyDocsProject.Application.Contracts/MyDocsProject.Application.Contracts.csproj src/MyDocsProject.Application.Contracts/
COPY src/MyDocsProject.EntityFrameworkCore/MyDocsProject.EntityFrameworkCore.csproj src/MyDocsProject.EntityFrameworkCore/
COPY src/MyDocsProject.DbMigrator/MyDocsProject.DbMigrator.csproj src/MyDocsProject.DbMigrator/

RUN dotnet restore src/MyDocsProject.DbMigrator/MyDocsProject.DbMigrator.csproj

COPY src/ src/
RUN dotnet publish src/MyDocsProject.DbMigrator/MyDocsProject.DbMigrator.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN chown -R $APP_UID:$APP_UID /app
USER $APP_UID

ENTRYPOINT ["dotnet", "MyDocsProject.DbMigrator.dll"]
