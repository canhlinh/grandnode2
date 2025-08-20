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
using Payments.Momo.Services;

namespace Payments.Momo.Controllers;

public class PaymentMomoController : BasePaymentController
{
    private readonly IContextAccessor _contextAccessor;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly MomoPaymentSettings _momoPaymentSettings;
    private readonly PaymentSettings _paymentSettings;
    private readonly MomoService _momoService;
    
    public PaymentMomoController(
        IContextAccessor contextAccessor,
        IPaymentService paymentService,
        IOrderService orderService,
        MomoPaymentSettings momoPaymentSettings,
        PaymentSettings paymentSettings,
        MomoService momoService)
    {
        _contextAccessor = contextAccessor;
        _paymentService = paymentService;
        _orderService = orderService;
        _paymentSettings = paymentSettings;
        _momoPaymentSettings = momoPaymentSettings;
        _momoService = momoService;
    }

    public async Task<IActionResult> ReturnHandler(RedirectionResult model)
    {
        if (model.PartnerCode != _momoPaymentSettings.PartnerCode)
        {
            return Content("Momo's PartnerCode không đúng");
        }
        if (_paymentService.LoadPaymentMethodBySystemName("Payments.Momo") is not MomoPaymentProvider processor ||
            !processor.IsPaymentMethodActive(_paymentSettings))
            return Content("MomoPayment module chưa được nạp");

        if (await _momoService.ProcessRedirection(model))
        {
            return RedirectToRoute("CheckoutCompleted", new { orderId = model.OrderId });
        }
        return RedirectToRoute("CheckoutFailed", new { orderId = model.OrderId }); 
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