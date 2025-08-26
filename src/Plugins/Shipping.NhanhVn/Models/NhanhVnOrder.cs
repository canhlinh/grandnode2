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
    public int Type { get; set; } = 1;
    public int DepotId { get; set; }
    public int SaleId { get; set; }
    public string Description { get; set; }
    public List<int> TagIds { get; set; }
}

public class NhanhVnOrderChannel
{
    public string AppOrderId { get; set; }
    public string SourceName { get; set; }
}

public class NhanhVnOrderShippingAddress
{
    public string Name { get; set; }
    public string Mobile { get; set; }
    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }
    public string Address { get; set; }
    public string LocationVersion { get; set; } = "v1";
}

public class NhanhVnOrderCarrier
{
    public int SendCarrierType { get; set; } = 2;
    public int Id { get; set; }
    public string ServiceCode { get; set; }
    public int AccountId { get; set; }
    public int ShopId { get; set; }
    public double CustomerShipFee { get; set; }
}

public class NhanhVnOrderProduct
{
    public long Id { get; set; }
    public double Price { get; set; }
    public double Discount { get; set; }
    public int Quantity { get; set; }
    public double Vat { get; set; }
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

// {
// "code": 1,
// "errorCode": null,
// "messages": [],
// "data": {
//     "id": "(int) ID đơn hàng",
//     "businessId": "(int) ID doanh nghiệp",
//     "shippingAddress": {
//         "name": "(string) Họ và tên",
//         "mobile": "(string) Số điện thoại"
//     },
//     "totalAmount": "(int) Tổng tiền đơn hàng"
// }
// }

public class NhanhVnOrderResponse
{
    public int Code { get; set; }
    public string ErrorCode { get; set; }
    public List<string> Messages { get; set; }
    public NhanhVnOrder Data { get; set; }
}

public class NhanhVnOrder
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public NhanhVnOrderShippingAddress ShippingAddress { get; set; }
    public double TotalAmount { get; set; }
}