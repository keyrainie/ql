using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Biz
{
    /// <summary>
    /// 业务外观管理类
    /// </summary>
    public class BizFacadeManager
    {
        static BizFacadeManager() 
        {
            BizFacadeManager.AutoSettledCommission = new AutoSettledCommissionManager();
        }
        /// <summary>
        /// 业务外观
        /// </summary>
        public static AutoSettledCommissionManager AutoSettledCommission { get; set; }
    }
}
