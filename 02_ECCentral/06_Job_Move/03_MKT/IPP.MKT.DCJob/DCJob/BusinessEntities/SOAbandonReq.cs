using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPP.ECommerceMgmt.ServiceJob.BusinessEntities
{
    public class SOAbandonReq
    {
        public List<int> SOSysNoList { get; set; }
        /// <summary>
        /// 是否立即返还库存
        /// </summary>
        public bool ImmediatelyReturnInventory { get; set; }
        /// <summary>
        /// 是否先生成负收款单，再作废订单
        /// </summary>
        public bool IsCreateAO { get; set; }
        /// <summary>
        /// 负收款单信息
        /// </summary>
        public ECCentral.BizEntity.Invoice.SOIncomeRefundInfo RefundInfo { get; set; }
    }
}
