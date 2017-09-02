using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Models
{
    /// <summary>
    /// 销售收款单查询条件ViewModel
    /// </summary>
    public class SaleIncomeQueryVM : ModelBase
    {
        public SaleIncomeQueryVM()
        {
            IncomeAmtOper = OperationSignType.LessThanOrEqual;
            PrepayAmtOper = OperationSignType.LessThanOrEqual;
            IsCombine = false;
            OrderType = SOIncomeOrderType.SO;
            CreateDateFrom = DateTime.Now.Date;
            CreateDateTo = DateTime.Now.AddDays(1).Date;
        }

        private String m_OrderID;
        [Validate(ValidateType.Regex, @"^[1-9][0-9]{0,9}(\.[1-9][0-9]{0,9})*$")]
        public String OrderID
        {
            get
            {
                return this.m_OrderID;
            }
            set
            {
                this.SetValue("OrderID", ref m_OrderID, value);
            }
        }

        private String m_CustomerSysNo;
        [Validate(ValidateType.Regex, @"^[1-9][0-9]{0,9}(\.[1-9][0-9]{0,9}){0,10}$")]
        public String CustomerSysNo
        {
            get
            {
                return this.m_CustomerSysNo;
            }
            set
            {
                this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value);
            }
        }

        private SOIncomeOrderType? m_OrderType;
        public SOIncomeOrderType? OrderType
        {
            get
            {
                return this.m_OrderType;
            }
            set
            {
                this.SetValue("OrderType", ref m_OrderType, value);
            }
        }

        private DateTime? m_CreateDateFrom;
        public DateTime? CreateDateFrom
        {
            get
            {
                return this.m_CreateDateFrom;
            }
            set
            {
                this.SetValue("CreateDateFrom", ref m_CreateDateFrom, value);
            }
        }

        private DateTime? m_CreateDateTo;
        public DateTime? CreateDateTo
        {
            get
            {
                return this.m_CreateDateTo;
            }
            set
            {
                this.SetValue("CreateDateTo", ref m_CreateDateTo, value);
            }
        }

        private DateTime? m_ConfirmDateFrom;
        public DateTime? ConfirmDateFrom
        {
            get
            {
                return this.m_ConfirmDateFrom;
            }
            set
            {
                this.SetValue("ConfirmDateFrom", ref m_ConfirmDateFrom, value);
            }
        }

        private DateTime? m_ConfirmDateTo;
        public DateTime? ConfirmDateTo
        {
            get
            {
                return this.m_ConfirmDateTo;
            }
            set
            {
                this.SetValue("ConfirmDateTo", ref m_ConfirmDateTo, value);
            }
        }

        private DateTime? m_SOOutDateFrom;
        public DateTime? SOOutDateFrom
        {
            get
            {
                return this.m_SOOutDateFrom;
            }
            set
            {
                this.SetValue("SOOutDateFrom", ref m_SOOutDateFrom, value);
            }
        }

        private DateTime? m_SOOutDateTo;
        public DateTime? SOOutDateTo
        {
            get
            {
                return this.m_SOOutDateTo;
            }
            set
            {
                this.SetValue("SOOutDateTo", ref m_SOOutDateTo, value);
            }
        }

        private DateTime? m_RORefundDateFrom;
        public DateTime? RORefundDateFrom
        {
            get
            {
                return this.m_RORefundDateFrom;
            }
            set
            {
                this.SetValue("RORefundDateFrom", ref m_RORefundDateFrom, value);
            }
        }

        private DateTime? m_RORefundDateTo;
        public DateTime? RORefundDateTo
        {
            get
            {
                return this.m_RORefundDateTo;
            }
            set
            {
                this.SetValue("RORefundDateTo", ref m_RORefundDateTo, value);
            }
        }

        private SOIncomeOrderStyle? m_IncomeType;
        public SOIncomeOrderStyle? IncomeType
        {
            get
            {
                return this.m_IncomeType;
            }
            set
            {
                this.SetValue("IncomeType", ref m_IncomeType, value);
            }
        }

        private SOIncomeStatus? m_IncomeStatus;
        public SOIncomeStatus? IncomeStatus
        {
            get
            {
                return this.m_IncomeStatus;
            }
            set
            {
                this.SetValue("IncomeStatus", ref m_IncomeStatus, value);
            }
        }

        private String m_PayTypeSysNo;
        public String PayTypeSysNo
        {
            get
            {
                return this.m_PayTypeSysNo;
            }
            set
            {
                this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value);
            }
        }

        private String m_ShipTypeSysNo;
        public String ShipTypeSysNo
        {
            get
            {
                return this.m_ShipTypeSysNo;
            }
            set
            {
                this.SetValue("ShipTypeSysNo", ref m_ShipTypeSysNo, value);
            }
        }

        private String m_IncomeConfirmer;
        public String IncomeConfirmer
        {
            get
            {
                return this.m_IncomeConfirmer;
            }
            set
            {
                this.SetValue("IncomeConfirmer", ref m_IncomeConfirmer, value);
            }
        }

        private String m_FreightMen;
        public String FreightMen
        {
            get
            {
                return this.m_FreightMen;
            }
            set
            {
                this.SetValue("FreightMen", ref m_FreightMen, value);
            }
        }

        private String m_OrderSysNo;
        /// <summary>
        /// 订单号
        /// </summary>
        [Validate(ValidateType.Regex, @"^([1-9][0-9]{0,7})?$")]
        public String OrderSysNo
        {
            get
            {
                return this.m_OrderSysNo;
            }
            set
            {
                this.SetValue("OrderSysNo", ref m_OrderSysNo, value);
            }
        }

        private bool m_IsAudit;
        public bool IsAudit
        {
            get
            {
                return this.m_IsAudit;
            }
            set
            {
                this.SetValue("IsAudit", ref m_IsAudit, value);
            }
        }

        private String m_BankName;
        public String BankName
        {
            get
            {
                return this.m_BankName;
            }
            set
            {
                this.SetValue("BankName", ref m_BankName, value);
            }
        }

        private String m_PostAddress;
        public String PostAddress
        {
            get
            {
                return this.m_PostAddress;
            }
            set
            {
                this.SetValue("PostAddress", ref m_PostAddress, value);
            }
        }

        private String m_ReferenceID;
        public String ReferenceID
        {
            get
            {
                return this.m_ReferenceID;
            }
            set
            {
                this.SetValue("ReferenceID", ref m_ReferenceID, value);
            }
        }

        private bool m_IsIncludeShipInfo;
        public bool IsIncludeShipInfo
        {
            get
            {
                return this.m_IsIncludeShipInfo;
            }
            set
            {
                this.SetValue("IsIncludeShipInfo", ref m_IsIncludeShipInfo, value);
            }
        }

        private bool m_IsIncludeReturnInfo;
        public bool IsIncludeReturnInfo
        {
            get
            {
                return this.m_IsIncludeReturnInfo;
            }
            set
            {
                this.SetValue("IsIncludeReturnInfo", ref m_IsIncludeReturnInfo, value);
            }
        }

        private bool m_IsCheckShip;
        public bool IsCheckShip
        {
            get
            {
                return this.m_IsCheckShip;
            }
            set
            {
                this.SetValue("IsCheckShip", ref m_IsCheckShip, value);
            }
        }

        private bool m_IsDifference;
        public bool IsDifference
        {
            get
            {
                return this.m_IsDifference;
            }
            set
            {
                this.SetValue("IsDifference", ref m_IsDifference, value);
            }
        }

        private SOStatus? m_SOOutStatus;
        public SOStatus? SOOutStatus
        {
            get
            {
                return this.m_SOOutStatus;
            }
            set
            {
                this.SetValue("SOOutStatus", ref m_SOOutStatus, value);
            }
        }

        private bool m_IsCash;
        public bool IsCash
        {
            get
            {
                return this.m_IsCash;
            }
            set
            {
                this.SetValue("IsCash", ref m_IsCash, value);
            }
        }

        private bool m_IsTotal;
        public bool IsTotal
        {
            get
            {
                return this.m_IsTotal;
            }
            set
            {
                this.SetValue("IsTotal", ref m_IsTotal, value);
            }
        }

        private bool m_IsException;
        public bool IsException
        {
            get
            {
                return this.m_IsException;
            }
            set
            {
                this.SetValue("IsException", ref m_IsException, value);
            }
        }

        private String m_WarehouseNumber;
        public String WarehouseNumber
        {
            get
            {
                return this.m_WarehouseNumber;
            }
            set
            {
                this.SetValue("WarehouseNumber", ref m_WarehouseNumber, value);
            }
        }

        private bool m_IsSyncSAPSales;
        public bool IsSyncSAPSales
        {
            get
            {
                return this.m_IsSyncSAPSales;
            }
            set
            {
                this.SetValue("IsSyncSAPSales", ref m_IsSyncSAPSales, value);
            }
        }

        private bool m_IsCombine;
        public bool IsCombine
        {
            get
            {
                return this.m_IsCombine;
            }
            set
            {
                this.SetValue("IsCombine", ref m_IsCombine, value);
            }
        }

        private bool m_IsByWareHourse;
        public bool IsByWareHourse
        {
            get
            {
                return this.m_IsByWareHourse;
            }
            set
            {
                this.SetValue("IsByWareHourse", ref m_IsByWareHourse, value);
            }
        }

        private String m_ByCustomer;
        public String ByCustomer
        {
            get
            {
                return this.m_ByCustomer;
            }
            set
            {
                this.SetValue("ByCustomer", ref m_ByCustomer, value);
            }
        }

        private string m_IncomeAmt;
        /// <summary>
        /// 实收金额
        /// </summary>
        [Validate(ValidateType.Regex, @"^(-)?[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string IncomeAmt
        {
            get
            {
                return this.m_IncomeAmt;
            }
            set
            {
                this.SetValue("IncomeAmt", ref m_IncomeAmt, value);
            }
        }

        private string m_PrepayAmt;
        /// <summary>
        /// 预收金额
        /// </summary>
        [Validate(ValidateType.Regex, @"^(-)?[0-9]+(\d{1,8})?(\.(\d){1,2})?$")]
        public string PrepayAmt
        {
            get
            {
                return this.m_PrepayAmt;
            }
            set
            {
                this.SetValue("PrepayAmt", ref m_PrepayAmt, value);
            }
        }

        private OperationSignType m_IncomeAmtOper;
        public OperationSignType IncomeAmtOper
        {
            get
            {
                return this.m_IncomeAmtOper;
            }
            set
            {
                this.SetValue("IncomeAmtOper", ref m_IncomeAmtOper, value);
            }
        }

        private OperationSignType m_PrepayAmtOper;
        public OperationSignType PrepayAmtOper
        {
            get
            {
                return this.m_PrepayAmtOper;
            }
            set
            {
                this.SetValue("PrepayAmtOper", ref m_PrepayAmtOper, value);
            }
        }

        private CashOnDeliveryType? _CashOnDeliveryType;
        public CashOnDeliveryType? CashOnDeliveryType
        {
            get { return _CashOnDeliveryType; }
            set
            {
                SetValue("CashOnDeliveryType", ref _CashOnDeliveryType, value);
            }
        }


        private String m_SapCoCode;
        public String SapCoCode
        {
            get
            {
                return this.m_SapCoCode;
            }
            set
            {
                this.SetValue("SapCoCode", ref m_SapCoCode, value);
            }
        }

        private bool m_IsRelated;
        public bool IsRelated
        {
            get
            {
                return this.m_IsRelated;
            }
            set
            {
                this.SetValue("IsRelated", ref m_IsRelated, value);
            }
        }

        private bool m_IsSalesOrder;
        public bool IsSalesOrder
        {
            get
            {
                return this.m_IsSalesOrder;
            }
            set
            {
                this.SetValue("IsSalesOrder", ref m_IsSalesOrder, value);
            }
        }

        private bool m_IsGiftCard;
        public bool IsGiftCard
        {
            get
            {
                return this.m_IsGiftCard;
            }
            set
            {
                this.SetValue("IsGiftCard", ref m_IsGiftCard, value);
            }
        }

        private DateTime? m_PayedDateFrom;
        public DateTime? PayedDateFrom
        {
            get
            {
                return this.m_PayedDateFrom;
            }
            set
            {
                this.SetValue("PayedDateFrom", ref m_PayedDateFrom, value);
            }
        }

        private DateTime? m_PayedDateTo;
        public DateTime? PayedDateTo
        {
            get
            {
                return this.m_PayedDateTo;
            }
            set
            {
                this.SetValue("PayedDateTo", ref m_PayedDateTo, value);
            }
        }

        private bool m_IsNetPay;
        public bool IsNetPay
        {
            get
            {
                return this.m_IsNetPay;
            }
            set
            {
                this.SetValue("IsNetPay", ref m_IsNetPay, value);
            }
        }

        private Int32? m_RMARefundPayType;
        public Int32? RMARefundPayType
        {
            get
            {
                return this.m_RMARefundPayType;
            }
            set
            {
                this.SetValue("RMARefundPayType", ref m_RMARefundPayType, value);
            }
        }

        public string CompanyCode
        {
            get
            {
                return CPApplication.Current.CompanyCode;
            }
        }

        #region 扩展属性

        /// <summary>
        /// 销售渠道列表
        /// </summary>
        public List<WebChannelVM> WebChannelList
        {
            get
            {
                var webchannleList = CPApplication.Current.CurrentWebChannelList.Convert<UIWebChannel, WebChannelVM>();
                webchannleList.Insert(0, new WebChannelVM()
                {
                    ChannelName = ResCommonEnum.Enum_All
                });
                return webchannleList;
            }
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderType?, string>> OrderTypeList
        {
            get
            {
                //return EnumConverter.GetKeyValuePairs<SOIncomeOrderType>();
                var orderTypeList = EnumConverter.GetKeyValuePairs<SOIncomeOrderType>(EnumConverter.EnumAppendItemType.All);
                //orderTypeList.RemoveAll(x => x.Key == SOIncomeOrderType.Group || x.Key == SOIncomeOrderType.GroupRefund);
                return orderTypeList;
            }
        }

        /// <summary>
        /// 收款单状态
        /// </summary>
        public List<KeyValuePair<SOIncomeStatus?, string>> IncomeStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 收款类型
        /// </summary>
        public List<KeyValuePair<SOIncomeOrderStyle?, string>> IncomeTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOIncomeOrderStyle>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 是、否状态列表
        /// </summary>
        public List<KeyValuePair<bool?, string>> YNList
        {
            get
            {
                //return EnumConverter.GetKeyValuePairs<YNStatus>(EnumConverter.EnumAppendItemType.All);
                return BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 订单状态列表
        /// </summary>
        public List<KeyValuePair<SOStatus?, string>> SOStatusList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
            }
        }

        /// <summary>
        /// 操作符号列表
        /// </summary>
        public List<KeyValuePair<OperationSignType?, string>> OperationSignTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<OperationSignType>(EnumConverter.EnumAppendItemType.None);
            }
        }


        /// <summary>
        /// 货到付款列表
        /// </summary>
        public List<KeyValuePair<CashOnDeliveryType?, string>> CashOnDeliveryTypeList
        {
            get
            {
                return EnumConverter.GetKeyValuePairs<CashOnDeliveryType>(EnumConverter.EnumAppendItemType.All);
            }
        }


        #endregion 扩展属性

        #region 延迟加载的属性

        private List<UserInfo> m_IncomeConfirmerList;
        /// <summary>
        /// 收款单审核人列表
        /// </summary>
        public List<UserInfo> IncomeConfirmerList
        {
            get
            {
                return m_IncomeConfirmerList;
            }
            set
            {
                SetValue("IncomeConfirmerList", ref m_IncomeConfirmerList, value);
            }
        }

        private List<UserInfo> m_FreightMenList;
        /// <summary>
        /// 投递员列表
        /// </summary>
        public List<UserInfo> FreightMenList
        {
            get
            {
                return m_FreightMenList;
            }
            set
            {
                SetValue("FreightMenList", ref m_FreightMenList, value);
            }
        }

        #endregion 延迟加载的属性
    }
}