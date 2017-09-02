using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class UpdatePostIncomeReq
    {
        /// <summary>
        /// 电汇收款单数据
        /// </summary>
        public PostIncomeInfo PostIncome
        {
            get;
            set;
        }

        /// <summary>
        /// CS确认的订单号，多个订单号之间用逗号隔开
        /// </summary>
        public string ConfirmedSOSysNo
        {
            get;
            set;
        }
    }
}