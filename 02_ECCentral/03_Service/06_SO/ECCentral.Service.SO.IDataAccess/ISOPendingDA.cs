using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.SO.IDataAccess
{
    public interface ISOPendingDA
    {
        void UpdateSOPendingStatus(int soSysNo , SOPendingStatus status);

        /// <summary>
        /// 获取订单出库仓库
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>出库仓库</returns>
        string GetOutStockString(int soSysNo);

        SOPending GetBySysNo(int soSysNo);

    }
}
