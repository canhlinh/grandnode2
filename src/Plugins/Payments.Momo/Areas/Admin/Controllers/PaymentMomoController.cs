using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Domain.Permissions;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Momo.Models;

namespace Payments.Momo.Areas.Admin.Controllers;

[AuthorizeAdmin]
[Area("Admin")]
[PermissionAuthorize(PermissionSystemName.PaymentMethods)]
public class PaymentMomoController : BasePaymentController
{
    #region Ctor

    public PaymentMomoController(ISettingService settingService,
        ITranslationService translationService,
        MomoPaymentSettings momoPaymentSettings)
    {
        _settingService = settingService;
        _translationService = translationService;
        _momoPaymentSettings = momoPaymentSettings;
    }

    #endregion

    #region Fields

    private readonly ISettingService _settingService;
    private readonly ITranslationService _translationService;
    private readonly MomoPaymentSettings _momoPaymentSettings;

    #endregion

    #region Methods

    public IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            ReturnURL = _momoPaymentSettings.ReturnURL,
            PartnerCode = _momoPaymentSettings.PartnerCode,
            AccessKey = _momoPaymentSettings.AccessKey,
            SecretKey = _momoPaymentSettings.SecretKey,
            PublicKey = _momoPaymentSettings.PublicKey,
            Environment = _momoPaymentSettings.Environment,
            HookURL = _momoPaymentSettings.HookURL,
            DisplayOrder = _momoPaymentSettings.DisplayOrder,
            Description = _momoPaymentSettings.Description,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return Configure();

        //save settings
        _momoPaymentSettings.ReturnURL = model.ReturnURL;
        _momoPaymentSettings.PublicKey = model.PublicKey;
        _momoPaymentSettings.AccessKey = model.AccessKey;
        _momoPaymentSettings.PartnerCode = model.PartnerCode;
        _momoPaymentSettings.SecretKey = model.SecretKey;
        _momoPaymentSettings.Environment = model.Environment;
        _momoPaymentSettings.DisplayOrder = model.DisplayOrder;
        _momoPaymentSettings.Description = model.Description;

        await _settingService.SaveSetting(_momoPaymentSettings);

        Success(_translationService.GetResource("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}