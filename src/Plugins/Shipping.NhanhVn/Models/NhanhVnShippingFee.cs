using System.Text.Json.Serialization;

namespace Shipping.NhanhVn.Models;

public class NhanhVnCarrier
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("accountId")]
    public int AccountId { get; init; }
    
    [JsonPropertyName("accountName")]
    public string AccountName { get; init; }
    
    [JsonPropertyName("shopId")]
    public string ShopId { get; init; }
}

public class NhanhVnService
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; }
    
    [JsonPropertyName("code")]
    public string Code { get; init; }
    
    [JsonPropertyName("description")]
    public string Description { get; init; }
}

public class NhanhVnShippingFee
{
    [JsonPropertyName("logo")]
    public string Logo { get; init; }
    
    [JsonPropertyName("carrier")]
    public NhanhVnCarrier Carrier { get; init; }
    
    [JsonPropertyName("service")]
    public NhanhVnService Service { get; init; }
    
    [JsonPropertyName("shipFee")]
    public int ShipFee { get; init; }
    
    [JsonPropertyName("codFee")]
    public int CodFee { get; init; }
    
    [JsonPropertyName("declaredFee")]
    public int DeclaredFee { get; init; }
    
    [JsonPropertyName("customerShipFee")]
    public int CustomerShipFee { get; init; }
}

public class NhanhVnShippingFeeResponse
{
    [JsonPropertyName("code")]
    public int Code { get; init; }
    
    [JsonPropertyName("errorCode")]
    
    public string ErrorCode { get; init; }
    
    [JsonPropertyName("messages")]
    public string Messages { get; init; }
    
    [JsonPropertyName("data")]
    public List<NhanhVnShippingFee> Data { get; init; }
}

public class NhanhVnShippingFeeRequest
{
    public NhanhVnShippingFeeFilters Filters { get; set; }
}

public class NhanhVnShippingFeeFilters
{
    public int Type { get; set; } = 1;
    public int ShippingWeight { get; set; }
    public double Price { get; set; }
    public NhanhVnShippingAddress ShippingFrom { get; set; }
    public NhanhVnShippingAddress ShippingTo { get; set; }
}

public class NhanhVnShippingAddress
{
    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }
    public string Address { get; set; }
    public string LocationVersion { get; set; } = "v1";
}