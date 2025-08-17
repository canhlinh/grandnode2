using Grand.Business.Core.Enums.Checkout;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Payments.Momo;

public class MomoPaymentProvider : IPaymentProvider
{
    private readonly MomoPaymentSettings _momoPaymentSettings;
    private readonly ICustomerService _customerService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITranslationService _translationService;
    private readonly IOrderService _orderService;

    public MomoPaymentProvider(
        ITranslationService translationService,
        ICustomerService customerService,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor,
        MomoPaymentSettings momoPaymentSettings)
    {
        _translationService = translationService;
        _customerService = customerService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
        _momoPaymentSettings = momoPaymentSettings;
    }

    public Task<string> GetControllerRouteName()
    {
        return Task.FromResult("Plugin.Payments.Momo.PaymentInfo");
    }

    /// <summary>
    ///     Init a process a payment transaction
    /// </summary>
    /// <returns>Payment transaction</returns>
    public async Task<PaymentTransaction> InitPaymentTransaction()
    {
        return await Task.FromResult<PaymentTransaction>(null);
    }

    public async Task<IList<string>> ValidatePaymentForm(IDictionary<string, string> model)
    {
        return await Task.FromResult(new List<string>());
    }

    public async Task<PaymentTransaction> SavePaymentInfo(IDictionary<string, string> model)
    {
        return await Task.FromResult<PaymentTransaction>(null);
    }

    /// <summary>
    ///     Process a payment
    /// </summary>
    /// <returns>Process payment result</returns>
    public Task<ProcessPaymentResult> ProcessPayment(PaymentTransaction paymentTransaction)
    {
        return Task.FromResult(new ProcessPaymentResult());
    }

    /// <summary>
    ///     Post process payment
    /// </summary>
    public Task PostProcessPayment(PaymentTransaction paymentTransaction)
    {
        //nothing
        return Task.CompletedTask;
    }
    private MomoPaymentRequest CreatePaymentRequest(Order order)
    {
        return new MomoPaymentRequest() {
            partnerCode = _momoPaymentSettings.PartnerCode,
            amount = (int)order.OrderTotal,
            orderId = order.OrderGuid.ToString(),
            requestId = Guid.NewGuid().ToString(),
            orderInfo = "Mã số đơn hàng: " + order.OrderNumber.ToString(),
            lang = "vi",
            ipnUrl = _momoPaymentSettings.ReturnURL,
            redirectUrl = _momoPaymentSettings.ReturnURL,
        };
    }

    /// <summary>
    ///     Post redirect payment (used by payment gateways that redirecting to a another URL)
    /// </summary>
    /// <param name="paymentTransaction">Payment transaction</param>
    public async Task<string> PostRedirectPayment(PaymentTransaction paymentTransaction)
    {
        var order = await _orderService.GetOrderByGuid(paymentTransaction.OrderGuid);
        var response = await MomoPaymentHelper.CreatePayment(CreatePaymentRequest(order), _momoPaymentSettings.GetCreatePaymentAPI(), _momoPaymentSettings.AccessKey, _momoPaymentSettings.SecretKey);
        if (response == null)
        {
            StringBuilder data = new StringBuilder();
            data.Append(WebUtility.UrlEncode("message") + " = " + WebUtility.UrlEncode("Không thể kết nối đến Momo") + "&");
            data.Append(WebUtility.UrlEncode("orderId") + " = " + WebUtility.UrlEncode(order.OrderGuid.ToString()) + "&");
            data.Append(WebUtility.UrlEncode("resultCode") + " = " + WebUtility.UrlEncode("20"));

            var redirectURL = _momoPaymentSettings.ReturnURL + "?" + data.ToString();
           // _httpContextAccessor.HttpContext.Response.Redirect(redirectURL);
            return redirectURL;
        }

        await _orderService.InsertOrderNote(new OrderNote {
            Note = string.Format("Đã gửi yêu cầu thanh toán đến Momo, result_code = {0}", response.resultCode),
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow,
            OrderId = order.Id,
        });
        return response.payUrl;

    }

    /// <summary>
    ///     Returns a value indicating whether payment method should be hidden during checkout
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>true - hide; false - display.</returns>
    public async Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
    {
        //you can put any logic here
        //for example, hide this payment method if all products in the cart are downloadable
        //or hide this payment method if current customer is from certain country
        //return false;
        return await Task.FromResult(false);
    }

    /// <summary>
    ///     Gets additional handling fee
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>Additional handling fee</returns>
    public async Task<double> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
    {
        double result = 0;
        //return result;
        return await Task.FromResult(result);
    }

    /// <summary>
    ///     Captures payment
    /// </summary>
    /// <returns>Capture payment result</returns>
    public async Task<CapturePaymentResult> Capture(PaymentTransaction paymentTransaction)
    {
        var result = new CapturePaymentResult();
        result.AddError("Capture method not supported");
        return await Task.FromResult(result);
    }

    /// <summary>
    ///     Refunds a payment
    /// </summary>
    /// <param name="refundPaymentRequest">Request</param>
    /// <returns>Result</returns>
    public async Task<RefundPaymentResult> Refund(RefundPaymentRequest refundPaymentRequest)
    {
        var result = new RefundPaymentResult();
        result.AddError("Refund method not supported");
        return await Task.FromResult(result);
    }

    /// <summary>
    ///     Voids a payment
    /// </summary>
    /// <returns>Result</returns>
    public async Task<VoidPaymentResult> Void(PaymentTransaction paymentTransaction)
    {
        var result = new VoidPaymentResult();
        result.AddError("Void method not supported");
        return await Task.FromResult(result);
    }

    /// <summary>
    ///     Cancel a payment
    /// </summary>
    /// <returns>Result</returns>
    public Task CancelPayment(PaymentTransaction paymentTransaction)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets a value indicating whether customers can complete a payment after order is placed but not completed (for
    ///     redirection payment methods)
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <returns>Result</returns>
    public async Task<bool> CanRePostRedirectPayment(PaymentTransaction paymentTransaction)
    {
        ArgumentNullException.ThrowIfNull(paymentTransaction);

        //it's not a redirection payment method. So we always return false
        return await Task.FromResult(false);
    }


    /// <summary>
    ///     Gets a value indicating whether capture is supported
    /// </summary>
    public async Task<bool> SupportCapture()
    {
        return await Task.FromResult(false);
    }

    /// <summary>
    ///     Gets a value indicating whether partial refund is supported
    /// </summary>
    public async Task<bool> SupportPartiallyRefund()
    {
        return await Task.FromResult(false);
    }

    /// <summary>
    ///     Gets a value indicating whether refund is supported
    /// </summary>
    public async Task<bool> SupportRefund()
    {
        return await Task.FromResult(false);
    }

    /// <summary>
    ///     Gets a value indicating whether void is supported
    /// </summary>
    public async Task<bool> SupportVoid()
    {
        return await Task.FromResult(false);
    }

    /// <summary>
    ///     Gets a payment method type
    /// </summary>
    public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

    public string ConfigurationUrl => MomoDefaults.ConfigurationUrl;

    public string SystemName => MomoDefaults.ProviderSystemName;

    public string FriendlyName => _translationService.GetResource(MomoDefaults.FriendlyName);

    public int Priority => _momoPaymentSettings.DisplayOrder;

    public IList<string> LimitedToStores => new List<string>();

    public IList<string> LimitedToGroups => new List<string>();

    /// <summary>
    ///     Gets a value indicating whether we should display a payment information page for this plugin
    /// </summary>
    public async Task<bool> SkipPaymentInfo()
    {
        return await Task.FromResult(false);
    }

    /// <summary>
    ///     Gets a payment method description that will be displayed on checkout pages in the public store
    /// </summary>
    public async Task<string> Description()
    {
        return await Task.FromResult(
            _translationService.GetResource("Plugins.Payments.Momo.PaymentMethodDescription"));
    }

    public string LogoURL => "/Plugins/Payments.Momo/logo.png";
}