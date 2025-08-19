using Grand.Domain.Localization;

namespace Grand.Module.Installer.Services;

public partial class InstallationService
{
    protected virtual async Task InstallLanguages()
    {
        var languages = new List<Language>
        {new Language
        {
            Name = "Vietnamese",
            LanguageCulture = "vi-VN",
            UniqueSeoCode = "vi",
            FlagImageFileName = "vn.png",
            Published = true,
            DisplayOrder = 1
        },
        new Language
        {
            Name = "English",
            LanguageCulture = "en-US",
            UniqueSeoCode = "en",
            FlagImageFileName = "us.png",
            Published = true,
            DisplayOrder = 2
        }
        };
        foreach (var language in languages)
        {
            await _languageRepository.InsertAsync(language);
        }
    }
}