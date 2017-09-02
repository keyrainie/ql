using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Models
{
    public class CommissionItemVM : ModelBase
    {
        private VendorCommissionItemType m_CommissionType;
        public VendorCommissionItemType CommissionType
        {
            get { return this.m_CommissionType; }
            set { this.SetValue("CommissionType", ref m_CommissionType, value); }
        }

        private Int32? m_ItemSysNo;
        public Int32? ItemSysNo
        {
            get { return this.m_ItemSysNo; }
            set { this.SetValue("ItemSysNo", ref m_ItemSysNo, value); }
        }

        private Int32? m_RuleSysNo;
        public Int32? RuleSysNo
        {
            get { return this.m_RuleSysNo; }
            set { this.SetValue("RuleSysNo", ref m_RuleSysNo, value); }
        }

        private Decimal? m_TotalSaleAmt;
        public Decimal? TotalSaleAmt
        {
            get { return this.m_TotalSaleAmt; }
            set { this.SetValue("TotalSaleAmt", ref m_TotalSaleAmt, value); }
        }

        private Decimal? m_TotalDeliveryFee;
        public Decimal? TotalDeliveryFee
        {
            get { return this.m_TotalDeliveryFee; }
            set { this.SetValue("TotalDeliveryFee", ref m_TotalDeliveryFee, value); }
        }

        private Decimal? m_TotalOrderCommissionFee;
        public Decimal? TotalOrderCommissionFee
        {
            get { return this.m_TotalOrderCommissionFee; }
            set { this.SetValue("TotalOrderCommissionFee", ref m_TotalOrderCommissionFee, value); }
        }

        private Int32? m_TotalQty;
        public Int32? TotalQty
        {
            get { return this.m_TotalQty; }
            set { this.SetValue("TotalQty", ref m_TotalQty, value); }
        }

        private Int32? m_OrderQty;
        public Int32? OrderQty
        {
            get { return this.m_OrderQty; }
            set { this.SetValue("OrderQty", ref m_OrderQty, value); }
        }

        private Int32? m_DeliveryQty;
        public Int32? DeliveryQty
        {
            get { return this.m_DeliveryQty; }
            set { this.SetValue("DeliveryQty", ref m_DeliveryQty, value); }
        }

        private Decimal? m_SalesCommissionFee;
        public Decimal? SalesCommissionFee
        {
            get { return this.m_SalesCommissionFee; }
            set { this.SetValue("SalesCommissionFee", ref m_SalesCommissionFee, value); }
        }

        private Decimal? m_OrderCommissionFee;
        public Decimal? OrderCommissionFee
        {
            get { return this.m_OrderCommissionFee; }
            set { this.SetValue("OrderCommissionFee", ref m_OrderCommissionFee, value); }
        }

        private Decimal? m_DeliveryFee;
        public Decimal? DeliveryFee
        {
            get { return this.m_DeliveryFee; }
            set { this.SetValue("DeliveryFee", ref m_DeliveryFee, value); }
        }

        private String m_ManufacturerName;
        public String ManufacturerName
        {
            get { return this.m_ManufacturerName; }
            set { this.SetValue("ManufacturerName", ref m_ManufacturerName, value); }
        }

        private String m_BrandName;
        public String BrandName
        {
            get { return this.m_BrandName; }
            set { this.SetValue("BrandName", ref m_BrandName, value); }
        }

        private String m_C2Name;
        public String C2Name
        {
            get { return this.m_C2Name; }
            set { this.SetValue("C2Name", ref m_C2Name, value); }
        }

        private String m_C3Name;
        public String C3Name
        {
            get { return this.m_C3Name; }
            set { this.SetValue("C3Name", ref m_C3Name, value); }
        }

        public string ManufacturerAndCategoryName
        {
            get
            {
                if (!string.IsNullOrEmpty(m_C3Name))
                {
                    return string.Format("{0}({1})({2})", m_ManufacturerName, m_BrandName, m_C3Name);
                }
                if (!string.IsNullOrEmpty(m_C2Name))
                {
                    return string.Format("{0}({1})({2})", m_ManufacturerName, m_BrandName, m_C2Name);
                }
                return string.Empty;
            }
        }

        private VendorStagedSaleRuleEntity saleRule;

        public VendorStagedSaleRuleEntity SaleRule
        {
            get { return saleRule; }
            set { this.SetValue("SaleRule", ref saleRule, value); }
        }

        private string saleRuleDisplayString;

        public string SaleRuleDisplayString
        {
            get { return saleRuleDisplayString; }
            set { this.SetValue("SaleRuleDisplayString", ref saleRuleDisplayString, value); }
        }

        private List<CommissionItemDetailVM> m_DetailList;
        public List<CommissionItemDetailVM> DetailList
        {
            get { return this.m_DetailList; }
            set { this.SetValue("DetailList", ref m_DetailList, value); }
        }

        private List<CommissionItemDetailVM> m_DetailOrderList;
        public List<CommissionItemDetailVM> DetailOrderList
        {
            get { return this.m_DetailOrderList; }
            set { this.SetValue("DetailOrderList", ref m_DetailOrderList, value); }
        }

        private List<CommissionItemDetailVM> m_DetailDeliveryList;
        public List<CommissionItemDetailVM> DetailDeliveryList
        {
            get { return this.m_DetailDeliveryList; }
            set { this.SetValue("DetailDeliveryList", ref m_DetailDeliveryList, value); }
        }
    }
}
