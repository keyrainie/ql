using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.ComponentModel;
using ECCentral.BizEntity.SO;
using System.Linq;
using ECCentral.Portal.Basic.Converters;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.Utilities.Validation;


namespace ECCentral.Portal.UI.Invoice.Models
{
    public class SalesStatisticsReportQueryVM : ModelBase
    {
        public SalesStatisticsReportQueryVM()
        {
            this.SOStatusListOptions = new ObservableCollection<SOStatusOption>()
            {
                new SOStatusOption {Code = -999                          , Name = ResCommonEnum.Enum_All, IsChecked = true},
                new SOStatusOption {Code = (int) SOStatus.WaitingOutStock, Name = SOStatus.WaitingOutStock.ToDescription(), IsChecked = true},
                new SOStatusOption {Code = (int) SOStatus.OutStock       , Name = SOStatus.OutStock.ToDescription(), IsChecked = true},
                new SOStatusOption {Code = (int) SOStatus.Complete       , Name = SOStatus.Complete.ToDescription(), IsChecked = true},
                //new SOStatusOption {Code = (int) SOStatus.Reported       , Name = SOStatus.Reported.ToDescription(), IsChecked = true},
            };

            this.SOStatusListOptions.ForEach(o => o.PropertyChanged += SOStatusOption_PropertyChanged);
        }

        /// <summary>
        /// 防止订单状态下拉框中的“所有”与其他单项控制上的控制上的死循环。
        /// </summary>
        private bool isSkipUpdateSOStatusOptionAll;

        private void SOStatusOption_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                var optionAll = SOStatusListOptions.FirstOrDefault(obj => obj.Code == -999);
                if (optionAll == null)
                    return;

                if (((SOStatusOption)sender).Code == -999)
                {
                    if (isSkipUpdateSOStatusOptionAll)
                        return;

                    SOStatusListOptions.Where(obj => obj.Code != ((SOStatusOption)sender).Code)
                        .ForEach(obj => obj.IsChecked = ((SOStatusOption)sender).IsChecked);
                }
                else
                {
                    if (((SOStatusOption)sender).IsChecked)
                    {
                        if (SOStatusListOptions.Count(obj => obj.Code != -999 && !obj.IsChecked) == 0 && !optionAll.IsChecked)
                            optionAll.IsChecked = true;
                    }
                    else
                    {
                        if (optionAll.IsChecked)
                        {
                            isSkipUpdateSOStatusOptionAll = true;
                            optionAll.IsChecked = false;
                            isSkipUpdateSOStatusOptionAll = false;
                        }
                    }
                }
            }
        }

        private List<int> m_SOStatusList;
        public List<int> SOStatusList
        {
            get { return m_SOStatusList; }
            set { base.SetValue("SOStatusList", ref m_SOStatusList, value); }
        }

        private ObservableCollection<SOStatusOption> m_SOStatusListOptions;
        public ObservableCollection<SOStatusOption> SOStatusListOptions
        {
            get { return m_SOStatusListOptions; }
            set { base.SetValue("SOStatusListOptions", ref m_SOStatusListOptions, value); }
        }

        private DateTime? m_SODateFrom;
        public DateTime? SODateFrom
        {
            get
            {
                return this.m_SODateFrom;
            }
            set
            {
                this.SetValue("SODateFrom", ref m_SODateFrom, value);
            }
        }

        private DateTime? m_SODateTo;
        public DateTime? SODateTo
        {
            get
            {
                return this.m_SODateTo;
            }
            set
            {
                this.SetValue("SODateTo", ref m_SODateTo, value);
            }
        }

        private string m_ProductID;
        public string ProductID
        {
            get { return m_ProductID; }
            set { base.SetValue("ProductID", ref m_ProductID, value); }
        }

        private string m_BrandName;
        public string BrandName
        {
            get { return m_BrandName; }
            set { base.SetValue("BrandName", ref m_BrandName, value); }
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

        private ObservableCollection<int> m_VendorSysNoList;
        public ObservableCollection<int> VendorSysNoList
        {
            get { return m_VendorSysNoList; }
            set { base.SetValue("VendorSysNoList", ref m_VendorSysNoList, value); }
        }

        private List<string> m_WarehouseNumberList;
        public List<string> WarehouseNumberList
        {
            get { return m_WarehouseNumberList; }
            set { base.SetValue("WarehouseNumberList", ref m_WarehouseNumberList, value); }
        }

        private ObservableCollection<WarehouseNumberOption> m_WarehouseNumberOption;
        public ObservableCollection<WarehouseNumberOption> WarehouseNumberOption
        {
            get { return m_WarehouseNumberOption; }
            set { base.SetValue("WarehouseNumberOption", ref m_WarehouseNumberOption, value); }
        }
    }

    public class WarehouseNumberOption : ModelBase
    {
        private bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { base.SetValue("IsChecked", ref isChecked, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { base.SetValue("Name", ref _name, value); }
        }
        private string _code;
        public string Code
        {
            get { return _code; }
            set { base.SetValue("Code", ref _code, value); }
        }
    }
}
