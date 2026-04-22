using MyDocsProject.Localization;
using Volo.Abp.Application.Services;

namespace MyDocsProject;

/* Inherit your application services from this class.
 */
public abstract class MyDocsProjectAppService : ApplicationService
{
    protected MyDocsProjectAppService()
    {
        LocalizationResource = typeof(MyDocsProjectResource);
    }
}
