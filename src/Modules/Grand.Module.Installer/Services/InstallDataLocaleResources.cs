using Grand.Domain.Localization;
using Grand.SharedKernel.Extensions;

namespace Grand.Module.Installer.Services;

public partial class InstallationService
{
    protected virtual async Task InstallLocaleResources()
    {
        await InstallEnglishLocale();
        await InstallVietnameseLocale();
    }


    private async Task InstallEnglishLocale()
    {
        //'English' language
        var language = _languageRepository.Table.Single(l => l.Name == "English");

        //save resources
        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data/Resources/EnglishLanguagePack.xml");
        var localesXml = await File.ReadAllTextAsync(filePath);

        var xmlDoc = XmlExtensions.LanguageXmlDocument(localesXml);

        var translateResources = XmlExtensions.ParseTranslationResources(xmlDoc);

        foreach (var item in translateResources)
        {
            _ = Enum.TryParse(item.Area, out TranslationResourceArea areaEnum);

            await _lsrRepository.InsertAsync(new TranslationResource
            {
                LanguageId = language.Id,
                Name = item.Name.ToLowerInvariant(),
                Value = item.Value,
                Area = areaEnum,
                CreatedBy = "System"
            });
        }
    }
    
    private async Task InstallVietnameseLocale()
    {
        //'Vietnamese' language
        var language = _languageRepository.Table.Single(l => l.Name == "Vietnamese");

        //save resources
        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data/Resources/VietnameseLanguagePack.xml");
        var localesXml = await File.ReadAllTextAsync(filePath);

        var xmlDoc = XmlExtensions.LanguageXmlDocument(localesXml);

        var translateResources = XmlExtensions.ParseTranslationResources(xmlDoc);

        foreach (var item in translateResources)
        {
            _ = Enum.TryParse(item.Area, out TranslationResourceArea areaEnum);

            await _lsrRepository.InsertAsync(new TranslationResource {
                LanguageId = language.Id,
                Name = item.Name.ToLowerInvariant(),
                Value = item.Value,
                Area = areaEnum,
                CreatedBy = "System"
            });
        }
    }
}