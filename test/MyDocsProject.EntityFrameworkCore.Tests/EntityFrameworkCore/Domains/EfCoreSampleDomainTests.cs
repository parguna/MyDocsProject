using MyDocsProject.Samples;
using Xunit;

namespace MyDocsProject.EntityFrameworkCore.Domains;

[Collection(MyDocsProjectTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyDocsProjectEntityFrameworkCoreTestModule>
{

}
