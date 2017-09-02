using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.SO.IDataAccess;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOLogDA))]
    public class SOLogDA : ISOLogDA
    {
        public void InsertSOLog(SOLogInfo logInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOLog");
            command.SetParameterValue<SOLogInfo>(logInfo, true, false);
            command.ExecuteNonQuery();
        }

        public void InsertSOChangeLogIfNotExist(SOChangeLog logInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Log_Insert_ChangeOrderLog");
            command.SetParameterValue<SOChangeLog>(logInfo, true, false);
            command.ExecuteNonQuery();
        }

        public void InsertSOInvoiceChangeLogInfo(SOInvoiceChangeLogInfo logInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_SOInvoiceChangeLog");
            command.SetParameterValue<SOInvoiceChangeLogInfo>(logInfo, true, false);
            command.SetParameterValue("@ChangeType", EnumHelper.GetDescription(logInfo.ChangeType));
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 添加商家订单出库日志
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="inUserSysNo"></param>
        /// <param name="metShipViaCode"></param>
        /// <param name="metPackpageNumber"></param>
        public void InsertMerchantShippingLog(int soSysNo, int inUserSysNo, string metShipViaCode, string metPackpageNumber)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_MerchantShippingLog");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@METShipViaCode", metShipViaCode);
            command.SetParameterValue("@METPackageNumber", metPackpageNumber);
            command.SetParameterValue("@InUser", inUserSysNo);
            command.ExecuteNonQuery();
        }

        public List<SOLogInfo> GetSOLogBySOSysNoAndLogType(int soSysNo, ECCentral.BizEntity.Common.BizLogType logType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_SOLogBySOSysNoAndLogType");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@OperationType", logType);
            return command.ExecuteEntityList<SOLogInfo>();
        }

        /// <summary>
        /// 根据日志编号更新日志Note
        /// </summary>
        /// <param name="logInfo"></param>
        public void UpdateSOLogNoteBySysNo(SOLogInfo logInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_SOLogNoteBySysNo");
            command.SetParameterValue<SOLogInfo>(logInfo, true, false);
            command.ExecuteNonQuery();
        }
    }
}