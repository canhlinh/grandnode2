using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Infrastructure.Plugins;

namespace Shipping.NhanhVn;

public class ShippingNhanhVnPlugin(
    IPluginTranslateResource pluginTranslateResource)
    : BasePlugin, IPlugin
{
    #region Methods

    /// <summary>
    ///     Install plugin
    /// </summary>
    public override async Task Install()
    {
        //locales       
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Shipping.NhanhVn.FriendlyName", "NhanhVn");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Shipping.NhanhVn.PluginName", "Shipping NhanhVn");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Shipping.NhanhVn.PluginDescription", "Choose a place where you can pick up your order");
        await pluginTranslateResource.AddOrUpdatePluginTranslateResource("Shipping.NhanhVn.Fields.Description", "Description");

        await base.Install();
    }

    /// <summary>
    ///     Uninstall plugin
    /// </summary>
    public override async Task Uninstall()
    {
        await pluginTranslateResource.DeletePluginTranslationResource("Shipping.NhanhVn.FriendlyName");
        await pluginTranslateResource.DeletePluginTranslationResource("Shipping.NhanhVn.PluginName");
        await pluginTranslateResource.DeletePluginTranslationResource("Shipping.NhanhVn.PluginDescription");
        await pluginTranslateResource.DeletePluginTranslationResource("Shipping.NhanhVn.Fields.Description");
        await base.Uninstall();
    }

    #endregion
}