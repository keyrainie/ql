using System;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Inventory.Models
{
    public class CostInAndCostOutReportQueryVM : ModelBase
    {
        public CostInAndCostOutReportQueryVM()
        {
            m_warehouseSysNoList = new ObservableCollection<int>();
            m_vendorSysNoList = new ObservableCollection<int>();
            m_brandSysNoList = new ObservableCollection<int>();
        }

        private string m_productID;

        public string ProductID
        {
            get { return m_productID; }
            set { base.SetValue("ProductID", ref m_productID, value); }
        }

        private string m_productName;

        public string ProductName
        {
            get { return m_productName; }
            set { base.SetValue("ProductName", ref m_productName, value); }
        }

        private int? m_category1SysNo;

        public int? Category1SysNo
        {
            get { return m_category1SysNo; }
            set { base.SetValue("Category1SysNo", ref m_category1SysNo, value); }
        }

        private int? m_category2SysNo;

        public int? Category2SysNo
        {
            get { return m_category2SysNo; }
            set { base.SetValue("Category2SysNo", ref m_category2SysNo, value); }
        }

        private int? m_category3SysNo;

        public int? Category3SysNo
        {
            get { return m_category3SysNo; }
            set { base.SetValue("Category3SysNo", ref m_category3SysNo, value); }
        }

        private ObservableCollection<int> m_warehouseSysNoList;

        public ObservableCollection<int> WarehouseSysNoList
        {
            get { return m_warehouseSysNoList; }
            set { base.SetValue("WarehouseSysNoList", ref m_warehouseSysNoList, value); }
        }

        private ObservableCollection<int> m_brandSysNoList;

        public ObservableCollection<int> BrandSysNoList
        {
            get { return m_brandSysNoList; }
            set { base.SetValue("BrandSysNoList", ref m_brandSysNoList, value); }
        }

        private ObservableCollection<int> m_vendorSysNoList;

        public ObservableCollection<int> VendorSysNoList
        {
            get { return m_vendorSysNoList; }
            set { base.SetValue("VendorSysNoList", ref m_vendorSysNoList, value); }
        }

        private DateTime? m_dateTimeFrom;

        [Validate(ValidateType.Required,ErrorMessage="开始时间不能为空。")]
        public DateTime? DateTimeFrom
        {
            get { return m_dateTimeFrom; }
            set { base.SetValue("DateTimeFrom", ref m_dateTimeFrom, value); }
        }

        private DateTime? m_dateTimeTo;

        [Validate(ValidateType.Required, ErrorMessage = "结束时间不能为空。")]
        public DateTime? DateTimeTo
        {
            get { return m_dateTimeTo; }
            set { base.SetValue("DateTimeTo", ref m_dateTimeTo, value); }
        }
    }
}