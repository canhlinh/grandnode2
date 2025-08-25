using Grand.Domain.Configuration;

namespace Shipping.NhanhVn;

public class ShippingNhanhVnSettings : ISettings
{
    public string AppId { get; set; }
    public string BusinessId { get; set; }
    
    public int DisplayOrder { get; set; }
}