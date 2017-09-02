using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace ECCentral.Service.SO.BizProcessor
{
    public partial class SOProcessor
    {
        /// <summary>
        /// 取得待审核的联通结算订单编号
        /// </summary>
        /// <returns></returns>
        public List<int> GetStatusIsOriginalBuyMobileSettlementSOSysNo()
        {
            return SODA.GetStatusIsOriginalBuyMobileSettlementSOSysNo();
        }


        /// <summary>
        /// 取得状态为已完成的联通合约机订单编号
        /// </summary>
        /// <returns></returns>
        public List<int> GetStatusIsCompleteUnicomFreeBuySOSysNo()
        {
            return SODA.GetStatusIsCompleteUnicomFreeBuySOSysNo();
        }
    }
}
