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
using Payments.Momo.Models;

namespace Payments.Momo.Controllers;

public class PaymentMomoController : BasePaymentController
{
    private readonly IContextAccessor _contextAccessor;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentMomoController> _logger; 
    private readonly IMediator _mediator;
    private readonly IPaymentTransactionService _paymentTransactionService;
    private readonly MomoPaymentSettings _momoPaymentSettings;
    private readonly PaymentSettings _paymentSettings;

    public PaymentMomoController(
        IContextAccessor contextAccessor,
        IPaymentService paymentService,
        IOrderService orderService,
        ILogger<PaymentMomoController> logger,
        IMediator mediator,
        IPaymentTransactionService paymentTransactionService,
        MomoPaymentSettings momoPaymentSettings,
        PaymentSettings paymentSettings)
    {
        _contextAccessor = contextAccessor;
        _paymentService = paymentService;
        _orderService = orderService;
        _logger = logger;
        _mediator = mediator;
        _paymentTransactionService = paymentTransactionService;
        _paymentSettings = paymentSettings;
        _momoPaymentSettings = momoPaymentSettings;
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

    public async Task<IActionResult> ReturnHandler(MomoPaymentResultModel model)
    {
        if (model.PartnerCode != _momoPaymentSettings.PartnerCode)
        {
            return Content("Momo's PartnerCode không đúng");
        }

        if (_paymentService.LoadPaymentMethodBySystemName("Payments.Momo") is not MomoPaymentProvider processor ||
            !processor.IsPaymentMethodActive(_paymentSettings))
            return Content("MomoPayment module chưa được nạp");


        Guid orderId = parseGuid(model.OrderId);
        if (orderId == Guid.Empty)
        {
            string errorStr = string.Format("MomoPayment không thể phân tích order_id = {0}", model.OrderId);
            _logger.LogError(errorStr);
            return Content(errorStr);
        }

        Order order = await _orderService.GetOrderByGuid(orderId);
        if (order == null)
        {
            string errorStr = string.Format("MomoPayment không tìm thấy order_id = {0}", orderId.ToString());
            _logger.LogError(errorStr);
            return Content(errorStr);
        }

        if (order != null)
        {
            if (!order.OrderTotal.Equals(model.Amount))
            {
                string errorStr = string.Format("Thanh toán lỗi, Momo trả về kết quả = {0} không khớp với tổng đơn = {1}", model.Amount, order.OrderTotal.ToString());
                _logger.LogError(errorStr);

                //order note
                await _orderService.InsertOrderNote(new OrderNote {
                    Note = errorStr,
                    OrderId = order.Id,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                return RedirectToAction("Index", "Home", new { area = "" });
            }

            if (model.ResultCode == 0)
            {
                await _orderService.InsertOrderNote(new OrderNote {
                    Note = string.Format("Thanh toán thành công. Mã giao dịch Momo = {0}", model.TransId, model.Amount),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                });

                var paymentTransaction = await _paymentTransactionService.GetOrderByGuid(order.OrderGuid);
                if (await _mediator.Send(new CanMarkPaymentTransactionAsPaidQuery() { PaymentTransaction = paymentTransaction }))
                {
                    paymentTransaction.AuthorizationTransactionId = model.TransId;
                    await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);
                    await _mediator.Send(new MarkAsPaidCommand() { PaymentTransaction = paymentTransaction });
                }
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            else
            {
                string errorStr = string.Format("Thanh toán lỗi, Momo TranId = {0}, ResponseCode = {1}.", model.TransId, model.ResultCode);
                _logger.LogError(errorStr);

                await _orderService.InsertOrderNote(new OrderNote {
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
        return View(new PaymentInfo(_momoPaymentSettings.Description));
    }

}