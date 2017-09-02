using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Enum.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RefundQueryReqVM : ModelBase
    {
        public RefundQueryReqVM()
        {
            this.Stocks = new List<StockInfo>();
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //修改UIWebChannelType.publicChennel 后放开
            //this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All, ChannelType = UIWebChannelType.publicChennel });

            this.YNList = BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            this.RefundStatusList = EnumConverter.GetKeyValuePairs<RMARefundStatus>(EnumConverter.EnumAppendItemType.All);
            this.AuditStatusList = EnumConverter.GetKeyValuePairs<RefundStatus>(EnumConverter.EnumAppendItemType.All);
            //this.AuditStatusList.RemoveAll(p => p.Key == RefundStatus.WaitingFinAudit || p.Key == RefundStatus.WaitingRefund);
        }

        private string sysNo;
        [Validate(ValidateType.Interger)]
        public string SysNo
        {
            get
            {
                return sysNo;
            }
            set
            {
                SetValue("SysNo", ref sysNo, value);
            }
        }

        private string refundID;
        public string RefundID
        {
            get
            {
                return refundID;
            }
            set
            {
                SetValue("RefundID", ref refundID, value);
            }
        }

        private string soSysNo;
        [Validate(ValidateType.Interger)]
        public string SOSysNo
        {
            get
            {
                return soSysNo;
            }
            set
            {
                SetValue("SOSysNo", ref soSysNo, value);
            }
        }

        private string soSysNoList;
        public string SOSysNoList
        {
            get
            {
                return soSysNoList;
            }
            set
            {
                string result = string.Empty;
                bool flag = GetSOSysNoString(value, out result);
                if (flag)
                {
                    this.SOSysNoString = result;
                    SetValue("SOSysNoList", ref soSysNoList, value);
                }
            }
        }

        private string soSysNoString;
        public string SOSysNoString
        {
            get
            {
                return soSysNoString;
            }
            set
            {
                SetValue("SOSysNoString", ref soSysNoString, value);
            }
        }

        private string customerSysNo;
        [Validate(ValidateType.Interger)]
        public string CustomerSysNo
        {
            get
            {
                return customerSysNo;
            }
            set
            {
                SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }

        private RMARefundStatus? status;
        public RMARefundStatus? Status
        {
            get
            {
                return status;
            }
            set
            {
                SetValue("Status", ref status, value);
            }
        }

        private int? productSysNo;
        [Validate(ValidateType.Interger)]
        public int? ProductSysNo
        {
            get
            {
                return productSysNo;
            }
            set
            {
                SetValue("ProductSysNo", ref productSysNo, value);
            }
        }

        private DateTime? createTimeFrom;
        public DateTime? CreateTimeFrom
        {
            get
            {
                return createTimeFrom;
            }
            set
            {
                SetValue("CreateTimeFrom", ref createTimeFrom, value);
            }
        }

        private DateTime? createTimeTo;
        public DateTime? CreateTimeTo
        {
            get
            {
                return createTimeTo;
            }
            set
            {
                SetValue("CreateTimeTo", ref createTimeTo, value);
            }
        }

        private DateTime? refundTimeFrom;
        public DateTime? RefundTimeFrom
        {
            get
            {
                return refundTimeFrom;
            }
            set
            {
                SetValue("RefundTimeFrom", ref refundTimeFrom, value);
            }
        }

        private DateTime? refundTimeTo;
        public DateTime? RefundTimeTo
        {
            get
            {
                return refundTimeTo;
            }
            set
            {
                SetValue("RefundTimeTo", ref refundTimeTo, value);
            }
        }

        private bool? isVIP;
        public bool? IsVIP
        {
            get
            {
                return isVIP;
            }
            set
            {
                SetValue("IsVIP", ref isVIP, value);
            }
        }

        private string warehouseCreated;
        public string WarehouseCreated
        {
            get
            {
                return warehouseCreated;
            }
            set
            {
                SetValue("WarehouseCreated", ref warehouseCreated, value);
            }
        }

        private string invoiceLocation;
        public string InvoiceLocation
        {
            get
            {
                return invoiceLocation;
            }
            set
            {
                SetValue("InvoiceLocation", ref invoiceLocation, value);
            }
        }

        private string warehouseShipped;
        public string WarehouseShipped
        {
            get
            {
                return warehouseShipped;
            }
            set
            {
                SetValue("WarehouseShipped", ref warehouseShipped, value);
            }
        }

        private RefundStatus? auditStatus;
        public RefundStatus? AuditStatus
        {
            get
            {
                return auditStatus;
            }
            set
            {
                SetValue("AuditStatus", ref auditStatus, value);
            }
        }

        private string webChannelID;
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string WebChannelID
        {
            get
            {
                return webChannelID;
            }
            set
            {
                SetValue("WebChannelID", ref webChannelID, value);
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

        #region 扩展信息
        public List<UIWebChannel> WebChannelList { get; set; }
        public List<StockInfo> Stocks { get; set; }
        public List<KeyValuePair<Boolean?, string>> YNList { get; set; }
        public List<KeyValuePair<Nullable<RMARefundStatus>, string>> RefundStatusList { get; set; }
        public List<KeyValuePair<Nullable<RefundStatus>, string>> AuditStatusList { get; set; }
        #endregion

        private bool GetSOSysNoString(string input, out string result)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var error = this.ValidationErrors.Where(p => p.MemberNames.Contains("SOSysNoList")).ToList();
                error.ForEach(p =>
                {
                    this.ValidationErrors.Remove(p);
                });

                IEnumerable<string> soSysNoList = input.Trim().Split('.', ' ', ',').Distinct();

                StringBuilder SoSysNoSB = new StringBuilder();
                List<string> members = new List<string>();
                members.Add("SOSysNoList");

                if (soSysNoList != null && soSysNoList.Count() > 0)
                {
                    int soSysNo;
                    foreach (string item in soSysNoList)
                    {
                        if (string.IsNullOrEmpty(item.Trim()))
                        {
                            continue;
                        }
                        if (Int32.TryParse(item.Trim(), out soSysNo))
                        {
                            SoSysNoSB.Append(item.Trim() + ",");
                        }
                        else
                        {
                            this.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(string.Format(ResRefundQuery.Msg_SoSysNoConvertInt, item), members));
                            break;
                        }
                    }
                }
                if (SoSysNoSB.Length > 0)
                {
                    SoSysNoSB.Length = SoSysNoSB.Length - 1;
                }
                string soSysNoStr = SoSysNoSB.ToString();
                if (input.Length > 7999)
                {
                    this.ValidationErrors.Add(new System.ComponentModel.DataAnnotations.ValidationResult(ResRefundQuery.Msg_SoSysNoMaxLength, members));
                }
                result = soSysNoStr;
                return !(this.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains("SOSysNoList")) != null);
            }
            result = string.Empty;
            return true;
        }
    }
}
