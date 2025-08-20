using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace Payments.VNPay
{
    public static class VnPayHelper
    {
        public static string CreateRequestUrl(string baseUrl, string vnp_HashSecret, IDictionary<string, string> queryParameters)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in queryParameters)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            String signData = queryString;
            if (signData.Length > 0)
            {

                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = VnPayHelper.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }

        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        public static string GetIpAddress(IHttpContextAccessor httpContextAccessor)
        {
            string ipAddress;
            try
            {
                ipAddress = httpContextAccessor.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown") || ipAddress.Length > 45)
                    ipAddress = httpContextAccessor.HttpContext.Request.Headers["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }
            if (string.IsNullOrEmpty(ipAddress)){
                return "127.0.0.1";
            }
 
            return ipAddress;
        }

        private static string GetResponseData(IDictionary<string, string> queryParameters)
        {

            StringBuilder data = new StringBuilder();
            if (queryParameters.ContainsKey("vnp_SecureHashType"))
            {
                queryParameters.Remove("vnp_SecureHashType");
            }
            if (queryParameters.ContainsKey("vnp_SecureHash"))
            {
                queryParameters.Remove("vnp_SecureHash");
            }
            foreach (KeyValuePair<string, string> kv in queryParameters)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }

        public static bool ValidateSignature(string inputHash, string secretKey, IDictionary<string, string> queryParameters)
        {
            string rspRaw = VnPayHelper.GetResponseData(queryParameters);
            string myChecksum = VnPayHelper.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
