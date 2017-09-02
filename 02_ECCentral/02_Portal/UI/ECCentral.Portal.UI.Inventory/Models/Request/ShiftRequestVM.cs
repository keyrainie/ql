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
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.SO;


namespace ECCentral.Portal.UI.Inventory.Models
{
    public class ShiftRequestVM : ModelBase
    {
        public ShiftRequestVM()
        {
            ShiftItemInfoList = new List<ShiftRequestItemVM>();
            ShiftShippingType = "普通移仓-每周五陆运移出";
            ShiftType = ShiftRequestType.Positive;
            RequestStatus = ShiftRequestStatus.Origin;
            ConsignFlag = null;
            
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

        private string createUserName;
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName
        {
            get { return createUserName; }
            set { base.SetValue("CreateUserName", ref createUserName, value); }
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

        private string editUserName;
        /// <summary>
        /// 更新人
        /// </summary>
        public string EditUserName
        {
            get { return editUserName; }
            set { base.SetValue("EditUserName", ref editUserName, value); }
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

        private string auditUserName;
        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditUserName
        {
            get { return auditUserName; }
            set { base.SetValue("AuditUserName", ref auditUserName, value); }
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

        private string outStockUserName;
        /// <summary>
        /// 出库人
        /// </summary>
        public string OutStockUserName
        {
            get { return outStockUserName; }
            set { base.SetValue("OutStockUserName", ref outStockUserName, value); }
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



        private int? inStockUserSysNo;
        /// <summary>
        /// 入库人
        /// </summary>
        public int? InStockUserSysNo
        {
            get { return inStockUserSysNo; }
            set { base.SetValue("InStockUserSysNo", ref inStockUserSysNo, value); }
        }

        private string inStockUserName;
        /// <summary>
        /// 入库人
        /// </summary>
        public string InStockUserName
        {
            get { return inStockUserName; }
            set { base.SetValue("InStockUserName", ref inStockUserName, value); }
        }

        private DateTime? inStockDate;
        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime? InStockDate
        {
            get { return inStockDate; }
            set { base.SetValue("InStockDate", ref inStockDate, value); }
        }

        private int? sourceStockSysNo;
        /// <summary>
        /// 移出渠道仓库
        /// </summary>
        public int? SourceStockSysNo
        {
            get { return sourceStockSysNo; }
            set { base.SetValue("SourceStockSysNo", ref sourceStockSysNo, value); }
        }

        private string sourceStockName;
        /// <summary>
        /// 移出渠道仓库
        /// </summary>
        public string SourceStockName
        {
            get { return sourceStockName; }
            set { base.SetValue("SourceStockName", ref sourceStockName, value); }
        }

        private int? targetStockSysNo;
        /// <summary>
        /// 移入渠道仓库
        /// </summary>
        public int? TargetStockSysNo
        {
            get { return targetStockSysNo; }
            set { base.SetValue("TargetStockSysNo", ref targetStockSysNo, value); }
        }

        private string targetStockName;
        /// <summary>
        /// 移入渠道仓库
        /// </summary>
        public string TargetStockName
        {
            get { return targetStockName; }
            set { base.SetValue("TargetStockName", ref targetStockName, value); }
        }

        private ShiftRequestStatus? requestStatus;
        /// <summary>
        /// 单据状态
        /// </summary>
        public ShiftRequestStatus? RequestStatus
        {
            get { return requestStatus; }
            set { base.SetValue("RequestStatus", ref requestStatus, value); }
        }

        private RequestConsignFlag? consignFlag;
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

        private ShiftRequestType shiftType;
        /// <summary>
        /// 移仓单类型
        /// </summary>
        public ShiftRequestType ShiftType
        {
            get { return shiftType; }
            set { base.SetValue("ShiftType", ref shiftType, value); }
        }

        private string shiftShippingType;
        /// <summary>
        /// 移仓配送方式
        /// </summary>
        public string ShiftShippingType
        {
            get { return shiftShippingType; }
            set { base.SetValue("ShiftShippingType", ref shiftShippingType, value); }
        }

        private List<ShiftRequestItemVM> shiftItemInfoList;
        /// <summary>
        /// 移仓商品列表
        /// </summary>
        public List<ShiftRequestItemVM> ShiftItemInfoList
        {
            get { return shiftItemInfoList; }
            set { base.SetValue("ShiftItemInfoList", ref shiftItemInfoList, value); }
        }

        #region Shipping相关属性

        private string trackingNumber;
        public string TrackingNumber
        {
            get { return trackingNumber; }
            set { base.SetValue("TrackingNumber", ref trackingNumber, value); }
        }

        #endregion Shipping相关属性

        #region SO相关属性

        private int? soSysNo;
        public int? SOSysNo
        {
            get { return soSysNo; }
            set { base.SetValue("SOSysNo", ref soSysNo, value); }
        }

        private SOStatus? soStatus;
        public SOStatus? SOStatus
        {
            get { return soStatus; }
            set { base.SetValue("SOStatus", ref soStatus, value); }
        }

        private DateTime? soOutStockDate;
        public DateTime? SOOutStockDate
        {
            get { return soOutStockDate; }
            set { base.SetValue("SOOutStockDate", ref soOutStockDate, value); }
        }        

        #endregion SO相关属性

        #region original properties to confirm

        #region 特殊移仓单相关属性

        private bool isSpecialShift;
        public bool IsSpecialShift
        {
            get { return isSpecialShift; }
            set { base.SetValue("IsSpecialShift", ref isSpecialShift, value); }
        }

        private SpecialShiftRequestType? specialShiftType;
        /// <summary>
        /// 单据状态
        /// </summary>
        public SpecialShiftRequestType? SpecialShiftType
        {
            get { return specialShiftType; }
            set { base.SetValue("SpecialShiftType", ref specialShiftType, value); }
        }

        private int? specialShiftSetUserSysNo;
        /// <summary>
        /// 特殊移仓单设置人
        /// </summary>
        public int? SpecialShiftSetUserSysNo
        {
            get { return specialShiftSetUserSysNo; }
            set { base.SetValue("SpecialShiftSetUserSysNo", ref specialShiftSetUserSysNo, value); }
        }

        private string specialShiftSetUserName;
        /// <summary>
        /// 特殊移仓单设置人
        /// </summary>
        public string SpecialShiftSetUserName
        {
            get { return specialShiftSetUserName; }
            set { base.SetValue("SpecialShiftSetUserName", ref specialShiftSetUserName, value); }
        }

        private DateTime? specialShiftSetDate;
        /// <summary>
        /// 特殊移仓单设置日期
        /// </summary>
        public DateTime? SpecialShiftSetDate
        {
            get { return specialShiftSetDate; }
            set { base.SetValue("SpecialShiftSetDate", ref specialShiftSetDate, value); }
        }

        private bool hasSpecialRightforCreate;
        public bool HasSpecialRightforCreate
        {
            get { return hasSpecialRightforCreate; }
            set { base.SetValue("HasSpecialRightforCreate", ref hasSpecialRightforCreate, value); }
        }

        #endregion 特殊移仓单相关属性

        private int? poSysNo;
        public int? POSysNo
        {
            get { return poSysNo; }
            set { base.SetValue("POSysNo", ref poSysNo, value); }
        }

        private bool? isScanned;
        public bool? IsScanned
        {
            get { return isScanned; }
            set { base.SetValue("IsScanned", ref isScanned, value); }
        }

        private decimal? totalAmount;
        public decimal? TotalAmount
        {
            get { return totalAmount; }
            set { base.SetValue("TotalAmount", ref totalAmount, value); }
        }

        private decimal? totalWeight;
        public decimal? TotalWeight
        {
            get { return totalWeight; }
            set { base.SetValue("TotalWeight", ref totalWeight, value); }
        }

        #endregion original properties to confirm


        private List<KeyValuePair<ShiftRequestType?, string>> shiftRequestTypeList;
        public List<KeyValuePair<ShiftRequestType?, string>> ShiftRequestTypeList
        {
            get
            {
                shiftRequestTypeList = shiftRequestTypeList ?? EnumConverter.GetKeyValuePairs<ShiftRequestType>();                
                return shiftRequestTypeList;
            }
        }

        private List<CodeNamePair> shiftShippingTypeList;
        public List<CodeNamePair> ShiftShippingTypeList
        {
            get { return shiftShippingTypeList; }
            set { base.SetValue("ShiftShippingTypeList", ref shiftShippingTypeList, value); }
        }

        #region UI Model

        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { SetValue("IsChecked", ref isChecked, value); }
        }

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
                return this.RequestStatus == ShiftRequestStatus.Origin;
            }
        }

        public bool IsPrintEnable
        {
            get
            {
                return !IsCreateMode;
            }
        }

        public bool IsMaintainLogEnable
        {
            get
            {
                return !IsCreateMode;
            }
        }

        public bool IsSpecialShiftTypeEnable
        {
            get
            {
                return !IsCreateMode;
            }
        }

        public Visibility SetSpecialShiftTypeVisibility
        {
            get
            {
                return this.SpecialShiftType == SpecialShiftRequestType.Default ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility CancelSetShiftTypeVisibility
        {
            get
            {
                return this.SpecialShiftType != SpecialShiftRequestType.Default ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsPrintInvoiceEnable
        {
            get
            {
                return this.RequestStatus == ShiftRequestStatus.InStock
                        || this.RequestStatus == ShiftRequestStatus.OutStock;
            }
        }

        #endregion UI Model
    }


    /// <summary>
    /// 移仓商品列表
    /// </summary>
    public class ShiftRequestItemVM : ModelBase
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

        /// <summary>
        ///  移仓商品编号
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
            get { return productName; }
            set { SetValue("ProductName", ref productName, value); }
        }

        private int shiftQuantity;
        /// <summary>
        /// 移仓数量
        /// </summary>
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Required)]
        [Validate(Newegg.Oversea.Silverlight.Utilities.Validation.ValidateType.Regex, @"^[1-9](\d{0,5})$", ErrorMessageResourceName = "Msg_Quantity_Format", ErrorMessageResourceType = typeof(ResShiftRequestMaintain))]
        public int ShiftQuantity
        {
            get { return shiftQuantity; }
            set
            {
                SetValue("ShiftQuantity", ref shiftQuantity, value);
                ShiftWeight = Weight * value;
                TotalCost = ShiftUnitCost.HasValue ? ShiftUnitCost * value : 0;
            }
        }

        private int? inStockQuantity;
        /// <summary>
        /// 移仓数量
        /// </summary>
        public int? InStockQuantity
        {
            get { return inStockQuantity; }
            set { SetValue("InStockQuantity", ref inStockQuantity, value); }
        }
        private decimal weight;
        /// <summary>
        /// 单个商品重量
        /// </summary>
        public decimal Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                ShiftWeight = weight * ShiftQuantity;
            }
        }

        private decimal? shiftWeight;
        /// <summary>
        /// 移仓总重量
        /// </summary>
        public decimal? ShiftWeight
        {
            get { return shiftWeight; }
            set { SetValue("ShiftWeight", ref shiftWeight, value); }
        }

        private decimal? totalWeight;
        public decimal? TotalWeight
        {
            get { return totalWeight; }
            set { SetValue("TotalWeight", ref totalWeight, value); }
        }
        

        private decimal? shiftUnitCost;
        /// <summary>
        /// 移仓成本
        /// </summary>
        public decimal? ShiftUnitCost
        {
            get { return shiftUnitCost; }
            set
            {
                SetValue("ShiftUnitCost", ref shiftUnitCost, value);
                if (value != null)
                {
                    TotalCost = value * ShiftQuantity;
                }
            }
        }


        private decimal? shiftUnitCostWithoutTax;
        /// <summary>
        /// 移仓去税成本
        /// </summary>
        public decimal? ShiftUnitCostWithoutTax
        {
            get { return shiftUnitCostWithoutTax; }
            set { SetValue("ShiftUnitCostWithoutTax", ref shiftUnitCostWithoutTax, value); }
        }

        private decimal? shippingCost;
        /// <summary>
        /// 配送成本
        /// </summary>
        public decimal? ShippingCost
        {
            get { return shippingCost; }
            set { SetValue("ShippingCost", ref shippingCost, value); }
        }

        private decimal? totalCost;
        /// <summary>
        /// 总成本
        /// </summary>
        public decimal? TotalCost
        {
            get { return totalCost; }
            set { SetValue("TotalCost", ref totalCost, value); }
        }

        #region UI扩展属性
        private ShiftRequestStatus? requestStatus;
        public ShiftRequestStatus? RequestStatus
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
                return this.RequestStatus == ShiftRequestStatus.Origin;
            }
        }
        #endregion UI扩展属性
    }
}
