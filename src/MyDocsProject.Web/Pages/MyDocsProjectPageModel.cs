using MyDocsProject.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyDocsProject.Web.Pages;

public abstract class MyDocsProjectPageModel : AbpPageModel
{
    protected MyDocsProjectPageModel()
    {
        LocalizationResourceType = typeof(MyDocsProjectResource);
    }
}
