using Ecommerce.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Logic
{
    public interface ICatalog
    {
        [Sql("EcommerceProductCatalog_GetAll")]
        List<Product> GetProducts();

        [Sql("EcommerceProductCatalog_GetById")]
        CatalogProduct GetByProductId(long Id, string Target, string StoreId = null);

        [Sql("EcommerceProductCatalog_Create")]
        long Create(CatalogProduct model);

        [Sql("EcommerceProductCatalog_UpdateIsFeatured")]
        void UpdateIsFeatured(long ProductId, bool IsFeatured);

        [Sql("EcommerceProductCatalog_UpdateIsBestSelling")]
        void UpdateIsBestSelling(long ProductId, bool IsBestSelling);

        [Sql("EcommerceProductCatalog_UpdateIsPublished")]
        void UpdateIsPublished(long ProductId, bool IsPublished);

        [Sql("EcommerceProductCatalog_UpdateIsPublishedTradeOnly")]
        void UpdateIsPublishedTradeOnly(long ProductId, bool IsPublishedTradeOnly);

        [Sql("EcommerceProductCatalog_UpdateIsPublishedFacebook")]
        void UpdateIsPublishedFacebook(long ProductId, bool IsPublishedFacebook);

    }
}
