using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Common.Security;
using Grand.Domain.Permissions;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Helpers;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipping.NhanhVn.Models;

namespace Shipping.NhanhVn.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area("Admin")]
[PermissionAuthorize(PermissionSystemName.ShippingSettings)]
public class NhanhVnController : BasePaymentController
{
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly ITranslationService _translationService;
    private readonly IAdminStoreService _adminStoreService;

    public NhanhVnController(
        ISettingService settingService,
        ITranslationService translationService,
        IPermissionService permissionService,
        IAdminStoreService adminStoreService)
    {
        _adminStoreService = adminStoreService;
        _settingService = settingService;
        _translationService = translationService;
        _permissionService = permissionService;
    }

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePaymentMethods))
            return AccessDeniedView();

        //load settings for a chosen store scope
        var storeScope = await _adminStoreService.GetActiveStore();
        var settings = await _settingService.LoadSetting<ShippingNhanhVnSettings>(storeScope);

        var model = new ConfigurationModel {
            AppId = settings.AppId,
            ApiKey = settings.ApiKey,
            BusinessId = settings.BusinessId,
            SecretKey = settings.SecretKey,
            DisplayOrder = settings.DisplayOrder
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.Authorize(StandardPermission.ManagePaymentMethods))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _adminStoreService.GetActiveStore();
        var settings = await _settingService.LoadSetting<ShippingNhanhVnSettings>(storeScope);

        //save settings
        settings.AppId = model.AppId;
        settings.ApiKey = model.ApiKey;
        settings.BusinessId = model.BusinessId;
        settings.SecretKey = model.SecretKey;
        settings.DisplayOrder = model.DisplayOrder;

        await _settingService.SaveSetting(settings, storeScope);

        //now clear settings cache
        await _settingService.ClearCache();

        Success(_translationService.GetResource("Admin.Plugins.Saved"));

        return await Configure();
    }
}