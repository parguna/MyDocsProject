using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Docs.GitHub.Projects;
using Volo.Docs.Projects;

namespace MyDocsProject.Docs;

/* Optionally registers a GitHub-backed Docs project from configuration/environment
 * variables (DocsGithub:*), so a fresh deployment (e.g. docker-compose) ends up with
 * its documentation source configured automatically instead of requiring a manual
 * Admin UI step. No-ops if DocsGithub:RootUrl isn't set. Never touches a project that
 * already exists, so manual Admin UI edits made after the first seed are preserved.
 */
public class DocsGithubProjectDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly IProjectRepository _projectRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<DocsGithubProjectDataSeedContributor> _logger;

    public DocsGithubProjectDataSeedContributor(
        IConfiguration configuration,
        IProjectRepository projectRepository,
        IGuidGenerator guidGenerator,
        ILogger<DocsGithubProjectDataSeedContributor> logger)
    {
        _configuration = configuration;
        _projectRepository = projectRepository;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        // Docs projects are a host-level concept (a shared documentation source), not per-tenant.
        if (context.TenantId != null)
        {
            return;
        }

        var section = _configuration.GetSection("DocsGithub");
        var rootUrl = section["RootUrl"];

        if (string.IsNullOrWhiteSpace(rootUrl))
        {
            return;
        }

        var shortName = section["ProjectShortName"];
        if (string.IsNullOrWhiteSpace(shortName))
        {
            _logger.LogWarning("DocsGithub:RootUrl is set but DocsGithub:ProjectShortName is missing — skipping GitHub docs project seed.");
            return;
        }

        if (await _projectRepository.ShortNameExistsAsync(shortName))
        {
            _logger.LogInformation("Docs project '{ShortName}' already exists — leaving it as-is.", shortName);
            return;
        }

        var name = section["ProjectName"].IsNullOrWhiteSpace() ? shortName : section["ProjectName"];
        var accessToken = section["AccessToken"];
        var userAgent = section["UserAgent"].IsNullOrWhiteSpace() ? "VoloDocs" : section["UserAgent"];
        var versionProviderSource = section["VersionProviderSource"].IsNullOrWhiteSpace() ? "Releases" : section["VersionProviderSource"];
        var format = section["Format"].IsNullOrWhiteSpace() ? "md" : section["Format"];
        var defaultDocumentName = section["DefaultDocumentName"].IsNullOrWhiteSpace() ? "index" : section["DefaultDocumentName"];
        var navigationDocumentName = section["NavigationDocumentName"].IsNullOrWhiteSpace() ? "docs-nav.json" : section["NavigationDocumentName"];
        var parametersDocumentName = section["ParametersDocumentName"].IsNullOrWhiteSpace() ? "docs-params.json" : section["ParametersDocumentName"];
        // Fallback version when GithubVersionProviderSource is "Releases" but the repo has no
        // GitHub Releases yet (e.g. a fresh/example repo) — without this, the version list is
        // empty and "/latest" has nothing to resolve to.
        var latestVersionBranchName = section["LatestVersionBranchName"].IsNullOrWhiteSpace() ? "master" : section["LatestVersionBranchName"];

        var project = new Project(
            _guidGenerator.Create(),
            name,
            shortName,
            "GitHub",
            format,
            defaultDocumentName,
            navigationDocumentName,
            parametersDocumentName)
        {
            LatestVersionBranchName = latestVersionBranchName
        };

        project.SetGitHubUrl(rootUrl);
        project.SetGitHubAccessToken(string.IsNullOrWhiteSpace(accessToken) ? null : accessToken);
        project.ExtraProperties["GitHubUserAgent"] = userAgent;
        project.ExtraProperties["GithubVersionProviderSource"] = versionProviderSource;

        await _projectRepository.InsertAsync(project);

        _logger.LogInformation(
            "Seeded GitHub docs project '{ShortName}' from configuration (access token supplied: {HasToken}).",
            shortName,
            !string.IsNullOrWhiteSpace(accessToken));
    }
}
