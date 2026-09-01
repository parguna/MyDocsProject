# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the files needed for restore first (better layer caching)
COPY common.props ./
COPY NuGet.Config ./
COPY src/MyDocsProject.Domain.Shared/MyDocsProject.Domain.Shared.csproj src/MyDocsProject.Domain.Shared/
COPY src/MyDocsProject.Domain/MyDocsProject.Domain.csproj src/MyDocsProject.Domain/
COPY src/MyDocsProject.Application.Contracts/MyDocsProject.Application.Contracts.csproj src/MyDocsProject.Application.Contracts/
COPY src/MyDocsProject.Application/MyDocsProject.Application.csproj src/MyDocsProject.Application/
COPY src/MyDocsProject.HttpApi.Client/MyDocsProject.HttpApi.Client.csproj src/MyDocsProject.HttpApi.Client/
COPY src/MyDocsProject.HttpApi/MyDocsProject.HttpApi.csproj src/MyDocsProject.HttpApi/
COPY src/MyDocsProject.EntityFrameworkCore/MyDocsProject.EntityFrameworkCore.csproj src/MyDocsProject.EntityFrameworkCore/
COPY src/MyDocsProject.Web/MyDocsProject.Web.csproj src/MyDocsProject.Web/

RUN dotnet restore src/MyDocsProject.Web/MyDocsProject.Web.csproj

# Now copy the rest of the source and publish
COPY src/ src/
RUN dotnet publish src/MyDocsProject.Web/MyDocsProject.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# openssl: generates the self-signed cert used for Kestrel TLS + OpenIddict signing (see docker-entrypoint.sh)
# curl: used by the compose healthcheck to probe /health-status from inside the container
RUN apt-get update \
    && apt-get install -y --no-install-recommends openssl curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
COPY src/MyDocsProject.Web/docker-entrypoint.sh /app/docker-entrypoint.sh

# /app/keys is where the persisted named volume (docker-compose.yml) mounts — a directory,
# since Docker can't mount a named volume onto a single file path that doesn't already
# exist as a file inside that volume. The entrypoint symlinks /app/openiddict.pfx (the
# literal relative path MyDocsProjectWebModule.cs already expects) into it, so no
# application source change is needed.
RUN mkdir -p /app/keys \
    && chmod +x /app/docker-entrypoint.sh \
    && chown -R $APP_UID:$APP_UID /app

USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["/app/docker-entrypoint.sh"]
