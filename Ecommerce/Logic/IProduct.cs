using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entity;
using Insight.Database;

namespace Ecommerce.Logic
{
    public interface IProduct
    {
        [Sql("EcommerceProduct_GetAll")]
        List<Product> GetAllProducts();

        [Sql("EcommerceProduct_GetById")]
        Product GetById(long Id);

        [Sql("EcommerceProduct_Create")]
        long Create(Product model);

        [Sql("EcommerceProduct_SetSupplier")]
        void SetSuppliers(long id, string sup);

        [Sql("EcommerceProduct_Update")]
        long Update(Product model);

        [Sql("EcommerceProductBulkPrices_Create")]
        void CreateBulkPrices(BulkPrices model);

        [Sql("EcommerceProductBulkPrices_GetAllById")]
        List<BulkPrices> GetBulkPricesById(long Id);

        [Sql("EcommerceProductBulkPrices_Delete")]
        void DeletePrices(long Id);

        //[Sql("EcommerceProductBulkPrices_Delete")]
        //string GetProductSKU();

        [Sql("EcommerceProduct_GetBySku")]
        Product GetBySku(string sku);

    }

    //public string GetProductSKU()
    //{
    //    //var repo = Db.As<IBillPaymentRepository>();
    //    var abbrv = new Dictionary<string, string>();
    //    abbrv.Add(Settings.Get.SkuPrefix, Settings.Get.SkuPrefix);
    //    var count = _database.ExecuteScalar<string>("sp_GetSKUCode").ToString();
    //    var hasProducts = _database.Fetch<Product>("select Title,ProductId from products").Count();
    //    var code = "";
    //    if (hasProducts > 0)
    //    {
    //        count = (Int32.Parse(count) + 1).ToString("D" + count.Length);
    //        code = string.Format("{0}{1:D5}", abbrv[Settings.Get.SkuPrefix], count);
    //    }
    //    return hasProducts > 0 ? code : count;
    //}

}
