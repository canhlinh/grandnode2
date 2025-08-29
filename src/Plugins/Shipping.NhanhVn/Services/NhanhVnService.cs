using System.Net.Http.Json;
using System.Text.Json;
using Grand.Business.Core.Interfaces.Catalog.Prices;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Orders;
using Grand.Infrastructure.Caching;
using Microsoft.Extensions.Logging;
using Shipping.NhanhVn.Models;

namespace Shipping.NhanhVn.Services;

public class NhanhVnService : INhanhVnService
{
    private const string BaseApiUrl = "https://pos.open.nhanh.vn/v3.0";
    private readonly ShippingNhanhVnSettings _settings;
    private readonly IProductService _productService;
    private readonly IPricingService _pricingService;
    private readonly ICountryService _countryService;
    private readonly ILogger<NhanhVnService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ICacheBase _cache;

    public NhanhVnService(
        ILogger<NhanhVnService> logger,
        ICacheBase cache,
        ShippingNhanhVnSettings settings,
        IProductService productService,
        IPricingService pricingService,
        ICountryService countryService)
    {
        _settings = settings;
        _cache = cache;
        _productService = productService;
        _pricingService = pricingService;
        _countryService = countryService;
        _logger = logger;
        _httpClient = new HttpClient();
    }

    public async Task<IList<NhanhVnShippingFee>> GetShippingFees(int weight, double price, Address from, Address to)
    {
        var fromProvince = await _countryService.GetProvinceById(from.ProvinceId);
        var fromDistrict = await _countryService.GetDistrictById(from.DistrictId);
        var fromWard = await _countryService.GetWardById(from.WardId);
        if (fromProvince == null || fromDistrict == null || fromWard == null)
            return [];
        var toProvince = await _countryService.GetProvinceById(to.ProvinceId);
        var toDistrict = await _countryService.GetDistrictById(to.DistrictId);
        var toWard = await _countryService.GetWardById(to.WardId);
        if (toProvince == null || toDistrict == null || toWard == null)
            return [];

        var requestBody = new NhanhVnShippingFeeRequest
        {
            Filters = new NhanhVnShippingFeeFilters
            {
                Type = 1,
                ShippingWeight = weight,
                Price = price,
                ShippingFrom = new NhanhVnShippingAddress
                {
                    CityId = fromProvince.NhanhVnId,
                    DistrictId = fromDistrict.NhanhVnId,
                    WardId = fromWard.NhanhVnId,
                    Address = from.Address1,
                    LocationVersion = $"v{from.DivisionVersion}",
                },
                ShippingTo = new NhanhVnShippingAddress
                {
                    CityId = toProvince.NhanhVnId,
                    DistrictId = toDistrict.NhanhVnId,
                    WardId = toWard.NhanhVnId,
                    Address = to.Address1,
                    LocationVersion = $"v{to.DivisionVersion}",
                }
            }
        };

        var endpoint = $"{BaseApiUrl}/shipping/fee?appId={_settings.AppId}&businessId={_settings.BusinessId}";
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Add("Authorization", _settings.ApiKey);
        var res = await _httpClient.SendAsync(request);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogError("Error getting shipping fees from NhanhVn: {StatusCode} - {ReasonPhrase}", res.StatusCode,
                res.ReasonPhrase);
            res.Content.Dispose();
            return [];
        }

        var resBody = await res.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<NhanhVnShippingFeeResponse>(resBody);
        if (result != null && result.Code != 0) return result is { Code: 1 } ? result.Data : [];
        _logger.LogError("Error getting shipping fees from NhanhVn: {Messages}", result?.Messages);
        return [];
    }

    public async Task<NhanhVnOrder> CreateOrder(NhanhVnOrderRequest orderRequest)
    {
        var endpoint = $"{BaseApiUrl}/orders?appId={_settings.AppId}&businessId={_settings.BusinessId}";
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(orderRequest)
        };
        request.Headers.Add("Authorization", _settings.ApiKey);
        var res = await _httpClient.SendAsync(request);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogError("Error creating order in NhanhVn: {StatusCode} - {ReasonPhrase}", res.StatusCode,
                res.ReasonPhrase);
            res.Content.Dispose();
            return null;
        }

        var resBody = await res.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<NhanhVnOrderResponse>(resBody);
        return result?.Data;
    }

    private async Task<double> GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem)
    {
        ArgumentNullException.ThrowIfNull(shoppingCartItem);

        var product = await _productService.GetProductById(shoppingCartItem.ProductId);
        if (product == null)
            return 0;

        //attribute weight
        double attributesTotalWeight = 0;
        if (shoppingCartItem.Attributes != null && shoppingCartItem.Attributes.Any())
        {
            var attributeValues = product.ParseProductAttributeValues(shoppingCartItem.Attributes);
            foreach (var attributeValue in attributeValues)
                switch (attributeValue.AttributeValueTypeId)
                {
                    case AttributeValueType.Simple:
                    {
                        //simple attribute
                        attributesTotalWeight += attributeValue.WeightAdjustment;
                    }
                        break;
                    case AttributeValueType.AssociatedToProduct:
                    {
                        //bundled product
                        var associatedProduct =
                            await _productService.GetProductById(attributeValue.AssociatedProductId);
                        if (associatedProduct is { IsShipEnabled: true })
                            attributesTotalWeight += associatedProduct.Weight * attributeValue.Quantity;
                    }
                        break;
                }
        }

        var weight = product.Weight + attributesTotalWeight;
        return weight;
    }

    public virtual async Task<double> GetShippingWeight(IList<GetShippingOptionRequest.PackageItem> items)
    {
        double totalWeight = 0;
        //shopping cart items
        foreach (var item in items)
            totalWeight += await GetShoppingCartItemWeight(item.ShoppingCartItem) * item.GetQuantity();
        return totalWeight;
    }

    public virtual async Task<double> GetShippingSubtotal(IList<GetShippingOptionRequest.PackageItem> items)
    {
        double subTotal = 0;
        //shopping cart items
        foreach (var item in items)
        {
            var product = await _productService.GetProductById(item.ShoppingCartItem.ProductId);
            if (product == null)
                continue;

            var subtotal = await _pricingService.GetSubTotal(item.ShoppingCartItem, product, false);
            subTotal += subtotal.subTotal;
        }

        return subTotal;
    }

    public async Task<IList<NhanhVnCarrier>> GetAllCarriers()
    {
        var cacheKey = $"Shipping.NhanhVn.Carriers.All-{_settings.AppId}-{_settings.BusinessId}";
        return await _cache.GetAsync(cacheKey, async () =>
        {
            var endpoint = $"{BaseApiUrl}/shipping/carrier?appId={_settings.AppId}&businessId={_settings.BusinessId}";
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Add("Authorization", _settings.ApiKey);
            var res = await _httpClient.SendAsync(request);
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting carriers from NhanhVn: {StatusCode} - {ReasonPhrase}", res.StatusCode,
                    res.ReasonPhrase);
                res.Content.Dispose();
                return [];
            }

            var resBody = await res.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NhanhVnCarrierResponse>(resBody);
            if (result != null && result.Code != 0) return result is { Code: 1 } ? result.Data : [];
            return [];
        }, 24 * 60 * 60);
    }

    public async Task<NhanhVnCarrier> GetCarrier(int carrierId)
    {
        var carriers = await GetAllCarriers();
        return carriers.FirstOrDefault(x => x.Id == carrierId);
    }
}