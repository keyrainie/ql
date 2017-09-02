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
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ConvertRequestVM : ModelBase
    {
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

        private string productLineSysno;
        /// <summary>
        /// 产品线
        /// </summary>
        public string ProductLineSysno
        {
            get { return productLineSysno; }
            set { base.SetValue("ProductLineSysno", ref productLineSysno, value); }
        }

        private ConvertRequestStatus? requestStatus;
        public ConvertRequestStatus? RequestStatus
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

        private string companyCode;
        public string CompanyCode
        {
            get { return companyCode; }
            set { base.SetValue("CompanyCode", ref companyCode, value); }
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

        private RequestConsignFlag consignFlag;
        /// <summary>
        /// 代销标识
        /// </summary>
        public RequestConsignFlag ConsignFlag
        {
            get
            {
                return consignFlag;
            }
            set
            {
                SetValue("ConsignFlag", ref consignFlag, value);
            }
        }

        private string note;
        /// <summary>
        /// 单据备注
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

        private List<ConvertRequestItemVM> convertItemInfoList;
        /// <summary>
        /// 转换商品列表
        /// </summary>
        public List<ConvertRequestItemVM> ConvertItemInfoList
        {
            get { return convertItemInfoList; }
            set
            {
                SetValue("ConvertItemInfoList", ref convertItemInfoList, value);
            }
        }


        private List<KeyValuePair<ConvertRequestStatus?, string>> requestStatusList;
        public List<KeyValuePair<ConvertRequestStatus?, string>> RequestStatusList
        {
            get
            {
                requestStatusList = requestStatusList ?? EnumConverter.GetKeyValuePairs<ConvertRequestStatus>();
                return requestStatusList;
            }
        }
        public ConvertRequestVM()
        {
            requestStatus = ConvertRequestStatus.Origin;
            convertItemInfoList = new List<ConvertRequestItemVM>();
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
                return this.RequestStatus == ConvertRequestStatus.Origin;
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

    public class ConvertRequestItemVM : ModelBase
    {
        public ConvertRequestItemVM()
        {
            this.BatchDetailsInfoList = new List<ProductBatchInfoVM>();
        }

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                SetValue("IsChecked", ref isChecked, value);
            }
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
        /// 商品编号
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

        private ConvertProductType convertType;
        /// <summary>
        /// 转换类型
        /// </summary>
        public ConvertProductType ConvertType
        {
            get
            {
                return convertType;
            }
            set
            {
                SetValue("ConvertType", ref convertType, value);
            }
        }
        private int convertQuantity;
        /// <summary>
        /// 转换数量
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, @"^[1-9](\d{0,5})$", ErrorMessageResourceName = "Msg_Quantity_Format", ErrorMessageResourceType = typeof(ResConvertRequestMaintain))]
        public int ConvertQuantity
        {
            get
            {
                return convertQuantity;
            }
            set
            {
                SetValue("ConvertQuantity", ref convertQuantity, value);
                TotalUnitCost = value * ConvertUnitCost;
                if (QuantityOrCostChanged != null)
                { QuantityOrCostChanged(); }
            }
        }
        public Action QuantityOrCostChanged;

        private decimal convertUnitCost;
        /// <summary>
        /// 转换成本
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, ConstValue.Format_Money, ErrorMessageResourceName = "Msg_Money_Format", ErrorMessageResourceType = typeof(ResConvertRequestMaintain))]
        public decimal ConvertUnitCost
        {
            get
            {
                return convertUnitCost;
            }
            set
            {
                SetValue("ConvertUnitCost", ref convertUnitCost, value);
                TotalUnitCost = value * ConvertQuantity;
                if (QuantityOrCostChanged != null)
                { QuantityOrCostChanged(); }
            }
        }

        private decimal convertUnitCostWithoutTax;
        /// <summary>
        /// 转换去税成本
        /// </summary>
        public decimal ConvertUnitCostWithoutTax
        {
            get
            {
                return convertUnitCostWithoutTax;
            }
            set
            {
                SetValue("ConvertUnitCostWithoutTax", ref convertUnitCostWithoutTax, value);
            }
        }

        private decimal totalUnitCost;
        public decimal TotalUnitCost
        {
            get
            {
                return totalUnitCost;
            }
            private set
            {
                SetValue("TotalUnitCost", ref totalUnitCost, value);
            }
        }

        public List<ProductBatchInfoVM> BatchDetailsInfoList { get; set; }

        public int IsHasBatch { get; set; }

        #region UI扩展属性
        private ConvertRequestStatus? requestStatus;
        public ConvertRequestStatus? RequestStatus
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

        public bool UnitCostIsEnabled
        {
            get
            {
                return ConvertType == ConvertProductType.Target && IsEditMode;
            }
        }

        public bool IsEditMode
        {
            get
            {
                return this.RequestStatus == ConvertRequestStatus.Origin && !HasBatchInfo;
            }
        }

        public bool HasBatchInfo
        {
            get
            {
                return this.BatchDetailsInfoList != null && this.BatchDetailsInfoList.Count > 0;
            }
        }

        #endregion UI扩展属性

    }

}
