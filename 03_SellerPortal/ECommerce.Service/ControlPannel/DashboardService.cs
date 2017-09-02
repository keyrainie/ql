using ECommerce.DataAccess.ControlPannel;
using ECommerce.Entity.ControlPannel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Service.ControlPannel
{
    public class DashboardService
    {
        /// <summary>
        /// 获取商家总商品数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static int GetTotalProductCount(DashboardQueryFilter queryFilter)
        {
            return DashboardDA.GetTotalProductCount(queryFilter);
        }
        /// <summary>
        /// 获取商家新订单数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static int GetNewOrderCount(DashboardQueryFilter queryFilter)
        {
            return DashboardDA.GetNewOrderCount(queryFilter);
        }
        /// <summary>
        /// 获取商家退款申请数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static int[] GetRMARequestCount(DashboardQueryFilter queryFilter)
        {
            return DashboardDA.GetRMARequestCount(queryFilter);
        }
    }
}
