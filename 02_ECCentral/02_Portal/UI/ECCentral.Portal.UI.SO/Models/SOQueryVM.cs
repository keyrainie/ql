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
using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using System.Linq;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOQueryVM : ModelBase
    {
        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        #region 常用查询条件
        private string soSysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^[,\. ]*\d+[\d,\. ]*$", ErrorMessageResourceName = "Msg_SOSysNo_Format", ErrorMessageResourceType = typeof(ResSO))]
        public string SOSysNo
        {
            get { return soSysNo; }
            set { SetValue("SOSysNo", ref soSysNo, value); }
        }
        private string customerSysNo;
        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set { SetValue("CustomerSysNo", ref customerSysNo, value); }
        }
        private DateTime? fromOrderTime;
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? FromOrderTime
        {
            get { return fromOrderTime; }
            set { SetValue<DateTime?>("FromOrderTime", ref fromOrderTime, value); }
        }
        private DateTime? toOrderTime;
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? ToOrderTime
        {
            get { return toOrderTime; }
            set { SetValue<DateTime?>("ToOrderTime", ref toOrderTime, value); }
        }
        private string fromTotalAmount;
        /// <summary>
        /// 开始订单总额
        /// </summary>
        [Validate(ValidateType.Regex, @"^-?\d+(.?\d+)?$", ErrorMessageResourceType = typeof(ResSO), ErrorMessageResourceName = "Msg_SOAmount_Format")]
        public string FromTotalAmount
        {
            get { return fromTotalAmount; }
            set { SetValue("FromTotalAmount", ref fromTotalAmount, value); }
        }
        private string toTotalAmount;
        /// <summary>
        /// 开始订单总额
        /// </summary>
        [Validate(ValidateType.Regex, @"^-?\d+(.?\d+)?$", ErrorMessageResourceType = typeof(ResSO), ErrorMessageResourceName = "Msg_SOAmount_Format")]
        public string ToTotalAmount
        {
            get
            {
                return toTotalAmount;
            }
            set
            {
                SetValue("ToTotalAmount", ref toTotalAmount, value);
            }
        }
        private int? payTypeSysNo;
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get { return payTypeSysNo; }
            set { SetValue<int?>("PayTypeSysNo", ref payTypeSysNo, value); }
        }
        private SOStatus? soStatus;
        /// <summary>
        /// 订单状态
        /// </summary>
        public SOStatus? SOStatus
        {
            get { return soStatus; }
            set { SetValue<SOStatus?>("SOStatus", ref soStatus, value); }
        }
        private string receiveName;
        /// <summary>
        /// 收件人
        /// </summary>
        public string ReceiveName
        {
            get { return receiveName; }
            set { SetValue<string>("ReceiveName", ref receiveName, value); }
        }
        private string receivePhone;
        /// <summary>
        /// 收件人电话
        /// </summary>
        public string ReceivePhone
        {
            get { return receivePhone; }
            set { SetValue<string>("ReceivePhone", ref receivePhone, value); }
        }
        private string receiveMobilePhone;
        /// <summary>
        /// 收件人手机
        /// </summary>
        public string ReceiveMobilePhone
        {
            get { return receiveMobilePhone; }
            set { SetValue<string>("ReceiveMobilePhone", ref receiveMobilePhone, value); }
        }

        private string receiveAddress;
        /// <summary>
        /// 配送地址
        /// </summary>
        public string ReceiveAddress
        {
            get { return receiveAddress; }
            set { SetValue<string>("ReceiveAddress", ref receiveAddress, value); }
        }
        private int? stockSysNo;
        /// <summary>
        /// 分仓编号
        /// </summary>
        public int? StockSysNo
        {
            get { return stockSysNo; }
            set { SetValue<int?>("StockSysNo", ref stockSysNo, value); }
        }

        public SOIncomeStatus? incomeStatus;
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public SOIncomeStatus? IncomeStatus
        {
            get { return incomeStatus; }
            set
            {
                if (value != null && (int)value == int.MinValue)
                {
                    NetPayStatus = BizEntity.Invoice.NetPayStatus.Origin;
                }
                else
                {
                    NetPayStatus = null;
                }
                SetValue<SOIncomeStatus?>("IncomeStatus", ref incomeStatus, value);
            }
        }

        public NetPayStatus? netPayStatus;
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public NetPayStatus? NetPayStatus
        {
            get { return netPayStatus; }
            set { SetValue<NetPayStatus?>("NetPayStatus", ref netPayStatus, value); }
        }
        #endregion

        #region 更多条件查询
        private string productID;
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get { return productID; }
            set { SetValue<string>("ProductID", ref productID, value); }
        }
        private DateTime? fromAuditTime;
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? FromAuditTime
        {
            get { return fromAuditTime; }
            set { SetValue<DateTime?>("FromAuditTime", ref fromAuditTime, value); }
        }
        private DateTime? toAuditTime;
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ToAuditTime
        {
            get { return toAuditTime; }
            set { SetValue<DateTime?>("ToAuditTime", ref toAuditTime, value); }
        }
        private int? auditUserSysNo;
        /// <summary>
        /// 审核用户编号
        /// </summary>
        public int? AuditUserSysNo
        {
            get { return auditUserSysNo; }
            set { SetValue<int?>("AuditUserSysNo", ref auditUserSysNo, value); }
        }
        private bool? isVIPCustomer;
        /// <summary>
        /// 是否是VIP客户
        /// </summary>
        public bool? IsVIPCustomer
        {
            get { return isVIPCustomer; }
            set { SetValue<bool?>("IsVIPCustomer", ref isVIPCustomer, value); }
        }
        private bool? isUsePromotion;
        /// <summary>
        /// 是否使用优惠券
        /// </summary>
        public bool? IsUsePromotion
        {
            get { return isUsePromotion; }
            set { SetValue<bool?>("IsUsePromotion", ref isUsePromotion, value); }
        }
        private bool? isPhoneOrder;
        /// <summary>
        /// 是否是电话下单
        /// </summary>
        public bool? IsPhoneOrder
        {
            get { return isPhoneOrder; }
            set { SetValue<bool?>("IsPhoneOrder", ref isPhoneOrder, value); }
        }
        private DateTime? deliveryDate;
        /// <summary>
        /// 配送时间
        /// </summary>
        public DateTime? DeliveryDate
        {
            get { return deliveryDate; }
            set { SetValue<DateTime?>("DeliveryDate", ref deliveryDate, value); }
        }
        private int? deliveryTimeRange;
        /// <summary>
        /// 配送时间段
        /// </summary>
        public int? DeliveryTimeRange
        {
            get { return deliveryTimeRange; }
            set { SetValue<int?>("DeliveryTimeRange", ref deliveryTimeRange, value); }
        }
        private int? category1SysNo;
        /// <summary>
        /// 商品大类
        /// </summary>
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { SetValue<int?>("Category1SysNo", ref category1SysNo, value); }
        }
        private int? category2SysNo;
        /// <summary>
        /// 商品中类
        /// </summary>
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { SetValue<int?>("Category2SysNo", ref category2SysNo, value); }
        }
        private int? category3SysNo;
        /// <summary>
        /// 商品小类
        /// </summary>
        public int? Category3SysNo
        {
            get { return category3SysNo; }
            set { SetValue<int?>("Category3SysNo", ref category3SysNo, value); }
        }
        private int? shipTypeSysNo;
        /// <summary>
        /// 运送方式编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get { return shipTypeSysNo; }
            set { SetValue<int?>("ShipTypeSysNo", ref shipTypeSysNo, value); }
        }
        private int? pmSysNo;
        /// <summary>
        /// PM系统编号
        /// </summary>
        public int? PMSysNo
        {
            get { return pmSysNo; }
            set { SetValue<int?>("PMSysNo", ref pmSysNo, value); }
        }
        private string invoiceNo;
        /// <summary>
        /// 发票编号
        /// </summary>
        public string InvoiceNo
        {
            get { return invoiceNo; }
            set { SetValue<string>("InvoiceNo", ref invoiceNo, value); }
        }
        private bool? includeHistory;
        /// <summary>
        /// 是否搜索历史记录
        /// </summary>
        public bool? IncludeHistory
        {
            get { return includeHistory; }
            set { SetValue<bool?>("IncludeHistory", ref includeHistory, value); }
        }

        private bool? isExpiateOrder;
        /// <summary>
        /// 是否补偿单
        /// </summary>
        public bool? IsExpiateOrder
        {
            get { return isExpiateOrder; }
            set { SetValue<bool?>("IsExpiateOrder", ref isExpiateOrder, value); }
        }

        private bool? isExperienceOrder;
        /// <summary>
        /// 是否体验厅订单
        /// </summary>
        public bool? IsExperienceOrder
        {
            get { return isExperienceOrder; }
            set { SetValue<bool?>("IsExperienceOrder", ref isExperienceOrder, value); }
        }

        private int? kfcType;
        /// <summary>
        /// 用户类型
        /// </summary>
        public int? KFCType
        {
            get { return kfcType; }
            set { SetValue<int?>("KFCType", ref kfcType, value); }
        }

        private bool? isBackOrder;
        public bool? IsBackOrder
        {
            get { return isBackOrder; }
            set { SetValue<bool?>("IsBackOrder", ref isBackOrder, value); }
        }

        private string customerIPAddress;
        /// <summary>
        /// 客户客户IP地址
        /// </summary>
        public string CustomerIPAddress
        {
            get { return customerIPAddress; }
            set { SetValue<string>("CustomerIPAddress", ref customerIPAddress, value); }
        }
        private SOType? soType;
        /// <summary>
        /// 订单类型
        /// </summary>
        public SOType? SOType
        {
            get { return soType; }
            set { SetValue<SOType?>("SOType", ref soType, value); }
        }

        /// <summary>
        /// 原订单编号
        /// </summary>
        public int? ContractSOSysNo
        {
            get;
            set;
        }

        private int? outSubStockSysNo;
        /// <summary>
        /// 断货分仓
        /// </summary
        public int? OutSubStockSysNo
        {
            get { return outSubStockSysNo; }
            set { SetValue<int?>("OutSubStockSysNo", ref outSubStockSysNo, value); }
        }
        private string promotionCodeSysNo;
        /// <summary>
        /// 优惠券系统编号
        /// </summary>
        public string PromotionCodeSysNo
        {
            get { return promotionCodeSysNo; }
            set { SetValue("PromotionCodeSysNo", ref promotionCodeSysNo, value); }
        }

        private int? fpStatus;
        public int? FPStatus
        {
            get { return fpStatus; }
            set { SetValue<int?>("FPStatus", ref fpStatus, value); }
        }

        private string simCardStatus;
        [Obsolete("此字段已弃用", true)]
        public string SIMCardStatus
        {
            get { return simCardStatus; }
            set { SetValue<string>("SIMCardStatus", ref simCardStatus, value); }
        }

        private int? isInputContractNumber;
        /// <summary>
        /// 是否已经录入16位合同号
        /// </summary>
        public int? IsInputContractNumber
        {
            get { return isInputContractNumber; }
            set { SetValue<int?>("IsInputContractNumber", ref isInputContractNumber, value); }
        }

        private int? merchantSysNo;
        /// <summary>
        /// 商家编号
        /// </summary>
        public int? MerchantSysNo
        {
            get { return merchantSysNo; }
            set { SetValue<int?>("MerchantSysNo", ref merchantSysNo, value); }
        }

        private StockType? stockType;
        /// <summary>
        /// 仓储类型
        /// </summary>
        public StockType? StockType
        {
            get { return stockType; }
            set { SetValue<StockType?>("StockType", ref stockType, value); }
        }
        private ECCentral.BizEntity.Invoice.DeliveryType? deliveryType;
        /// <summary>
        /// 配送类型
        /// </summary>
        public ECCentral.BizEntity.Invoice.DeliveryType? DeliveryType
        {
            get { return deliveryType; }
            set { SetValue<ECCentral.BizEntity.Invoice.DeliveryType?>("DeliveryType", ref deliveryType, value); }
        }
        private InvoiceType? invoiceType;
        /// <summary>
        /// 发票类型
        /// </summary>
        public InvoiceType? InvoiceType
        {
            get { return invoiceType; }
            set { SetValue<InvoiceType?>("InvoiceType", ref invoiceType, value); }
        }

        private string synSOSysNo;
        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string SynSOSysNo
        {
            get { return synSOSysNo; }
            set { SetValue<string>("SynSOSysNo", ref synSOSysNo, value); }
        }
        private string synSOType;
        /// <summary>
        /// 第三方订单类型
        /// </summary>
        public string SynSOType
        {
            get { return synSOType; }
            set { SetValue<string>("SynSOType", ref synSOType, value); }
        }

        private int? outStockUserSysNo;
        /// <summary>
        /// 出库人系统编号
        /// </summary>
        public int? OutStockUserSysNo
        {
            get { return outStockUserSysNo; }
            set { SetValue<int?>("OutStockUserSysNo", ref outStockUserSysNo, value); }
        }

        private string companyCode;
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode
        {
            get { return companyCode; }
            set { SetValue<string>("CompanyCode", ref companyCode, value); }
        }
        private string storeCompanyCode;
        public string StoreCompanyCode
        {
            get { return storeCompanyCode; }
            set { SetValue<string>("StoreCompanyCode", ref storeCompanyCode, value); }
        }
        private string webChannelID;
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string WebChannelID
        {
            get { return webChannelID; }
            set { SetValue<string>("WebChannelID", ref webChannelID, value); }
        }

        private bool? isVAT;
        /// <summary>
        /// 是否需开增票
        /// </summary>
        public bool? IsVAT
        {

            get { return isVAT; }
            set { SetValue<bool?>("IsVAT", ref isVAT, value); }
        }
        private bool? vatIsPrinted;
        /// <summary>
        /// 是否已开增票
        /// </summary>
        public bool? VATIsPrinted
        {
            get { return vatIsPrinted; }
            set { SetValue<bool?>("VATIsPrinted", ref vatIsPrinted, value); }
        }
        private bool? isVIP;
        /// <summary>
        /// 是否是VIP订单
        /// </summary>
        public bool? IsVIP
        {
            get { return isVIP; }
            set { SetValue<bool?>("IsVIP", ref isVIP, value); }
        }

        private bool? includeFailedGroupBuyingProduct;
        /// <summary>
        /// 是否包括团购失败的商品
        /// </summary>
        public bool? IncludeFailedGroupBuyingProduct
        {
            get { return includeFailedGroupBuyingProduct; }
            set { SetValue<bool?>("IncludeFailedGroupBuyingProduct", ref includeFailedGroupBuyingProduct, value); }
        }
        private string fromLinkSource;
        public string FromLinkSource
        {
            get { return fromLinkSource; }
            set { SetValue<string>("FromLinkSource", ref fromLinkSource, value); }
        }
        private int? searchMode;
        /// <summary>
        /// 查询模式
        /// </summary>
        public int? SearchMode
        {
            get { return searchMode; }
            set { SetValue<int?>("SearchMode", ref searchMode, value); }
        }

        private string shoppingCartNo;
        /// <summary>
        /// 购物车编号
        /// </summary>
        [Validate(ValidateType.Regex, @"^[\d+]{1,10}$", ErrorMessageResourceName = "Msg_ShoppingCartNo_Format", ErrorMessageResourceType = typeof(ResSO))]
        public string ShoppingCartNo
        {
            get { return shoppingCartNo; }
            set { this.SetValue("ShoppingCartNo", ref shoppingCartNo, value); }
        }

        private string membershipCard;
        /// <summary>
        /// 礼品卡
        /// </summary>
        public string MembershipCard
        {
            get { return membershipCard; }
            set { this.SetValue("MembershipCard", ref membershipCard, value); }
        }
        private string productName;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get { return productName; }
            set { this.SetValue("ProductName", ref productName, value); }
        }

        private DateTime? inputTime;
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? InputTime
        {
            get { return inputTime; }
            set { this.SetValue("InputTime", ref inputTime, value); }
        }

        #endregion

        #region 页面条件绑定数据源
        private List<KeyValuePair<SOStatus?, string>> soStatusList;
        /// <summary>
        /// 订单状态列表
        /// </summary>
        public List<KeyValuePair<SOStatus?, string>> SOStatusList
        {
            get
            {
                soStatusList = soStatusList ?? EnumConverter.GetKeyValuePairs<SOStatus>(EnumConverter.EnumAppendItemType.All);
                return soStatusList;
            }
        }

        private List<KeyValuePair<SOType?, string>> soTypeList;
        /// <summary>
        /// 订单类型列表
        /// </summary>
        public List<KeyValuePair<SOType?, string>> SOTypeList
        {
            get
            {
                soTypeList = soTypeList ?? EnumConverter.GetKeyValuePairs<SOType>(EnumConverter.EnumAppendItemType.All);
                KeyValuePair<Nullable<SOType>, string> item = new KeyValuePair<Nullable<SOType>, string>(BizEntity.SO.SOType.ElectronicCard, "电子卡");
                soTypeList.Remove(item);
                return soTypeList;
            }
        }

        private List<KeyValuePair<bool?, string>> booleanList;
        /// <summary>
        /// Boolean类型列表
        /// </summary>
        public List<KeyValuePair<bool?, string>> BooleanList
        {
            get
            {
                booleanList = booleanList ?? BooleanConverter.GetKeyValuePairs(EnumConverter.EnumAppendItemType.All);
                return booleanList;
            }
        }

        private List<KeyValuePair<ECCentral.BizEntity.Invoice.StockType?, string>> stockTypeList;
        /// <summary>
        /// 仓储类型列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.Invoice.StockType?, string>> StockTypeList
        {
            get
            {
                stockTypeList = stockTypeList ?? EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Invoice.StockType>(EnumConverter.EnumAppendItemType.All);
                return stockTypeList;
            }
        }
        private List<KeyValuePair<ECCentral.BizEntity.Invoice.DeliveryType?, string>> deliveryTypeList;
        /// <summary>
        /// 配送类型列表
        /// </summary>
        public List<KeyValuePair<ECCentral.BizEntity.Invoice.DeliveryType?, string>> DeliveryTypeList
        {
            get
            {
                deliveryTypeList = deliveryTypeList ?? EnumConverter.GetKeyValuePairs<ECCentral.BizEntity.Invoice.DeliveryType>(EnumConverter.EnumAppendItemType.All);

                return deliveryTypeList;
            }
        }
        private List<KeyValuePair<InvoiceType?, string>> invoiceTypeList;
        /// <summary>
        /// 发票类型列表
        /// </summary>
        public List<KeyValuePair<InvoiceType?, string>> InvoiceTypeList
        {
            get
            {
                invoiceTypeList = invoiceTypeList ?? EnumConverter.GetKeyValuePairs<InvoiceType>(EnumConverter.EnumAppendItemType.All);
                return invoiceTypeList;
            }
        }

        /// <summary>
        /// 通道列表
        /// </summary>
        public List<UIWebChannel> WebChannelList
        {
            get
            {
                List<UIWebChannel> list = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
                list.Insert(0, new UIWebChannel
                {
                    ChannelID = null,
                    ChannelName = ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All,
                });
                return list;
            }

        }
        public List<KeyValuePair<SOIncomeStatus?, string>> soIncomeStatusList;
        /// <summary>
        /// 通道列表
        /// </summary>
        public List<KeyValuePair<SOIncomeStatus?, string>> SOIncomeStatusList
        {
            get
            {
                soIncomeStatusList = soIncomeStatusList ?? EnumConverter.GetKeyValuePairs<SOIncomeStatus>(EnumConverter.EnumAppendItemType.All);
                soIncomeStatusList.RemoveAll(item => { return item.Key == SOIncomeStatus.Abandon; });
                soIncomeStatusList.Insert(1, new KeyValuePair<SOIncomeStatus?, string>((SOIncomeStatus)int.MinValue, ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__Paied));
                soIncomeStatusList.Add(new KeyValuePair<SOIncomeStatus?, string>(SOIncomeStatus.Abandon, ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay));
                return soIncomeStatusList;
            }
        }




        public List<KeyValuePair<NetPayStatus?, string>> netPayStatusList;
        /// <summary>
        /// 通道列表
        /// </summary>
        public List<KeyValuePair<NetPayStatus?, string>> NetPayStatusList
        {
            get
            {
                netPayStatusList = netPayStatusList ?? EnumConverter.GetKeyValuePairs<NetPayStatus>(EnumConverter.EnumAppendItemType.All);
                netPayStatusList.RemoveAll(item => { return item.Key == ECCentral.BizEntity.Invoice.NetPayStatus.Abandon; });
                netPayStatusList.Add(new KeyValuePair<NetPayStatus?, string>(ECCentral.BizEntity.Invoice.NetPayStatus.Abandon, ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay));
                return netPayStatusList;
            }

        }
        private List<CodeNamePair> fpStatusList;
        /// <summary>
        /// FP状态列表
        /// </summary>
        public List<CodeNamePair> FPStatusList
        {
            get { return fpStatusList; }
            set { SetValue<List<CodeNamePair>>("FPStatusList", ref fpStatusList, value); }
        }

        private List<CodeNamePair> kfcTypeList;
        /// <summary>
        /// KFC状态列表
        /// </summary>
        public List<CodeNamePair> KFCTypeList
        {
            get { return kfcTypeList; }
            set { SetValue<List<CodeNamePair>>("KFCTypeList", ref kfcTypeList, value); }
        }

        private List<CodeNamePair> timeRangeList;
        /// <summary>
        /// 时间段列表
        /// </summary>
        public List<CodeNamePair> TimeRangeList
        {
            get { return timeRangeList; }
            set { SetValue<List<CodeNamePair>>("TimeRangeList", ref timeRangeList, value); }
        }
        private List<StockInfo> stockList;
        /// <summary>
        /// 仓库列表
        /// </summary>
        public List<StockInfo> StockList
        {
            get { return stockList; }
            set { SetValue<List<StockInfo>>("StockList", ref stockList, value); }
        }
        private List<CodeNamePair> searchModeList;
        /// <summary>
        /// 查询方式列表
        /// </summary>
        public List<CodeNamePair> SearchModeList
        {
            get { return searchModeList; }
            set { SetValue<List<CodeNamePair>>("SearchModeList", ref searchModeList, value); }
        }
        #endregion

    }

    public class SOQueryDataVM : ModelBase
    {
        private bool isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                base.SetValue<bool>("IsChecked", ref isChecked, value);
            }
        }
        private bool isEnableQueryPayFlow;

        public bool IsEnableQueryPayFlow
        {
            get 
            {
                if (PayTypeSysNo.Equals(111) || PayTypeSysNo.Equals(114))
                    return true;
                else
                    return false; 
            }
            set
            {
                base.SetValue<bool>("IsEnableQueryPayFlow", ref isEnableQueryPayFlow, value);
            }
        }

        private int? sysNo;
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SysNo
        {
            get { return sysNo; }
            set { sysNo = value; }
        }

        private string soID;
        /// <summary>
        /// 订单编号
        /// </summary>
        public string SOID
        {
            get { return soID; }
            set { soID = value; }
        }
        private SOStatus? status;
        /// <summary>
        /// 系统状态
        /// </summary>
        public SOStatus? Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 订单支付状态
        /// </summary>
        public SOIncomeStatus? SOIncomeStatus
        {
            get;
            set;
        }
        public SOType? SOType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否可以处理订单，虚拟团购订单无法处理
        /// </summary>
        public bool IsProcessor
        {
            get 
            {
                if (this.SOType == ECCentral.BizEntity.SO.SOType.VirualGroupBuy)
                {
                    return false;
                }
                return true;
            }
            set
            {
                IsProcessor = value;
            }
        }

        public string SOIncomeStatusText
        {
            get
            {
                if (SOIncomeStatus == null || SOIncomeStatus == BizEntity.Invoice.SOIncomeStatus.Abandon)
                {
                    if (NetPayStatus.HasValue && NetPayStatus == BizEntity.Invoice.NetPayStatus.Origin)
                    {
                        return ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__Paied;
                    }
                    else
                    {
                        return ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay;
                    }
                }
                return SOIncomeStatus.ToDescription();
            }
        }

        private int? customerSysNo;
        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo
        {
            get { return customerSysNo; }
            set { customerSysNo = value; }
        }

        private string customerID;
        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }

        private string receiveContact;
        /// <summary>
        /// 收件人名称
        /// </summary>
        public string ReceiveContact
        {
            get { return receiveContact; }
            set { receiveContact = value; }
        }

        private string receiveMobilePhone;
        /// <summary>
        /// 收件人手机
        /// </summary>
        public string ReceiveMobilePhone
        {
            get { return receiveMobilePhone; }
            set { receiveMobilePhone = value; }
        }
        private string receivePhone;
        /// <summary>
        /// 收件人电话
        /// </summary>
        public string ReceivePhone
        {
            get { return receivePhone; }
            set { receivePhone = value; }
        }
        private decimal? soAmount;
        /// <summary>
        /// 订单总额
        /// </summary>
        public decimal? SOAmount
        {
            get { return soAmount; }
            set { soAmount = value; }
        }

        private decimal? discountAmount;
        /// <summary>
        /// 订单折扣总额
        /// </summary>
        public decimal? DiscountAmount
        {
            get { return discountAmount; }
            set { discountAmount = value; }
        }

        private int? pointPay;
        /// <summary>
        /// 积分支付
        /// </summary>
        public int? PointPay
        {
            get { return pointPay; }
            set { pointPay = value; }
        }

        private int? point;
        /// <summary>
        /// 获得积分
        /// </summary>
        public int? Point
        {
            get { return point; }
            set { point = value; }
        }

        private DateTime? orderTime;
        /// <summary>
        /// 订单时间
        /// </summary>
        public DateTime? OrderTime
        {
            get { return orderTime; }
            set { orderTime = value; }
        }
        private DateTime? auditTime;
        /// <summary>
        /// 订单审核时间
        /// </summary>
        public DateTime? AuditTime
        {
            get { return auditTime; }
            set { auditTime = value; }
        }
        private DateTime? outStockTime;
        /// <summary>
        /// 订单出库时间
        /// </summary>
        public DateTime? OutStockTime
        {
            get { return outStockTime; }
            set { outStockTime = value; }
        }
        private int? payTypeSysNo;
        /// <summary>
        /// 订单支付类型编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get { return payTypeSysNo; }
            set { payTypeSysNo = value; }
        }
        private string payTypeName;
        /// <summary>
        /// 订单支付类型名称 
        /// </summary>
        public string PayTypeName
        {
            get { return payTypeName; }
            set { payTypeName = value; }
        }
        private ECCentral.BizEntity.Invoice.NetPayStatus? netPayStatus;
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public ECCentral.BizEntity.Invoice.NetPayStatus? NetPayStatus
        {
            get { return netPayStatus; }
            set { netPayStatus = value; }
        }

        private ECCentral.BizEntity.Invoice.PostPayStatus? postPayStatus;
        /// <summary>
        /// 订单支付状态
        /// </summary>
        public ECCentral.BizEntity.Invoice.PostPayStatus? PostPayStatus
        {
            get { return postPayStatus; }
            set { postPayStatus = value; }
        }


        public string PayStatusText
        {
            get
            {
                return NetPayStatus == null ? ECCentral.BizEntity.Enum.Resources.ResSOEnum.SOPayStatus__NotPay : NetPayStatus.ToDescription();
            }
        }
        private bool? isPhoneOrder;
        /// <summary>
        /// 是否通过电话订购
        /// </summary>
        public bool? IsPhoneOrder
        {
            get { return isPhoneOrder; }
            set { isPhoneOrder = value; }
        }
        private string webChannelID;
        /// <summary>
        /// 所属通道编号
        /// </summary>
        public string WebChannelID
        {
            get { return webChannelID; }
            set { webChannelID = value; }
        }
        private string webChannelName;
        /// <summary>
        /// 所属通道名称
        /// </summary>
        public string WebChannelName
        {
            get { return webChannelName; }
            set { webChannelName = value; }
        }

        private string shoppingCartNo;
        /// <summary>
        /// 购物车编号
        /// </summary>
        public string ShoppingCartNo
        {
            get { return shoppingCartNo; }
            set { shoppingCartNo = value; }
        }

        public DateTime? InputTime { get; set; }

        public string AuditUserName { get; set; }

        #region 社团
        private string societyID;
        /// <summary>
        /// 社团ID
        /// </summary>
        public string SocietyID
        {
            get { return societyID; }
            set { societyID = value; }
        }
        private decimal? commissionAmount;
        /// <summary>
        /// 订单返佣总额
        /// </summary>
        public decimal? CommissionAmount
        {
            get { return commissionAmount; }
            set { commissionAmount = value; }
        }
        #endregion
    }

    public class SOQueryView : ModelBase
    {
        public SOQueryVM QueryInfo { get; set; }
        private List<SOQueryDataVM> result;
        public List<SOQueryDataVM> Result
        {
            get { return result; }
            set { SetValue<List<SOQueryDataVM>>("Result", ref result, value); }
        }
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set { SetValue<int>("TotalCount", ref totalCount, value); }
        }

        public SOQueryView()
        {
            QueryInfo = new SOQueryVM()
            {
                IncludeHistory = false,
                IncludeFailedGroupBuyingProduct = false
            };
        }
    }
}
