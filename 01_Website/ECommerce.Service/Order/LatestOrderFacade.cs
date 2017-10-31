using ECommerce.DataAccess.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Order
{
    public class LatestOrderFacade
    {
        public static List<Entity.Order.OrderInfo> GetLatestOrders(int pageID,
            int pageType,
            int posID,
            int count)
        {
            return LatestOrderDA.GetLatestOrder();
        }
    }
}
