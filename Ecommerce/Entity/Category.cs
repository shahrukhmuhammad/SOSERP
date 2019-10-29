using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entity
{
    public class Category
    {
        public long CategoryId { get; set; }
        public string Title { get; set; }
        public long? ParentId { get; set; }
        public string Description { get; set; }
        //public string ParentTitle { get; set; }
        public string Slug { get; set; }
    }

}
