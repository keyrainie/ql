using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购单返点信息
    /// </summary>
    public class PurchaseOrderEIMSInfo
    {
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContractNumber { get; set; }

        /// <summary>
        ///  返点信息列表
        /// </summary>
        public List<EIMSInfo> EIMSInfoList { get; set; }
    }
}
