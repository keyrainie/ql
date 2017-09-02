using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.UserControls;
using ECCentral.Portal.UI.Invoice.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views.FinancialReport
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class SOFreightStatReport : PageBase
    {
        private SOFreightStatDetailQueryVM queryVM;
        private FinancialReportFacade reportFacade;

        public SOFreightStatReport()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SOFreightStatReport_Loaded);
        }

        private void SOFreightStatReport_Loaded(object sender, EventArgs e)
        {
            queryVM = new SOFreightStatDetailQueryVM();
            reportFacade = new FinancialReportFacade(this);
            this.DataContext = queryVM;
            
            cmbSOFreightConfirml.ItemsSource = EnumConverter.GetKeyValuePairs<CheckStatus>(EnumConverter.EnumAppendItemType.All);
            cmbSOFreightConfirml.SelectedIndex = 0;

            cmbRealFreightConfirm.ItemsSource = EnumConverter.GetKeyValuePairs<RealFreightStatus>(EnumConverter.EnumAppendItemType.All);
            cmbRealFreightConfirm.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.dgQueryResult.Bind();
        }

        private void dgQueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            if (!ValidationManager.Validate(this)) return;

            reportFacade.QuerySOFreightStatDetai(queryVM, e.PageSize, e.PageIndex, e.SortField, args =>
            {
                this.dgQueryResult.ItemsSource = args.Rows.ToList("IsChecked", false);
                this.dgQueryResult.TotalCount = args.TotalCount;
            });
        }

        private void dgQueryResult_ExportAllClick(object sender, EventArgs e)
        {
            if (!ValidationManager.Validate(this)) return;

            ColumnSet col = new ColumnSet(this.dgQueryResult);
            reportFacade.ExportSOFreightStatDetai(queryVM, new ColumnSet[] { col });
        }

        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgQueryResult.SelectedItem as dynamic;
            if (cur != null)
            {
                string url = String.Format(ConstValue.SOMaintainUrlFormat, cur.SOSysNo);
                this.Window.Navigate(url, null, true);
            }
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var itemSource = dgQueryResult.ItemsSource as dynamic;
            if (itemSource != null)
            {
                foreach (var item in itemSource)
                {
                    item.IsChecked = ((CheckBox)sender).IsChecked ?? false;
                }
            }
        }

        private void btnRealFreightConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count == 0)
            {
                Window.Alert(ResNetPayQuery.Message_AtLeastChooseOneRecord);
                return;
            }

            reportFacade.BatchRealFreightConfirm(selectedSysNoList, msg =>
             {
                 Window.Alert(msg, () => this.dgQueryResult.Bind());
             });
        }

        private void btnSOFreightConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedSysNoList = GetSelectedSysNoList();
            if (selectedSysNoList.Count == 0)
            {
                Window.Alert(ResNetPayQuery.Message_AtLeastChooseOneRecord);
                return;
            }

            reportFacade.BatchSOFreightConfirm(selectedSysNoList, msg =>
            {
                Window.Alert(msg, () => this.dgQueryResult.Bind());
            });
        }

        private List<int> GetSelectedSysNoList()
        {
            var selectedSysNoList = new List<int>();
            var itemSource = dgQueryResult.ItemsSource as dynamic;
            if (itemSource != null)
            {
                foreach (var item in itemSource)
                {
                    if (item.IsChecked)
                    {
                        selectedSysNoList.Add(item.SysNo);
                    }
                }
            }
            return selectedSysNoList;
        }
    }
}
