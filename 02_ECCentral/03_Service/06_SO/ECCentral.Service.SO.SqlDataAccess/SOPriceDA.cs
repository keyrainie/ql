using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.SO.IDataAccess;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOPriceDA))]
    public class SOPriceDA : ISOPriceDA
    {

        /// <summary>
        /// 根据订单编号取得订单拆分的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public List<SOPriceMasterInfo> GetSOPriceBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Get_SOPriceBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<SOPriceMasterInfo>();
        }

        /// <summary>
        /// 根据订单编号删除订单的价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void DeleteSOPriceBySOSysNo(int soSysNo)
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
        public void InsertSOPrice(SOPriceMasterInfo info, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Insert_SOPrice");
            command.SetParameterValue<SOPriceMasterInfo>(info, true, false);
            command.SetParameterValue("@CompanyCode", companyCode);
            info.SysNo = command.ExecuteScalar<int>();
        }
        /// <summary>
        /// 添加订单商品价格信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="companyCode"></param>
        public void InsertSOPriceItem(SOPriceItemInfo info, int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Insert_SOPriceItem");
            command.SetParameterValue<SOPriceItemInfo>(info, true, false);
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 作废订单价格信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void AbandonSOPriceBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOPrice_Update_AbandonSOPrice");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.ExecuteNonQuery();
        }
    }
}
