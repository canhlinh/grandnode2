using DotLiquid;
using Grand.Business.Core.Commands.Checkout.Orders;
using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Queries.Checkout.Orders;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Grand.Web.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payments.VNPay.Models;

namespace Payments.VNPay.Controllers;

public class PaymentVNPayController : BasePaymentController
{
    private readonly IContextAccessor _contextAccessor;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentVNPayController> _logger;
    private readonly IMediator _mediator;
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly VNPayPaymentSettings _vnpayPaymentSettings;
    private readonly PaymentSettings _paymentSettings;

    public PaymentVNPayController(
        IContextAccessor contextAccessor,
        IPaymentService paymentService,
        IOrderService orderService,
        ILogger<PaymentVNPayController> logger,
        IMediator mediator,
        IPaymentTransactionService paymentTransactionService,
        VNPayPaymentSettings vnpayPaymentSettings,
        PaymentSettings paymentSettings)
    {
        _contextAccessor = contextAccessor;
        _paymentService = paymentService;
        _orderService = orderService;
        _logger = logger;
        _mediator = mediator;
        _paymentTransactionService = paymentTransactionService;
        _paymentSettings = paymentSettings;
        _vnpayPaymentSettings = vnpayPaymentSettings;
    }

    private Guid parseGuid(string s)
    {
        try
        {
            var orderId = Guid.Parse(s);
            return orderId;
        }
        catch (Exception)
        {
            return Guid.Empty;
        }
    }

    public async Task<IActionResult> ReturnHandler(VNPayRedirectModel model)
    {
        if (Request.Query.Count <= 0)
        {
            return Content("VnPay không thể tiến xử lý dư liệu rỗng");
        }

        if (_paymentService.LoadPaymentMethodBySystemName("Payments.VnPay") is not VNPayPaymentProvider processor ||
            !processor.IsPaymentMethodActive(_paymentSettings))
            return Content("VnPay module chưa được nạp");


        var vnpayData = new SortedList<string, string>();
        foreach (var key in Request.Query.Keys)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnpayData.Add(key, Request.Query[key].ToString());
            }
        }

        bool isValid = VnPayHelper.ValidateSignature(model.SecureHash, _vnpayPaymentSettings.HashSecret, vnpayData);
        if (!isValid)
        {
            return Content("VnPay không thể xác thực dữ liệu trả về");
        }

        Guid orderId = Guid.Empty;
        try
        {
            orderId = Guid.Parse(model.TxnRef);
        }
        catch (Exception)
        {
            string errorStr = string.Format("VnPayPayment không thể phân tích order_id = {0}", model.TxnRef);
            _logger.LogError(errorStr);
            return Content(errorStr);
        }

        Order order = await _orderService.GetOrderByGuid(orderId);
        if (order != null)
        {
            if (!order.OrderTotal.Equals(model.Amount / 100))
            {
                string errorStr = string.Format("VnPay trả về kết quả = {0} không khớp với tổng đơn = {1}. Mã đơn# {2}.", model.Amount / 100, order.OrderTotal.ToString(), order.OrderNumber);
                _logger.LogError(errorStr);
                await _orderService.InsertOrderNote(new OrderNote
                {
                    Note = errorStr,
                    OrderId = order.Id,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                return Content(errorStr);
            }

            if (model.ResponseCode == "00" && model.TransactionStatus == "00")
            {
                var paymentTransaction = await _paymentTransactionService.GetOrderByGuid(order.OrderGuid);
                if (await _mediator.Send(new CanMarkPaymentTransactionAsPaidQuery() { PaymentTransaction = paymentTransaction }))
                {
                    paymentTransaction.AuthorizationTransactionId = model.TransactionNo;
                    await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);
                    await _mediator.Send(new MarkAsPaidCommand() { PaymentTransaction = paymentTransaction });
                }

                await _orderService.InsertOrderNote(new OrderNote
                {
                    Note = string.Format("Thanh toán thành công. Mã giao dịch VnPay = {0}", model.TransactionNo),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                });
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            else
            {
                string errorStr = string.Format("Thanh toán lỗi, OrderNumber ={0}, VnPay TranNo = {1}, ResponseCode = {2}.", order.OrderNumber, model.TransactionNo, model.ResponseCode);
                _logger.LogError(errorStr);

                await _orderService.InsertOrderNote(new OrderNote
                {
                    Note = errorStr,
                    OrderId = order.Id,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                return RedirectToRoute("CheckoutFailed", new { orderId = order.Id });
            }
        }
        return RedirectToAction("Index", "Home", new { area = "" });
    }

    public async Task<IActionResult> CancelOrder(string orderId)
    {
        var order = await _orderService.GetOrderById(orderId);
        if (order != null && order.CustomerId == _contextAccessor.WorkContext.CurrentCustomer.Id)
            return RedirectToRoute("OrderDetails", new { orderId = order.Id });

        return RedirectToRoute("HomePage");
    }

    public IActionResult PaymentInfo()
    {
        return View(new PaymentInfo(_vnpayPaymentSettings.Description));
    }

}