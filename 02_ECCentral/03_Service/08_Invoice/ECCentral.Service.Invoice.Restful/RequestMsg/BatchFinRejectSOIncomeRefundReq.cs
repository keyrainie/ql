using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    /// <summary>
    /// 批量财务审核拒绝请求
    /// </summary>
    public class BatchFinRejectSOIncomeRefundReq
    {
        /// <summary>
        /// 退款单系统编号
        /// </summary>
        public List<int> SysNoList
        {
            get;
            set;
        }

        /// <summary>
        /// 财务附加备注
        /// </summary>
        public string FinAppendNote
        {
            get;
            set;
        }
    }
}