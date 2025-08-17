using Microsoft.AspNetCore.Mvc;

namespace Payments.Momo.Components;

[ViewComponent(Name = "PaymentBrainTreeScripts")]
public class PaymentBrainTreeScripts : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}