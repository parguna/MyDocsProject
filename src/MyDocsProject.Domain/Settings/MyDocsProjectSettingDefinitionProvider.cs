using Volo.Abp.Settings;

namespace MyDocsProject.Settings;

public class MyDocsProjectSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MyDocsProjectSettings.MySetting1));
    }
}
