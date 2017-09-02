using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class BatchSetSOIncomeReferenceIDReq
    {
        /// <summary>
        /// 收款单系统编号列表
        /// </summary>
        public List<int> SysNoList
        {
            get;
            set;
        }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string ReferenceID
        {
            get;
            set;
        }
    }
}