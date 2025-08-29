using System.Text.Json;
using System.Xml.Serialization;
using Grand.Domain.Shipping;
using Grand.SharedKernel.Extensions;
using MediatR;
using Grand.Business.Core.Events.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Customers;
using Shipping.NhanhVn.Domain;
using Shipping.NhanhVn.Models;
using Shipping.NhanhVn.Services;

namespace Shipping.NhanhVn.Events
{
    public class OrderPaidEventHandler : INotificationHandler<OrderPaidEvent>
    {
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly ShippingNhanhVnSettings _settings;
        private readonly INhanhVnService _nhanhVnService;
        private readonly ISettingService _settingService;

        public OrderPaidEventHandler(
            ICountryService countryService,
            ICustomerService customerService,
            IPaymentTransactionService paymentTransactionService,
            IOrderService orderService,
            IShipmentService shipmentService,
            ShippingNhanhVnSettings settings,
            INhanhVnService nhanhVnService,
            ISettingService settingService
)
        {
            _countryService = countryService;
            _customerService = customerService;
            _paymentTransactionService = paymentTransactionService;
            _orderService = orderService;
            _settings = settings;
            _shipmentService = shipmentService;
            _nhanhVnService = nhanhVnService;
            _settingService = settingService;
        }

        public async Task Handle(OrderPaidEvent notification, CancellationToken cancellationToken)
        {
            var order = notification.Order;
            if (order == null) return;

            var paymentTransaction = await _paymentTransactionService.GetOrderByGuid(order.OrderGuid);

            if (order.ShippingRateProviderSystemName == ShippingNhanhVnDefaults.SystemName)
            {
                if (!string.IsNullOrEmpty(order.ShippingOptionAttribute))
                {
                    // deserialize shipping option
                    
                    var shippingOption = JsonSerializer.Deserialize<ShippingNhanhVnSerializable>(order.ShippingOptionAttribute);
                    if (shippingOption != null && order.ShippingAddress != null && shippingOption.CarrierId > 0 && shippingOption.ServiceId > 0)
                    {
                        var customer = await _customerService.GetCustomerById(order.CustomerId);
                        if (customer == null)
                            return;

                        var shipment = new Shipment() {
                            OrderId = order.Id,
                            SeId = order.SeId,
                            TotalWeight = null,
                            ShippedDateUtc = null,
                            DeliveryDateUtc = null,
                            AdminComment = "Hệ thống tự động gửi đơn hàng sang nhanh.vn",
                            CreatedOnUtc = DateTime.UtcNow,
                            StoreId = order.StoreId,
                            NhanhCarrierId = shippingOption.CarrierId,
                            NhanhCarrierName = shippingOption.CarrierName,
                            NhanhServiceId = shippingOption.ServiceId,
                            NhanhServiceName = shippingOption.ServiceName,
                        };

                        double? totalWeight = null;
                        foreach (var orderItem in order.OrderItems)
                        {
                            var orderItemTotalWeight = orderItem.ItemWeight.HasValue ? orderItem.ItemWeight * orderItem.Quantity : null;
                            if (orderItemTotalWeight.HasValue)
                            {
                                if (!totalWeight.HasValue)
                                    totalWeight = 0;
                                totalWeight += orderItemTotalWeight.Value;
                            }
                            shipment.ShipmentItems.Add(new ShipmentItem() {
                                ProductId = orderItem.ProductId,
                                OrderItemId = orderItem.Id,
                                Quantity = orderItem.Quantity,
                                Attributes = orderItem.Attributes,
                            });
                        }
                        shipment.TotalWeight = totalWeight;
                        await _shipmentService.InsertShipment(shipment);
                        
                        var shippingSettings = await _settingService.LoadSetting<ShippingSettings>(order.StoreId);
                        var fromAddress = shippingSettings.ShippingOriginAddress;
                        if (fromAddress == null)
                            throw new Exception("Shipping origin address is not set");
                        var fromProvince = await _countryService.GetProvinceById(shippingSettings.ShippingOriginAddress.ProvinceId);
                        var fromDistrict = await _countryService.GetDistrictById(shippingSettings.ShippingOriginAddress.DistrictId);
                        var fromWard = await _countryService.GetWardById(shippingSettings.ShippingOriginAddress.DistrictId);
                        if (fromProvince == null || fromDistrict == null || fromWard == null)
                            return;
                        var toProvince = await _countryService.GetProvinceById(order.ShippingAddress.ProvinceId);
                        var toDistrict = await _countryService.GetDistrictById(order.ShippingAddress.DistrictId);
                        var toWard = await _countryService.GetWardById(order.ShippingAddress.DistrictId);
                        if (toProvince == null || toDistrict == null || toWard == null)
                            return;
                        
                        var nhanhVnOrder = await _nhanhVnService.CreateOrder(new NhanhVnOrderRequest()
                        {
                            Info   = new NhanhVnOrderInfo()
                            {
                                type = 1,
                                description = $"Order number {order.OrderNumber}",
                            },
                            Channel = new NhanhVnOrderChannel()
                            {
                                appOrderId = order.Id,
                                sourceName = "G1",
                            },
                            ShippingAddress = new NhanhVnOrderShippingAddress()
                            {
                                name = $"{order.ShippingAddress.FirstName} {order.ShippingAddress.LastName}",
                                mobile = order.ShippingAddress.PhoneNumber,
                                address = order.ShippingAddress.Address1,
                                cityId = toProvince.NhanhVnId,
                                districtId = toDistrict.NhanhVnId,
                                wardId = toWard.NhanhVnId,
                                locationVersion = $"v{order.ShippingAddress.DivisionVersion}",
                            },
                            Carrier = new NhanhVnOrderCarrier()
                            {
                                sendCarrierType = 1,
                                id = shippingOption.CarrierId,
                                serviceId = shippingOption.ServiceId,
                                allowTesting = 1,
                            },
                            Products = order.OrderItems.Select(item => new NhanhVnOrderProduct()
                            {
                                id = 1,
                                quantity = item.Quantity,
                                price = item.UnitPriceExclTax,
                                discount = 0,
                                vat = item.TaxRate,
                            }).ToList(),
                        });

                        // var createNhanhOrderData = new CreateOrderData() {
                        //     id = order.Id,
                        //     type = "Shipping",
                        //     customerName = order.ShippingAddress.FirstName + customer.ShippingAddress.LastName,
                        //     customerMobile = order.ShippingAddress.PhoneNumber,
                        //     customerEmail = customer.Email,
                        //     customerAddress = order.ShippingAddress.Address1,
                        //     customerCityName = (await _countryService.GetProvinceById(order.ShippingAddress.ProvinceId))?.Name,
                        //     customerDistrictName = (await _countryService.GetDistrictById(order.ShippingAddress.DistrictId))?.Name,
                        //     customerWardLocationName = (await _countryService.GetWardById(order.ShippingAddress.DistrictId, order.ShippingAddress.WardId))?.Name,
                        //     paymentGateway = order.PaymentMethodSystemName,
                        //     paymentCode = paymentTransaction.AuthorizationTransactionId,
                        //     carrierId = shippingOption.CarrierId,
                        //     carrierServiceId = shippingOption.ServiceId,
                        //     customerShipFee = (int)shippingOption.Rate,
                        //     description = string.Format("order number {0}", order.OrderNumber),
                        //     productList = order.OrderItems.Select(item => new ProductList() {
                        //         id = item.ProductId,
                        //         quantity = item.Quantity,
                        //         name = item.Name,
                        //         code = item.Sku,
                        //         price = (int)item.UnitPriceInclTax,
                        //         weight =  (int)item.ItemWeight,
                        //     }).ToList(),
                        // };
                        //
                        // var result = await NhanhVnHelper.CreateOrder(_settings, createNhanhOrderData);
                        // if (result.code == 1 )
                        // {
                        //     await _orderService.InsertOrderNote(new OrderNote {
                        //         Note = String.Format("Đã gửi thành công đơn hàng sang nhanh.vn. nhanhOrderId = {0}", result.data.orderId),
                        //         OrderId = order.Id,
                        //         DisplayToCustomer = false,
                        //         CreatedOnUtc = DateTime.UtcNow
                        //     });
                        //
                        //     shipment.TrackingNumber = result.data.carrierCode;
                        //     shipment.NhanhOrderId = result.data.orderId;
                        //     await _shipmentService.InsertShipment(shipment);
                        // } 
                        // else
                        // {
                        //     await _orderService.InsertOrderNote(new OrderNote {
                        //         Note = string.Format("Lỗi khi gửi đơn hàng sang nhanh.vn", result.Content),
                        //         OrderId = order.Id,
                        //         DisplayToCustomer = false,
                        //         CreatedOnUtc = DateTime.UtcNow
                        //     });
                        // }
                    }
                }
            }
        }
    }
}
