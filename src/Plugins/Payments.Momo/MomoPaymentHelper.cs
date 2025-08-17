using MongoDB.Bson.IO;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Momo
{
    public class MomoPaymentHelper
    {
        public static async Task<MomoPaymentResponse> CreatePayment(MomoPaymentRequest momoPaymentRequest, string apiEndPoint, string accessKey, string secretKey)
        {
            momoPaymentRequest.HashRequest(accessKey, secretKey);
            var reqContent = new StringContent(JsonSerializer.Serialize(momoPaymentRequest), Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, apiEndPoint);
            req.Content = reqContent;
            var res = client.Send(req);
            var resContent = await res.Content.ReadAsStringAsync();
            if (res.IsSuccessStatusCode)
            {
                var response = JsonSerializer.Deserialize<MomoPaymentResponse>(resContent);
                return response;
            }
            return null;
        }
    }

    public class MomoPaymentRequest
    {
        public MomoPaymentRequest()
        {
            requestType = "captureWallet";
            lang = "vi";
            extraData = "";
            requestId = Guid.NewGuid().ToString();
        }
        private string signSHA256(string message, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-", "").ToLower();
                return hex;

            }
        }
        public string partnerCode { get; set; }
        public string requestType { get; set; }
        public string ipnUrl { get; set; }
        public string redirectUrl { get; set; }
        public string orderId { get; set; }
        public int amount { get; set; }
        public string lang { get; set; }
        public string orderInfo { get; set; }
        public string requestId { get; set; }
        public string extraData { get; set; }
        public string signature { get; set; }

        public void HashRequest(string accessKey, string secretKey)
        {
            string rawHash = "accessKey=" + accessKey +
                 "&amount=" + amount +
                 "&extraData=" + extraData +
                 "&ipnUrl=" + ipnUrl +
                 "&orderId=" + orderId +
                 "&orderInfo=" + orderInfo +
                 "&partnerCode=" + partnerCode +
                 "&redirectUrl=" + redirectUrl +
                 "&requestId=" + requestId +
                 "&requestType=" + requestType;
            signature = signSHA256(rawHash, secretKey);
        }
    }

    public class MomoPaymentResponse
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public int amount { get; set; }
        public string message { get; set; }
        public int resultCode { get; set; }
        public string payUrl { get; set; }
        public string deeplink { get; set; }
        public string qrCodeUrl { get; set; }
    }
}