using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace MyDocsProject.Pages;

[Collection(MyDocsProjectTestConsts.CollectionDefinitionName)]
public class Index_Tests : MyDocsProjectWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
