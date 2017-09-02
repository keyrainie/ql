using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage
{
    public class WMSHoldMessage : IEventMessage
    {
        public int SOSysNo { get; set; }
        /// <summary>
        /// 仓库列表
        /// </summary>
        public List<int> WarehouseSysNoList { get; set; }
        /// <summary>
        /// 锁定类型
        /// </summary>
        public WMSActionType ActionType { get; set; }
        /// <summary>
        /// 操作用户编号
        /// </summary>
        public int UserSysNo { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        public string Reason { get; set; }

        public string Subject
        {
            get { return "WMSHoldMessage"; }
        }
    }

    public enum WMSActionType
    {
        /// <summary>
        /// 锁定
        /// </summary>
        Hold = 0,//'H',
        /// <summary>
        /// 取消审核订单时锁定
        /// </summary>
        CancelAuditHold = 1,//'C',
        /// <summary>
        /// 作废订单时锁定
        /// </summary>
        AbandonHold = 2,// 'D',
        /// <summary>
        /// 解锁
        /// </summary>
        UnHold = 3,// 'U',
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = 4,// 'V'
    }

    public class WMSSOActionRequestMessage : IEventMessage
    {
        public int TransactionSysNo { get; set; }

        public int SOSysNo
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public DateTime ActionDate
        {
            get;
            set;
        }

        public int? ActionUser
        {
            get;
            set;
        }

        public string ActionReason
        {
            get;
            set;
        }

        public int StockSysNo
        {
            get;
            set;
        }

        public string StockID
        {
            get;
            set;
        }

        public string Language { get; set; }

        public WMSActionType ActionType { get; set; }

        public string Subject
        {
            get { return "WMSSOActionRequestMessage"; }
        }
    }
}
