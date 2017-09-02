using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class FinanceQueryVM : ModelBase
    {
        public List<KeyValuePair<FinanceStatus?, string>> StatusList { get; set; }

        public FinanceQueryVM()
        {
            this.StatusList = new List<KeyValuePair<FinanceStatus?, string>>() 
            {
                   new KeyValuePair<FinanceStatus?, string>(null,"--所有--"),
                new KeyValuePair<FinanceStatus?, string>(FinanceStatus.Paid,"已付款"),
                new KeyValuePair<FinanceStatus?, string>(FinanceStatus.Settled,"已结算"),
                new KeyValuePair<FinanceStatus?, string>(FinanceStatus.Unsettled,"未结算"),

            };
                
        }

        private string customerID;
        public string CustomerID
        {
            get
            {
                return customerID;
            }
            set
            {
                base.SetValue("CustomerID", ref customerID, value);
            }
        }

        private FinanceStatus? status;
        public FinanceStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        public string SysNoList { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        private DateTime? settleDateFrom;
        public DateTime? SettleDateFrom 
        {
            get { return settleDateFrom; }
            set { SetValue("SettleDateFrom", ref settleDateFrom, value); }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        private DateTime? settleDateTo;
        public DateTime? SettleDateTo 
        {
            get { return settleDateTo; }
            set { SetValue("SettleDateTo", ref settleDateTo, value); }
        }
    }
}
