using Grand.Domain.Configuration;

namespace Payments.Momo;

public class MomoPaymentSettings : ISettings
{

    #region const

    public const string Production = "production";
    public const string Sandbox = "sandbox";

    public const string ProductionURL = "https://payment.momo.vn";

    public const string SandboxURL = "https://test-payment.momo.vn";

    #endregion const

    public string ReturnURL { get; set; }

    public string HookURL { get; set; }

    public string PartnerCode { get; set; }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string PublicKey { get; set; }

    public string Environment { get; set; }

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public string GetCreatePaymentAPI()
    {
        var baseURL = Environment == Production ? ProductionURL : SandboxURL;
        return baseURL + "/v2/gateway/api/create";
    }
}