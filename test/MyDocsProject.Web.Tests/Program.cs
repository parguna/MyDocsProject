using Microsoft.AspNetCore.Builder;
using MyDocsProject;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("MyDocsProject.Web.csproj"); 
await builder.RunAbpModuleAsync<MyDocsProjectWebTestModule>(applicationName: "MyDocsProject.Web");

public partial class Program
{
}
