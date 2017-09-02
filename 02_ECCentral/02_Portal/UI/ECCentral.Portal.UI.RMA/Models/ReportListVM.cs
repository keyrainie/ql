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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.RMA;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class OutBoundNotReturnListVM : ModelBase
    {
        #region 查询结果
        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }
        public int? OutboundSysNo { get; set; }
        public DateTime? OutTime { get; set; }
        public int? IsSendMail { get; set; }
        public int? PMUserSysNo { get; set; }
        public string PMName { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int? ProductSysNo { get; set; }
        public decimal? ProductCost { get; set; }
        public int? RegisterSysNo { get; set; }

        public bool? IsWithin7Days { get; set; }
        public string ResponseDesc { get; set; }

        public string Memo { get; set; }

        public RMARefundStatus? RefundStatus { get; set; }

        public RMARevertStatus? RevertStatus { get; set; }

        public int? SOSysNo { get; set; }

        public DateTime? SODate { get; set; }

        public string Warranty { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? ValidDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public decimal? ContractAmt { get; set; }

        public decimal? TotalPOMoney { get; set; }

        public string PayPeriodType { get; set; }

        public VendorStatus? Vendor_Status { get; set; }

        public string Category3Name { get; set; }

        public int? WarrantyDays { get; set; }

        #endregion

        public string SendMailCount
        {
            get
            {
                return IsSendMail.HasValue ? string.Format(ResRMAReports.OutBound_SendMailCount, IsSendMail.Value.ToString()) : ResRMAReports.OutBound_NotSendMail;
            }
        }

        public bool? IsContact { get; set; }
       

    }

}
