using Grand.Business.Core.Enums.Checkout;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Orders;
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
        throw new NotImplementedException();
    }

    public Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
    {
        throw new NotImplementedException();
    }

    public Task<double?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
    {
        throw new NotImplementedException();
    }

    public Task<IList<string>> ValidateShippingForm(string shippingOption, IDictionary<string, string> data)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetControllerRouteName()
    {
        throw new NotImplementedException();
    }
    
    #endregion
}