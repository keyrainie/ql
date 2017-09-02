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
    public class VendorQueryVM : ModelBase
    {

        public VendorQueryVM()
        {
        }
        private string vendorSysNo;

        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.MaxLength, 9)]
        public string VendorSysNo
        {
            get { return vendorSysNo; }
            set
            {
                base.SetValue("VendorSysNo", ref vendorSysNo, value);
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
        private string address;

        public string Address
        {
            get { return address; }
            set
            {
                base.SetValue("Address", ref address, value);
            }
        }
        private string contact;

        public string Contact
        {
            get { return contact; }
            set
            {
                base.SetValue("Contact", ref contact, value);
            }
        }
        private string phone;

        public string Phone
        {
            get { return phone; }
            set
            {
                base.SetValue("Phone", ref phone, value);
            }
        }
        private VendorStatus? status;

        public VendorStatus? Status
        {
            get { return status; }
            set
            {
                base.SetValue("Status", ref status, value);
            }
        }
        private string vendorType;

        public string VendorType
        {
            get { return vendorType; }
            set
            {
                base.SetValue("VendorType", ref vendorType, value);
            }
        }
        private string rANK;

        public string RANK
        {
            get { return rANK; }
            set
            {
                base.SetValue("RANK", ref rANK, value);
            }
        }
        //等级状态
        private VendorRankStatus? rANKStatus;

        public VendorRankStatus? RANKStatus
        {
            get { return rANKStatus; }
            set
            {
                base.SetValue("RANKStatus", ref rANKStatus, value);
            }
        }
        //付款结算公司
        private PaySettleCompany? paySettleCompany;

        public PaySettleCompany? PaySettleCompany
        {
            get { return paySettleCompany; }
            set
            {
                base.SetValue("PaySettleCompany", ref paySettleCompany, value);
            }
        }
        //代理品牌
        private string manufacturerSysNo;

        public string ManufacturerSysNo
        {
            get { return manufacturerSysNo; }
            set
            {
                base.SetValue("ManufacturerSysNo", ref manufacturerSysNo, value);
            }
        }
        private string manufacturerName;

        public string ManufacturerName
        {
            get { return manufacturerName; }
            set
            {
                base.SetValue("ManufacturerName", ref manufacturerName, value);
            }
        }
        //代理级别
        private string agentLevel;

        public string AgentLevel
        {
            get { return agentLevel; }
            set
            {
                base.SetValue("AgentLevel", ref agentLevel, value);
            }
        }
        //财务审核状态
        private string requestStatus;

        public string RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                base.SetValue("RequestStatus", ref requestStatus, value);
            }
        }
        //代理类别
        private string c1SysNo;

        public string C1SysNo
        {
            get
            {
                return c1SysNo;
            }
            set
            {
                base.SetValue("C1SysNo", ref c1SysNo, value);
            }
        }
        private string c2SysNo;

        public string C2SysNo
        {
            get
            {
                return c2SysNo;
            }
            set
            {
                base.SetValue("C2SysNo", ref c2SysNo, value);
            }
        }
        private string c3SysNo;

        public string C3SysNo
        {
            get
            {
                return c3SysNo;
            }
            set
            {
                base.SetValue("C3SysNo", ref c3SysNo, value);
            }
        }
        //合作到期时间从
        private DateTime? expiredDateFrom;

        public DateTime? ExpiredDateFrom
        {
            get
            {
                return expiredDateFrom;
            }
            set
            {
                base.SetValue("ExpiredDateFrom", ref expiredDateFrom, value);
            }
        }
        //合作到期时间到
        private DateTime? expiredDateTo;

        public DateTime? ExpiredDateTo
        {
            get
            {
                return expiredDateTo;
            }
            set
            {
                base.SetValue("ExpiredDateTo", ref expiredDateTo, value);
            }
        }
        /// <summary>
        /// 银行账号
        /// </summary>
        private string account;

        public string Account
        {
            get
            {
                return account;
            }
            set
            {
                base.SetValue("Account", ref account, value);
            }
        }
        /// <summary>
        /// 是否为代销
        /// </summary>
        private VendorConsignFlag? isConsign;

        public VendorConsignFlag? IsConsign
        {
            get
            {
                return isConsign;
            }
            set
            {
                base.SetValue("IsConsign", ref isConsign, value);
            }
        }

        /// <summary>
        /// 开票方式
        /// </summary>
        private string invoiceType;

        public string InvoiceType
        {
            get
            {
                return invoiceType;
            }
            set
            {
                base.SetValue("InvoiceType", ref invoiceType, value);
            }
        }

        /// <summary>
        /// 仓储方式
        /// </summary>
        private string stockType;

        public string StockType
        {
            get
            {
                return stockType;
            }
            set
            {
                base.SetValue("StockType", ref stockType, value);
            }
        }

        /// <summary>
        /// 配送方式
        /// </summary>
        private string shippingType;

        public string ShippingType
        {
            get
            {
                return shippingType;
            }
            set
            {
                base.SetValue("ShippingType", ref shippingType, value);
            }
        }
        /// <summary>
        /// 跨境电子口岸
        /// </summary>
        private int? ePortSysNo;
        public int? EPortSysNo
        {
            get {
                return ePortSysNo;
            }
            set
            {
                base.SetValue("EPortSysNo", ref ePortSysNo, value);
            }
        }

    }
}
