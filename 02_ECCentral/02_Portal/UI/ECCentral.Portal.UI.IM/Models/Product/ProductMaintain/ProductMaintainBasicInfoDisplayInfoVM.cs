using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Models.Product.ProductMaintain
{
    public class ProductMaintainBasicInfoDisplayInfoVM : ModelBase
    {
        public ProductMaintainBasicInfoDisplayInfoVM()
        {
            this.TradeTypeList = EnumConverter.GetKeyValuePairs<TradeType>(EnumConverter.EnumAppendItemType.Select);
            this.StoreTypeList = EnumConverter.GetKeyValuePairs<StoreType>(EnumConverter.EnumAppendItemType.Select);
        }

        private String _productID;

        public String ProductID
        {
            get { return _productID; }
            set { SetValue("ProductID", ref _productID, value); }
        }

        private String _productName;

        public String ProductName
        {
            get { return _productName; }
            set { SetValue("ProductName", ref _productName, value); }
        }

        private String _productTitle;

        [Validate(ValidateType.Required, ErrorMessageResourceName = "ProductMaintain_ProductBasicInfoProductTitleEmpty", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [Validate(ValidateType.MaxLength, 90, ErrorMessageResourceName = "ProductMaintain_ProductBasicInfoProductTitleInvalid", ErrorMessageResourceType = typeof(ResProductMaintain))]
        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String ProductTitle
        {
            get { return _productTitle; }
            set
            {
                SetValue("ProductTitle", ref _productTitle, value);
                SetValue("ProductName", ref _productName, value + _promotionTitle);
            }
        }

        private String _promotionTitle;

        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String PromotionTitle
        {
            get { return _promotionTitle; }
            set
            {
                SetValue("PromotionTitle", ref _promotionTitle, value);
                SetValue("ProductName", ref _productName, _productTitle + value);
            }
        }

        private String _productBriefName;

        public String ProductBriefName
        {
            get { return _productBriefName; }
            set { SetValue("ProductBriefName", ref _productBriefName, value); }
        }

        private String _productBriefTitle;

        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String ProductBriefTitle
        {
            get { return _productBriefTitle; }
            set
            {
                SetValue("ProductBriefTitle", ref _productBriefTitle, value);
                SetValue("ProductBriefName", ref _productBriefName, value + _productBriefAddition);
            }
        }

        private String _productBriefAddition;

        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String ProductBriefAddition
        {
            get { return _productBriefAddition; }
            set
            {
                SetValue("ProductBriefAddition", ref _productBriefAddition, value);
                SetValue("ProductBriefName", ref _productBriefName, _productBriefTitle + value);
            }
        }

        private String _keywords;

        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String Keywords
        {
            get { return _keywords; }
            set { SetValue("Keywords", ref _keywords, value); }
        }

        private String _normalPromotionTitle;

        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String NormalPromotionTitle
        {
            get { return _normalPromotionTitle; }
            set { SetValue("NormalPromotionTitle", ref _normalPromotionTitle, value); }
        }

        private String _timelyPromotionTitle;

        [SpecialCHarValidation(ErrorMessageResourceType = typeof(ResProductMaintain), ErrorMessageResourceName = "ProductMaintain_TextSpecial")]
        public String TimelyPromotionTitle
        {
            get { return _timelyPromotionTitle; }
            set { SetValue("TimelyPromotionTitle", ref _timelyPromotionTitle, value); }
        }

        private DateTime? _timelyPromotionBeginDate;

        public DateTime? TimelyPromotionBeginDate
        {
            get { return _timelyPromotionBeginDate; }
            set { SetValue("TimelyPromotionBeginDate", ref _timelyPromotionBeginDate, value); }
        }

        private DateTime? _timelyPromotionEndDate;

        public DateTime? TimelyPromotionEndDate
        {
            get { return _timelyPromotionEndDate; }
            set { SetValue("TimelyPromotionEndDate", ref _timelyPromotionEndDate, value); }
        }

        private string _TaxNo;
        public string TaxNo
        {
            get { return _TaxNo; }
            set { SetValue("TaxNo", ref _TaxNo, value); }
        }

        private string _EntryRecord;
        public string EntryRecord
        {
            get { return _EntryRecord; }
            set { SetValue("EntryRecord", ref _EntryRecord, value); }
        }

        private string _TariffPrice;
        public string TariffPrice
        {
            get { return _TariffPrice; }
            set { SetValue("TariffPrice", ref _TariffPrice, value); }
        }

        public bool HasItemBasicInformationPMUpdatePermission
        {
            get { return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationPMUpdate); }
        }

        public bool HasItemBasicInformationProductTitleAndBriefNameUpdatePermission
        {
            get
            {
                return AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationPMUpdate) ||
                       AuthMgr.HasFunctionPoint(
                           AuthKeyConst.IM_ProductMaintain_ItemBasicInformationProductTitleOrBriefNameMaintain);
            }
        }

        public bool HasItemBasicInformationKeywordsMaintainPermission
        {
            get
            {
                return
                    AuthMgr.HasFunctionPoint(AuthKeyConst.IM_ProductMaintain_ItemBasicInformationProductKeywordsMaintain);
            }
        }

        public List<KeyValuePair<TradeType?, string>> TradeTypeList { get; set; }
        public List<KeyValuePair<StoreType?, string>> StoreTypeList { get; set; }

        /// <summary>
        /// 泰隆默认是国内商品
        /// </summary>
        private TradeType _TradeType = TradeType.Internal;
        public TradeType TradeType
        {
            get { return _TradeType; }
            set { SetValue("TradeType", ref _TradeType, value); }
        }


        private StoreType _StoreType;
        public StoreType StoreType
        {
            get { return _StoreType; }
            set { SetValue("StoreType", ref _StoreType, value); }
        }


        private String _ShoppingGuideURL;
        public String ShoppingGuideURL
        {
            get { return _ShoppingGuideURL; }
            set { SetValue("ShoppingGuideURL", ref _ShoppingGuideURL, value); }
        }


        /// <summary>
        /// 警戒库存
        /// </summary>
        private string _SafeQty;
        [Validate(ValidateType.Regex, @"^-?[1-9]?\d{0,8}$", ErrorMessage="请输入整数")]
        public string SafeQty
        {
            get { return _SafeQty; }
            set { SetValue("SafeQty", ref _SafeQty, value); }
        }

    }
}
