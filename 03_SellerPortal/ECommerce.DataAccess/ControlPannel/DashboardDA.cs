using ECommerce.Entity.ControlPannel;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.ControlPannel
{
    public class DashboardDA
    {
        /// <summary>
        /// 获取商家总商品数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static int GetTotalProductCount(DashboardQueryFilter queryFilter)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Dashboard_GetTotalProductCount");
            command.SetParameterValue(queryFilter);
            return command.ExecuteScalar<int>();
        }
        /// <summary>
        /// 获取商家新订单数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static int GetNewOrderCount(DashboardQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Dashboard_GetNewOrderCount");
            command.SetParameterValue(queryFilter);
            return command.ExecuteScalar<int>();
        }
        /// <summary>
        /// 获取商家退款申请数量
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static int[] GetRMARequestCount(DashboardQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Dashboard_GetRMARequestCount");
            command.SetParameterValue(queryFilter);
            DataRow row  = command.ExecuteDataRow();
            return new int[] { row.Field<int>(0), row.Field<int>(1) };
        }
    }
}
