using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Entity;
using Insight.Database;

namespace Ecommerce.Logic
{
    public interface IManufacturer
    {
        [Sql("EcommerceManufacturers_GetAll")]
        List<Manufacturer> GetAll();

        [Sql("EcommerceManufacturers_GetById")]
        Manufacturer GetById(long Id);
    }
}
