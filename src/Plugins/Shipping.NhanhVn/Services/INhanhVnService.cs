using Grand.Domain.Common;
using Shipping.NhanhVn.Models;

namespace Shipping.NhanhVn.Services;

public interface INhanhVnService
{
    Task<IList<NhanhVnShippingFee>> GetShippingFees(int weight, double price , Address from, Address to);
}