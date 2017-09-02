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
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CostChangeBasicInfoVM : ModelBase
    {
        public CostChangeBasicInfoVM()
        {
        }

        /// <summary>
        /// 变价原因
        /// </summary>
        private string memo;
        [Validate(ValidateType.Required)]
        public string Memo
        {
            get { return memo; }
            set { this.SetValue("Memo", ref memo, value); }
        }

        /// <summary>
        /// 审核/退回/作废原因
        /// </summary>
        private string auditMemo;

        public string AuditMemo
        {
            get { return auditMemo; }
            set { this.SetValue("AuditMemo", ref auditMemo, value); }
        }

        /// <summary>
        /// 供应商编号
        /// </summary>
        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { this.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }

        /// <summary>
        /// PM编号
        /// </summary>
        private int? pmSysNo;

        public int? PMSysNo
        {
            get { return pmSysNo; }
            set { this.SetValue("PMSysNo", ref pmSysNo, value); }
        }

        /// <summary>
        /// 变价总金额
        /// </summary>
        private decimal? totalDiffAmt;

        public decimal? TotalDiffAmt
        {
            get { return totalDiffAmt; }
            set { this.SetValue("TotalDiffAmt", ref totalDiffAmt, value); }
        }

        /// <summary>
        /// 单据状态
        /// </summary>
        private CostChangeStatus? status;

        public CostChangeStatus? Status
        {
            get { return status; }
            set { this.SetValue("Status", ref status, value); }
        }
    }
}
