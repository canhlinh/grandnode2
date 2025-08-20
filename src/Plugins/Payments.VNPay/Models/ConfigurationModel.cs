using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Payments.VNPay.Models
{
    public class ConfigurationModel : BaseModel
    {
        [GrandResourceDisplayName("Plugins.Payments.VNPay.Fields.ReturnURL")]
        public string ReturnURL { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.VNPay.Fields.PaymentURL")]
        public string PaymentURL { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.VNPay.Fields.TmnCode")]
        public string TmnCode { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.VNPay.Fields.HashSecret")]
        public string HashSecret { get; set; }
        
        [GrandResourceDisplayName("Plugins.Payments.VNPay.Fields.Description")]
        public string Description { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.VNPay.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}