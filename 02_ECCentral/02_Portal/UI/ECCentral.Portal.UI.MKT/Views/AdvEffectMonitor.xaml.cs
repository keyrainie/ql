using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Service.MKT.Restful.ResponseMsg;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class AdvEffectMonitor : PageBase
    {
        private AdvEffectQueryFilter filter;
        private AdvEffectMonitorVM model;
        private AdvEffectQueryFilter filterVM;
        private AdvEffectMonitorFacade facade;
        private List<AdvEffectMonitorVM> vmList;
        private string tips = string.Empty;
        private bool loadedSoAmtLevel = false;

        public AdvEffectMonitor()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            model = new AdvEffectMonitorVM();
            model.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            model.ChannelID = "1";       
            QuerySection.DataContext = model;
            this.dpkStartDate.SelectedRangeType = RangeType.Today;
            facade = new AdvEffectMonitorFacade(this);
            filter = new AdvEffectQueryFilter();
            comIsPhone.ItemsSource = EnumConverter.GetKeyValuePairs<NYNStatus>(EnumConverter.EnumAppendItemType.All);
            comIsEmailConfirmed.ItemsSource = EnumConverter.GetKeyValuePairs<NYNStatus>(EnumConverter.EnumAppendItemType.All);
            comIsValidSO.ItemsSource = EnumConverter.GetKeyValuePairs<NYNStatus>(EnumConverter.EnumAppendItemType.All);
            CodeNamePairHelper.GetList("MKT", "AdvEffectMonitorOperationType",CodeNamePairAppendItemType.All, (obj2, args2) =>
            {
                comOperationType.ItemsSource = args2.Result;
            //comOperationType.SelectedIndex = 0;
            });
            CodeNamePairHelper.GetList("MKT", "SOAmtLevel", CodeNamePairAppendItemType.All, (obj2, args2) =>
            {
                cbSOAmtLevel.ItemsSource = args2.Result;
                loadedSoAmtLevel = true;
              //  cbSOAmtLevel.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(cbShowComment_SelectionChanged);
            });
            base.OnPageLoad(sender, e);
        }

        /// <summary>
        /// 数据全部导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            if (filterVM == null || this.QueryResultGrid.TotalCount < 1)
            {
                Window.Alert(ResNewsInfo.Information_ExportFailed);
                return;
            }
            ColumnSet col = new ColumnSet(this.QueryResultGrid);
            filter = model.ConvertVM<AdvEffectMonitorVM, AdvEffectQueryFilter>();
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()   //这个PagingInfo是我们在ECCentral.BizEntity里自己定义的，只有这3个属性
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = string.Empty
            };

            //col.Insert(0, "ProductId", ResRMAReports.Excel_ProductID, 20) .SetWidth("ProductName", 30);
            facade.ExportExcelFile(filterVM, new ColumnSet[] { col });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            
            decimal Sum = 0;

            facade.QueryAdvEffect(QueryResultGrid.QueryCriteria as AdvEffectQueryFilter, e.PageSize, e.PageIndex, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                vmList = DynamicConverter<AdvEffectMonitorVM>.ConvertToVMList<List<AdvEffectMonitorVM>>(args.Result.Rows);// args.Result.Rows.conv;
               
                if (vmList != null && vmList.Count > 0)
                {
                    btnStatistics.IsEnabled = true;
                    decimal allTotalPrice = 0;// vmList[0].TotalSOAmount.Value;
                    foreach (AdvEffectMonitorVM item in vmList)
                    {
                        if (item.SOAmount.HasValue)
                            Sum += item.SOAmount.Value;
                        if (item.TotalSOAmount > 0)
                            allTotalPrice = item.TotalSOAmount.Value;

                        decimal SOAmount = (item.SOAmount.HasValue ? item.SOAmount.Value : 0) - (item.RefundAmt.HasValue ? item.RefundAmt.Value : 0);
                        if (SOAmount == 0)
                            item.ShowSOAmtLevel = "Z";
                        else if (0 < SOAmount && SOAmount < 100)
                            item.ShowSOAmtLevel = "A";
                        else if (100 < SOAmount && SOAmount < 300)
                            item.ShowSOAmtLevel = "B";
                        else if (300 < SOAmount && SOAmount < 500)
                            item.ShowSOAmtLevel = "C";
                        else if (500 < SOAmount && SOAmount < 800)
                            item.ShowSOAmtLevel = "D";
                        else if (800 < SOAmount && SOAmount < 1000)
                            item.ShowSOAmtLevel = "E";
                        else if (1000 < SOAmount && SOAmount < 1500)
                            item.ShowSOAmtLevel = "F";
                        else if (1500 < SOAmount && SOAmount < 2000)
                            item.ShowSOAmtLevel = "G";
                        else if (2000 < SOAmount && SOAmount < 3000)
                            item.ShowSOAmtLevel = "H";
                        else if (3000 < SOAmount && SOAmount < 5000)
                            item.ShowSOAmtLevel = "I";
                        else if (5000 < SOAmount && SOAmount < 8000)
                            item.ShowSOAmtLevel = "J";
                        else if (8000 < SOAmount && SOAmount < 10000)
                            item.ShowSOAmtLevel = "K";
                        else
                            item.ShowSOAmtLevel = "L";
                    }
                    foreach (var item in vmList)
                    {
                        if (item.IsPhone == null)
                            item.IsPhone = NYNStatus.No;
                    }
                    QueryResultGrid.ItemsSource = vmList;
                    QueryResultGrid.TotalCount = args.Result.TotalCount;
                    tips = string.Format(ResNewsInfo.Content_Statistics, Sum, allTotalPrice);
                }
                else
                {
                    QueryResultGrid.ItemsSource = vmList;
                    QueryResultGrid.TotalCount = 0;
                    btnStatistics.IsEnabled = false;
                }
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QuerySection))
            {
                filter = model.ConvertVM<AdvEffectMonitorVM, AdvEffectQueryFilter>();
                filterVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<AdvEffectQueryFilter>(filter);
                QueryResultGrid.QueryCriteria = this.filter;
                QueryResultGrid.Bind();
            }
        }

        /// <summary>
        /// BBS推广报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBBSClickReport_Click(object sender, RoutedEventArgs e)
        {
            Window.Navigate(ConstValue.MKT_AdvEffectMonitorBBSUrlFormat, null, true);
        }

        /// <summary>
        /// 本页小计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Alert(tips);
        }

        private void cbShowComment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbShowComment.SelectedIndex == 0)
                cbRefund.IsEnabled = false;
            else
            {
                if (loadedSoAmtLevel && (ECCentral.BizEntity.SO.SOStatus)cbShowComment.SelectedValue == ECCentral.BizEntity.SO.SOStatus.OutStock)
                    cbRefund.IsEnabled = true;
                else
                    cbRefund.IsEnabled = false;
            }
        }

        private void comIsValidSO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comIsValidSO.SelectedIndex == 0)
            {        
                cbShowComment.IsEnabled = true;
            }
            else
            {
                cbShowComment.SelectedIndex = 0;
                cbShowComment.IsEnabled = false;
                cbRefund.IsEnabled = false;
                cbRefund.IsChecked = false;
            }
        }
        /// <summary>
        /// 订单连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkSo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.MKT_SOMaintainUrlFormat, btn.Content);
            this.Window.Navigate(url, null, true);
        }
        /// <summary>
        /// 顾客连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkCustomer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.MKT_CustomerMaintainUrlFormat, btn.Tag);
            this.Window.Navigate(url, null, true);
        }
    }

}