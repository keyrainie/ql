using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    public class InvoiceInputQueryVM : ModelBase
    {
        public InvoiceInputQueryVM()
        {
            this.WebChannelList = new List<UIWebChannel>(CPApplication.Current.CurrentWebChannelList);
            this.WebChannelList.Insert(0, new UIWebChannel
            {
                ChannelName = ResCommonEnum.Enum_All,
                ChannelType = UIWebChannelType.Publicity
            });
            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.InvoiceInputStatusList = EnumConverter.GetKeyValuePairs<APInvoiceMasterStatus>(EnumConverter.EnumAppendItemType.All);
           
        }

        private int? vendorSysNo;
        public int? VendorSysNo
        {
            get
            {
                return vendorSysNo;
            }
            set
            {
                base.SetValue("VendorSysNo", ref vendorSysNo, value);
            }
        }
        private string vendorName;
        public string VendorName
        {
            get
            {
                return vendorName;
            }
            set
            {
                base.SetValue("VendorName", ref vendorName, value);
            }
        }
        private DateTime? createDateFrom;
        public DateTime? CreateDateFrom
        {
            get
            {
                return createDateFrom;
            }
            set
            {
                base.SetValue("CreateDateFrom", ref createDateFrom, value);
            }
        }

        private DateTime? createDateTo;
        public DateTime? CreateDateTo
        {
            get
            {
                return createDateTo;
            }
            set
            {
                base.SetValue("CreateDateTo", ref createDateTo, value);
            }
        }
        private DateTime? confirmDateFrom;
        public DateTime? ConfirmDateFrom
        {
            get
            {
                return confirmDateFrom;
            }
            set
            {
                base.SetValue("ConfirmDateFrom", ref confirmDateFrom, value);
            }
        }
        private DateTime? confirmDateTo;
        public DateTime? ConfirmDateTo
        {
            get
            {
                return confirmDateTo;
            }
            set
            {
                base.SetValue("ConfirmDateTo", ref confirmDateTo, value);
            }
        }
        private ECCentral.BizEntity.PO.PaySettleCompany? paySettleCompany;
        public ECCentral.BizEntity.PO.PaySettleCompany? PaySettleCompany
        {
            get
            {
                return paySettleCompany;
            }
            set
            {
                SetValue("PaySettleCompany", ref paySettleCompany, value);
            }
        }
        private string poList;
        [Validate(ValidateType.Regex, @"^\s*[1-9][0-9]{0,9}(\s*\.\s*[1-9][0-9]{0,9}\s*)*$", ErrorMessageResourceName = "Msg_ValidateNoList", ErrorMessageResourceType = typeof(ResInvoiceInputQuery))]
        public string POList
        {
            get
            {
                return poList;
            }
            set
            {
                base.SetValue("POList", ref poList, value);
            }
        }
        private string apList;
        [Validate(ValidateType.Regex, @"^\s*[1-9][0-9]{0,9}(\s*\.\s*[1-9][0-9]{0,9}\s*)*$", ErrorMessageResourceName = "Msg_ValidateNoList", ErrorMessageResourceType = typeof(ResInvoiceInputQuery))]
        public string APList
        {
            get
            {
                return apList;
            }
            set
            {
                base.SetValue("APList", ref apList, value);
            }
        }
        private APInvoiceMasterStatus? status;
        public APInvoiceMasterStatus? Status
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
        private bool? hasDiff;
        public bool? HasDiff
        {
            get
            {
                return hasDiff;
            }
            set
            {
                base.SetValue("HasDiff", ref hasDiff, value);
            }
        }
        private string docNo;
        [Validate(ValidateType.Interger)]
        public string DocNo
        {
            get
            {
                return docNo;
            }
            set
            {
                base.SetValue("DocNo", ref docNo, value);
            }
        }
        private int? comeFrom;
        public int? ComeFrom
        {
            get
            {
                return comeFrom;
            }
            set
            {
                base.SetValue("ComeFrom", ref comeFrom, value);
            }
        }

        private string companyCode;
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                SetValue("CompanyCode", ref companyCode, value);
            }
        }

        private string channelID;
        public string ChannelID
        {
            get
            {
                return channelID;
            }
            set
            {
                SetValue("ChannelID", ref channelID, value);
            }
        }

        public List<UIWebChannel> WebChannelList
        {
            get;
            set;
        }
        public List<KeyValuePair<Boolean?, string>> YNList
        {
            get;
            set;
        }

        public List<KeyValuePair<APInvoiceMasterStatus?, string>> InvoiceInputStatusList
        {
            get;
            set;
        }

        public List<KeyValuePair<ECCentral.BizEntity.PO.PaySettleCompany?, string>> PaySettleCompanyList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.PO.PaySettleCompany>(EnumConverter.EnumAppendItemType.All);
            }
        }
    }
}