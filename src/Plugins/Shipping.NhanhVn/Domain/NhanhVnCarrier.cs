using Grand.Domain;

namespace Shipping.NhanhVn.Domain;

public class NhanhVnCarrier: BaseEntity
{
    public int CarrierId { get; set; }
    public string CarrierName { get; set; }
    public string CarrierLogo { get; set; }
}