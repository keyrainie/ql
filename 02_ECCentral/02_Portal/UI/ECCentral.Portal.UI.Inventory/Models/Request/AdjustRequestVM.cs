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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Inventory;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class AdjustRequestVM : ModelBase
    {
        public AdjustRequestVM()
        {
            AdjustItemInfoList = new List<AdjustRequestItemVM>();
            InvoiceInfo = new AdjustRequestInvoiceVM();
            AdjustProperty = AdjustRequestProperty.CheckStock;
        }
        private int? sysNo;
        public int? SysNo
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

        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        public int? UserSysNo { get; set; }

        private string productLineSysno;
        /// <summary>
        /// 产品线
        /// </summary>
        public string ProductLineSysno
        {
            get { return productLineSysno; }
            set { base.SetValue("ProductLineSysno", ref productLineSysno, value); }
        }

        private string requestID;
        /// <summary>
        ///  单据编号
        /// </summary>
        public string RequestID
        {
            get
            {
                return requestID;
            }
            set
            {
                SetValue("RequestID", ref requestID, value);
            }
        }
        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
        }

        private int? createUserSysNo;
        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreateUserSysNo
        {
            get { return createUserSysNo; }
            set { base.SetValue("CreateUserSysNo", ref createUserSysNo, value); }
        }

        private DateTime? createDate;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate
        {
            get { return createDate; }
            set { base.SetValue("CreateDate", ref createDate, value); }
        }

        private int? editUserSysNo;
        /// <summary>
        /// 更新人
        /// </summary>
        public int? EditUserSysNo
        {
            get { return editUserSysNo; }
            set { base.SetValue("EditUserSysNo", ref editUserSysNo, value); }
        }

        private DateTime? editDate;
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate
        {
            get { return editDate; }
            set { base.SetValue("EditDate", ref editDate, value); }
        }

        private int? auditUserSysNo;
        /// <summary>
        /// 审核人
        /// </summary>
        public int? AuditUserSysNo
        {
            get { return auditUserSysNo; }
            set { base.SetValue("AuditUserSysNo", ref auditUserSysNo, value); }
        }

        private DateTime? auditDate;
        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate
        {
            get { return auditDate; }
            set { base.SetValue("AuditDate", ref auditDate, value); }
        }

        private int? outStockUserSysNo;
        /// <summary>
        /// 出库人
        /// </summary>
        public int? OutStockUserSysNo
        {
            get { return outStockUserSysNo; }
            set { base.SetValue("OutStockUserSysNo", ref outStockUserSysNo, value); }
        }

        private DateTime? outStockDate;
        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime? OutStockDate
        {
            get { return outStockDate; }
            set { base.SetValue("OutStockDate", ref outStockDate, value); }
        }

        private int? stockSysNo;
        /// <summary>
        /// 源渠道仓库
        /// </summary>
        public int? StockSysNo
        {
            get
            {
                return stockSysNo;
            }
            set
            {
                SetValue("StockSysNo", ref stockSysNo, value);
            }
        }

        private AdjustRequestStatus? requestStatus = AdjustRequestStatus.Origin;
        /// <summary>
        /// 单据状态
        /// </summary>
        public AdjustRequestStatus? RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                SetValue("RequestStatus", ref requestStatus, value);
            }
        }

        private RequestConsignFlag? consignFlag = null;
        /// <summary>
        /// 代销标识
        /// </summary>
        public RequestConsignFlag? ConsignFlag
        {
            get { return consignFlag; }
            set { base.SetValue("ConsignFlag", ref consignFlag, value); }
        }

        private string note;
        /// <summary>
        /// 单据备注
        /// </summary>
        public string Note
        {
            get { return note; }
            set { base.SetValue("Note", ref note, value); }
        }

        private AdjustRequestProperty? adjustProperty;
        /// <summary>
        /// 损益单类型
        /// </summary> 
        public AdjustRequestProperty? AdjustProperty
        {
            get { return adjustProperty; }
            set { base.SetValue("AdjustProperty", ref adjustProperty, value); }
        }

        private List<AdjustRequestItemVM> adjustItemInfoList;
        /// <summary>
        /// 损益商品明细表
        /// </summary> 
        public List<AdjustRequestItemVM> AdjustItemInfoList
        {
            get { return adjustItemInfoList; }
            set { base.SetValue("AdjustItemInfoList", ref adjustItemInfoList, value); }
        }

        private AdjustRequestInvoiceVM invoiceInfo;
        /// <summary>
        /// 损益单发票
        /// </summary> 
        public AdjustRequestInvoiceVM InvoiceInfo
        {
            get { return invoiceInfo; }
            set { base.SetValue("InvoiceInfo", ref invoiceInfo, value); }

        }


        private bool isShippingMaster;
        public bool IsShippingMaster
        {
            get { return isShippingMaster; }
            set { base.SetValue("IsShippingMaster", ref isShippingMaster, value); }
        }


        private List<KeyValuePair<RequestConsignFlag?, string>> consignFlagList;
        public List<KeyValuePair<RequestConsignFlag?, string>> ConsignFlagList
        {
            get
            {
                consignFlagList = consignFlagList ?? EnumConverter.GetKeyValuePairs<RequestConsignFlag>();
                return consignFlagList;
            }
        }

        private List<KeyValuePair<AdjustRequestProperty?, string>> adjustRequestPropertyList;
        public List<KeyValuePair<AdjustRequestProperty?, string>> AdjustRequestPropertyList
        {
            get
            {
                adjustRequestPropertyList = adjustRequestPropertyList ?? EnumConverter.GetKeyValuePairs<AdjustRequestProperty>();
                return adjustRequestPropertyList;
            }
        }

        #region UI Model

        public bool IsCreateMode
        {
            get
            {
                return (this.SysNo == null || this.SysNo <= 0);
            }
        }

        public bool IsEditMode
        {
            get
            {
                return this.RequestStatus == AdjustRequestStatus.Origin;
            }
        }

        public bool IsInvoiceEnable
        {
            get
            {
                return this.AdjustProperty == AdjustRequestProperty.AllShipping
                    && this.RequestStatus == AdjustRequestStatus.OutStock;
            }
        }

        public bool IsPrintInvoiceEnable
        {
            get
            {
                return IsInvoiceEnable
                    && (this.InvoiceInfo != null && this.InvoiceInfo.SysNo != null);
            }
        }

        public bool IsPrintEnable
        {
            get
            {
                return !IsCreateMode;
            }
        }

        #endregion UI Model
    }

    /// <summary>
    /// 损益单发票
    /// </summary>
    public class AdjustRequestInvoiceVM : ModelBase
    {
        private int? sysNo;
        /// <summary>
        /// 发票系统编号
        /// </summary>
        public int? SysNo
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

        private string receiveName;
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string ReceiveName
        {
            get
            {
                return receiveName;
            }
            set
            {
                SetValue("ReceiveName", ref receiveName, value);
            }
        }
       
        private string contactAddress;

        /// <summary>
        /// 联系地址
        /// </summary>
        public string ContactAddress
        {
            get
            {
                return contactAddress;
            }
            set
            {
                SetValue("ContactAddress", ref contactAddress, value);
            }
        }

        private string contactShippingAddress;
        /// <summary>
        /// 收件地址
        /// </summary>
        public string ContactShippingAddress
        {
            get
            {
                return contactShippingAddress;
            }
            set
            {
                SetValue("ContactShippingAddress", ref contactShippingAddress, value);
            }
        }

        private string contactPhoneNumber;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhoneNumber
        {
            get
            {
                return contactPhoneNumber;
            }
            set
            {
                SetValue("ContactPhoneNumber", ref contactPhoneNumber, value);
            }
        }

        private string customerID;
        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerID
        {
            get
            {
                return customerID;
            }
            set
            {
                SetValue("CustomerID", ref customerID, value);
            }
        }

        private string invoiceNumber;
        /// <summary>
        /// 发票编号
        /// </summary>
        public string InvoiceNumber
        {
            get
            {
                return invoiceNumber;
            }
            set
            {
                SetValue("InvoiceNumber", ref invoiceNumber, value);
            }
        }

        private string note;
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                SetValue("Note", ref note, value);
            }
        }

    }

    public class AdjustRequestItemVM : ModelBase
    {

        public AdjustRequestItemVM()
        {
            this.BatchDetailsInfoList = new List<ProductBatchInfoVM>();
        }

        private int? sysNo;
        public int? SysNo
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

        /// <summary>
        ///  商品编号
        /// </summary>
        private int? productSysNo;
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

        private string productID;
        public string ProductID
        {
            get
            {
                return productID;
            }
            set
            {
                SetValue("ProductID", ref productID, value);
            }
        }

        private string productName;
        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                SetValue("ProductName", ref productName, value);
            }
        }

        private int adjustQuantity;
        /// <summary>
        /// 损益数量
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, @"^-?[1-9](\d{0,5})$", ErrorMessageResourceName = "Msg_Quantity_Format", ErrorMessageResourceType = typeof(ResAdjustRequestMaintain))]
        public int AdjustQuantity
        {
            get
            {
                return adjustQuantity;
            }
            set
            {
                SetValue("AdjustQuantity", ref adjustQuantity, value);
                TotalAdjustCost = value * AdjustCost;
            }
        }

        private decimal adjustCost;
        /// <summary>
        /// 损益成本
        /// </summary>
        public decimal AdjustCost
        {
            get
            {
                return adjustCost;
            }
            set
            {
                SetValue("AdjustCost", ref adjustCost, value);
                TotalAdjustCost = value * AdjustQuantity;
            }
        }

        private decimal totalAdjustCost;
        public decimal TotalAdjustCost
        {
            get
            {
                return totalAdjustCost;
            }
            set
            {
                SetValue("TotalAdjustCost", ref totalAdjustCost, value);
            }
        }

        public List<ProductBatchInfoVM> BatchDetailsInfoList { get; set; }

        public int IsHasBatch { get; set; }

        #region UI扩展属性
        private AdjustRequestStatus? requestStatus;
        public AdjustRequestStatus? RequestStatus
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

        public bool IsEditMode
        {
            get
            {
                return this.RequestStatus == AdjustRequestStatus.Origin;
            }
        }

        public bool HasBatchInfo
        {
            get
            {
                return this.BatchDetailsInfoList != null && this.BatchDetailsInfoList.Count > 0;
            }
        }

        public bool IsBatchMode
        {
            get
            {
                if (this.RequestStatus == AdjustRequestStatus.Origin)
                {
                    if (HasBatchInfo)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion UI扩展属性
    }
}
