using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.Momo;

public class EndpointProvider : IEndpointProvider
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {

        //PaymentInfo
        endpointRouteBuilder.MapControllerRoute(MomoDefaults.PaymentInfo,
            "Plugins/PaymentMomo/PaymentInfo",
            new { controller = "PaymentMomo", action = "PaymentInfo", area = "" }
        );

        endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Momo.ReturnHandler",
            "Plugins/PaymentMomo/ReturnHandler",
            new { controller = "PaymentMomo", action = "ReturnHandler" }
        );

        //Cancel
        endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Momo.CancelOrder",
             "Plugins/PaymentMomo/CancelOrder",
             new { controller = "PaymentMomo", action = "CancelOrder" }
        );
    }

    public int Priority => 0;
}