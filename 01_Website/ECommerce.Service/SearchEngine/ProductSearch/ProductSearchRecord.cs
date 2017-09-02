using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Attributes;

namespace ECommerce.Facade.SearchEngine
{
    public class ProductSearchRecord
    {
        private string _Sysno;

        [SolrUniqueKey("p_sysno")]
        public string Sysno
        {
            get
            {
                return _Sysno;
            }
            set
            {
                _Sysno = value;
            }
        }

        private float _BasicPrice;

        [SolrField("p_basicprice")]
        public float BasicPrice
        {
            get
            {
                return _BasicPrice;
            }
            set
            {
                _BasicPrice = value;
            }
        }

        private string _BrandHasLogo;

        [SolrField("p_brandhaslogo")]
        public string BrandHasLogo
        {
            get
            {
                return _BrandHasLogo;
            }
            set
            {
                _BrandHasLogo = value;
            }
        }

        private string _BrandUrl;

        [SolrField("p_brandurl")]
        public string BrandUrl
        {
            get
            {
                return _BrandUrl;
            }
            set
            {
                _BrandUrl = value;
            }
        }

        private string _BrandN;

        private string _BrandID;

        [SolrField("p_brandid")]
        public string BrandID
        {
            get
            {
                return _BrandID;
            }
            set
            {
                _BrandID = value;
            }
        }


        private string _BrandName;

        [SolrField("p_brandname")]
        public string BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                _BrandName = value;
            }
        }

        private string _BrandPinYin;

        [SolrField("p_brandpinyin")]
        public string BrandPinYin
        {
            get
            {
                return _BrandPinYin;
            }
            set
            {
                _BrandPinYin = value;
            }
        }

        private string _BrandType;

        [SolrField("p_brandtype")]
        public string BrandType
        {
            get
            {
                return _BrandType;
            }
            set
            {
                _BrandType = value;
            }
        }

        private int _C3SysNONvalue;

        [SolrField("p_c3sysno")]
        public int C3SysNONvalue
        {
            get
            {
                return _C3SysNONvalue;
            }
            set
            {
                _C3SysNONvalue = value;
            }
        }

        private string _Categorysysno3Nvalue;
        
        [SolrField("p_categorysysno3_n")]
        public string Categorysysno3Nvalue
        {
            get
            {
                return _Categorysysno3Nvalue;
            }
            set
            {
                _Categorysysno3Nvalue = value;
            }
        }

        private float _CashRebate;

        [SolrField("p_cashrebate")]
        public float CashRebate
        {
            get
            {
                return _CashRebate;
            }
            set
            {
                _CashRebate = value;
            }
        }


        private string _CompanyCode;

        [SolrField("p_companycode")]
        public string CompanyCode
        {
            get
            {
                return _CompanyCode;
            }
            set
            {
                _CompanyCode = value;
            }
        }

        private string _CountDown;

        [SolrField("p_countdown")]
        public string CountDown
        {
            get
            {
                return _CountDown;
            }
            set
            {
                _CountDown = value;
            }
        }

        private float _CurrentPrice;

        [SolrField("p_currentprice")]
        public float CurrentPrice
        {
            get
            {
                return _CurrentPrice;
            }
            set
            {
                _CurrentPrice = value;
            }
        }

        private string _Depreciate;

        [SolrField("p_depreciate")]
        public string Depreciate
        {
            get
            {
                return _Depreciate;
            }
            set
            {
                _Depreciate = value;
            }
        }

        private float _Discount;

        [SolrField("p_discount")]
        public float Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                _Discount = value;
            }
        }

        private string _EvalStatus;

        [SolrField("p_evalstatus")]
        public string EvalStatus
        {
            get
            {
                return _EvalStatus;
            }
            set
            {
                _EvalStatus = value;
            }
        }

        private DateTime _FirstOnLineTime;

        [SolrField("p_firstonlinetime")]
        public DateTime FirstOnLineTime
        {
            get
            {
                return _FirstOnLineTime;
            }
            set
            {
                _FirstOnLineTime = value;
            }
        }

        private string _HotReviewSysNO;

        [SolrField("p_hotreviewsysno")]
        public string HotReviewSysNO
        {
            get
            {
                return _HotReviewSysNO;
            }
            set
            {
                _HotReviewSysNO = value;
            }
        }


        private string _HotReviewTitle;

        [SolrField("p_hotreviewtitle")]
        public string HotReviewTitle
        {
            get
            {
                return _HotReviewTitle;
            }
            set
            {
                _HotReviewTitle = value;
            }
        }

        private string _HotReviewProns;

        [SolrField("p_hotreviewprons")]
        public string HotReviewProns
        {
            get
            {
                return _HotReviewProns;
            }
            set
            {
                _HotReviewProns = value;
            }
        }

        private string _HotSort;

        [SolrField("p_hotsort")]
        public string HotSort
        {
            get
            {
                return _HotSort;
            }
            set
            {
                _HotSort = value;
            }
        }

        private string _ImageVersion;

        [SolrField("p_imageversion")]
        public string ImageVersion
        {
            get
            {
                return _ImageVersion;
            }
            set
            {
                _ImageVersion = value;
            }
        }

        private string _InvoiceType;

        [SolrField("p_invoicetype")]
        public string InvoiceType
        {
            get
            {
                return _InvoiceType;
            }
            set
            {
                _InvoiceType = value;
            }
        }

        private string _Is360Show;

        [SolrField("p_is360show")]
        public string Is360Show
        {
            get
            {
                return _Is360Show;
            }
            set
            {
                _Is360Show = value;
            }
        }

        private string _IsHaveValidGift;

        [SolrField("p_ishavevalidgift")]
        public string IsHaveValidGift
        {
            get
            {
                return _IsHaveValidGift;
            }
            set
            {
                _IsHaveValidGift = value;
            }
        }

        private string _IsPolymeric1;

        [SolrField("p_ispolymeric1")]
        public string IsPolymeric1
        {
            get
            {
                return _IsPolymeric1;
            }
            set
            {
                _IsPolymeric1 = value;
            }
        }

        private string _IsPolymeric2;

        [SolrField("p_ispolymeric2")]
        public string IsPolymeric2
        {
            get
            {
                return _IsPolymeric2;
            }
            set
            {
                _IsPolymeric2 = value;
            }
        }

        private string _IsShowStore;

        [SolrField("p_isshowstore")]
        public string IsShowStore
        {
            get
            {
                return _IsShowStore;
            }
            set
            {
                _IsShowStore = value;
            }
        }

        private string _IsVideo;

        [SolrField("p_isvideo")]
        public string IsVideo
        {
            get
            {
                return _IsVideo;
            }
            set
            {
                _IsVideo = value;
            }
        }

        private string _LanguageCode;

        [SolrField("p_languagecode")]
        public string LanguageCode
        {
            get
            {
                return _LanguageCode;
            }
            set
            {
                _LanguageCode = value;
            }
        }

        private string _MerchantCount;

        [SolrField("p_merchantcount")]
        public string MerchantCount
        {
            get
            {
                return _MerchantCount;
            }
            set
            {
                _MerchantCount = value;
            }
        }

        private int _MerchantSysNO;

        [SolrField("p_merchantsysno")]
        public int MerchantSysNO
        {
            get
            {
                return _MerchantSysNO;
            }
            set
            {
                _MerchantSysNO = value;
            }
        }

        private string _NewOrderBySubcatregory;

        [SolrField("p_neworderbysubcategory")]
        public string NewOrderBySubcatregory
        {
            get
            {
                return _NewOrderBySubcatregory;
            }
            set
            {
                _NewOrderBySubcatregory = value;
            }
        }

        private string _NewProduct;

        [SolrField("p_newproduct")]
        public string NewProduct
        {
            get
            {
                return _NewProduct;
            }
            set
            {
                _NewProduct = value;
            }
        }

        private int _OnlineQty;

        [SolrField("p_onlineqty")]
        public int OnlineQty
        {
            get
            {
                return _OnlineQty;
            }
            set
            {
                _OnlineQty = value;
            }
        }

        private int _Point;

        [SolrField("p_point")]
        public int Point
        {
            get
            {
                return _Point;
            }
            set
            {
                _Point = value;
            }
        }

        private string _PointType;

        [SolrField("p_pointtype")]
        public string PointType
        {
            get
            {
                return _PointType;
            }
            set
            {
                _PointType = value;
            }
        }

        private float _PriceSort;

        [SolrField("p_pricesort")]
        public float PriceSort
        {
            get
            {
                return _PriceSort;
            }
            set
            {
                _PriceSort = value;
            }
        }

        private string _ProductDescription;

        [SolrField("p_productdescription")]
        public string ProductDescription
        {
            get
            {
                return _ProductDescription;
            }
            set
            {
                _ProductDescription = value;
            }
        }

        private string _ProductID;

        [SolrField("p_productid")]
        public string ProductID
        {
            get
            {
                return _ProductID;
            }
            set
            {
                _ProductID = value;
            }
        }

        private string _ProductMode;

        [SolrField("p_productmode")]
        public string ProductMode
        {
            get
            {
                return _ProductMode;
            }
            set
            {
                _ProductMode = value;
            }
        }

        private string _ProductName;

        [SolrField("p_productname")]
        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                _ProductName = value;
            }
        }

        private string _ProductStatus;

        [SolrField("p_productstatus")]
        public string ProductStatus
        {
            get
            {
                return _ProductStatus;
            }
            set
            {
                _ProductStatus = value;
            }
        }

        private string _ProductTitle;

        [SolrField("p_producttitle")]
        public string ProductTitle
        {
            get
            {
                return _ProductTitle;
            }
            set
            {
                _ProductTitle = value;
            }
        }

        private string _ProductType;

        [SolrField("p_producttype")]
        public string ProductType
        {
            get
            {
                return _ProductType;
            }
            set
            {
                _ProductType = value;
            }
        }

        private string _PromotionTitle;

        [SolrField("p_promotiontitle")]
        public string PromotionTitle
        {
            get
            {
                return _PromotionTitle;
            }
            set
            {
                _PromotionTitle = value;
            }
        }

        private string _PromotionType;

        [SolrField("p_promotiontype")]
        public string PromotionType
        {
            get
            {
                return _PromotionType;
            }
            set
            {
                _PromotionType = value;
            }
        }

        private string _RelatedHotSaleProductSysNO;

        [SolrField("p_relatedhotSaleproductsysno")]
        public string RelatedHotSaleProductSysNO
        {
            get
            {
                return _RelatedHotSaleProductSysNO;
            }
            set
            {
                _RelatedHotSaleProductSysNO = value;
            }
        }

        private string _RelatedHotSaleProductReviewCount;

        [SolrField("p_relatedhotsaleproductreviewcount")]
        public string RelatedHotSaleProductReviewCount
        {
            get
            {
                return _RelatedHotSaleProductReviewCount;
            }
            set
            {
                _RelatedHotSaleProductReviewCount = value;
            }
        }

        private string _RemarkCount;

        [SolrField("p_remarkcount")]
        public string RemarkCount
        {
            get
            {
                return _RemarkCount;
            }
            set
            {
                _RemarkCount = value;
            }
        }

        private string _RemarkScore;

        [SolrField("p_remarkscore")]
        public string RemarkScore
        {
            get
            {
                return _RemarkScore;
            }
            set
            {
                _RemarkScore = value;
            }
        }

        private string _ReviewCount;

        [SolrField("p_reviewcount")]
        public string ReviewCount
        {
            get
            {
                return _ReviewCount;
            }
            set
            {
                _ReviewCount = value;
            }
        }

        private float _ReviewScore;

        [SolrField("p_reviewscore")]
        public float ReviewScore
        {
            get
            {
                return _ReviewScore;
            }
            set
            {
                _ReviewScore = value;
            }
        }

        private string _ShippingType;

        [SolrField("p_shippingtype")]
        public string ShippingType
        {
            get
            {
                return _ShippingType;
            }
            set
            {
                _ShippingType = value;
            }
        }

        private float _SortNum;

        [SolrField("p_sortnum")]
        public float SortNum
        {
            get
            {
                return _SortNum;
            }
            set
            {
                _SortNum = value;
            }
        }

        private string _StoreCompanyCode;

        [SolrField("p_storecompanycode")]
        public string StoreCompanyCode
        {
            get
            {
                return _StoreCompanyCode;
            }
            set
            {
                _StoreCompanyCode = value;
            }
        }

        private string _ValueDescription1;

        [SolrField("p_valuedescription1")]
        public string ValueDescription1
        {
            get
            {
                return _ValueDescription1;
            }
            set
            {
                _ValueDescription1 = value;
            }
        }

        private string _ValueDescription2;

        [SolrField("p_valuedescription2")]
        public string ValueDescription2
        {
            get
            {
                return _ValueDescription2;
            }
            set
            {
                _ValueDescription2 = value;
            }
        }

        private string _VendorBriefName;

        [SolrField("p_vendorbriefname")]
        public string VendorBriefName
        {
            get
            {
                return _VendorBriefName;
            }
            set
            {
                _VendorBriefName = value;
            }
        }

        private string _WeekTrend;

        [SolrField("p_weektrend")]
        public string WeekTrend
        {
            get
            {
                return _WeekTrend;
            }
            set
            {
                _WeekTrend = value;
            }
        }

        private string _Weight;

        [SolrField("p_weight")]
        public string Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                _Weight = value;
            }
        }

        private string _ProductGroupName;

        [SolrField("p_productgroupname")]
        public string ProductGroupName
        {
            get
            {
                return _ProductGroupName;
            }
            set
            {
                _ProductGroupName = value;
            }
        }

        private string _DefaultImage;

        [SolrField("p_defaultimage")]
        public string DefaultImage
        {
            get
            {
                return _DefaultImage;
            }
            set
            {
                _DefaultImage = value;
            }
        }

        private string _Barcode;

        [SolrField("p_barcode")]
        public string Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                _Barcode = value;
            }
        }

        private string _PolymericProductCount;

        [SolrField("p_polymericproductcount")]
        public string PolymericProductCount
        {
            get
            {
                return _PolymericProductCount;
            }
            set
            {
                _PolymericProductCount = value;
            }
        }

        private string _ProductGroupValue;

        [SolrField("p_productgroupvalue")]
        public string ProductGroupValue
        {
            get
            {
                return _ProductGroupValue;
            }
            set
            {
                _ProductGroupValue = value;
            }
        }

        private string _MinCountPerOrder;
        [SolrField("p_mincountperorder")]
        public string MinCountPerOrder
        {
            get
            {
                return _MinCountPerOrder;
            }
            set
            {
                _MinCountPerOrder = value;
            }
        }

        private string _PolymericShowName1;
        [SolrField("p_polymericShowName1")]
        public string PolymericShowName1
        {
            get
            {
                return _PolymericShowName1;
            }
            set
            {
                _PolymericShowName1 = value;
            }
        }

        private string _PolymericShowName2;
        [SolrField("p_polymericShowName2")]
        public string PolymericShowName2
        {
            get
            {
                return _PolymericShowName2;
            }
            set
            {
                _PolymericShowName2 = value;
            }
        }

        #region 团购搜索新加
        private int _ProductGroupBuyingSysno;

        [SolrField("p_productgroupbuyingsysno")]
        public int ProductGroupBuyingSysno
        {
            get { return _ProductGroupBuyingSysno; }
            set { _ProductGroupBuyingSysno = value; }
        }

        private string _GroupBuyingTitle;

        [SolrField("p_groupbuyingtitle")]
        public string GroupBuyingTitle
        {
            get { return _GroupBuyingTitle; }
            set { _GroupBuyingTitle = value; }
        }

        private string _GroupBuyingDesc;

        [SolrField("p_groupbuyingdesc")]
        public string GroupBuyingDesc
        {
            get { return _GroupBuyingDesc; }
            set { _GroupBuyingDesc = value; }
        }

        private string _GroupBuyingPicUrl;

        [SolrField("p_groupbuyingpicurl")]
        public string GroupBuyingPicUrl
        {
            get { return _GroupBuyingPicUrl; }
            set { _GroupBuyingPicUrl = value; }
        }

        private string _GroupBuyingSmallPicUrl;

        [SolrField("p_groupbuyingsmallpicurl")]
        public string GroupBuyingSmallPicUrl
        {
            get { return _GroupBuyingSmallPicUrl; }
            set { _GroupBuyingSmallPicUrl = value; }
        }

        private string _GroupBuyingBeginDate;

        [SolrField("p_groupbuyingbegindate")]
        public string GroupBuyingBeginDate
        {
            get { return _GroupBuyingBeginDate; }
            set { _GroupBuyingBeginDate = value; }
        }

        private string _GroupBuyingEndDate;

        [SolrField("p_groupbuyingenddate")]
        public string GroupBuyingEndDate
        {
            get { return _GroupBuyingEndDate; }
            set { _GroupBuyingEndDate = value; }
        }

        private string _GroupBuyingIsByGroup;

        [SolrField("p_groupbuyingisbygroup")]
        public string GroupBuyingIsByGroup
        {
            get { return _GroupBuyingIsByGroup; }
            set { _GroupBuyingIsByGroup = value; }
        }

        private string _GroupBuyingSuccessDate;

        [SolrField("p_groupbuyingsuccessdate")]
        public string GroupBuyingSuccessDate
        {
            get { return _GroupBuyingSuccessDate; }
            set { _GroupBuyingSuccessDate = value; }
        }

        private decimal _GroupBuyingOriginalPric;

        [SolrField("p_groupbuyingoriginalpric")]
        public decimal GroupBuyingOriginalPric
        {
            get { return _GroupBuyingOriginalPric; }
            set { _GroupBuyingOriginalPric = value; }
        }

        private decimal _GroupBuyingDealPrice;

        [SolrField("p_groupbuyingdealprice")]
        public decimal GroupBuyingDealPrice
        {
            get { return _GroupBuyingDealPrice; }
            set { _GroupBuyingDealPrice = value; }
        }

        private int _GroupBuyingCurrentSellCount;

        [SolrField("p_groupbuyingcurrentsellcount")]
        public int GroupBuyingCurrentSellCount
        {
            get { return _GroupBuyingCurrentSellCount; }
            set { _GroupBuyingCurrentSellCount = value; }
        }

        private int _GroupBuyingTypeSysNo;

        [SolrField("p_groupbuyingtypesysno")]
        public int GroupBuyingTypeSysNo
        {
            get { return _GroupBuyingTypeSysNo; }
            set { _GroupBuyingTypeSysNo = value; }
        }

        private string _GroupBuyingStatus;

        [SolrField("p_groupbuyingstatus")]
        public string GroupBuyingStatus
        {
            get { return _GroupBuyingStatus; }
            set { _GroupBuyingStatus = value; }
        }

        private string _GroupBuyingReasons;

        [SolrField("p_groupbuyingreasons")]
        public string GroupBuyingReasons
        {
            get { return _GroupBuyingReasons; }
            set { _GroupBuyingReasons = value; }
        }

        private int _GroupBuyingPriority;

        [SolrField("p_groupbuyingpriority")]
        public int GroupBuyingPriority
        {
            get { return _GroupBuyingPriority; }
            set { _GroupBuyingPriority = value; }
        }

        private int _GroupBuyingAreaSysNo;

        [SolrField("p_groupbuyingareasysno")]
        public int GroupBuyingAreaSysNo
        {
            get { return _GroupBuyingAreaSysNo; }
            set { _GroupBuyingAreaSysNo = value; }
        }

        private string _GroupBuyingMiddlePicUrl;

        [SolrField("p_groupbuyingmiddlepicurl")]
        public string GroupBuyingMiddlePicUrl
        {
            get { return _GroupBuyingMiddlePicUrl; }
            set { _GroupBuyingMiddlePicUrl = value; }
        }

        private string _GroupBuyingRules;

        [SolrField("p_groupbuyingrules")]
        public string GroupBuyingRules
        {
            get { return _GroupBuyingRules; }
            set { _GroupBuyingRules = value; }
        }

        private decimal _GroupBuyingBasicPrice;

        [SolrField("p_groupbuyingbasicprice")]
        public decimal GroupBuyingBasicPrice
        {
            get { return _GroupBuyingBasicPrice; }
            set { _GroupBuyingBasicPrice = value; }
        }

        private int _GroupBuyingVendorSysNo;

        [SolrField("p_groupbuyingvendorsysno")]
        public int GroupBuyingVendorSysNo
        {
            get { return _GroupBuyingVendorSysNo; }
            set { _GroupBuyingVendorSysNo = value; }
        }

        private int _GroupBuyingCategoryType;

        [SolrField("p_groupbuyingcategorytype")]
        public int GroupBuyingCategoryType
        {
            get { return _GroupBuyingCategoryType; }
            set { _GroupBuyingCategoryType = value; }
        }

        private string _GroupBuyingCouponValidDate;

        [SolrField("p_groupbuyingcouponvaliddate")]
        public string GroupBuyingCouponValidDate
        {
            get { return _GroupBuyingCouponValidDate; }
            set { _GroupBuyingCouponValidDate = value; }
        }

        private string _GroupBuyingLotteryRule;

        [SolrField("p_groupbuyinglotteryrule")]
        public string GroupBuyingLotteryRule
        {
            get { return _GroupBuyingLotteryRule; }
            set { _GroupBuyingLotteryRule = value; }
        }

        private int _GroupBuyingCategorySysNo;

        [SolrField("p_groupbuyingcategorysysno")]
        public int GroupBuyingCategorySysNo
        {
            get { return _GroupBuyingCategorySysNo; }
            set { _GroupBuyingCategorySysNo = value; }
        }

        private int _GroupBuyingIsWithoutReservation;
        [SolrField("p_groupbuyingiswithoutreservation")]
        public int GroupBuyingIsWithoutReservation
        {
            get { return _GroupBuyingIsWithoutReservation; }
            set { _GroupBuyingIsWithoutReservation = value; }
        }

        private int _GroupBuyingIsVouchers;
        [SolrField("p_groupbuyingisvouchers")]
        public int GroupBuyingIsVouchers
        {
            get { return _GroupBuyingIsVouchers; }
            set { _GroupBuyingIsVouchers = value; }
        }

        private decimal _GroupBuyingPrice;
        [SolrField("p_groupbuyingprice")]
        public decimal GroupBuyingPrice
        {
            get { return _GroupBuyingPrice; }
            set { _GroupBuyingPrice = value; }
        }


        private int _InventoryType;
        [SolrField("p_inventorytype")]
        public int InventoryType
        {
            get { return _InventoryType; }
            set { _InventoryType = value; }
        }

        #endregion

        private decimal _giftVoucherPrice;
        [SolrField("p_giftprice")]
        public decimal GiftVoucherPrice
        {
            get { return _giftVoucherPrice; }
            set { _giftVoucherPrice = value; }
        }


        public float _productTariffAmt;
        [SolrField("p_tariffprice")]
        public float ProductTariffAmt
        {
            get { return _productTariffAmt; }
            set { _productTariffAmt = value; }
        }


        public float _totalPrice;
        [SolrField("p_totalprice")]
        public float TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; }
        }
        //public float _productTariffAmtWithRebate;
        //[SolrField("p_producttariffamtwithrebate")]
        //public float ProductTariffAmtWithRebate
        //{
        //    get { return _productTariffAmtWithRebate; }
        //    set { _productTariffAmtWithRebate = value; }
        //}
        public int _productTradeType;
        [SolrField("p_producttradetype")]
        public int ProductTradeType
        {
            get { return _productTradeType; }
            set { _productTradeType = value; }
        }
    }
}
