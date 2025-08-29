using System.Text.Json.Serialization;

namespace Shipping.NhanhVn.Models;

public class NhanhVnCarrierResponse
{
    [JsonPropertyName("code")]
    public int Code { get; init; }
    
    [JsonPropertyName("data")]
    public List<NhanhVnCarrier> Data { get; init; }
}

public class NhanhVnCarrier
{
    [JsonPropertyName("id")]
    public int Id { get; init; }
    
    [JsonPropertyName("name")]
    public string Name { get; init; }
    
    [JsonPropertyName("logo")]
    public string Logo { get; init; }
    
    [JsonPropertyName("status")]
    public int Status { get; init; }
    
    [JsonPropertyName("shortName")]
    public string ShortName { get; init; }
    
    [JsonPropertyName("services")]
    public List<NhanhVnService> Services { get; init; }
}