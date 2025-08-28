using Grand.Business.Core.Enums.Checkout;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Common;
using Grand.Domain.Orders;
using Grand.Domain.Shipping;
using Grand.Infrastructure;
using Shipping.NhanhVn.Services;

namespace Shipping.NhanhVn;

public class ShippingNhanhVnProvider : IShippingRateCalculationProvider
{
    private readonly ITranslationService _translationService;
    private readonly IContextAccessor _contextAccessor;
    private readonly ICustomerService _customerService;
    private readonly ICountryService _countryService;
    private readonly ICurrencyService _currencyService;
    private readonly ISettingService _settingService;
    private readonly INhanhVnService _nhanhVnService;
    private readonly ShippingNhanhVnSettings _shippingNhanhVnSettings;

    public ShippingNhanhVnProvider(
        ITranslationService translationService,
        IContextAccessor contextAccessor,
        ICustomerService customerService,
        ICountryService countryService,
        ICurrencyService currencyService,
        ISettingService settingService,
        INhanhVnService nhanhVnService,
        ShippingNhanhVnSettings shippingNhanhVnSettings
        )
    {
        _translationService = translationService;
        _contextAccessor = contextAccessor;
        _customerService = customerService;
        _countryService = countryService;
        _currencyService = currencyService;
        _settingService = settingService;
        _nhanhVnService = nhanhVnService;
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
    
    public async Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
    {
        var shippingSettings = await _settingService.LoadSetting<ShippingSettings>(getShippingOptionRequest.StoreId);
        var fromAddress = shippingSettings.ShippingOriginAddress;
        if (fromAddress == null)
            throw new Exception("Shipping origin address is not set");
        if (string.IsNullOrEmpty(fromAddress.ProvinceId) || string.IsNullOrEmpty(fromAddress.DistrictId) || string.IsNullOrEmpty(fromAddress.WardId))
            throw new Exception("Shipping origin address is not set properly. Province, District and Ward are required");
        if (getShippingOptionRequest.ShippingAddress == null)
            throw new Exception("Shipping address is not set");
        if (getShippingOptionRequest.ShippingAddress.CountryId == null)
            throw new Exception("Shipping address is not set properly. Country is required");
        var country = await _countryService.GetCountryById(getShippingOptionRequest.ShippingAddress.CountryId);
        if (country == null)
            throw new Exception("Shipping address is not set properly. Country is required");
        if (!country.AllowsShipping)
            throw new Exception("Shipping is not allowed to the country");

        var shippingWeight = await _nhanhVnService.GetShippingWeight(getShippingOptionRequest.Items);
        var shippingTotal = await _nhanhVnService.GetShippingSubtotal(getShippingOptionRequest.Items);
        var shippingFees = await _nhanhVnService.GetShippingFees((int)shippingWeight, shippingTotal, fromAddress, getShippingOptionRequest.ShippingAddress);

        var response = new GetShippingOptionResponse();
        foreach (var shippingFee in shippingFees)
        {
            response.ShippingOptions.Add(new ShippingOption() {
                ShippingRateProviderSystemName = ShippingNhanhVnDefaults.SystemName,
                Name = shippingFee.Service.Name,
                Rate = shippingFee.ShipFee,
                Logo = shippingFee.Logo,
                NhanhVnCarrierId = shippingFee.Carrier.Id,
                NhanhVnServiceId = shippingFee.Service.Id,
                Description = "",
            });   
        }
        return await Task.FromResult(response);
    }

    public Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
    {
        return Task.FromResult(false);
    }

    public Task<double?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
    {
        return Task.FromResult(default(double?));
    }

    public async Task<IList<string>> ValidateShippingForm(string shippingOption, IDictionary<string, string> data)
    {
        return await Task.FromResult(new List<string>());
    }

    public Task<string> GetControllerRouteName()
    {
        return Task.FromResult("");
    }
    
    #endregion
}