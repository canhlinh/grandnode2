using Grand.Business.Core.Interfaces.Cms;
using Grand.Business.Core.Interfaces.Common.Localization;

namespace Payments.Momo;

public class MomoWidgetProvider : IWidgetProvider
{
    private readonly MomoPaymentSettings _momoPaymentSettings;
    private readonly ITranslationService _translationService;

    public MomoWidgetProvider(ITranslationService translationService,
        MomoPaymentSettings momoPaymentSettings)
    {
        _translationService = translationService;
        _momoPaymentSettings = momoPaymentSettings;
    }

    public string ConfigurationUrl => MomoDefaults.ConfigurationUrl;

    public string SystemName => MomoDefaults.ProviderSystemName;

    public string FriendlyName => _translationService.GetResource(MomoDefaults.FriendlyName);

    public int Priority => _momoPaymentSettings.DisplayOrder;

    public IList<string> LimitedToStores => new List<string>();

    public IList<string> LimitedToGroups => new List<string>();

    public async Task<IList<string>> GetWidgetZones()
    {
        return await Task.FromResult(new[] { "checkout_payment_info_top" });
    }

    public Task<string> GetPublicViewComponentName(string widgetZone)
    {
        return Task.FromResult("PaymentMomoScripts");
    }
}