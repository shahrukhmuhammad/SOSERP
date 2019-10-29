using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ecommerce.Entity
{
    public class Product
    {
        public long ProductId { get; set; }
        public string Sku { get; set; }
        public string EAN { get; set; }
        public string MPN { get; set; }
        public string UPC { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public long CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string Slug { get; set; }
        public long ManufacturerId { get; set; }
        public string ManufacturerTitle { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public int GroupId { get; set; }
        public ProductType Type { get; set; }
        public long? ParentId { get; set; }

        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public EANCodeType EANCodeType { get; set; }

        public string SupplierId { get; set; }
        public string LotCode { get; set; }
        public string ProductVat { get; set; }
        public string ProductTotal { get; set; }
        public string PackageSize { get; set; }
        public bool ApplyVAT { get; set; }

        public DateTime CreatedOn { get; set; }
        public long CreatedById { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsBestSelling { get; set; }

        public bool IsPublished { get; set; }

        public bool IsPublishedTradeOnly { get; set; }

        public bool IsPublishedFacebook { get; set; }

        public string Tags { get; set; }

        public decimal TradeSalePrice { get; set; }

        public AmzProductCondition Condition { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal OldPrice { get; set; }

        public PriorityLevel PriorityLevel { get; set; }

        public long EconomyPackageId { get; set; }
        public long StandardPackageId { get; set; }
        public long ExpressPackageId { get; set; }
        public List<SpecAttribute> Attributes { get; set; }
        public Guid SelectedSuppliers { get; set; }
        public string StrSalePrices { get; }
        public string StrPurchasePrice { get; }
        public DataSet ProductVariants { get; set; }
        public StockRecordModel Stock { get; set; }
        [RegularExpression("^.+(.jpg|.JPG|.jpeg|.JPEG)$")]
        public HttpPostedFileBase Image { get; set; }
        public List<BulkPrices> BulkPrices { get; set; }
    }
    public class StockRecordModel
    {
        public long StockId { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public long OrderId { get; set; }
        public StockType StockType { get; set; }
        [Display(Name = "Supplier")]
        public long SupplierId { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Lot Code")]
        public string LotCode { get; set; }
        [Required]
        [Display(Name = "Product")]
        public long ProductId { get; set; }
        [Required]
        [Range(0, 9999999)]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "Sale")]
        public decimal SalePrice { get; set; }
        [Required]
        [Display(Name = "Purchase")]
        public decimal PurchasePrice { get; set; }
    }
    public class SpecAttribute
    {
        public long Id { get; set; }
        public int AttrId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public float Price { get; set; }
    }
    public class BulkPrices
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }

    public enum AmzProductCondition
    {
        New, Refurbished, UsedLikeNew, UsedVeryGood, UsedGood, UsedAcceptable, CollectibleLikeNew, CollectibleVeryGood, CollectibleGood, CollectibleAcceptable
    }

    public enum EANCodeType
    {
        EAN13 = 1, EAN8 = 2, UPC = 3, GTIN = 4, GCID = 5
    }

    public enum ProductStatus
    {
        Active = 0, Discontinue = 1, History = 2
    }
    public enum ProductType
    {
        Single, WithVariants
    }

}
