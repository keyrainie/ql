using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class NetPayQueryResultVM : ModelBase
    {
        public NetPayQueryResultVM()
        {
            ResultList = new List<NetPayVM>();
        }

        private List<NetPayVM> m_ResultList;
        public List<NetPayVM> ResultList
        {
            get
            {
                return m_ResultList.DefaultIfNull();
            }
            set
            {
                base.SetValue("ResultList", ref m_ResultList, value);
            }
        }

        /// <summary>
        /// 查询结果总记录数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }
    }

    public class NetPayVM : ModelBase
    {
        private int? m_SysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get
            {
                return m_SysNo;
            }
            set
            {
                base.SetValue("SysNo", ref m_SysNo, value);
            }
        }

        private bool m_IsChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                base.SetValue("IsChecked", ref m_IsChecked, value);
            }
        }

        private NetPayStatus? m_Status;
        /// <summary>
        /// 状态
        /// </summary>
        public NetPayStatus? Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                base.SetValue("Status", ref m_Status, value);
            }
        }

        private NetPaySource? m_Source;
        /// <summary>
        /// 来源
        /// </summary>
        public NetPaySource? Source
        {
            get
            {
                return m_Source;
            }
            set
            {
                base.SetValue("Source", ref m_Source, value);
            }
        }

        private int? m_SOSysNo;
        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo
        {
            get
            {
                return m_SOSysNo;
            }
            set
            {
                base.SetValue("SOSysNo", ref m_SOSysNo, value);
            }
        }

        private SOStatus? m_SOStatus;
        /// <summary>
        /// 订单状态
        /// </summary>
        public SOStatus? SOStatus
        {
            get
            {
                return m_SOStatus;
            }
            set
            {
                base.SetValue("SOStatus", ref m_SOStatus, value);
            }
        }

        /// <summary>
        /// 团购状态
        /// </summary>
        private GroupBuyingSettlementStatus? m_SettlementStatus;
        public GroupBuyingSettlementStatus? SettlementStatus
        {
            get
            {
                return m_SettlementStatus;
            }
            set
            {
                base.SetValue("SettlementStatus", ref m_SettlementStatus, value);
            }
        }

        private string m_PayType;
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType
        {
            get
            {
                return m_PayType;
            }
            set
            {
                base.SetValue("PayType", ref m_PayType, value);
            }
        }

        private decimal? m_SOCashAmount;
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal? SOCashAmount
        {
            get
            {
                return m_SOCashAmount;
            }
            set
            {
                base.SetValue("SOCashAmount", ref m_SOCashAmount, value);
            }
        }

        private decimal? m_PrepayAmt;
        /// <summary>
        /// 预付款
        /// </summary>
        public decimal? PrepayAmt
        {
            get
            {
                return m_PrepayAmt;
            }
            set
            {
                base.SetValue("PrepayAmt", ref m_PrepayAmt, value);
            }
        }

        private decimal? m_GiftCardPayAmt;
        /// <summary>
        /// 礼品卡金额
        /// </summary>
        public decimal? GiftCardPayAmt
        {
            get
            {
                return m_GiftCardPayAmt;
            }
            set
            {
                base.SetValue("GiftCardPayAmt", ref m_GiftCardPayAmt, value);
            }
        }

        private decimal? m_PayAmount;
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? PayAmount
        {
            get
            {
                return m_PayAmount;
            }
            set
            {
                base.SetValue("PayAmount", ref m_PayAmount, value);
            }
        }

        private string m_ShipType;
        /// <summary>
        /// 配送方式
        /// </summary>
        public string ShipType
        {
            get
            {
                return m_ShipType;
            }
            set
            {
                base.SetValue("ShipType", ref m_ShipType, value);
            }
        }

        private string m_CreateUser;
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser
        {
            get
            {
                return m_CreateUser;
            }
            set
            {
                base.SetValue("CreateUser", ref m_CreateUser, value);
            }
        }

        private DateTime? m_CreateTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get
            {
                return m_CreateTime;
            }
            set
            {
                base.SetValue("CreateTime", ref m_CreateTime, value);
            }
        }

        private string m_ReviewUser;
        /// <summary>
        /// 复查人
        /// </summary>
        public string ReviewUser
        {
            get
            {
                return m_ReviewUser;
            }
            set
            {
                base.SetValue("ReviewUser", ref m_ReviewUser, value);
            }
        }

        private DateTime? m_ReviewedTime;
        /// <summary>
        /// 复查时间
        /// </summary>
        public DateTime? ReviewedTime
        {
            get
            {
                return m_ReviewedTime;
            }
            set
            {
                base.SetValue("ReviewedTime", ref m_ReviewedTime, value);
            }
        }
    }
}