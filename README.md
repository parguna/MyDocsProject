# MyDocsProject

## About this solution

This is a layered startup solution based on [Domain Driven Design (DDD)](https://abp.io/docs/latest/framework/architecture/domain-driven-design) practises. All the fundamental ABP modules are already installed. Check the [Application Startup Template](https://abp.io/docs/latest/solution-templates/layered-web-application) documentation for more info.

### Pre-requirements

* [.NET10.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 or 20](https://nodejs.org/en)

### Configurations

The solution comes with a default configuration that works out of the box. However, you may consider to change the following configuration before running your solution:

* Check the `ConnectionStrings` in `appsettings.json` files under the `MyDocsProject.Web` and `MyDocsProject.DbMigrator` projects and change it if you need.

### Before running the application

* Run `abp install-libs` command on your solution folder to install client-side package dependencies. This step is automatically done when you create a new solution, if you didn't especially disabled it. However, you should run it yourself if you have first cloned this solution from your source control, or added a new client-side package dependency to your solution.
* Run `MyDocsProject.DbMigrator` to create the initial database. This step is also automatically done when you create a new solution, if you didn't especially disabled it. This should be done in the first run. It is also needed if a new database migration is added to the solution later.

#### Generating a Signing Certificate

In the production environment, you need to use a production signing certificate. ABP Framework sets up signing and encryption certificates in your application and expects an `openiddict.pfx` file in your application.

To generate a signing certificate, you can use the following command:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 54bbfc6c-9352-445c-856e-1c0ca9e7de9e
```

> `54bbfc6c-9352-445c-856e-1c0ca9e7de9e` is the password of the certificate, you can change it to any password you want.

It is recommended to use **two** RSA certificates, distinct from the certificate(s) used for HTTPS: one for encryption, one for signing.

For more information, please refer to: [OpenIddict Certificate Configuration](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)

> Also, see the [Configuring OpenIddict](https://abp.io/docs/latest/Deployment/Configuring-OpenIddict#production-environment) documentation for more information.

### Solution structure

This is a layered monolith application that consists of the following applications:

* `MyDocsProject.DbMigrator`: A console application which applies the migrations and also seeds the initial data. It is useful on development as well as on production environment.
* `MyDocsProject.Web`: ASP.NET Core MVC / Razor Pages application that is the essential web application of the solution.

#### Test Projects

The `test` folder contains the following test projects:

* `MyDocsProject.Application.Tests`: Application layer tests.
* `MyDocsProject.Domain.Tests`: Domain layer tests.
* `MyDocsProject.EntityFrameworkCore.Tests`: Entity Framework Core integration tests.




## Deploying the application

Deploying an ABP application follows the same process as deploying any .NET or ASP.NET Core application. However, there are important considerations to keep in mind. For detailed guidance, refer to ABP's [deployment documentation](https://abp.io/docs/latest/Deployment/Index).

### Additional resources

You can see the following resources to learn more about your solution and the ABP Framework:

* [Web Application Development Tutorial](https://abp.io/docs/latest/tutorials/book-store/part-1)
* [Application Startup Template](https://abp.io/docs/latest/startup-templates/application/index)

## Docker Deployment

A self-contained `docker-compose.yml` runs the whole stack — SQL Server, the `MyDocsProject.DbMigrator` migration/seed job, and the `MyDocsProject.Web` application — with no dependency on any local SQL Server install. Both application images are built from source in multi-stage Dockerfiles (no pre-publish step required).

### Prerequisites

* Docker Desktop (or Docker Engine + Compose) with **Compose v2.20 or newer** — required for `depends_on: condition: service_completed_successfully`. Check with:
  ```bash
  docker compose version
  ```

### 1. Configure environment variables

```bash
cp .env.example .env
```

Edit `.env` and set real values:

* `SA_PASSWORD` — SQL Server SA password (must satisfy SQL Server's complexity policy: upper+lower+digit+symbol, 8+ characters).
* `CERT_PASSPHRASE` — passphrase for the self-signed certificate the `web` container generates on first run (used for both its HTTPS listener and OpenIddict token signing/encryption — see "OpenIddict & certificates" below).
* `STRING_ENCRYPTION_PASSPHRASE` — ABP's string-encryption passphrase. Keep this **stable** across restarts/redeploys, or previously encrypted data becomes unreadable.
* `DOCS_GITHUB_*` — optional. If `DOCS_GITHUB_ROOT_URL` and `DOCS_GITHUB_PROJECT_SHORT_NAME` are set, the migrator automatically registers a GitHub-backed Docs project on first run. Leave `DOCS_GITHUB_TOKEN` blank for a public repository; set it for a private one. The token is never written to any tracked file, never baked into an image, and never logged — it only flows through `.env` → the `dbmigrator` container's environment.

`.env` is git-ignored — never commit it.

### 2. Build the containers

```bash
docker compose build
```

### 3. Start Compose

```bash
docker compose up
```

(Add `-d` to run in the background once you've confirmed it comes up cleanly.)

### 4. Wait for database / migration

Watch the logs for, in order:

1. `sqlserver` becomes `healthy` in `docker compose ps` (its healthcheck runs `sqlcmd` against the database, not just "container started").
2. `dbmigrator` logs ABP's migration/seed output and **exits with code 0** — check with `docker compose ps -a` or:
   ```bash
   docker inspect --format='{{.State.ExitCode}}' $(docker compose ps -q dbmigrator)
   ```
3. `web` only starts after `dbmigrator` exits successfully (`depends_on: condition: service_completed_successfully`). Its first-run log includes `Generating a new self-signed certificate (first run only)...`; on every later start you'll instead see `Reusing existing persisted certificate...`.

### 5. Open the application

Browse **https://localhost:44335** (the container generates its own self-signed certificate, so your browser will show a certificate warning the first time — this is expected for a local self-signed cert; proceed past it). Log in with the seeded admin account:

* Username: `admin`
* Password: `1q2w3E*`

### 6. Verify Docs

If you configured `DOCS_GITHUB_*` in `.env`, browse:

```
https://localhost:44335/documents/en/<DOCS_GITHUB_PROJECT_SHORT_NAME>/latest
```

to confirm the GitHub-sourced documentation renders. You can also check the health endpoint from the host:

```bash
curl -k https://localhost:44335/health-status
```

### 7. Stop / restart

```bash
docker compose down          # stops and removes containers — SQL Server data and the cert both persist
docker compose up            # starts again against the same data, no re-seed, no new cert
docker compose restart web   # restart just the app container
```

### 8. Full reset / backup

```bash
docker compose down -v       # drops the SQL Server data volume AND the cert volume — next `up` is a clean re-seed
```

To back up the SQL Server data volume without tearing it down:

```bash
docker run --rm -v mydocsproject_sqlserver_data:/data -v "$PWD":/backup busybox tar czf /backup/sqlserver_data_backup.tar.gz /data
```

### Architecture notes

* **Connection strings** use the Compose service name (`Server=sqlserver;...`) with SQL authentication — not `localhost`, not Windows `Trusted_Connection` (which only makes sense on a Windows host with domain/local accounts, not inside a Linux container).
* **Database resilience**: the EF Core SQL Server provider has `EnableRetryOnFailure` enabled, so the app and migrator both retry transient connection failures (e.g. a brief SQL Server restart) instead of crashing outright — Compose's healthcheck-gated startup ordering handles the *first* connection, retry-on-failure handles everything after that.
* **OpenIddict & certificates**: the `web` container runs with `ASPNETCORE_ENVIRONMENT=Production`, so it needs a real certificate file (not ABP's ephemeral, regenerated-every-restart development certificate). On first run, the container generates one self-signed certificate via `openssl` and stores it on the persisted `openiddict_certs` volume; it's reused — never regenerated — on every later start, so restarting containers does not invalidate existing tokens or persisted authentication data. That same certificate is used both for the app's own HTTPS listener and for OpenIddict token signing/encryption — a deliberate simplification for a self-contained local/dev deployment. For a genuine production deployment, put a reverse proxy (nginx/Traefik) with a CA-issued certificate in front, and use a separate, dedicated signing key.
* **Networking**: all three services share an internal Compose network. Only the `web` service publishes a host port (`44335`); SQL Server is not reachable from outside the Docker network by default (uncomment the commented-out `ports:` block under `sqlserver` in `docker-compose.yml` if you need external DB tool access for local troubleshooting).
* **Non-root**: both the `web` and `dbmigrator` final images run as the built-in non-root `$APP_UID` user, not root.
* **`appsettings.secrets.json` caveat**: `MyDocsProject.DbMigrator.csproj` requires this file to exist at publish time, so it (currently empty, `{}`) is included in the Docker build context and image layer — unlike `.env`, it is *not* excluded via `.dockerignore`. All real secrets for the Docker deployment flow through `.env`/Compose environment variables instead. If you also use `appsettings.secrets.json` for local (non-Docker) secret overrides, keep that in mind before running `docker compose build`.
