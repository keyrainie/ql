using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class RefundPrepayCardInfo : IIdentity, ICompany
    {
        /// <summary>
        /// 神州运通退款申请单号
        /// </summary>
        public int? SOIncomeBankInfoSysNo { get; set; }

        public int? UserSysNo { get; set; }

        public string CompanyCode
        {
            get;
            set;
        }

        public int? SysNo
        {
            get;
            set;
        }
    }
}
