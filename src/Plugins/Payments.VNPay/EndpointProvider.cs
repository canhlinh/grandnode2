using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.VNPay;

public class EndpointProvider : IEndpointProvider
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {

        //PaymentInfo
        endpointRouteBuilder.MapControllerRoute(VNPayDefaults.PaymentInfo,
            "Plugins/PaymentVNPay/PaymentInfo",
            new { controller = "PaymentVNPay", action = "PaymentInfo", area = "" }
        );

        endpointRouteBuilder.MapControllerRoute("Plugin.Payments.VNPay.ReturnHandler",
            "Plugins/PaymentVNPay/ReturnHandler",
            new { controller = "PaymentVNPay", action = "ReturnHandler" }
        );

        //Cancel
        endpointRouteBuilder.MapControllerRoute("Plugin.Payments.VNPay.CancelOrder",
             "Plugins/PaymentVNPay/CancelOrder",
             new { controller = "PaymentVNPay", action = "CancelOrder" }
        );
    }

    public int Priority => 0;
}