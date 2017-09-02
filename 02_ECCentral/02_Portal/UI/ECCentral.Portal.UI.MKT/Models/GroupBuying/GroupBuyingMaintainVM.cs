using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class GroupBuyingMaintainVM : ModelBase
    {
        public GroupBuyingMaintainVM()
        {
            //this.ChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            //this.ChannelID = this.ChannelList.FirstOrDefault().ChannelID;
            this.GroupBuyingTypeList = new Dictionary<int, String>();
            this.GroupBuyingAreaList = new Dictionary<int, String>();
            this.CategoryTypeList = EnumConverter.GetKeyValuePairs<GroupBuyingCategoryType>(EnumConverter.EnumAppendItemType.None);
            this.CategoryType = GroupBuyingCategoryType.Physical;
            this.VendorStoreList = new ObservableCollection<VendorStoreVM>();
            this.GroupBuyingCategoryList = new ObservableCollection<GroupBuyingCategoryVM>();
            this._groupBuyingPicUrl = string.Empty;
            this._groupBuyingMiddlePicUrl = string.Empty;
        }        

        private string selectedVendorStoreSysNo;
        public string SelectedVendorStoreSysNo
        {
            get
            {
                return selectedVendorStoreSysNo;
            }
            set
            {
                SetValue("SelectedVendorStoreSysNo",ref selectedVendorStoreSysNo,value);
            }
        }

        #region 业务属性
        public int? SysNo { get; set; }
        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        [Validate(ValidateType.Required)]
        public string CompanyCode
        {
            get
            {
                return _companyCode;
            }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private int _groupBuyingTypeSysNo;
        /// <summary>
        /// 团购类型系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int GroupBuyingTypeSysNo
        {
            get { return _groupBuyingTypeSysNo; }
            set
            {
                base.SetValue("GroupBuyingTypeSysNo", ref _groupBuyingTypeSysNo, value);
            }
        }
        private string _groupBuyingTypeName;
        /// <summary>
        /// 团购类型名
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupBuyingTypeName
        {
            get { return _groupBuyingTypeName; }
            set
            {
                base.SetValue("GroupBuyingTypeName", ref _groupBuyingTypeName, value);
            }
        }
        private int _groupBuyingAreaSysNo;
        /// <summary>
        /// 团购地区系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int GroupBuyingAreaSysNo
        {
            get { return _groupBuyingAreaSysNo; }
            set
            {
                base.SetValue("GroupBuyingAreaSysNo", ref _groupBuyingAreaSysNo, value);
            }
        }
        private string _groupBuyingAreaName;
        /// <summary>
        /// 团购地区名
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupBuyingAreaName
        {
            get { return _groupBuyingAreaName; }
            set
            {
                base.SetValue("GroupBuyingAreaName", ref _groupBuyingAreaName, value);
            }
        }

        private int _groupBuyingVendorSysNo;
        /// <summary>
        /// 商家系统编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int GroupBuyingVendorSysNo
        {
            get { return _groupBuyingVendorSysNo; }
            set
            {
                base.SetValue("GroupBuyingVendorSysNo", ref _groupBuyingVendorSysNo, value);
            }
        }
        private string _groupBuyingVendorName;
        /// <summary>
        /// 商家名
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupBuyingVendorName
        {
            get
            {
                return string.IsNullOrEmpty(_groupBuyingVendorName) ? "泰隆优选" : _groupBuyingVendorName;
            }
            set
            {
                base.SetValue("GroupBuyingVendorName", ref _groupBuyingVendorName, value);
            }
        }

        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public string ChannelID
        {
            get
            {
                return _channelID;
            }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }
        private string _productID;
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ProductID
        {
            get
            {
                return _productID;
            }
            set
            {
                base.SetValue("ProductID", ref _productID, value);
            }
        }
        private string _productSysNo;
        /// <summary>
        /// 商品系统编号
        /// </summary>
        [Validate(ValidateType.Interger)]
        public string ProductSysNo
        {
            get
            {
                return _productSysNo;
            }
            set
            {

                base.SetValue("ProductSysNo", ref _productSysNo, value);
            }
        }
        private string _groupBuyingTitle;
        /// <summary>
        /// 团购标题
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupBuyingTitle
        {
            get
            {
                return _groupBuyingTitle;
            }
            set
            {
                base.SetValue("GroupBuyingTitle", ref _groupBuyingTitle, value);
            }
        }
        private string _groupBuyingDesc;
        /// <summary>
        /// 团购简述
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupBuyingDesc
        {
            get
            {
                return _groupBuyingDesc;
            }
            set
            {
                base.SetValue("GroupBuyingDesc", ref _groupBuyingDesc, value);
            }
        }
        private string _groupBuyingDescLong;
        /// <summary>
        /// 团购详情描述
        /// </summary>
        [Validate(ValidateType.Required)]
        public string GroupBuyingDescLong
        {
            get
            {
                return _groupBuyingDescLong;
            }
            set
            {
                base.SetValue("GroupBuyingDescLong", ref _groupBuyingDescLong, value);
            }
        }
        private string _groupBuyingPicUrl;
        /// <summary>
        /// 团购商品促销大图
        /// </summary>
        [Validate(ValidateType.URL)]
        [Validate(ValidateType.Required)]
        public string GroupBuyingPicUrl
        {
            get
            {
                return _groupBuyingPicUrl;
            }
            set
            {
                base.SetValue("GroupBuyingPicUrl", ref _groupBuyingPicUrl, value);
            }
        }

        private string _groupBuyingMiddlePicUrl;
        /// <summary>
        /// 团购商品促销小图
        /// </summary>
        [Validate(ValidateType.URL)]
        [Validate(ValidateType.Required)]
        public string GroupBuyingMiddlePicUrl
        {
            get
            {
                return _groupBuyingMiddlePicUrl;
            }
            set
            {
                base.SetValue("GroupBuyingMiddlePicUrl", ref _groupBuyingMiddlePicUrl, value);
            }
        }

        private string _groupBuyingSmallPicUrl;
        /// <summary>
        /// 团购商品促销小图
        /// </summary>
        [Validate(ValidateType.URL)]
        [Validate(ValidateType.Required)]
        public string GroupBuyingSmallPicUrl
        {
            get
            {
                return _groupBuyingSmallPicUrl;
            }
            set
            {
                base.SetValue("GroupBuyingSmallPicUrl", ref _groupBuyingSmallPicUrl, value);
            }
        }
        private DateTime? _beginDate;
        /// <summary>
        /// 开始时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate
        {
            get
            {
                return _beginDate;
            }
            set
            {
                base.SetValue("BeginDate", ref _beginDate, value);
            }
        }
        private DateTime? _endDate;
        /// <summary>
        /// 结束时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                base.SetValue("EndDate", ref _endDate, value);
            }
        }
        private string _maxPerOrder;
        /// <summary>
        /// 每个团购订单最大可购买该商品数量
        /// </summary>        
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "每单限购必须是整数，且大于等于1")]
        [Validate(ValidateType.Required) ]
        public string MaxCountPerOrder
        {
            get
            {
                return _maxPerOrder;
            }
            set
            {
                base.SetValue("MaxCountPerOrder", ref _maxPerOrder, value);
            }
        }
        private int? _minPerOrder;
        /// <summary>
        /// 每个团购订单最小购买该商品数量
        /// </summary>
        public int? MinCountPerOrder
        {
            get
            {
                return _minPerOrder;
            }
            set
            {
                base.SetValue("MinCountPerOrder", ref _minPerOrder, value);
            }
        }
        private int _limitOrderCount;
        /// <summary>
        /// 每个客户可参团次数限制
        /// </summary>
        public int LimitOrderCount
        {
            get
            {
                return _limitOrderCount;
            }
            set
            {
                base.SetValue("LimitOrderCount", ref _limitOrderCount, value);
            }
        }

        private string _priority;
        /// <summary>
        /// 显示优先级
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]\d*$", ErrorMessage = "优先级必须是整数，且大于等于0")]
        public string Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                base.SetValue("Priority", ref _priority, value);
            }
        }

        private bool _isByGroup;
        /// <summary>
        /// 是否商品组团购
        /// </summary>
        public bool IsByGroup
        {
            get
            {
                return _isByGroup;
            }
            set
            {
                base.SetValue("IsByGroup", ref _isByGroup, value);
            }
        }

        private GroupBuyingStatus? m_Status;
        public GroupBuyingStatus? Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                base.SetValue("Status", ref m_Status, value);
            }

        }

        private string groupBuyingRules;
        [Validate(ValidateType.MaxLength, 500)]
        public string GroupBuyingRules
        {
            get
            {
                return groupBuyingRules;
            }
            set
            {
                SetValue("GroupBuyingRules", ref groupBuyingRules, value);
            }
        }

        private string groupBuyingReason;
        [Validate(ValidateType.MaxLength, 300)]
        public string GroupBuyingReason
        {
            get
            {
                return groupBuyingReason;
            }
            set
            {
                SetValue("GroupBuyingReason", ref groupBuyingReason, value);
            }
        }

        private int? _requestSysNo;
        /// <summary>
        /// SellerPortal团购编号
        /// </summary>
        [Validate(ValidateType.Required)]
        public int? RequestSysNo
        {
            get { return _requestSysNo; }
            set
            {
                base.SetValue("RequestSysNo", ref _requestSysNo, value);
            }
        }

        private GroupBuyingCategoryType? categoryType;
        /// <summary>
        /// 团购类型
        /// </summary>        
        public GroupBuyingCategoryType? CategoryType
        {
            get { return categoryType; }
            set
            {
                if(value== GroupBuyingCategoryType.Physical){
                    PhysicalVisibility = Visibility.Visible;
                    VirtualVisibility = Visibility.Collapsed;
                    ZeroLotteryVisibility = Visibility.Collapsed;
                }
                else if (value == GroupBuyingCategoryType.Virtual)
                {
                    PhysicalVisibility = Visibility.Collapsed;
                    VirtualVisibility = Visibility.Visible;
                    ZeroLotteryVisibility = Visibility.Collapsed;
                    IsByGroup = false;
                }
                else
                {
                    PhysicalVisibility = Visibility.Visible;
                    VirtualVisibility = Visibility.Collapsed;
                    ZeroLotteryVisibility = Visibility.Collapsed;            
                }
                base.SetValue("CategoryType", ref categoryType, value);
            }
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
                SetValue("VendorSysNo",ref vendorSysNo,value);
            }
        }

        private int? vendorStoreSysNo;
        public int? VendorStoreSysNo
        {
            get
            {
                return vendorStoreSysNo;
            }
            set
            {
                SetValue("VendorStoreSysNo", ref vendorStoreSysNo, value);
            }
        }

        private string couponValidDate;           
        public string CouponValidDate
        {
            get
            {
                return couponValidDate;
            }
            set
            {
                SetValue("CouponValidDate", ref couponValidDate, value);
            }
        }

        private string lotteryRule;
        [Validate(ValidateType.Required)]
        public string LotteryRule
        {
            get
            {
                return lotteryRule;
            }
            set
            {
                SetValue("LotteryRule", ref lotteryRule, value);
            }
        }

        private int? groupBuyingCategorySysNo;
        [Validate(ValidateType.Required)]
        public int? GroupBuyingCategorySysNo
        {
            get
            {
                return groupBuyingCategorySysNo;
            }
            set
            {
                SetValue("GroupBuyingCategorySysNo", ref groupBuyingCategorySysNo, value);
            }
        }

        private string price;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]|([1-9][0-9]{0,9}))((\.[0-9]{1,2})?))$", ErrorMessage = "请输入有效金额，大于等于0的数字，最多两位小数")]
        public string Price
        {
            get
            {
                return price;
            }
            set
            {
                SetValue("Price", ref price, value);
                this.GroupBuyingPrice1 = value;
                this.SellCount1 = "1";
            }
        }

        private string costAmt;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]|([1-9][0-9]{0,9}))((\.[0-9]{1,2})?))$", ErrorMessage = "请输入有效金额，大于等于0的数字，最多两位小数")]
        public string CostAmt
        {
            get
            {
                return costAmt;
            }
            set
            {
                SetValue("CostAmt", ref costAmt, value);               
            }
        }

        /// <summary>
        /// 免预约
        /// </summary>
        private bool isWithoutReservation;
        public bool IsWithoutReservation
        {
            get
            {
                return isWithoutReservation;
            }
            set
            {
                SetValue("IsWithoutReservation", ref isWithoutReservation, value);
            }
        }

        private bool isVouchers;
        public bool IsVouchers
        {
            get
            {
                return isVouchers;
            }
            set
            {
                SetValue("IsVouchers", ref isVouchers, value);
            }
        }

        #region 阶梯价格

        private decimal? m_OriginalPrice;
        /// <summary>
        /// 商品原价
        /// </summary>
        public decimal? OriginalPrice
        {
            get
            {
                return m_OriginalPrice;
            }
            set
            {
                base.SetValue("OriginalPrice", ref m_OriginalPrice, value);
            }

        }
        private decimal? m_BasicPrice;
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal? BasicPrice
        {
            get
            {
                return m_BasicPrice;
            }
            set
            {
                base.SetValue("BasicPrice", ref m_BasicPrice, value);
            }

        }

        private int? _groupBuyingPoint;
        /// <summary>
        /// 积分
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]\d*$", ErrorMessage = "积分必须是整数，且大于等于0")]
        public int? GroupBuyingPoint
        {
            get
            {
                return _groupBuyingPoint;
            }
            set
            {
                base.SetValue("GroupBuyingPoint", ref _groupBuyingPoint, value);
            }
        }


        private string _sellCount1;
        /// <summary>
        /// 阶梯数量1
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "阶梯数量必须是整数，且大于0")]
        public string SellCount1
        {
            get
            {
                return _sellCount1;
            }
            set
            {
                base.SetValue("SellCount1", ref _sellCount1, value);
            }
        }
        private string _groupBuyingPrice1;
        /// <summary>
        /// 阶梯价格1
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[0-9][0-9]*)|([0-9]*[0-9][0-9]*\.[0-9]+)|([0-9]*[0-9][0-9]*))$", ErrorMessage = "请输入大于等于0的数字")]
        public string GroupBuyingPrice1
        {
            get
            {
                return _groupBuyingPrice1;
            }
            set
            {
                base.SetValue("GroupBuyingPrice1", ref _groupBuyingPrice1, value);
            }
        }
        public int SysNo1 { get; set; }

        private string _sellCount2;
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "阶梯数量必须是整数，且大于0")]
        public string SellCount2
        {
            get
            {
                return _sellCount2;
            }
            set
            {
                base.SetValue("SellCount2", ref _sellCount2, value);
            }
        }
        private string _groupBuyingPrice2;
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[0-9][0-9]*)|([0-9]*[0-9][0-9]*\.[0-9]+)|([0-9]*[0-9][0-9]*))$", ErrorMessage = "请输入大于等于0的数字")]
        public string GroupBuyingPrice2
        {
            get
            {
                return _groupBuyingPrice2;
            }
            set
            {
                base.SetValue("GroupBuyingPrice2", ref _groupBuyingPrice2, value);
            }
        }
        public int SysNo2 { get; set; }

        private string _sellCount3;
        [Validate(ValidateType.Regex, @"^[1-9]\d*$", ErrorMessage = "阶梯数量必须是整数，且大于0")]
        public string SellCount3
        {
            get
            {
                return _sellCount3;
            }
            set
            {
                base.SetValue("SellCount3", ref _sellCount3, value);
            }
        }
        private string _groupBuyingPrice3;
        [Validate(ValidateType.Regex, @"^(([0-9]+\.[0-9]*[0-9][0-9]*)|([0-9]*[0-9][0-9]*\.[0-9]+)|([0-9]*[0-9][0-9]*))$", ErrorMessage = "请输入大于等于0的数字")]
        public string GroupBuyingPrice3
        {
            get
            {
                return _groupBuyingPrice3;
            }
            set
            {
                base.SetValue("GroupBuyingPrice3", ref _groupBuyingPrice3, value);
            }
        }
        public int SysNo3 { get; set; }

        #endregion 阶梯价格

        #endregion 业务属性

        #region 界面扩展属性

        public List<UIWebChannel> ChannelList { get; set; }

        public Dictionary<int, String> GroupBuyingTypeList { get; set; }

        public Dictionary<int, String> GroupBuyingAreaList { get; set; }

        private bool _isLimitOrderCountOneChecked;
        /// <summary>
        /// 限购一次
        /// </summary>
        public bool IsLimitOrderCountOneChecked
        {
            get
            {
                return this.LimitOrderCount == 1;
            }
            set
            {
                this.LimitOrderCount = value ? 1 : 0;
                base.SetValue("IsLimitOrderCountOneChecked", ref _isLimitOrderCountOneChecked, value);
            }
        }

        public List<KeyValuePair<GroupBuyingCategoryType?, string>> CategoryTypeList { get; set; }

        private Visibility physicalVisibility;
        public Visibility PhysicalVisibility
        {
            get
            {
                return physicalVisibility;
            }
            set
            {
                SetValue("PhysicalVisibility", ref physicalVisibility, value);
            }
        }

        private Visibility virtualVisibility;
        public Visibility VirtualVisibility
        {
            get
            {
                return virtualVisibility;
            }
            set
            {
                SetValue("VirtualVisibility", ref virtualVisibility, value);
            }
        }

        private Visibility zeroLotteryVisibility;
        public Visibility ZeroLotteryVisibility
        {
            get
            {
                return zeroLotteryVisibility;
            }
            set
            {
                SetValue("ZeroLotteryVisibility", ref zeroLotteryVisibility, value);
            }
        }

        #endregion 界面扩展属性

        #region 方法


        public List<PSPriceDiscountRule> ConvertPriceRank()
        {
            List<PSPriceDiscountRule> priceRankList = new List<PSPriceDiscountRule>(3);
            if (!string.IsNullOrEmpty(this.SellCount1)
                && !string.IsNullOrEmpty(this.GroupBuyingPrice1))
            {
                PSPriceDiscountRule rank = new PSPriceDiscountRule();
                rank.DiscountType = PSDiscountTypeForProductPrice.ProductPriceFinal;
                rank.MinQty = int.Parse(this.SellCount1);
                rank.DiscountValue = decimal.Parse(this.GroupBuyingPrice1);
                rank.ProductSysNo = this.SysNo1;
                priceRankList.Add(rank);
            }

            if (!string.IsNullOrEmpty(this.SellCount2)
               && !string.IsNullOrEmpty(this.GroupBuyingPrice2))
            {
                PSPriceDiscountRule rank = new PSPriceDiscountRule();
                rank.DiscountType = PSDiscountTypeForProductPrice.ProductPriceFinal;
                rank.MinQty = int.Parse(this.SellCount2);
                rank.DiscountValue = decimal.Parse(this.GroupBuyingPrice2);
                rank.ProductSysNo = this.SysNo2;
                priceRankList.Add(rank);
            }

            if (!string.IsNullOrEmpty(this.SellCount3)
               && !string.IsNullOrEmpty(this.GroupBuyingPrice3))
            {
                PSPriceDiscountRule rank = new PSPriceDiscountRule();
                rank.DiscountType = PSDiscountTypeForProductPrice.ProductPriceFinal;
                rank.MinQty = int.Parse(this.SellCount3);
                rank.DiscountValue = decimal.Parse(this.GroupBuyingPrice3);
                rank.ProductSysNo = this.SysNo3;
                priceRankList.Add(rank);
            }
            return priceRankList;
        }

        #endregion 方法

        public ObservableCollection<VendorStoreVM> VendorStoreList { get; set; }

        public ObservableCollection<GroupBuyingCategoryVM> GroupBuyingCategoryList { get; set; }

        public List<int> VendorStoreSysNoList { get; set; }

        public bool HasGroupBuyingApprovePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.MKT_GroupBuying_Approve); }
        }


    }

    public class VendorStoreVM : ModelBase
    {
        public int? SysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public string Name { get; set; }
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
    }
}