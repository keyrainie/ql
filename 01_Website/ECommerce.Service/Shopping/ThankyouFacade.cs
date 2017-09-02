using System.Collections.Generic;

using ECommerce.Entity.Shopping;
using ECommerce.DataAccess.Shopping;

namespace ECommerce.Facade.Shopping
{
    /// <summary>
    /// Thankyou页面
    /// </summary>
    public static class ThankyouFacade
    {
        /// <summary>
        /// 根据购物车ID获取订单简单信息列表
        /// </summary>
        /// <param name="shoppingCartID">购物车ID</param>
        /// <returns></returns>
        public static List<ThankyouOrderInfo> GetOrderListByShoppingCartID(int shoppingCartID)
        {
            List<ThankyouOrderInfo> result = ShoppingOrderDA.GetOrderListByShoppingCartID(shoppingCartID);

            if (result == null)
                result = new List<ThankyouOrderInfo>();
            return result;
        }
    }
}
