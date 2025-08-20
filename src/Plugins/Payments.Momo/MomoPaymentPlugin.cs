using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Infrastructure.Plugins;

namespace Payments.Momo;

public class MomoPaymentPlugin(
    ISettingService settingService,
    IPluginTranslateResource pluginTranslateResource)
    : BasePlugin, IPlugin
{

    #region Methods

    public override string ConfigurationUrl()
    {
        return MomoDefaults.ConfigurationUrl;
    }

    public override async Task Install()
    {
        //settings
        var settings = new MomoPaymentSettings
        {
            PartnerCode = "MOMODCRX20211219",
            AccessKey = "Z0Ab19Lo2UgyWkHJ",
            SecretKey = "l4y7NrpEQLvVn4quwzVSwVnAbHsE43Pt",
            Environment = MomoPaymentSettings.Sandbox,
            ReturnURL = "https://localhost:44350/Plugins/PaymentMomo/ReturnHandler",
            HookURL = "https://webhook.site/cc1da3fb-8c03-4c78-88b3-928135538e37",
            Description = "Thanh toán bằng ví Momo",
            DisplayOrder = 0,
        };
        await settingService.SaveSetting(settings);

        //locales
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugin.Payments.Momo.FriendlyName", "Momo");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.ReturnURL", "ReturnURL");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Plugins.Payments.Momo.Fields.ReturnURL", "HookURL");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.PartnerCode", "PartnerCode");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.AccessKey", "AccessKey");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.SecretKey", "SecretKey");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.PublicKey", "PublicKey");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.MerchantId", "Merchant ID");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.Environment", "Environment");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.PublicKey", "Public Key");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.DisplayOrder", "Display order");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource( "Plugins.Payments.Momo.Fields.Description", "Description");

        await base.Install();
    }

    public override async Task Uninstall()
    {
        //settings
        await settingService.DeleteSetting<MomoPaymentSettings>();

        //locales
        await pluginTranslateResource.DeletePluginTranslationResource("Plugin.Payments.Momo.FriendlyName");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.ReturnURL");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.HookURL");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.PartnerCode");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.AccessKey");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.SecretKey");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.PublicKey");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.Environment");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.DisplayOrder");
        await pluginTranslateResource.DeletePluginTranslationResource("Plugins.Payments.Momo.Fields.Description");

        await base.Uninstall();
    }

    #endregion
}