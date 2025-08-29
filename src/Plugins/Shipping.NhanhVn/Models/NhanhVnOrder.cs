using System.Text.Json.Serialization;

namespace Shipping.NhanhVn.Models;

public class NhanhVnOrderRequest
{
    public NhanhVnOrderInfo Info { get; set; }
    public NhanhVnOrderChannel Channel { get; set; }
    public NhanhVnOrderShippingAddress ShippingAddress { get; set; }
    public NhanhVnOrderCarrier Carrier { get; set; }
    public List<NhanhVnOrderProduct> Products { get; set; }
    public NhanhVnOrderPayment Payment { get; set; }
}

public class NhanhVnOrderInfo
{
    public int type { get; set; } = 1;
    public int depotId { get; set; }
    public int saleId { get; set; }
    public string description { get; set; }
    public List<int> tagIds { get; set; }
}

public class NhanhVnOrderChannel
{
    public string appOrderId { get; set; }
    public string sourceName { get; set; }
}

public class NhanhVnOrderShippingAddress
{
    public string name { get; set; }
    public string mobile { get; set; }
    public int cityId { get; set; }
    public int districtId { get; set; }
    public int wardId { get; set; }
    public string address { get; set; }
    public string locationVersion { get; set; } = "v1";
}

public class NhanhVnOrderCarrier
{
    public int sendCarrierType { get; set; } = 1;
    public int id { get; set; }
    public string serviceCode { get; set; }
    public int serviceId { get; set; }
    public int accountId { get; set; }
    public int shopId { get; set; }
    public double customerShipFee { get; set; }
    public int allowTesting { get; set; } = 0;
}

public class NhanhVnOrderProduct
{
    public long id { get; set; }
    public double price { get; set; }
    public double discount { get; set; }
    public int quantity { get; set; }
    public double vat { get; set; }
    public List<NhanhVnOrderProductGift> Gifts { get; set; }
}

public class NhanhVnOrderProductGift
{
    public long Id { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}

public class NhanhVnOrderPayment
{
    public double DepositAmount { get; set; }
    public long DepositAccountId { get; set; }
    public double TransferAmount { get; set; }
    public long TransferAccountId { get; set; }
}

public class NhanhVnOrderResponse
{
    public int code { get; set; }
    public string errorCode { get; set; }
    public List<string> messages { get; set; }
    public NhanhVnOrder data { get; set; }
}

public class NhanhVnOrder
{
    public int id { get; set; }
    public int businessId { get; set; }
    public NhanhVnOrderShippingAddress shippingAddress { get; set; }
    public double totalAmount { get; set; }
}