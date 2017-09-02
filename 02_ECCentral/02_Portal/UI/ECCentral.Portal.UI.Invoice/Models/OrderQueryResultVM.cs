using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class OrderQueryResultVM : ModelBase
    {
        private List<OrderVM> m_ResultList;
        public List<OrderVM> ResultList
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

    public class OrderVM : ModelBase
    {
        private bool m_IsChecked;
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

        /// <summary>
        /// 单据编号
        /// </summary>
        public int? OrderSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public SOIncomeOrderType? OrderType
        {
            get;
            set;
        }

        /// <summary>
        /// 单据金额
        /// </summary>
        public decimal? OrderAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 单据状态
        /// </summary>
        public SOIncomeStatus? OrderStatus
        {
            get;
            set;
        }
    }
}