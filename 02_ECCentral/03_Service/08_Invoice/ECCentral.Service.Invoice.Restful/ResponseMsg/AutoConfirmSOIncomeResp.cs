using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.ResponseMsg
{
    /// <summary>
    /// 自动确认收款单
    /// </summary>
    public class AutoConfirmSOIncomeResp
    {
        /// <summary>
        /// 成功确认的订单编号列表
        /// </summary>
        public List<int> SuccessSysNoList
        {
            get;
            set;
        }
        /// <summary>
        /// 确认失败的订单系统编号列表，包括匹配失败的记录
        /// </summary>
        public List<int> FailedSysNoList
        {
            get;
            set;
        }
        /// <summary>
        /// 成功提交确认的订单数
        /// </summary>
        public int SubmitConfirmCount
        {
            get;
            set;
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string FailedMessage
        {
            get;
            set;
        }
    }
}