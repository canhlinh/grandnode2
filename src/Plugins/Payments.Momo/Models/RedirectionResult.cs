using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Payments.Momo.Models;

public class RedirectionResult : BaseModel
{
    public string PartnerCode { get; set; }
    public string OrderId { get; set; }

    public string Message { get; set; }

    public int ResultCode { get; set; }

    public string TransId { get; set; }

    public int Amount { get; set; }

    public Guid GetParsedOrderId()
    {
        return Guid.Parse(OrderId);
    }
}