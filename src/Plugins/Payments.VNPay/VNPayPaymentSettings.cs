using Grand.Domain.Configuration;

namespace Payments.VNPay
{
    public class VNPayPaymentSettings : ISettings
    {
        public string ReturnURL { get; set; }
        public string PaymentURL { get; set; }
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }
    }
}