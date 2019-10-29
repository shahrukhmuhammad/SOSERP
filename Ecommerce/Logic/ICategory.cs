using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entity;
using Insight.Database;

namespace Ecommerce.Logic
{
    public interface ICategory
    {
        [Sql("EcommerceCategory_GetAll")]
        List<Category> GetAll();

        [Sql("EcommerceCategory_GetById")]
        Category GetById(long Id);
    }
}
