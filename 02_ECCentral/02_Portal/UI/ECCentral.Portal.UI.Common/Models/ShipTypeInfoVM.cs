using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.Common
{
    public class ShipTypeInfoVM : ModelBase
    {
        public ShipTypeInfoVM()
        {

            //是否前台显示
            this.ListIsOnLineShow = EnumConverter.GetKeyValuePairs<HYNStatus>();
            //是否专用配送
            this.ListIsSpecified = EnumConverter.GetKeyValuePairs<IsSpecial>();
            //是否24小时配送
            this.ListDeliveryPromise = EnumConverter.GetKeyValuePairs<DeliveryStatusFor24H>();
            //服务时限
            this.ListDeliveryType = EnumConverter.GetKeyValuePairs<ShipDeliveryType>();
            //是否收取包裹费
            this.ListIsWithPackFee = EnumConverter.GetKeyValuePairs<SYNStatus>();

            this.ListPackStyle = EnumConverter.GetKeyValuePairs<ShippingPackStyle>();
            //配送方式类型
            this.ListShipTypeEnum=EnumConverter.GetKeyValuePairs<ShippingTypeEnum>();

            this.ListStoreType = EnumConverter.GetKeyValuePairs<StoreType>();
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? _sysNo;
        public int? SysNo
        {
            get { return _sysNo; }
            set { SetValue("SysNo", ref _sysNo, value); }
        }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? _companyCode;
        public int? CompanyCode
        {
            get { return _companyCode; }
            set { SetValue("CompanyCode", ref _companyCode, value); }
        }

        /// <summary>
        /// 渠道编号
        /// </summary>
        private string _channelID;
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                return CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            }
        }

        /// <summary>
        /// 配送方式ID
        /// </summary>
        public string _shipTypeID;
        [Validate(ValidateType.Required)]
        public string ShipTypeID
        {
            get { return _shipTypeID; }
            set { SetValue("ShipTypeID", ref _shipTypeID, value); }
        }
        /// <summary>
        /// 配送方式名称
        /// </summary>
        public string _shippingTypeName;
        [Validate(ValidateType.Required)]
        public string ShippingTypeName
        {
            get { return _shippingTypeName; }
            set { SetValue("ShippingTypeName", ref _shippingTypeName, value); }
        }
        /// <summary>
        /// 配送周期
        /// </summary>
        public string _period;
        [Validate(ValidateType.Required)]
        public string Period
        {
            get { return _period; }
            set { SetValue("Period", ref _period, value); }
        }
        /// <summary>
        /// 提供方
        /// </summary>
        public string _provider;
        [Validate(ValidateType.Required)]
        public string Provider
        {
            get { return _provider; }
            set { SetValue("Provider", ref _provider, value); }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string _shipTypeDesc;
        [Validate(ValidateType.Required)]
        public string ShipTypeDesc
        {
            get { return _shipTypeDesc; }
            set { SetValue("ShipTypeDesc", ref _shipTypeDesc, value); }
        }
        /// <summary>
        /// 运费费率
        /// </summary>
        public string _premiumRate;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(1|1\.[0]*|0?\.(?!0+$)[\d]+)$")]
        public string PremiumRate
        {
            get { return _premiumRate; }
            set { SetValue("PremiumRate", ref _premiumRate, value); }

        }
        /// <summary>
        /// 免保价金额
        /// </summary>
        public string _premiumBase;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$")]
        public string PremiumBase
        {
            get { return _premiumBase; }
            set { SetValue("PremiumBase", ref _premiumBase, value); }
        }
        /// <summary>
        /// 赔付金额上限
        /// </summary>
        public string _compensationLimit;
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$")]
        public string CompensationLimit
        {
            get { return _compensationLimit; }
            set { SetValue("CompensationLimit", ref _compensationLimit, value); }
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public string _orderNumber;
        [Validate(ValidateType.Required)]
        public string OrderNumber
        {
            get { return _orderNumber; }
            set { SetValue("OrderNumber", ref _orderNumber, value); }
        }
        /// <summary>
        /// 是否前台显示
        /// </summary>
        public HYNStatus? _isOnLineShow;
        [Validate(ValidateType.Required)]
        public HYNStatus? IsOnLineShow
        {
            get { return _isOnLineShow; }
            set { SetValue("IsOnLineShow", ref _isOnLineShow, value); }

        }
        /// <summary>
        /// 前台显示名称
        /// </summary>
        public string _displayShipName;
        [Validate(ValidateType.Required)]
        public string DisplayShipName
        {
            get { return _displayShipName; }
            set { SetValue("DisplayShipName", ref _displayShipName, value); }

        }
        /// <summary>
        /// 是否收取包裹费
        /// </summary>
        public SYNStatus? _isWithPackFee;
        [Validate(ValidateType.Required)]
        public SYNStatus? IsWithPackFee
        {
            get { return _isWithPackFee; }
            set { SetValue("IsWithPackFee", ref _isWithPackFee, value); }

        }
        /// <summary>
        /// 配送方式名称简称
        /// </summary>
        public string _shortName;
        [Validate(ValidateType.Required)]
        public string ShortName
        {
            get { return _shortName; }
            set { SetValue("ShortName", ref _shortName, value); }

        }
        /// <summary>
        /// DS(并单)
        /// </summary>
        public int? _dsSysNo;
        [Validate(ValidateType.Interger)]
        public int? DsSysNo
        {
            get { return _dsSysNo; }
            set { SetValue("DsSysNo", ref _dsSysNo, value); }

        }
        /// <summary>
        /// 配送方式类型
        /// </summary>
        public ShippingTypeEnum? _shipTypeEnum;
        public ShippingTypeEnum? ShipTypeEnum
        {
            get { return _shipTypeEnum; }
            set { SetValue("ShipTypeEnum", ref _shipTypeEnum, value); }

        }
        /// <summary>
        /// 本地仓库
        /// </summary>
        public int? _onlyForStockSysNo;
        public int? OnlyForStockSysNo
        {
            get { return _onlyForStockSysNo; }
            set { SetValue("OnlyForStockSysNo", ref _onlyForStockSysNo, value); }

        }
        /// <summary>
        /// 免运费金额
        /// </summary>
        public decimal _freeShipBase;
        [Validate(ValidateType.Required)]
        public decimal FreeShipBase
        {
            get { return _freeShipBase; }
            set { SetValue("FreeShipBase", ref _freeShipBase, value); }

        }
        /// <summary>
        /// 打包材料
        /// </summary>
        public ShippingPackStyle? _packStyle;
        public ShippingPackStyle? PackStyle
        {
            get { return _packStyle; }
            set { SetValue("PackStyle", ref _packStyle, value); }

        }
        /// <summary>
        /// 是否专用配送方式
        /// </summary>
        public IsSpecial? _isSpecified;
        public IsSpecial? IsSpecified
        {
            get { return _isSpecified; }
            set { SetValue("IsSpecified", ref _isSpecified, value); }

        }
        /// <summary>
        /// 是否提供24小时配送
        /// </summary>
        public DeliveryStatusFor24H? _deliveryPromise;
        public DeliveryStatusFor24H? DeliveryPromise
        {
            get { return _deliveryPromise; }
            set { SetValue("DeliveryPromise", ref _deliveryPromise, value); }

        }
        /// <summary>
        /// 服务时限
        /// </summary>
        public ShipDeliveryType? _deliveryType;
        public ShipDeliveryType? DeliveryType
        {
            get { return _deliveryType; }
            set { SetValue("DeliveryType", ref _deliveryType, value); }

        }

        public string _availsection;
        [Validate(ValidateType.Interger)]
        public string Availsection
        {
            get { return _availsection; }
            set { SetValue("Availsection", ref _availsection, value); }

        }

        public string _intervalDays;
        [Validate(ValidateType.Interger)]
        public string IntervalDays
        {
            get { return _intervalDays; }
            set { SetValue("IntervalDays", ref _intervalDays, value); }

        }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string _contactPhoneNumber;
        [Validate(ValidateType.Phone)]
        public string ContactPhoneNumber
        {
            get { return _contactPhoneNumber; }
            set { SetValue("ContactPhoneNumber", ref _contactPhoneNumber, value); }

        }
        /// <summary>
        /// 公司网址
        /// </summary>
        public string _officialWebsite;
        [Validate(ValidateType.URL)]
        public string OfficialWebsite
        {
            get { return _officialWebsite; }
            set { SetValue("OfficialWebsite", ref _officialWebsite, value); }

        }
        /// <summary>
        /// 配送方式扩展
        /// </summary>
        //public ShipType_Ex _shipType_Ex;
        //public ShipType_Ex ShipType_Ex
        //{
        //    get { return _shipType_Ex; }
        //    set { SetValue("ShipType_Ex", ref _shipType_Ex, value); }

        //}
        //地区编号
        public int? _areaSysNo;
        [Validate(ValidateType.Required)]
        public int? AreaSysNo 
        {
            get { return _areaSysNo; }
            set { SetValue("AreaSysNo",ref _areaSysNo,value);}
        }
        //自提点联系人
        public string _contactName;
        public string ContactName
        {
            get { return _contactName; }
            set { SetValue("ContactName", ref _contactName, value); }
        }
        //自提点联系电话
        public string _phone;
        [Validate(ValidateType.Phone)]
        public string Phone
        {
            get { return _phone; }
            set { SetValue("Phone", ref _phone, value); }
        }
        //邮箱
        public string _email;
        [Validate(ValidateType.Email)]
        public string Email
        {
            get { return _email; }
            set { SetValue("Email", ref _email, value); }
        }
        //自提点地址
        public string _address;
        public string Address
        {
            get { return _address; }
            set { SetValue("Address", ref _address, value); }
        }

        /// <summary>
        /// 存储方式
        /// </summary>
        public StoreType? _storeType;
        [Validate(ValidateType.Required)]
        public StoreType? StoreType
        {
            get { return _storeType; }
            set { SetValue("StoreType", ref _storeType, value); }

        }

        /// <summary>
        /// List区域
        /// </summary>
        public List<KeyValuePair<HYNStatus?, string>> ListIsOnLineShow { get; set; }
        public List<KeyValuePair<SYNStatus?, string>> ListIsWithPackFee { get; set; }
        public List<KeyValuePair<IsSpecial?, string>> ListIsSpecified { get; set; }
        public List<KeyValuePair<DeliveryStatusFor24H?,string>> ListDeliveryPromise { get; set; }
        public List<KeyValuePair<ShipDeliveryType?, string>> ListDeliveryType { get; set; }
        public List<KeyValuePair<ShippingTypeEnum?, string>> ListShipTypeEnum { get; set; }
        public List<KeyValuePair<ShippingPackStyle?, string>> ListPackStyle { get; set; }

        //UI 属性
        public Visibility ShipType_ExVisibility
        {
            get 
            {
                return ShipTypeEnum == ShippingTypeEnum.SelfGetInCity? Visibility.Visible:Visibility.Collapsed;
            }
        }

        public List<KeyValuePair<StoreType?, string>> ListStoreType { get; set; }
    }

}
