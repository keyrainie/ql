using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    /// <summary>
    /// 核对银行电汇-邮局汇款
    /// </summary>
    public class PostPayResp
    {
        public List<PostIncomeConfirmInfo> ConfirmedOrderList
        {
            get;
            set;
        }

        public SOBaseInfo SOBaseInfo
        {
            get;
            set;
        }

        public decimal? RemainAmt
        {
            get;
            set;
        }
        public decimal? IncomeAmt
        {
            get;
            set;
        }
        public decimal? CheckedOrderAmt
        {
            get;
            set;
        }
        public decimal? RefundAmt
        {
            get;
            set;
        }
    }
}