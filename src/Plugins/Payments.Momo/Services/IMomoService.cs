using Grand.Domain.Orders;

namespace Payments.Momo.Services;

public interface IMomoService
{
    Task<string> CreateRedirectUrl(Order order);
    Task<bool> WebHookProcessPayment(string stripeSignature, string json);
}