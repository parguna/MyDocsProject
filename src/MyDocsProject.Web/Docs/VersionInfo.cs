using System;
using System.Collections.Generic;

namespace MyDocsProject.Web.Docs;

public class VersionInfo
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public bool IsLatest { get; set; }
    public DateTime LastUpdateTime { get; set; }

    public VersionInfo(string name, string displayName, bool isLatest, DateTime lastUpdateTime)
    {
        Name = name;
        DisplayName = displayName;
        IsLatest = isLatest;
        LastUpdateTime = lastUpdateTime;
    }
}

public class LanguageConfig
{
    public string DefaultLanguage { get; set; }
    public List<string> Languages { get; set; }

    public LanguageConfig(string defaultLanguage, List<string> languages)
    {
        DefaultLanguage = defaultLanguage;
        Languages = languages;
    }
}