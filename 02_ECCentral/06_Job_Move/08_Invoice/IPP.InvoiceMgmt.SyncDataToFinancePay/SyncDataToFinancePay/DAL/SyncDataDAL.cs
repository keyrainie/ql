using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;

namespace IPPOversea.Invoicemgmt.SyncDataToFinancePay.DAL
{
    /// <summary>
    /// 同步更新数据操作类
    /// </summary>
    public class SyncDataDAL
    {
        /// <summary>
        /// 同步数据到 Financial_Pay
        /// </summary>
        /// <param name="syncDataCount">更新行数</param>
        ///<param name="processName">要调用的PS名称</param>
        public static void SyncDataToFinancePayProcess(int syncDataCount,string processName)
        {
            DataCommand command = DataCommandManager.GetDataCommand(processName);
            command.SetParameterValue("@SyncDataCount", syncDataCount);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);
            command.ExecuteNonQuery();
        }
    }
}
