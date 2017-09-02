using System;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class SaleGift_GiftItemViewModel:ModelBase
    {
        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        /**** 下面的都是用来显示和UI操作的 *****/
        private String m_ProductID;
        public String ProductID
        {
            get { return this.m_ProductID; }
            set { this.SetValue("ProductID", ref m_ProductID, value); }
        }

        private String m_ProductName;
        public String ProductName
        {
            get { return this.m_ProductName; }
            set { this.SetValue("ProductName", ref m_ProductName, value); }
        }

        private Int32? m_AvailableQty;
        public Int32? AvailableQty
        {
            get { return this.m_AvailableQty; }
            set { this.SetValue("AvailableQty", ref m_AvailableQty, value); }
        }

        private Int32? m_ConsignQty;
        public Int32? ConsignQty
        {
            get { return this.m_ConsignQty; }
            set { this.SetValue("ConsignQty", ref m_ConsignQty, value); }
        }

        private Int32? m_VirtualQty;
        public Int32? VirtualQty
        {
            get { return this.m_VirtualQty; }
            set { this.SetValue("VirtualQty", ref m_VirtualQty, value); }
        }

        private Decimal? m_UnitCost;
        public Decimal? UnitCost
        {
            get { return this.m_UnitCost; }
            set { this.SetValue("UnitCost", ref m_UnitCost, value); }
        }

        private Decimal? m_CurrentPrice;
        public Decimal? CurrentPrice
        {
            get { return this.m_CurrentPrice; }
            set { this.SetValue("CurrentPrice", ref m_CurrentPrice, value); }
        }

        private Decimal? m_GrossMarginRate;
        public Decimal? GrossMarginRate
        {
            get { return this.m_GrossMarginRate; }
            set { this.SetValue("GrossMarginRate", ref m_GrossMarginRate, value); }
        }

        private string m_Priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,5}$", ErrorMessage = "请输入1至999999的整数！")]
        public string Priority
        {
            get { return this.m_Priority; }
            set { this.SetValue("Priority", ref m_Priority, value); }
        }

        private string m_Count;
        [Validate(ValidateType.Regex, @"^[0-9]\d{0,3}$", ErrorMessage = "请输入1至9999的整数！")]
        public string Count
        {
            get { return this.m_Count; }
            set { this.SetValue("Count", ref m_Count, value); }
        }

        private string m_PlusPrice;
        [Validate(ValidateType.Regex, @"^[0-9]+\.?[0-9]{0,2}$", ErrorMessage = "必须是大于等于0的数字")]
        public string PlusPrice
        {
            get { return this.m_PlusPrice; }
            set { this.SetValue("PlusPrice", ref m_PlusPrice, value); }
        }

        private bool? isChecked=false;
        public bool? IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }

    }
}
