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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.PO.Models
{
    public class SettleQueryVM : ModelBase
    {
        public SettleQueryVM()
        {

        }

        private string settleSysNo;

        [Validate(ValidateType.Interger)]
        public string SettleSysNo
        {
            get { return settleSysNo; }
            set
            {
                base.SetValue("SettleSysNo", ref settleSysNo, value);
            }
        }

        private string vendorName;

        public string VendorName
        {
            get { return vendorName; }
            set
            {
                base.SetValue("VendorName", ref vendorName, value);
            }
        }

        private string vendorSysNo;

        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set
            {
                base.SetValue("VendorSysNo", ref vendorSysNo, value);
            }
        }


        private POSettleStatus? status;

        public POSettleStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }

        private DateTime? createTime;

        public DateTime? CreateTime
        {
            get
            {
                return createTime;
            }
            set
            {
                base.SetValue("CreateTime", ref createTime, value);
            }
        }

        private DateTime? auditTime;

        public DateTime? AuditTime
        {
            get
            {
                return auditTime;
            }
            set
            {
                base.SetValue("AuditTime", ref auditTime, value);
            }
        }
    }
}
