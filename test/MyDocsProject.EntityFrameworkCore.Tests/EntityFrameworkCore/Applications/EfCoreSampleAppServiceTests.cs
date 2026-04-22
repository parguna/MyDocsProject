using MyDocsProject.Samples;
using Xunit;

namespace MyDocsProject.EntityFrameworkCore.Applications;

[Collection(MyDocsProjectTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyDocsProjectEntityFrameworkCoreTestModule>
{

}
