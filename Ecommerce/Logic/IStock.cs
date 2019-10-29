using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entity;
using Insight.Database;

namespace Ecommerce.Logic
{
    public interface IStock
    {
        [Sql("EcommerceStock_GetAll")]
        List<Stock> GetAll();

        [Sql("EcommerceStock_GetById")]
        Stock GetById(long Id);
    }
}
