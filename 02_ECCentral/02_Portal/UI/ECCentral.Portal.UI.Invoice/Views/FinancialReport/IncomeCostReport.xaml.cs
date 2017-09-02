using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Resources;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.Service.Utility;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.ObjectModel;

namespace ECCentral.Portal.UI.Invoice.Views.FinancialReport
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class IncomeCostReport : PageBase
    {
        #region private fileds
        private IncomeCostReportQueryVM queryVM;
        private IncomeCostReportQueryVM lastQueryVM;
        private FinancialReportFacade financialReportFacade;
        #endregion

        public IncomeCostReport()
        {
            InitializeComponent();
        }

        private void IncomeCostReport_OnOnLoad(object sender, EventArgs e)
        {
            InitData();
            financialReportFacade = new FinancialReportFacade(this);
        }

        private void InitData()
        {
            queryVM = new IncomeCostReportQueryVM();
            gridQueryBuilder.DataContext = lastQueryVM = queryVM;
            queryVM.SOStatusListOptions.ForEach(option => option.PropertyChanged += OnSOStatusCheck);
            SetSOStatusText();
        }
        
        private void OnSOStatusCheck(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                SetSOStatusText();
            }
        }

        private void DataGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (lastQueryVM == null || this.dgdResult.TotalCount <= 0)
            {
                Window.Alert(ResCommon.Message_NoData2Export);
                return;
            }
            financialReportFacade.IncomeCostReportExportExcelFile(lastQueryVM, new[] { new ColumnSet(dgdResult) });
        }

        private void DataGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            financialReportFacade.IncomeCostReportQuery(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, resultVM =>
            {
                dgdResult.ItemsSource = resultVM.ResultList;
                dgdResult.TotalCount = resultVM.TotalCount;

                if (resultVM.ResultList != null && resultVM.ResultList.Count > 0)
                {
                    svStatisticInfo.Visibility = Visibility.Visible;
                    txtCurrentPageStatisticInfo.Text = resultVM.StatisticList[0].ToStatisticText();
                    txtAllStatisticInfo.Text = resultVM.StatisticList[1].ToStatisticText();
                }
                else
                {
                    svStatisticInfo.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            bool flag = ValidationManager.Validate(this.gridQueryBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone(queryVM);
                this.dgdResult.Bind();
            }
        }

        private void Hyperlink_OrderID_Click(object sender, RoutedEventArgs e)
        {
            var cur = this.dgdResult.SelectedItem as IncomeCostReportVM;
            if (cur != null)
            {
                string url = String.Format(ConstValue.SOMaintainUrlFormat, cur.SOSysNo);
                this.Window.Navigate(url, null, true);
            }
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = this.dgdResult.ItemsSource as List<IncomeCostReportVM>;
            if (dataSource != null)
            {
                dataSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }

        private void ComboBox_SOStatusList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_SOStatusList.SelectedItem = null;
            SetSOStatusText();
        }

        private void SetSOStatusText()
        {
            var textBuilder = new StringBuilder();
            
            if (queryVM.SOStatusListOptions.ToList().Exists(option => option.IsChecked && option.Code == -999))
            {
                queryVM.SOStatusList = new List<int>();
                textBuilder.Append(BizEntity.Enum.Resources.ResCommonEnum.Enum_All);
            }
            else
            {
                queryVM.SOStatusList = queryVM.SOStatusListOptions.ToList().Where(option => option.IsChecked).Select(option => option.Code).ToList();
                queryVM.SOStatusListOptions.Where(option => option.IsChecked).ForEach(option => textBuilder.Append(option.Name).Append(","));
            }

            TextBlock_SOStatusComboBoxText.Text = textBuilder.ToString().TrimEnd(',');
        }

        private void TextBlock_SOStatusComboBoxText_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ComboBox_SOStatusList.IsDropDownOpen = true;
        }

        private void btnChooseVendor_Click(object sender, RoutedEventArgs e)
        {
            UCVendorQuery selectDialog = new UCVendorQuery();
            selectDialog.SelectionMode = SelectionMode.Multiple;
            List<string> vendorList = new List<string>();
            queryVM.VendorSysNoList = new ObservableCollection<int>();
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("供应商查询", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    List<DynamicXml> getSelectedVendors = args.Data as List<DynamicXml>;
                    if (null != getSelectedVendors)
                    {
                        foreach (DynamicXml getSelectedVendor in getSelectedVendors)
                        {
                            vendorList.Add(getSelectedVendor["VendorName"].ToString());
                            queryVM.VendorSysNoList.Add(Convert.ToInt32(getSelectedVendor["SysNo"]));
                        }
                        this.txtVendorName.Text = vendorList.Join(",");
                    }
                }
            }, new Size(750, 650));
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //重置查询条件:
            InitData();
            this.txtVendorName.Text = string.Empty;
        }
    }
}
