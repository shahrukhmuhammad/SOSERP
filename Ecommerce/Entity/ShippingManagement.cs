using System;
using System.Web;

namespace Ecommerce.Entity
{
    public class ShippingCompanyModel
    {
        public long ShippingCompanyId { get; set; }
        public string Title { get; set; }
        public string CompanyWebAddrress { get; set; }
        public string TrackingUrl { get; set; }
        public bool RequiresShippingCode { get; set; }
        public HttpPostedFileBase ShippingStampImage { get; set; }
        public decimal W1000 { get; set; }
        public decimal W1250 { get; set; }
        public decimal W1500 { get; set; }
        public decimal W1750 { get; set; }
        public decimal W2000 { get; set; }
        public decimal W5000 { get; set; }
        public decimal W30000 { get; set; }
    }

    public class ShippingPackageModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long ShippingCompanyId { get; set; }
        public PackageType PackageType { get; set; }
        public string Details { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal Price { get; set; }
        public decimal AdditionalPrice { get; set; }
        public decimal RestrictedPostagePrice { get; set; }
        public decimal AdditionalRestrictedPostagePrice { get; set; }
        public decimal PerPalletPrice { get; set; }
        public decimal AdditionalPalletPrice { get; set; }
        public int WeightFrom { get; set; }
        public int WeightTo { get; set; }
        public string DeliveryTime { get; set; }
        public string ShippingCompany { get; set; }
        public string CompanyTitle { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
    }

    public class PostcodeModel
    {
        public long Id { get; set; }
        public string Postcode { get; set; }
        public PostcodeRestrictedLevel RestrictionLevels { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal FirstItemPrice { get; set; }
        public decimal EachAdditionalItemPrice { get; set; }
        public string StrRestrictionLevels { get; set; }
    }
    public enum PriorityLevel
    {
        Economy = 1, Standard = 2, Express = 3
    }
    public enum PackageType
    {
        Letter = 1, LargeEnvelope = 2, Package = 3, LargePackage = 4, StockPile = 5
    }
    public enum PostcodeRestrictedLevel
    {
        NoPostage = 1, HighPostage = 2
    }
}
