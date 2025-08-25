using Grand.Business.Core.Enums.Checkout;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Orders;
using Grand.Domain.Shipping;
using Grand.Infrastructure;

namespace Shipping.NhanhVn;

public class ShippingNhanhVnProvider : IShippingRateCalculationProvider
{
    private readonly ITranslationService _translationService;
    private readonly IContextAccessor _contextAccessor;
    private readonly ICustomerService _customerService;
    private readonly ICountryService _countryService;
    private readonly ICurrencyService _currencyService;

    private readonly ShippingNhanhVnSettings _shippingNhanhVnSettings;

    public ShippingNhanhVnProvider(
        ITranslationService translationService,
        IContextAccessor contextAccessor,
        ICustomerService customerService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ShippingNhanhVnSettings shippingNhanhVnSettings
        )
    {
        _translationService = translationService;
        _contextAccessor = contextAccessor;
        _customerService = customerService;
        _countryService = countryService;
        _currencyService = currencyService;
        _shippingNhanhVnSettings = shippingNhanhVnSettings;
    }
    
    #region Properties
    public string ConfigurationUrl => ShippingNhanhVnDefaults.ConfigurationUrl;
    public string SystemName =>  ShippingNhanhVnDefaults.SystemName;
    public string FriendlyName => ShippingNhanhVnDefaults.FriendlyName;
    public int Priority => 0;
    public IList<string> LimitedToStores => new List<string>();

    public IList<string> LimitedToGroups => new List<string>();
    public ShippingRateCalculationType ShippingRateCalculationType => ShippingRateCalculationType.Off;
    public IShipmentTracker ShipmentTracker => null;
    
    #endregion
    
    #region Methods
    
    public Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
    {
        var response = new GetShippingOptionResponse();
        response.ShippingOptions.Add(new ShippingOption() {
            ShippingRateProviderSystemName = ShippingNhanhVnDefaults.SystemName,
            Name = "Viettel Post Shipping",
            Rate = 100000,
            Description = "",
            Logo = "https://carrier.nvncdn.com/carrier/carr_1692349658_386.png",
        });
        response.ShippingOptions.Add(new ShippingOption() {
            ShippingRateProviderSystemName = ShippingNhanhVnDefaults.SystemName,
            Name = "GHN Shipping",
            Rate = 100000,
            Description = "",
            Logo = "https://carrier.nvncdn.com/carrier/carr_1692352251_813.png",
        });
        return Task.FromResult(response);
    }

    public Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(false);
    }

    public Task<double?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
    {
        return Task.FromResult(default(double?));
    }

    public Task<List<string>> ValidateShippingForm(string shippingOption, IDictionary<string, string> data)
    {
        return Task.FromResult(new List<string>());
    }

    public Task<string> GetControllerRouteName()
    {
        return Task.FromResult("");
    }
    
    #endregion
}