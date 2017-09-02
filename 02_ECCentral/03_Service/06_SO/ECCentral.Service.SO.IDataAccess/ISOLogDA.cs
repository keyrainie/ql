using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOLogDA
    {
        void InsertSOLog(SOLogInfo logInfo);
        /// <summary>
        /// 根据订单编号查询是否存在修改日志，如果还存在则添加
        /// </summary>
        /// <param name="logInfo"></param>
        void InsertSOChangeLogIfNotExist(SOChangeLog logInfo);
        /// <summary>
        /// 添加发票更改日志
        /// </summary>
        /// <param name="logInfo"></param>
        void InsertSOInvoiceChangeLogInfo(SOInvoiceChangeLogInfo logInfo);
        /// <summary>
        /// 添加商家订单出库日志
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="inUserSysNo"></param>
        /// <param name="metShipViaCode"></param>
        /// <param name="metPackpageNumber"></param>
        void InsertMerchantShippingLog(int soSysNo, int inUserSysNo, string metShipViaCode, string metPackpageNumber);

        List<SOLogInfo> GetSOLogBySOSysNoAndLogType(int soSysNo, ECCentral.BizEntity.Common.BizLogType logType);

        /// <summary>
        /// 根据日志编号更新日志Note
        /// </summary>
        /// <param name="logInfo"></param>
        void UpdateSOLogNoteBySysNo(SOLogInfo logInfo);
    }
}
