using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Payments.Momo.Models;

public class ConfigurationModel : BaseModel
{
    public ConfigurationModel()
    {
        AvailableEnvironments =
        [
            new SelectListItem { Text = "Production", Value = MomoPaymentSettings.Production },
            new SelectListItem { Text = "Sandbox", Value = MomoPaymentSettings.Sandbox },
        ];
    }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.ReturnURL")]
    public string ReturnURL { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.HookURL")]
    public string HookURL { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.PartnerCode")]
    public string PartnerCode { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.AccessKey")]
    public string AccessKey { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.SecretKey")]
    public string SecretKey { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.PublicKey")]
    public string PublicKey { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.Environment")]
    public string Environment { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.Environment")]
    public IList<SelectListItem> AvailableEnvironments { get; set; }

    [GrandResourceDisplayName("Plugins.Payments.Momo.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }
}