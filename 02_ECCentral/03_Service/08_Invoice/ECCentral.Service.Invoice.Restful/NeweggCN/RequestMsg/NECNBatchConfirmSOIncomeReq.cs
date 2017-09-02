using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    /// <summary>
    /// 中蛋批量确认收款单RequestMessage
    /// </summary>
    public class NECNBatchConfirmSOIncomeReq
    {
        /// <summary>
        /// 收款单系统编号列表
        /// </summary>
        public List<int> SysNoList
        {
            get;
            set;
        }

        public bool? HasRight
        {
            get;
            set;
        }
    }
}