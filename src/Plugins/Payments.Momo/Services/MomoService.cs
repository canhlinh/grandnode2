using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Domain.Orders;
using Payments.Momo.Models;
using System.Text.Json;

namespace Payments.Momo.Services;

public class MomoService(
    IOrderService orderService,
    MomoPaymentSettings momoPaymentSettings)
    : IMomoService
{
    
    public async Task<string> CreateRedirectUrl(Order order)
    {
        var paymentRequest = new MomoPaymentRequest() {
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
        var response = JsonSerializer.Deserialize<MomoPaymentResponse>(resContent);
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
}