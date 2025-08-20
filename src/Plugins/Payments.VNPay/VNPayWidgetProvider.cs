using Grand.Business.Core.Interfaces.Cms;
using Grand.Business.Core.Interfaces.Common.Localization;

namespace Payments.VNPay
{
    public class VNPayWidgetProvider : IWidgetProvider
    {
        private readonly ITranslationService _translationService;
        private readonly VNPayPaymentSettings _vnpayPaymentSettings;

        public VNPayWidgetProvider(ITranslationService translationService, VNPayPaymentSettings vnpayPaymentSettings)
        {
            _translationService = translationService;
            _vnpayPaymentSettings = vnpayPaymentSettings;
        }

        public string ConfigurationUrl => VNPayDefaults.ConfigurationUrl;

        public string SystemName => VNPayDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(VNPayDefaults.FriendlyName);

        public int Priority => _vnpayPaymentSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();

        public async Task<IList<string>> GetWidgetZones()
        {
            return await Task.FromResult(new string[] { "checkout_payment_info_top" });
        }

        public Task<string> GetPublicViewComponentName(string widgetZone)
        {
            return Task.FromResult("PaymentVNPayScripts");
        }
    }
}