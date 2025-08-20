using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Payments.Momo.Models;

public class MomoPaymentRequest
{
    [JsonPropertyName("partnerCode")]
    public string PartnerCode { get; set; }
    
    [JsonPropertyName("requestType")]
    public string RequestType { get; set; }
    
    [JsonPropertyName("ipnUrl")]
    public string IpnUrl { get; set; }
    
    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; }
    
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    
    [JsonPropertyName("lang")]
    public string Language { get; set; } = "vi";
    
    [JsonPropertyName("orderInfo")]
    public string OrderInfo { get; set; }
    
    [JsonPropertyName("requestId")]
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    
    [JsonPropertyName("extraData")]
    public string ExtraData { get; set; } = "";
    
    [JsonPropertyName("signature")]
    public string Signature { get; set; }

    public void HashRequest(string accessKey, string secretKey)
    {
        var rawHash = "accessKey=" + accessKey +
                      "&amount=" + Amount +
                      "&extraData=" + ExtraData +
                      "&ipnUrl=" + IpnUrl +
                      "&orderId=" + OrderId +
                      "&orderInfo=" + OrderInfo +
                      "&partnerCode=" + PartnerCode +
                      "&redirectUrl=" + RedirectUrl +
                      "&requestId=" + RequestId +
                      "&requestType=" + RequestType;
        Signature = SignSha256(rawHash, secretKey);
    }
    
    private string SignSha256(string message, string key)
    {
        var keyByte = Encoding.UTF8.GetBytes(key);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var hmacSha256 = new HMACSHA256(keyByte);
        var hashMessage = hmacSha256.ComputeHash(messageBytes);
        var hex = BitConverter.ToString(hashMessage);
        hex = hex.Replace("-", "").ToLower();
        return hex;
    }
}