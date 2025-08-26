using System.Text.Json;
using Grand.Business.Core.Interfaces.Catalog.Prices;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Orders;
using Microsoft.Extensions.Logging;
using Shipping.NhanhVn.Models;

namespace Shipping.NhanhVn.Services;

public class NhanhVnService: INhanhVnService
{
    private const string BaseApiUrl = "https://pos.open.nhanh.vn/v3.0";
    private readonly ShippingNhanhVnSettings _settings;
    private readonly IProductService _productService;
    private readonly IPricingService _pricingService;
    private readonly ICountryService _countryService;
    private readonly ILogger<NhanhVnService> _logger;
    public NhanhVnService(
        ILogger<NhanhVnService> logger,
        ShippingNhanhVnSettings settings, 
        IProductService productService, 
        IPricingService pricingService, 
        ICountryService countryService)
    {
        _settings = settings;
        _productService = productService;
        _pricingService = pricingService;
        _countryService = countryService;
        _logger = logger;
    }
    
    public async Task<IList<NhanhVnShippingFee>> GetShippingFees(int weight, double price, Address from, Address to)
    {
        var fromProvince = await _countryService.GetProvinceById(from.ProvinceId);
        var fromDistrict = await _countryService.GetDistrictById(from.DistrictId);
        var fromWard = await _countryService.GetWardById(from.WardId);
        if (fromProvince == null || fromDistrict == null || fromWard == null)
            return null;
        var toProvince = await _countryService.GetProvinceById(to.ProvinceId);
        var toDistrict = await _countryService.GetDistrictById(to.DistrictId);
        var toWard = await _countryService.GetWardById(to.WardId);
        if (toProvince == null || toDistrict == null || toWard == null)
            return null;
        
        var requestBody = new NhanhVnShippingFeeFilters
        {
            Type = 1,
            ShippingWeight = weight,
            Price = price,
            TotalCod = 0,
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
        };

        var endpoint = $"{BaseApiUrl}/shipping/fee?appId={_settings.AppId}&businessId={_settings.BusinessId}";
        var client = new HttpClient();
        var reqContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var req = new HttpRequestMessage(HttpMethod.Post, endpoint);
        req.Content = reqContent;
        req.Headers.TryAddWithoutValidation("Authorization", _settings.ApiKey);
        var res = await client.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode) return [];
        var result = JsonSerializer.Deserialize<NhanhVnShippingFeeResponse>(resContent);
        if (result != null && result.Code != 0) return result is { Code: 1 } ? result.Data : [];
        _logger.LogError("Error getting shipping fees from NhanhVn: {Messages}", result?.Messages);
        return [];
    }
    
    public async Task<NhanhVnOrder> CreateOrder(NhanhVnOrderRequest orderRequest)
    {
        var endpoint = $"{BaseApiUrl}/orders?appId={_settings.AppId}&businessId={_settings.BusinessId}";
        var client = new HttpClient();
        var reqContent = new StringContent(JsonSerializer.Serialize(orderRequest), Encoding.UTF8, "application/json");
        var req = new HttpRequestMessage(HttpMethod.Post, endpoint);
        req.Content = reqContent;
        req.Headers.TryAddWithoutValidation("Content-Type", "application/json");
        req.Headers.TryAddWithoutValidation("Authorization", _settings.ApiKey);
        var res = await client.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode) return null;
        var result = JsonSerializer.Deserialize<NhanhVnOrderResponse>(resContent)?.Data;
        return result;
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
}