using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// EIMS返点信息
    /// </summary>
    public class EIMSInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? EIMSSysNo { get; set; }

        /// <summary>
        /// 返点名称
        /// </summary>
        public string EIMSName { get; set; }

        /// <summary>
        /// 返点编号
        /// </summary>
        public int? EIMSNo { get; set; }

        /// <summary>
        /// 管理采购单编号
        /// </summary>
        public int? PurchaseOrderSysNo { get; set; }

        /// <summary>
        /// 返点使用金额
        /// </summary>
        public decimal? EIMSAmt { get; set; }

        /// <summary>
        /// 下单时返点使用情况（已使用金额）
        /// </summary>
        public decimal? AlreadyUseAmt { get; set; }

        /// <summary>
        /// 下单时返点剩余金额
        /// </summary>
        public decimal? EIMSLeftAmt { get; set; }

        /// <summary>
        /// 返点剩余金额（每次入库修改）
        /// </summary>
        public decimal? LeftAmt { get; set; }

        /// <summary>
        /// 返点总金额
        /// </summary>
        public decimal? EIMSTotalAmt { get; set; }


        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal? ReceivedAmount { get; set; }

        /// <summary>
        /// 关联金额
        /// </summary>
        public decimal? RelateAmount { get; set; }

    }
}
