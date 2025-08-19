using Grand.Domain.Localization;

namespace Grand.Module.Installer.Services;

public partial class InstallationService
{
    protected virtual async Task InstallLanguages()
    {
        var language = new Language {
            Name = "Vietnamese",
            LanguageCulture = "vi-VN",
            UniqueSeoCode = "vi",
            FlagImageFileName = "vn.png",
            Published = true,
            DisplayOrder = 1
        };
        await _languageRepository.InsertAsync(language);
    }
}