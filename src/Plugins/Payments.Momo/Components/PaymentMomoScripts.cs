using Microsoft.AspNetCore.Mvc;

namespace Payments.Momo.Components;

[ViewComponent(Name = "PaymentMomoScripts")]
public class PaymentMomoScripts : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}