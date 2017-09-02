using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.Biz
{
    /// <summary>
    /// 业务外观管理类
    /// </summary>
    public class BizFacadeManager
    {
        static BizFacadeManager() 
        {
            BizFacadeManager.SyncCommissionSettlement = new SyncCommissionSettlementManager();
        }
        /// <summary>
        /// 业务外观
        /// </summary>
        public static SyncCommissionSettlementManager SyncCommissionSettlement { get; set; }
    }
}
