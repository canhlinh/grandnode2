namespace Shipping.NhanhVn.Models;

public class NhanhVnCarrier
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; }
    public string ShopId { get; set; }
}

public class NhanhVnService
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
}

public class NhanhVnShippingFee
{
    public string Logo { get; set; }
    public NhanhVnCarrier Carrier { get; set; }
    public NhanhVnService Service { get; set; }
    public double ShipFee { get; set; }
    public double CodFee { get; set; }
    public double DeclaredFee { get; set; }
    public double CustomerShipFee { get; set; }
}

public class NhanhVnShippingFeeResponse
{
    public int Code { get; set; }
    
    public string Messages { get; set; }
    public List<NhanhVnShippingFee> Data { get; set; }
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
    public double TotalCod { get; set; }
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