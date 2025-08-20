using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Domain.Permissions;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.VNPay.Models;

namespace Payments.VNPay.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.PaymentMethods)]
    public class PaymentVNPayController : BasePaymentController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly VNPayPaymentSettings _vnPayPaymentSettings;

        #endregion

        #region Ctor

        public PaymentVNPayController(ISettingService settingService,
            ITranslationService translationService,
            VNPayPaymentSettings vnpayPaymentSettings)
        {
            _settingService = settingService;
            _translationService = translationService;
            _vnPayPaymentSettings = vnpayPaymentSettings;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                ReturnURL = _vnPayPaymentSettings.ReturnURL,
                PaymentURL = _vnPayPaymentSettings.PaymentURL,
                TmnCode = _vnPayPaymentSettings.TmnCode,
                HashSecret = _vnPayPaymentSettings.HashSecret,
                DisplayOrder = _vnPayPaymentSettings.DisplayOrder,
                Description = _vnPayPaymentSettings.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _vnPayPaymentSettings.ReturnURL = model.ReturnURL;
            _vnPayPaymentSettings.PaymentURL = model.PaymentURL;
            _vnPayPaymentSettings.TmnCode = model.TmnCode;
            _vnPayPaymentSettings.HashSecret = model.HashSecret;
            _vnPayPaymentSettings.DisplayOrder = model.DisplayOrder;
            _vnPayPaymentSettings.Description = model.Description;

            await _settingService.SaveSetting(_vnPayPaymentSettings);

            Success(_translationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}