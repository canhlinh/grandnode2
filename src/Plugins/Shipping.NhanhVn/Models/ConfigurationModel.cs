using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.NhanhVn.Models;

public class ConfigurationModel : BaseModel
{
    [GrandResourceDisplayName("Plugins.Shipping.NhanhVn.Fields.AppId")]
    public string AppId { get; set; }
    
    [GrandResourceDisplayName("Plugins.Shipping.NhanhVn.Fields.BusinessId")]
    public string BusinessId { get; set; }
    
    [GrandResourceDisplayName("Plugins.Shipping.NhanhVn.Fields.ApiKey")]
    public string ApiKey { get; set; }
    
    [GrandResourceDisplayName("Plugins.Shipping.NhanhVn.Fields.SecretKey")]
    public string SecretKey { get; set; }
    
    [GrandResourceDisplayName("Plugins.Shipping.NhanhVn.Fields.DisplayOrder")]
    public int DisplayOrder { get; set; }
}