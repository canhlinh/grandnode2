using Grand.Business.Core.Commands.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Queries.Checkout.Orders;
using Grand.Domain.Orders;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.Momo.Models;
using System.Text.Json;

namespace Payments.Momo.Services;

public class MomoService(
    IOrderService orderService,
    IPaymentTransactionService  paymentTransactionService,
    IMediator mediator,
    ILogger<MomoService> logger,
    MomoPaymentSettings momoPaymentSettings)
    : IMomoService
{
    
    public async Task<string> CreateRedirectUrl(Order order)
    {
        var paymentRequest = new CreatePaymentRequest() {
            RequestType = "captureWallet",
            PartnerCode = momoPaymentSettings.PartnerCode,
            Amount = (int)order.OrderTotal,
            OrderId = order.OrderGuid.ToString(),
            RequestId = Guid.NewGuid().ToString(),
            OrderInfo = "Mã số đơn hàng: " + order.OrderNumber.ToString(),
            Language = "vi",
            IpnUrl = momoPaymentSettings.ReturnURL,
            RedirectUrl = momoPaymentSettings.ReturnURL,
        };
        paymentRequest.HashRequest(momoPaymentSettings.AccessKey, momoPaymentSettings.SecretKey);
        var reqContent = new StringContent(JsonSerializer.Serialize(paymentRequest), Encoding.UTF8, "application/json");
        var client = new HttpClient();
        var req = new HttpRequestMessage(HttpMethod.Post, momoPaymentSettings.GetCreatePaymentAPI());
        req.Content = reqContent;
        var res = await client.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode)
        {
            await orderService.InsertOrderNote(new OrderNote {
                Note = $"KẾt nối thấi bại, data = {resContent}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = order.Id,
            });
            return null;
        }
        var response = JsonSerializer.Deserialize<CreatePaymentResponse>(resContent);
        await orderService.InsertOrderNote(new OrderNote {
            Note = $"Đã gửi yêu cầu thanh toán đến Momo, message = {response.message}",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow,
            OrderId = order.Id,
        });
        return response.payUrl;
    }

    public Task<bool> WebHookProcessPayment(string stripeSignature, string json)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ProcessRedirection(RedirectionResult result)
    {
        Order order = await orderService.GetOrderByGuid(result.GetParsedOrderId());
        if (order == null)
        {
            return false;
        }
        
        if (!order.OrderTotal.Equals(result.Amount))
        {
            var errorStr =
                $"Thanh toán lỗi, Momo trả về kết quả = {result.Amount} không khớp với tổng đơn = {order.OrderTotal.ToString()}";
            await orderService.InsertOrderNote(new OrderNote {
                Note = errorStr,
                OrderId = order.Id,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            return false;
        }
        
        if (result.ResultCode == 0)
        {
            await orderService.InsertOrderNote(new OrderNote {
                Note = string.Format("Thanh toán thành công. Mã giao dịch Momo = {0}", result.TransId, result.Amount),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = order.Id,
            });

            var paymentTransaction = await paymentTransactionService.GetOrderByGuid(result.GetParsedOrderId());
            if (paymentTransaction == null)
            {
                logger.LogError("paymentTransaction is null or currency is not equal");
                return false;
            }

            try
            {
                paymentTransaction.AuthorizationTransactionId = result.TransId;
                paymentTransaction.PaidAmount += result.Amount;
                await mediator.Send(new MarkAsPaidCommand { PaymentTransaction = paymentTransaction });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error in CreatePaymentTransaction");
                return false;
            }
            return true;
        }
        else
        {
            var errorStr = $"Thanh toán lỗi, Momo TranId = {result.TransId}, ResponseCode = {result.ResultCode}.";
            logger.LogError(errorStr);

            await orderService.InsertOrderNote(new OrderNote {
                Note = errorStr,
                OrderId = order.Id,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            return false;
        }
    }
}