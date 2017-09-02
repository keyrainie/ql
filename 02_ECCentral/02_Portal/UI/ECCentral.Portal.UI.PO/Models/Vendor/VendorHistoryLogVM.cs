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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class VendorHistoryLogVM : ModelBase
    {
        private int? vendorSysNo;

        public int? VendorSysNo
        {
            get { return vendorSysNo; }
            set { base.SetValue("VendorSysNo", ref vendorSysNo, value); }
        }
        /// <summary>
        /// 日期
        /// </summary>
        private DateTime? historyDate;
        [Validate(ValidateType.Required)]
        public DateTime? HistoryDate
        {
            get { return historyDate; }
            set { base.SetValue("HistoryDate", ref historyDate, value); }
        }

        public string HistoryDateString
        {
            get
            {
                if (!string.IsNullOrEmpty(auditUserName) && auditTime.HasValue)
                {
                    return historyDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + " -> " + auditTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    return historyDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }

        /// <summary>
        /// 原因
        /// </summary>
        private string historyReason;
        [Validate(ValidateType.Required)]
        public string HistoryReason
        {
            get { return historyReason; }
            set { base.SetValue("HistoryReason", ref historyReason, value); }
        }

        private VendorModifyRequestType? requestType;

        public VendorModifyRequestType? RequestType
        {
            get { return requestType; }
            set { base.SetValue("RequestType", ref requestType, value); }
        }

        private VendorModifyActionType? actionType;

        public VendorModifyActionType? ActionType
        {
            get { return actionType; }
            set { base.SetValue("ActionType", ref actionType, value); }
        }

        public string ActionDisplayString
        {
            get
            {
                return (EnumConverter.GetDescription(actionType, typeof(VendorModifyActionType)) + EnumConverter.GetDescription(requestType, typeof(VendorModifyRequestType)));

            }
        }

        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { base.SetValue("DisplayName", ref displayName, value); }
        }

        /// <summary>
        /// 内容
        /// </summary>
        private string content;

        public string Content
        {
            get { return content; }
            set { base.SetValue("Content", ref content, value); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string memo;

        public string Memo
        {
            get { return memo; }
            set { base.SetValue("Memo", ref memo, value); }
        }


        private VendorModifyRequestStatus? status;

        public VendorModifyRequestStatus? Status
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private DateTime? auditTime;

        public DateTime? AuditTime
        {
            get { return auditTime; }
            set { base.SetValue("AuditTime", ref auditTime, value); }
        }

        private string auditUserName;

        public string AuditUserName
        {
            get { return auditUserName; }
            set { base.SetValue("AuditUserName", ref auditUserName, value); }
        }

        private string companyCode;

        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }
    }
}
