using Grand.Domain.Orders;
using Payments.Momo.Models;

namespace Payments.Momo.Services;

public interface IMomoService
{
    Task<string> CreateRedirectUrl(Order order);
    Task<bool> WebHookProcessPayment(string stripeSignature, string json);

    Task<bool> ProcessRedirection(RedirectionResult model);
}