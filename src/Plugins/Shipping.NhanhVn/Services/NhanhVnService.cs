using System.Text.Json;
using Grand.Domain.Common;
using Shipping.NhanhVn.Models;

namespace Shipping.NhanhVn.Services;

public class NhanhVnService: INhanhVnService
{
    private const string BaseApiUrl = "https://pos.open.nhanh.vn/v3.0";
    private readonly ShippingNhanhVnSettings _settings;
    
    public NhanhVnService(ShippingNhanhVnSettings settings)
    {
        _settings = settings;
    }
    
    public async Task<IList<NhanhVnShippingFee>> GetShippingFees(int weight, double price, Address from, Address to)
    {
        var requestBody = new NhanhVnShippingFeeFilters
        {
            Type = 1,
            ShippingWeight = weight,
            Price = price,
            TotalCod = 0,
            ShippingFrom = new NhanhVnShippingAddress
            {
                CityId = int.Parse(from.ProvinceId),
                DistrictId = int.Parse(from.DistrictId),
                WardId = int.Parse(from.WardId),
                Address = from.Address1,
                LocationVersion = $"v{from.DivisionVersion}",
            },
            ShippingTo = new NhanhVnShippingAddress
            {
                CityId = int.Parse(to.ProvinceId),
                DistrictId = int.Parse(to.DistrictId),
                WardId = int.Parse(to.WardId),
                Address = to.Address1,
                LocationVersion = $"v{to.DivisionVersion}",
            }
        };

        var endpoint = $"{BaseApiUrl}/shipping/fee?appId={_settings.AppId}&businessId={_settings.BusinessId}";
        var client = new HttpClient();
        var reqContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var req = new HttpRequestMessage(HttpMethod.Post, endpoint);
        req.Content = reqContent;
        var res = await client.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode) return null;
        var result = JsonSerializer.Deserialize<NhanhVnShippingFeeResponse>(resContent)?.Data;
        return result;
    }
}