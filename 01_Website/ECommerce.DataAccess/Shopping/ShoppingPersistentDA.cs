using ECommerce.Entity.Shopping;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.Shopping
{
    public class ShoppingPersistentDA
    {
        /// <summary>
        /// 保存购物车数据
        /// </summary>
        /// <param name="entity">购物车数据</param>
        public static void SaveShoppingCart(ShoppingCartPersistent shoppingCart)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShoppingPersistent_SaveShoppingCart");
            cmd.SetParameterValue("@Key", "");
            cmd.SetParameterValue("@KeyAscii", 0);
            cmd.SetParameterValue("@CustomerSysNo", shoppingCart.CustomerSysNo);
            cmd.SetParameterValue("@ShoppingCart", shoppingCart.ShoppingCart);
            cmd.SetParameterValue("@ShoppingCartMini", shoppingCart.ShoppingCartMini);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据用户编号获取购物车 
        /// </summary>
        /// <param name="customerSysNo">用户编号</param>
        /// <returns></returns>
        public static ShoppingCartPersistent GetShoppingCartByCustomer(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShoppingPersistent_GetShoppingCartByCustomer");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntity<ShoppingCartPersistent>();
        }

    }
}
