using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using MyDocsProject.Localization;

namespace MyDocsProject.Web;

[Dependency(ReplaceServices = true)]
public class MyDocsProjectBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyDocsProjectResource> _localizer;

    public MyDocsProjectBrandingProvider(IStringLocalizer<MyDocsProjectResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
