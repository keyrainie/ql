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
using ECCentral.BizEntity.Inventory;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class VirtualRequestVM : ModelBase
    {
        private int? sysNo;

        public int? SysNo
        {
            get { return sysNo; }
            set { base.SetValue("SysNo", ref sysNo, value); }
        }
        private int? productSysNo;

        public int? ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }
        private string productID;

        public string ProductID
        {
            get { return productID; }
            set { base.SetValue("ProductID", ref productID, value); }
        }
        private string productName;

        public string ProductName
        {
            get { return productName; }
            set { base.SetValue("ProductName", ref productName, value); }
        }
        private int? stockSysNo;

        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { base.SetValue("StockSysNo", ref stockSysNo, value); }
        }
        private string stockName;

        public string StockName
        {
            get { return stockName; }
            set { base.SetValue("StockName", ref stockName, value); }
        }

        private string virtualQuantity;
        [Validate(ValidateType.Interger)]
        public string VirtualQuantity
        {
            get { return virtualQuantity; }
            set { base.SetValue("VirtualQuantity", ref virtualQuantity, value); }
        }
        private DateTime? createDate;

        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }
        private string createUserName;

        public string CreateUserName
        {
            get { return createUserName; }
            set { base.SetValue("CreateUserName", ref createUserName, value); }
        }
        private int? activeVirtualQuantity;

        public int? ActiveVirtualQuantity
        {
            get { return activeVirtualQuantity; }
            set { base.SetValue("ActiveVirtualQuantity", ref activeVirtualQuantity, value); }
        }
        private string requestNote;

        public string RequestNote
        {
            get { return requestNote; }
            set { base.SetValue("RequestNote", ref requestNote, value); }
        }
        private string auditNote;

        public string AuditNote
        {
            get { return auditNote; }
            set { base.SetValue("AuditNote", ref auditNote, value); }
        }
        private DateTime? auditDate;

        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { base.SetValue("AuditDate", ref auditDate, value); }
        }
        private VirtualRequestStatus? requestStatus;

        public VirtualRequestStatus? RequestStatus
        {
            get { return requestStatus; }
            set { base.SetValue("RequestStatus", ref requestStatus, value); }
        }
        private int? virtualType = 0;

        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public int? VirtualType
        {
            get { return virtualType; }
            set { base.SetValue("VirtualType", ref virtualType, value); }
        }
        private string virtualTypeString;
        public string VirtualTypeString
        {
            get { return virtualTypeString; }
            set { base.SetValue("VirtualTypeString", ref virtualTypeString, value); }
        }
        private DateTime? startDate;
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        public DateTime? StartDate
        {
            get { return startDate; }
            set { base.SetValue("StartDate", ref startDate, value); }
        }
        private DateTime? endDate;

        public DateTime? EndDate
        {
            get { return endDate; }
            set { base.SetValue("EndDate", ref endDate, value); }
        }


    }
}
