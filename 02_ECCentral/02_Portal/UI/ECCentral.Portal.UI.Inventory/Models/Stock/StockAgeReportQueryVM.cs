using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class StockAgeReportQueryVM : ModelBase
    {
        public StockAgeReportQueryVM()
        {
            this.m_StockAgeTypeList = new List<SelectionOptionVM>();
        }

        private DateTime? m_StatisticDate;

        public DateTime? StatisticDate
        {
            get { return m_StatisticDate; }
            set { base.SetValue("StatisticDate", ref m_StatisticDate, value); }
        }

        private int? m_C1SysNo;

        public int? C1SysNo
        {
            get { return m_C1SysNo; }
            set { base.SetValue("C1SysNo", ref m_C1SysNo, value); }
        }

        private int? m_C2SysNo;

        public int? C2SysNo
        {
            get { return m_C2SysNo; }
            set { base.SetValue("C2SysNo", ref m_C2SysNo, value); }
        }

        private int? m_C3SysNo;

        public int? C3SysNo
        {
            get { return m_C3SysNo; }
            set { base.SetValue("C3SysNo", ref m_C3SysNo, value); }
        }

        private int? m_ProductSysNo;

        public int? ProductSysNo
        {
            get { return m_ProductSysNo; }
            set { base.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private int? m_StockSysNo;

        public int? StockSysNo
        {
            get { return m_StockSysNo; }
            set { base.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private int? m_VendorSysNo;

        public int? VendorSysNo
        {
            get { return m_VendorSysNo; }
            set { base.SetValue("VendorSysNo", ref m_VendorSysNo, value); }
        }

        private List<SelectionOptionVM> m_StockAgeTypeList;

        public List<SelectionOptionVM> StockAgeTypeList
        {
            get { return m_StockAgeTypeList; }
            set { base.SetValue("StockAgeTypeList", ref m_StockAgeTypeList, value); }
        }
    }

    public class SelectionOptionVM : ModelBase
    {
        private string m_Text;

        public string Text
        {
            get { return m_Text; }
            set { base.SetValue("Text", ref m_Text, value); }
        }

        private string m_Value;

        public string Value
        {
            get { return m_Value; }
            set { base.SetValue("Value", ref m_Value, value); }
        }

        private bool m_Selected;

        public bool Selected
        {
            get { return m_Selected; }
            set { base.SetValue("Selected", ref m_Selected, value); }
        }
    }
}