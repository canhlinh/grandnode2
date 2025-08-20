using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Infrastructure.Plugins;

namespace Payments.VNPay
{
    public class VNPayPaymentPlugin(
    ISettingService settingService,
    IPluginTranslateResource pluginTranslateResource)
    : BasePlugin, IPlugin
    {

        #region Methods

        public override string ConfigurationUrl()
        {
            return VNPayDefaults.ConfigurationUrl;
        }

        public override async Task Install()
        {
            //settings
            var settings = new VNPayPaymentSettings
            {
                TmnCode = "6UKLS5BJ",
                HashSecret = "FNLJYVFHEEYPPPAMBKPDIWIJIKCQAAXL",
                PaymentURL = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
                ReturnURL = "https://localhost:44350/Plugins/PaymentVNPay/ReturnHandler",
                Description = "Thanh toán bằng VNPay",
                DisplayOrder = 1
            };
            await settingService.SaveSetting(settings);

            //locales
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Payments.VNPay.FriendlyName", "VNPay");
            await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.VNPay.Fields.Description", "Mô tả phương thức thanh toán VNPay");


            await base.Install();
        }

        public override async Task Uninstall()
        {
            //settings
            await settingService.DeleteSetting<VNPayPaymentSettings>();

            //locales
            await pluginTranslateResource.DeletePluginTranslateResource("Payments.VNPay.FriendlyName");
            await pluginTranslateResource.DeletePluginTranslateResource("Plugins.Payments.VNPay.Fields.Description");

            await base.Uninstall();
        }

        #endregion

    }
}
