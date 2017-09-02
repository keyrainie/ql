using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.SO;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.SO
{
    public class SOPriceDA
    {
        /// <summary>
        /// 根据订单编号删除订单的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public static void DeleteSOPriceBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Delete_SOPriceBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加订单价格信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="companyCode"></param>
        public static void InsertSOPrice(SOPriceMasterInfo info, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Insert_SOPrice");
            command.SetParameterValue(info, true, false);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@InUser", string.Empty);
            info.SysNo = command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 添加订单商品价格信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        public static void InsertSOPriceItem(SOPriceItemInfo info, int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Insert_SOPriceItem");
            command.SetParameterValue(info, true, false);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@InUser", string.Empty);
            command.ExecuteNonQuery();
        }
    }
}
