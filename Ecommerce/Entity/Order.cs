using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entity
{
    public class Order
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public DateTime DateTime { get; set; }
        public Guid ContactId { get; set; }
        public decimal Total { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderType OrderType { get; set; }
        public string Description { get; set; }
        public OrderSource OrderSource { get; set; }
        public string ContactTitle { get; set; }
        public string ShippingDetails { get; set; }
        public string Comments { get; set; }
        public long ShippingAdminId { get; set; }
        public string ShippingAdmin { get; set; }
        public long ShippingPackagesId { get; set; }
        public long ShippingCopmanyId { get; set; }
        public string ShippingCompany { get; set; }
        public string ShippingCode { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }
        public int Rating { get; set; }
        public long ReceivingAdminId { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get { return SubTotal - Discount + Tax + ShippingCost; } }
        public List<OrderDetail> Details { get; set; }
        public string StoreId { get; set; }

        public LocalPickup IsLocalPickup { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public decimal OrderTotal { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDropShipper { get; set; }
        public bool IsTrader { get; set; }
        public decimal Price { get; set; }
        public long Quantity { get; set; }
        public string ProductDescription { get; set; }
        public string shippingpackagesDetails { get; set; }
        public string shippingpackagesTitle { get; set; }
        public string shippingpackagesDeliveryTime { get; set; }
        public decimal shippingpackagesPrice { get; set; }
        public List<OrderAddressDetails> OrderAddressDetails { get; set; }
        public PriorityLevel PostageType { get; set; }
        public OrderAddressDetails OrderBillingDetails { get; set; }

        public OrderAddressDetails OrderShippingDetails { get; set; }
    }

    public class OrderDetail
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string EAN { get; set; }
        public string MPN { get; set; }
        public string UPC { get; set; }
        public string ProductDetails { get; set; }
        public EANCodeType EANCodeType { get; set; }
        public long CategoryId { get; set; }
        public long ManufacturerId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public string ShortDescription { get; set; }
        public ProductType Type { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool ApplyVAT { get; set; }
        public string PackageSize { get; set; }
        public decimal TradeSalePrice { get; set; }
        public decimal Total { get { return Quantity * Price; } }
        public long? ProductVariantId { get; set; }

        public PriorityLevel PriorityLevel { get; set; }
}

    public class OrderAddressDetails
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string PostalAddress { get; set; }
        public string PostalAddress2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public OrderAddressType AddressType { get; set; }

    }

    public enum OrderStatus
    {
        New = 1,
        Processing = 2,
        Completed = 3,
        Cancelled = 4,
        Pending = 5,
    }
    public enum OrderType
    {
        Purchase = 1, Sale = 2, SalesReturn = 3, SalesExchange = 4
    }
    public enum OrderSource
    {
        Website = 1, Inventory = 2, Amazon = 3, Ebay = 4//,WholeSale=5,Pos=6
    }
    public enum LocalPickup
    {
        No = 1, Yes = 2
    }
    public enum PaymentMethod
    {
        CashOnDelivery = 1, OnlinePayment = 2, BankTransfer = 3
    }
    public enum OrderAddressType
    {
        Shipping = 1, Billing = 2
    }
}
