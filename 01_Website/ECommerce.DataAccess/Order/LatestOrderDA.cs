using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.Order
{
    public class LatestOrderDA
    {

        public static List<Entity.Order.OrderInfo> GetLatestOrder()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Order_GetLatestOrder");
            return cmd.ExecuteEntityList<Entity.Order.OrderInfo>();
        }
    }
}
