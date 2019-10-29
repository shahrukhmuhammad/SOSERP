using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Entity
{
    public class Stock
    {
        public long StockId { get; set; }
        public DateTime DateTime { get; set; }
        public long OrderId { get; set; }
        public long SupplierId { get; set; }
        public StockType StockType { get; set; }
        public string Description { get; set; }
        public string LotCode { get; set; }
        public long ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
    }
    public enum StockType
    {
        StockIn = 1, StockOut = 2, Reconcile = 3
    }

}
