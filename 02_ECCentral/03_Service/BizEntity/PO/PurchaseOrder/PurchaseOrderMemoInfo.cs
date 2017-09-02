using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 采购单备忘录信息
    /// </summary>
    public class PurchaseOrderMemoInfo
    {
        /// <summary>
        /// PM确认时间
        /// </summary>
        public DateTime? PMConfirmTime { get; set; }

        /// <summary>
        /// PM确认人
        /// </summary>
        public string PMConfirmUserName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 采购单备忘录
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        public string RefuseMemo { get; set; }

        /// <summary>
        /// 入库备注
        /// </summary>
        public string InStockMemo { get; set; }

        /// <summary>
        /// PM申请理由
        /// </summary>
        public string PMRequestMemo { get; set; }

        /// <summary>
        /// TL申请理由
        /// </summary>
        public string TLRequestMemo { get; set; }

    }
}
