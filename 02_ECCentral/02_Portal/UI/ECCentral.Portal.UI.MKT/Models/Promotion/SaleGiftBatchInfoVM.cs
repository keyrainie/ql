using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Components.Models;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleGiftBatchInfoVM : ModelBase
    {
        public SaleGiftBatchInfoVM()
        {
            this.ProductItems1 = new List<ProductItemVM>();
            this.ProductItems2 = new List<ProductItemVM>();
            this.Gifts = new List<ProductItemVM>();

            this.RebateCountModePairList = EnumConverter.GetKeyValuePairs<SaleGiftDiscountBelongType>();
            this.GiftTypePairList = EnumConverter.GetKeyValuePairs<SaleGiftType>();
            this.CombineTypePairList = EnumConverter.GetKeyValuePairs<SaleGiftCombineType>();

            this.GiftTypePairList.RemoveAt(2);

            this.RebateCaculateMode = this.RebateCountModePairList[0].Key;
            this.SaleGiftType = this.GiftTypePairList[0].Key;
            this.CombineType = this.CombineTypePairList[0].Key;

        }

        /// <summary>
        /// 
        /// </summary>
        public List<KeyValuePair<SaleGiftType?, string>> GiftTypePairList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<KeyValuePair<SaleGiftDiscountBelongType?, string>> RebateCountModePairList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<KeyValuePair<SaleGiftCombineType?, string>> CombineTypePairList { get; set; }

        private string _PromotionName;

        /// <summary>
        /// 规则名称
        /// </summary>
        [Validate(ValidateType.MaxLength,100)]
        public string PromotionName
        {
            get { return this._PromotionName; }
            set { this.SetValue("PromotionName", ref _PromotionName, value); }
        }

        private SaleGiftType? _SaleGiftType;
        /// <summary>
        /// 赠品类型
        /// </summary>
        public SaleGiftType? SaleGiftType
        {
            get { return this._SaleGiftType; }
            set {
                if (value == ECCentral.BizEntity.MKT.SaleGiftType.Single || value == ECCentral.BizEntity.MKT.SaleGiftType.Multiple)
                {
                    VendorSysNo = "1";
                    VendorName = "泰隆优选";
                }
                this.SetValue("SaleGiftType", ref _SaleGiftType, value); }
        }


        private SaleGiftStatus? m_Status = SaleGiftStatus.Init;
        public SaleGiftStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        private string _PromotionDescription;

        /// <summary>
        /// 规则描述
        /// </summary>
        public string PromotionDescription
        {
            get { return this._PromotionDescription; }
            set { this.SetValue("PromotionDescription", ref _PromotionDescription, value); }
        }

        private string m_BeginDate;
        [Validate(ValidateType.Required)]
        public string BeginDate
        {
            get { return this.m_BeginDate; }
            set {

                this.SetValue("BeginDate", ref m_BeginDate, value); }
        }

        private string m_EndDate;
        [Validate(ValidateType.Required)]
        public string EndDate
        {
            get { return this.m_EndDate; }
            set {

                this.SetValue("EndDate", ref m_EndDate, value); }
        }

        private String m_PromotionLink;
        /// <summary>
        /// 活动链接
        /// </summary>
        [Validate(ValidateType.URL)]
        public String PromotionLink
        {
            get { return this.m_PromotionLink; }
            set { this.SetValue("PromotionLink", ref m_PromotionLink, value); }
        }

        private SaleGiftDiscountBelongType? _RebateCaculateMode;

        /// <summary>
        /// 折扣记入方式
        /// </summary>
        public SaleGiftDiscountBelongType? RebateCaculateMode
        {
            get { return this._RebateCaculateMode; }
            set { this.SetValue("RebateCaculateMode", ref _RebateCaculateMode, value); }
        }

        private bool _IsSpecifiedGift;

        /// <summary>
        /// 是否指定赠品
        /// </summary>
        public bool IsSpecifiedGift
        {
            get { return this._IsSpecifiedGift; }
            set { this.SetValue("IsSpecifiedGift", ref _IsSpecifiedGift, value); }
        }

        private string _TotalQty;

        /// <summary>
        /// 总数量
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^0$|^[0-9]\d{0,4}$", ErrorMessage = "请输入0至99999的整数！")]
        public string TotalQty
        {
            get { return this._TotalQty; }
            set { this.SetValue("TotalQty", ref _TotalQty, value); }
        }


        private List<ProductItemVM> _ProductItems1;

        /// <summary>
        /// 购买的商品，左边列表
        /// </summary>
        public List<ProductItemVM> ProductItems1
        {
            get { return this._ProductItems1; }
            set { this.SetValue("ProductItems1", ref _ProductItems1, value); }
        }

        private List<ProductItemVM> _ProductItems2;

        /// <summary>
        /// 购买的商品，右边列表
        /// </summary>
        public List<ProductItemVM> ProductItems2
        {
            get { return this._ProductItems2; }
            set { this.SetValue("ProductItems2", ref _ProductItems2, value); }
        }

        private List<ProductItemVM> _Gifts;

        /// <summary>
        /// 赠品
        /// </summary>
        public List<ProductItemVM> Gifts
        {
            get { return this._Gifts; }
            set { this.SetValue("Gifts", ref _Gifts, value); }
        }

        private SaleGiftCombineType? _CombineType;

        /// <summary>
        /// 组合类型
        /// </summary>
        public SaleGiftCombineType? CombineType
        {
            get { return this._CombineType; }
            set
            {
                this.SetValue("CombineType", ref _CombineType, value);
                RaisePropertyChanged("CombineTip");
            }
        }

        private string combineTip;
		public string CombineTip 
		{ 
			get
			{
                if (CombineType == SaleGiftCombineType.Assemble)
                {
                    return ResBatchCreateSaleGift.TextBlock_CombineAssembleTip;
                }
                else
                {
                    return ResBatchCreateSaleGift.TextBlock_CombineCrossTip;
                }
			}			
			set
			{
				SetValue("CombineTip", ref combineTip, value);
			} 
		}		

        public SaleGiftBatchInfo ToEntity()
        {
            SaleGiftBatchInfo info=this.ConvertVM<SaleGiftBatchInfoVM, SaleGiftBatchInfo>();
            info.RuleName = new BizEntity.LanguageContent();
            info.RuleDescription = new BizEntity.LanguageContent();
            info.RuleName.Content = this.PromotionName;
            info.RuleDescription.Content = this.PromotionDescription;
   
            return info;
        }

        public bool ValidateQty(out string errorInfo)
        {
            if (this.SaleGiftType == ECCentral.BizEntity.MKT.SaleGiftType.Single || this.SaleGiftType == ECCentral.BizEntity.MKT.SaleGiftType.Vendor)
            {
                if (this.ProductItems1 == null || this.ProductItems1.Count <= 0)
                {
                    errorInfo = Resources.ResBatchCreateSaleGift.Info_NoProduct1;
                    return false;
                }
            }

            if (this.SaleGiftType == ECCentral.BizEntity.MKT.SaleGiftType.Multiple)
            {
                if (this.ProductItems1 == null || this.ProductItems1.Count <= 0)
                {
                    errorInfo = Resources.ResBatchCreateSaleGift.Info_NoProduct1ForMultiple;
                    return false;
                }

                if (this.ProductItems2 == null || this.ProductItems2.Count <= 0)
                {
                    errorInfo = Resources.ResBatchCreateSaleGift.Info_NoProduct2ForMultiple;
                    return false;
                }
            }

            if (this.Gifts == null || this.Gifts.Count <= 0)
            {
                errorInfo = Resources.ResBatchCreateSaleGift.Info_NoGifts;
                return false;
            }

            foreach (ProductItemVM item in this.Gifts)
            {
                if (item.Priority == null)
                {
                    errorInfo = Resources.ResBatchCreateSaleGift.Info_InputPriority;
                    return false;
                }
            }

            if (this.IsSpecifiedGift)
            {
                foreach (ProductItemVM item in this.Gifts)
                {
                    if (item.HandselQty == null)
                    {
                        errorInfo = Resources.ResBatchCreateSaleGift.Info_InputGiftQty;
                        return false;
                    }
                }

            }
            else
            {
                if (this.TotalQty == null)
                {
                    errorInfo = Resources.ResBatchCreateSaleGift.Info_InputGiftTotalQty;
                    return false;
                }
 
            }

            errorInfo = "";
            return true;
        }

        public bool BrandEnabled { get { return false;} }

        private string m_VendorSysNo="1";
        public string VendorSysNo
        {
            get
            {
          
                return m_VendorSysNo;
            }
            set
            {
                this.SetValue("VendorSysNo", ref m_VendorSysNo, value);
            }
        }
        private string m_VendorName = "泰隆优选";
        public string VendorName
        {
            get
            {
               
                return m_VendorName;
            }
            set
            {
                this.SetValue("VendorName", ref m_VendorName, value);
            }
        }

    
    }
}
