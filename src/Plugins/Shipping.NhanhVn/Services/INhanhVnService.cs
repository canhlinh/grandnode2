using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Common;
using Shipping.NhanhVn.Models;

namespace Shipping.NhanhVn.Services;

public interface INhanhVnService
{
    Task<IList<NhanhVnShippingFee>> GetShippingFees(int weight, double price , Address from, Address to);
    
    Task<double> GetShippingWeight(IList<GetShippingOptionRequest.PackageItem> items);
    Task<double> GetShippingSubtotal(IList<GetShippingOptionRequest.PackageItem> items);

    Task<IList<NhanhVnCarrier>> GetAllCarriers();

    Task<NhanhVnCarrier> GetCarrier(int carrierId);
}