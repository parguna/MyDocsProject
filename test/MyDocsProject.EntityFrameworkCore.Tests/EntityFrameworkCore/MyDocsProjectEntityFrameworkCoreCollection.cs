using Xunit;

namespace MyDocsProject.EntityFrameworkCore;

[CollectionDefinition(MyDocsProjectTestConsts.CollectionDefinitionName)]
public class MyDocsProjectEntityFrameworkCoreCollection : ICollectionFixture<MyDocsProjectEntityFrameworkCoreFixture>
{

}
