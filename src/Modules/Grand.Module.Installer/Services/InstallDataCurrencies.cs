using Grand.Domain.Directory;

namespace Grand.Module.Installer.Services;

public partial class InstallationService
{
    protected virtual Task InstallCurrencies()
    {
        var currencies = new List<Currency> {
            new Currency{
                Name = "VND",
                CurrencyCode = "VND",
                Rate = 1,
                DisplayLocale = "vi-VN",
                CustomFormatting = "",
                NumberDecimal = 0,
                Published = true,
                DisplayOrder = 1,
                RoundingTypeId = RoundingType.Rounding001,
                MidpointRoundId = MidpointRounding.ToEven
            },
            new Currency{
                Name = "USD",
                CurrencyCode = "USD",
                Rate = 1,
                DisplayLocale = "en-US",
                CustomFormatting = "",
                NumberDecimal = 2,
                Published = true,
                DisplayOrder = 1,
                RoundingTypeId = RoundingType.Rounding001,
                MidpointRoundId = MidpointRounding.ToEven
            },
        };
        currencies.ForEach(x => _currencyRepository.Insert(x));
        return Task.CompletedTask;
    }
}