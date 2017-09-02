//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理QueryModels
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Common;
using System.Text.RegularExpressions;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductChannelVM : ModelBase
    {
        public List<KeyValuePair<ProductChannelInfoStatus?, string>> StatusList { get; set; }

        public ProductChannelVM()
        {
            this.StatusList = EnumConverter.GetKeyValuePairs<ProductChannelInfoStatus>(EnumConverter.EnumAppendItemType.None);
        }

        /// <summary>
        /// 
        /// </summary>
        public int? SysNo { get; set; }


        public ChannelVM ChannelInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 渠道商品ID
        /// </summary>
        public string SynProductID { get; set; }

        /// <summary>
        /// 淘宝sku码
        /// </summary>
        public string TaoBaoSku { get; set; }

        /// <summary>
        /// 可销售数量
        /// </summary>
        private string channelSellCount;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,3}$", ErrorMessageResourceType=typeof(ResProductChannelManagement),ErrorMessageResourceName="Error_InputZeroTo9999Int")]
        public string ChannelSellCount
        {
            get { return channelSellCount; }
            set { base.SetValue("ChannelSellCount", ref  channelSellCount, value); }
        }

        /// <summary>
        /// 安全库存
        /// </summary>
        private string safeInventoryQty;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[1-9]\d{0,3}$", ErrorMessageResourceType=typeof(ResProductChannelManagement),ErrorMessageResourceName="Error_InputZeroTo9999Int")]
        public string SafeInventoryQty
        {
            get { return safeInventoryQty; }
            set { base.SetValue("SafeInventoryQty", ref  safeInventoryQty, value); }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductChannelInfoStatus Status { get; set; }

        /// <summary>
        /// 是否同步促销
        /// </summary>
        public BooleanEnum IsUsePromotionPrice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUsePromotionPriceDisplay
        {
            get
            {
                return IsUsePromotionPrice == BooleanEnum.Yes;
            }
            set
            {
                IsUsePromotionPrice = value ? BooleanEnum.Yes : BooleanEnum.No;
            }
        }

        /// <summary>
        /// 是否指定库存
        /// </summary>
        public BooleanEnum IsAppointInventory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAppointInventoryDisplay
        {
            get
            {
                return IsAppointInventory == BooleanEnum.Yes;
            }
            set
            {
                IsAppointInventory = value ? BooleanEnum.Yes : BooleanEnum.No;
            }
        }

        /// <summary>
        /// 是否清库
        /// </summary>
        public BooleanEnum IsClearInventory { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 最大分仓数
        /// </summary>
        public int? MaxStockQty { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int? OnlineQty { get; set; }


        /// <summary>
        /// 泰隆优选价格
        /// </summary>
        public decimal? CurrentPrice { get; set; }

        /// <summary>
        /// 一致库存比例
        /// </summary>
        private string inventoryPercent;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^100$|^0$|^[1-9]\d{0,1}$", ErrorMessageResourceType=typeof(ResProductChannelManagement),ErrorMessageResourceName="Error_InputZeroTo100Int")]
        public string InventoryPercent
        {
            get { return inventoryPercent; }
            set { base.SetValue("InventoryPercent", ref  inventoryPercent, value); }
        }


        /// <summary>
        /// 渠道价格比例
        /// </summary>
        private string channelPricePercent;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[1-9]\d{0,3}$", ErrorMessageResourceType=typeof(ResProductChannelManagement),ErrorMessageResourceName="Error_Input100To9999Int")]
        public string ChannelPricePercent
        {
            get { return channelPricePercent; }
            set
            {
                base.SetValue("ChannelPricePercent", ref  channelPricePercent, value);

                Regex rgx = new Regex(@"^[1-9]\d{0,3}$");

                if (!string.IsNullOrEmpty(value) && rgx.IsMatch(value))
                {
                    if (int.Parse(value) < 100)
                    {
                        string error = "";
                        base.SetValue("ChannelPricePercent", ref error, "error");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _hasTaoBaoSkuVisible = "Collapsed";
        public string HasTaoBaoSkuVisible
        {
            get
            {
                if (ChannelInfo != null 
                    && !string.IsNullOrWhiteSpace(ChannelInfo.ChannelName) 
                    && ChannelInfo.ChannelName.Contains("淘宝"))
                {
                     _hasTaoBaoSkuVisible = "Visible";
                }
                else
                {
                    _hasTaoBaoSkuVisible = "Collapsed"; 
                }
                return _hasTaoBaoSkuVisible;
            }
           
        }

        public List<int> SysNoList { set; get; }

    }

    public class ChannelVM : ModelBase
    {
        public int? SysNo { get; set; }

        /// <summary>
        /// 订单渠道编号
        /// </summary>
        public string SOChannelCode { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道类型
        /// </summary>
        public int ChannelType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 是否泰隆优选退货
        /// </summary>
        public BooleanEnum IsRMAByNewegg { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

    }

    public class ProductChannelPeriodPriceVM : ModelBase
    {
        public int? SysNo { get; set; }

        public ProductChannelVM ChannelProductInfo { get; set; }


        /// <summary>
        /// 时段价格
        /// </summary>
        private string periodPrice;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^0\.\d{1,2}$|^[1-9][0-9]{0,7}\.\d{1,2}$|^[1-9][0-9]{1,7}$|^[1-9]$", ErrorMessageResourceType=typeof(ResProductChannelManagement),ErrorMessageResourceName="Error_PeriodPriceMessage")]
        public string PeriodPrice
        {
            get
            {
                return periodPrice;
            }
            set
            {
                decimal result;
                if (decimal.TryParse(value, out result))
                {
                    value = result.ToString("0.00");
                }

                base.SetValue("PeriodPrice", ref periodPrice, value);

                if (result < 0.1m)
                {
                    string error = "";
                    base.SetValue("PeriodPrice", ref error, "error");
                }
            }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Validate(ValidateType.Required)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 活动说明
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductChannelPeriodPriceStatus Status { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public ProductChannelPeriodPriceOperate Operate { get; set; }

        /// <summary>
        /// 是否更改价格
        /// </summary>
        public BooleanEnum IsChangePrice { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public UserInfo AuditUser { get; set; }

        public bool HasChannelProductPeriodPriceMaintainPermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_Product_ChannelProductPeriodPriceMaintain); }
        }
    }
}
