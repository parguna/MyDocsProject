namespace MyDocsProject.Permissions;

public static class MyDocsProjectPermissions
{
    public const string GroupName = "MyDocsProject";

    public static class Dashboard
    {
        public const string Host = GroupName + ".Dashboard.Host";
        public const string Tenant = GroupName + ".Dashboard.Tenant";
    }
}
