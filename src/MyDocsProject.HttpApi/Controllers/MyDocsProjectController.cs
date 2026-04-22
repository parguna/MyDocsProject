using MyDocsProject.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyDocsProject.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyDocsProjectController : AbpControllerBase
{
    protected MyDocsProjectController()
    {
        LocalizationResource = typeof(MyDocsProjectResource);
    }
}
