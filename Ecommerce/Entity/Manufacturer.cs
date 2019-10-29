using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entity
{
    public class Manufacturer
    {
        public long ManufacturerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

}
