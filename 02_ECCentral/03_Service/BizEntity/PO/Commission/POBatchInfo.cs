using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO.Commission
{
    public class POBatchInfo
    {
        //采购单号
        public int? POSysNo { get; set; }

        //批次号
        public int? BatchNumber { get; set; }

        //批次入库金额
        public decimal? BatchInStockAmt { get; set; }

        //操作人
        public int OperationUserSysNo { get; set; }

        //po状态 部分入库=6 或 已入库=4
        public int POStatus { get; set; }

    }
}
