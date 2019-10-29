using Ecommerce.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Logic
{
    public interface IOrder
    {
        [Sql("EcommerceOrders_GetSaleOrders")]
        List<Order> GetSaleOrders(int type1, int type2, int type3);

        [Sql("EcommerceOrders_GetOrderDetails")]
        List<OrderDetail> GetOrderDetails(); 

        [Sql("EcommerceOrders_GetById")] 
        Order GetById(long Id);

        [Sql("EcommerceOrders_SaleOrderGetById")]
        Order GetSaleOrderById(long Id);

        [Sql("EcommerceOrderDetails_GetById")]
        List<OrderDetail> GetByIdOrderDetails(long Id);

        [Sql("EcommerceOrderAddressDetails_GetById")]
        List<OrderAddressDetails> GetByIdOrderAddressDetails(long Id);

        [Sql("EcommerceOrders_Create")]
        long Create(Order model);

        [Sql("EcommerceOrderDetails_Create")]
        void Create(OrderDetail model);

        [Sql("EcommerceOrder_Update")]
        long Update(Order model);

        [Sql("EcommerceOrderAddressDetails_Create")]
        long Create(OrderAddressDetails model);

        [Sql("EcommerceOrders_GetPurchaseOrder")]
        List<Order> GetPOList(int type);
    }
}
