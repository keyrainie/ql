using System;
using System.Collections.Generic;

using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.RMA.Models
{
    public class RefundItemVM : ModelBase
    {
        public RefundItemVM()
        {            
            this.ReturnPriceTypeList = EnumConverter.GetKeyValuePairs<ReturnPriceType>();
            this.RefundPriceType = ReturnPriceType.TenPercentsOff;
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_RefundSysNo;
        public Int32? RefundSysNo
        {
            get { return this.m_RefundSysNo; }
            set { this.SetValue("RefundSysNo", ref m_RefundSysNo, value); }
        }

        private Int32? m_RegisterSysNo;
        public Int32? RegisterSysNo
        {
            get { return this.m_RegisterSysNo; }
            set { this.SetValue("RegisterSysNo", ref m_RegisterSysNo, value); }
        }

        private Decimal? m_OrgPrice;
        public Decimal? OrgPrice
        {
            get { return this.m_OrgPrice; }
            set { this.SetValue("OrgPrice", ref m_OrgPrice, value); }
        }

        private Decimal? m_UnitDiscount;
        public Decimal? UnitDiscount
        {
            get { return this.m_UnitDiscount; }
            set { this.SetValue("UnitDiscount", ref m_UnitDiscount, value); }
        }

        private Decimal? m_ProductValue;
        public Decimal? ProductValue
        {
            get { return this.m_ProductValue; }
            set { this.SetValue("ProductValue", ref m_ProductValue, value); }
        }

        private Int32? m_OrgPoint;
        public Int32? OrgPoint
        {
            get { return this.m_OrgPoint; }
            set { this.SetValue("OrgPoint", ref m_OrgPoint, value); }
        }

        private Decimal? m_RefundPrice;
        public Decimal? RefundPrice
        {
            get { return this.m_RefundPrice; }
            set { this.SetValue("RefundPrice", ref m_RefundPrice, value); }
        }

        private Decimal? m_RealOrgPrice;
        public Decimal? RealOrgPrice
        {
            get { return this.m_RealOrgPrice; }
            set { this.SetValue("RealOrgPrice", ref m_RealOrgPrice, value); }
        }

        private ProductPayType? m_PointType;
        public ProductPayType? PointType
        {
            get { return this.m_PointType; }
            set { this.SetValue("PointType", ref m_PointType, value); }
        }

        private Decimal? m_RefundCash;
        public Decimal? RefundCash
        {
            get { return this.m_RefundCash; }
            set { this.SetValue("RefundCash", ref m_RefundCash, value); }
        }

        private Int32? m_RefundPoint;
        public Int32? RefundPoint
        {
            get { return this.m_RefundPoint; }
            set { this.SetValue("RefundPoint", ref m_RefundPoint, value); }
        }

        private ReturnPriceType? m_RefundPriceType;
        public ReturnPriceType? RefundPriceType
        {
            get { return this.m_RefundPriceType; }
            set
            {
                this.SetValue("RefundPriceType", ref m_RefundPriceType, value);
                if (value == ReturnPriceType.OriginPrice)
                {
                    this.RefundPrice = this.RealOrgPrice;
                }
                if (value == ReturnPriceType.TenPercentsOff && this.RealOrgPrice.HasValue)
                {
                    this.RefundPrice = decimal.Round(this.RealOrgPrice.Value * 0.9M, 2);
                }
            }
        }

        private Decimal? m_RefundCost;
        public Decimal? RefundCost
        {
            get { return this.m_RefundCost; }
            set { this.SetValue("RefundCost", ref m_RefundCost, value); }
        }

        private Int32? m_RefundCostPoint;
        public Int32? RefundCostPoint
        {
            get { return this.m_RefundCostPoint; }
            set { this.SetValue("RefundCostPoint", ref m_RefundCostPoint, value); }
        }

        private Decimal? m_RefundCostWithoutTax;
        public Decimal? RefundCostWithoutTax
        {
            get { return this.m_RefundCostWithoutTax; }
            set { this.SetValue("RefundCostWithoutTax", ref m_RefundCostWithoutTax, value); }
        }

        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private Decimal? m_OrgGiftCardAmt;
        public Decimal? OrgGiftCardAmt
        {
            get { return this.m_OrgGiftCardAmt; }
            set { this.SetValue("OrgGiftCardAmt", ref m_OrgGiftCardAmt, value); }
        }

        private SOProductType? m_ProductType;
        public SOProductType? ProductType
        {
            get { return this.m_ProductType; }
            set { this.SetValue("ProductType", ref m_ProductType, value); }
        }

        private bool canChangeRefundPriceType;
		public bool CanChangeRefundPriceType 
		{ 
			get
			{
				return canChangeRefundPriceType;
			}			
			set
			{
				SetValue("CanChangeRefundPriceType", ref canChangeRefundPriceType, value);
			} 
		}

        public string ProductID { get; set; }

        public string ProductName { get; set; }
		
        public List<KeyValuePair<ReturnPriceType?, string>> ReturnPriceTypeList { get; set; }
    }
}
