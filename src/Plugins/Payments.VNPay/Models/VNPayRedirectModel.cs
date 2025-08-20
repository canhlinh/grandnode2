using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Payments.VNPay.Models;

public class VNPayRedirectModel
{
    
    [FromQuery(Name = "vnp_Amount")]
    public int Amount { get; set; }

    [FromQuery(Name = "vnp_ResponseCode")]
    public string ResponseCode { get; set; }

    [FromQuery(Name = "vnp_TransactionStatus")]
    public string TransactionStatus { get; set; }

    [FromQuery(Name = "Vnp_TxnRef")]
    public string TxnRef { get; set; }

    [FromQuery(Name = "vnp_SecureHash")]
    public string SecureHash { get; set; }

    [FromQuery(Name = "vnp_TransactionNo")]
    public string TransactionNo { get; set; }

    // var vnpayData = new SortedList<string, string>();
    public SortedList<string, string> VnpayData { get; set; } = new();

    // bool isValid = VnPayHelper.ValidateSignature(vnp_SecureHash, _vnPayPaymentSettings.HashSecret, vnpayData);
    public bool IsValid { get; set; }
    
    // Guid orderId = Guid.Empty;
    public Guid OrderId { get; set; } = Guid.Empty; 
}