using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.SO.Restful.RequestMsg
{
    public class SOCreateGiftReq
    {
        public SOInfo SOInfo { get; set; }
        public int MasterSOSysNo { get; set; }
    }

    public class SOAuditReq
    {
        /// <summary>
        /// 要审核的订单列表
        /// </summary>
        public List<int> SOSysNoList { get; set; }

        /// <summary>
        /// 是否强制审核
        /// </summary>
        public bool IsForce { get; set; }
        /// <summary>
        /// 是否是主管审核
        /// </summary>
        public bool IsManagerAudit { get; set; }
        /// <summary>
        /// 是否同时审核网上支付
        /// </summary>
        public bool IsAuditNetPay { get; set; }

        public SOAuditReq()
        {
            IsForce = false;
            IsManagerAudit = false;
            IsAuditNetPay = false;
        }
    }

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

    public class SOHoldReq
    {
        public int SOSysNo { get; set; }

        /// <summary>
        /// 锁单/解锁原因，备注
        /// </summary>
        public string Note { get; set; }
    }

    public class SOSpliteInvoiceReq
    {
        public int SOSysNo { get; set; }

        public List<ECCentral.BizEntity.Invoice.SubInvoiceInfo> InvoiceItems
        {
            get;
            set;
        }
    }

}
