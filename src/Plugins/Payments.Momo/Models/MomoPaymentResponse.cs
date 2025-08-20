namespace Payments.Momo.Models;

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