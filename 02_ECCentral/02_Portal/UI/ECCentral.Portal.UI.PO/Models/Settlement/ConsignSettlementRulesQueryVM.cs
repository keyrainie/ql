using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.Models
{
    public class ConsignSettlementRulesQueryVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }

        /// <summary>
        /// 规则编码
        /// </summary>
        private string settleRuleCode;

        public string SettleRuleCode
        {
            get { return settleRuleCode; }
            set { base.SetValue("SettleRuleCode", ref settleRuleCode, value); }
        }

        /// <summary>
        /// 规则名称
        /// </summary>
        private string settleRuleName;

        public string SettleRuleName
        {
            get { return settleRuleName; }
            set { base.SetValue("SettleRuleName", ref settleRuleName, value); }
        }

        private DateTime? createDateFrom;

        public DateTime? CreateDateFrom
        {
            get { return createDateFrom; }
            set { base.SetValue("CreateDateFrom", ref createDateFrom, value); }
        }

        private DateTime? createDateTo;

        public DateTime? CreateDateTo
        {
            get { return createDateTo; }
            set { base.SetValue("CreateDateTo", ref createDateTo, value); }
        }

        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }

        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        /// <summary>
        ///  状态
        /// </summary>
        private ConsignSettleRuleStatus? status;

        public ConsignSettleRuleStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }
    }
}
