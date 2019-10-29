using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entity
{
    public class CatalogProduct
    {
        public long CatalogId { get; set; }
        public long ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public bool IsBestSelling { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Tags { get; set; }
        public long? ParentId { get; set; }
        public string Target { get; set; }
        public string ASIN { get; set; }
        public AmzProductCondition Condition { get; set; }
        public bool IsPending { get; set; }
        public bool IsUploaded { get; set; }
        public string Sku { get; set; }
        public long CategoryId { get; set; }
        public long ManufacturerId { get; set; }
        public List<CatalogProduct> Variants { get; set; }
        public List<SpecAttribute> Attribures { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string StoreId { get; set; }
        public bool IsPublishedFacebook { get; set; }
        public bool IsPublishedTradeOnly { get; set; }
        public string CategoryTitle { get; set; }
        public string Slug { get; set; }
        public string CategorySlug { get; set; }
        public decimal RetailPrice { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedById { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
        public long EconomyPackageId { get; set; }
        public long StandardPackageId { get; set; }
        public long ExpressPackageId { get; set; }
        public string ManufacturerTitle { get; set; }
    }
}
