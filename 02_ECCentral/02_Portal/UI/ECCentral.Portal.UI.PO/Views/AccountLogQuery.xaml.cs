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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.PO.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class AccountLogQuery : PageBase
    {
        public ConsignToAccountLogQueryVM queryVM;
        public ConsignToAccountLogQueryFilter queryFilter;
        public ConsignToAccountLogFacade serviceFacade;

        public AccountLogQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryVM = new ConsignToAccountLogQueryVM();
            serviceFacade = new ConsignToAccountLogFacade(this);
            LoadComboBoxData();
            this.DataContext = queryVM;

            queryFilter = new ConsignToAccountLogQueryFilter()
            {
                PageInfo = new QueryFilter.Common.PagingInfo()
            };
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                //如果有传入的SysNo参数:
                string getSysNoList = this.Request.Param;

                string[] arr = getSysNoList.Split(new Char[] { '-' });
                List<int> sysnoList = new List<int>();
                for (int i = 0; i < arr.Length; i++)
                {
                    sysnoList.Add(Convert.ToInt32(arr[i]));
                }
                queryVM.SysNoList = sysnoList;
                btnSearch_Click(null, null);
            }
            else
            {

                queryFilter = new ConsignToAccountLogQueryFilter()
                {
                    PageInfo = new PagingInfo()
                };
            }
        }

        private void LoadComboBoxData()
        {
            //代销转财务结算单状态:
            this.cmbStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ConsignToAccountLogStatus>(EnumConverter.EnumAppendItemType.All);
            this.cmbStatus.SelectedIndex = 0;
            //代销转财务结算单类型:
            this.cmbReferenceType.ItemsSource = EnumConverter.GetKeyValuePairs<ConsignToAccountType>(EnumConverter.EnumAppendItemType.All);
            this.cmbReferenceType.SelectedIndex = 0;
            //结算类型:
            this.cmbSettleType.ItemsSource = EnumConverter.GetKeyValuePairs<SettleType>(EnumConverter.EnumAppendItemType.All);
            this.cmbSettleType.SelectedIndex = 0;
        }

        #region [Events]
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null) queryVM.SysNoList = null;
            queryFilter = EntityConverter<ConsignToAccountLogQueryVM, ConsignToAccountLogQueryFilter>.Convert(queryVM);
            this.QueryResultGrid.Bind();
        }
        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryFilter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = QueryResultGrid.PageSize,
                PageIndex = QueryResultGrid.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.PageInfo.SortBy = e.SortField;
            serviceFacade.QueryConsignToAccountLog(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var consignList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;
                QueryResultGrid.TotalCount = totalCount;
                QueryResultGrid.ItemsSource = consignList;
                //读取统计信息:
                QueryTotalAmtStatistics(queryFilter);
            });
        }

        private void QueryTotalAmtStatistics(ConsignToAccountLogQueryFilter queryFilter)
        {
            decimal totalCreateCost = 0;
            decimal totalSettleCost = 0;
            decimal totalFoldCost = 0;
            serviceFacade.QueryConsignToAccountLogTotalAmt(queryFilter, (obj, args) =>
           {
               if (args.FaultsHandle())
               {
                   return;
               }
               var getTotalData = args.Result.Rows;
               try
               {
                   totalCreateCost = getTotalData[0] == null || getTotalData[0]["CreateCost"] == null ? 0 : decimal.Parse(getTotalData[0]["CreateCost"].ToString());
                   totalSettleCost = getTotalData[0] == null || getTotalData[0]["SettleCost"] == null ? 0 : decimal.Parse(getTotalData[0]["SettleCost"].ToString());
               }
               catch
               {

               }
               this.lblShowTotalAmt.Visibility = Visibility.Visible;
               this.lblShowTotalAmt.Text = string.Format(ResAccountLogQuery.Label_TotalAmtFormatString, totalCreateCost.ToString("N"), totalFoldCost.ToString("N"), totalSettleCost.ToString("N"));
           });

        }
        #endregion

        private void QueryResultGrid_ExportAllClick(object sender, EventArgs e)
        {
            //权限控制:
            //导出全部数据:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Consign_ExportConsignToAccountList))
            {
                Window.Alert("对不起，你没有权限进行此操作!");
                return;
            }

            //导出全部:
            if (null != queryFilter)
            {
                ConsignToAccountLogQueryFilter exportQueryRequest = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<ConsignToAccountLogQueryFilter>(queryFilter);
                exportQueryRequest.PageInfo = new QueryFilter.Common.PagingInfo() { PageIndex = 0, PageSize = ConstValue.MaxRowCountLimit };

                ColumnSet columnSet = new ColumnSet(QueryResultGrid, true);
                serviceFacade.ExportDataDForConsignToAccountLog(exportQueryRequest, new ColumnSet[] { columnSet });
            }
        }

        /// <summary>
        /// 查看代销转财务信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.QueryResultGrid.SelectedItem as dynamic;
            if (data != null)
            {
                UCViewAccountLog uc = new UCViewAccountLog(data);
                uc.Dialog = this.Window.ShowDialog(ResAccountLogQuery.Title_AccountLogInfo, uc);
            }
        }

        /// <summary>
        /// 查看商品信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperLink_ProductSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.QueryResultGrid.SelectedItem as dynamic;
            if (data != null && data.ProductSysNo != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.PO_ProductMaintain, int.Parse(data.ProductSysNo.ToString())), null, true);
            }
        }

        //查看供应商信息
        private void HyperLink_VendorSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.QueryResultGrid.SelectedItem as dynamic;
            if (data != null
                && data.VendorSysNo != null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.PO_VendorMaintain, int.Parse(data.VendorSysNo.ToString())), null, true);
            }
        }

        /// <summary>
        /// 查看单据信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperLink_OrderSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.QueryResultGrid.SelectedItem as dynamic;
            if (data != null
                && data.OrderSysNo != null)
            {
                var type = (ConsignToAccountType?)data.ReferenceType;
                switch (type)
                {
                    case ConsignToAccountType.SO:
                        CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, data.OrderSysNo.ToString()), null, true);
                        break;
                    case ConsignToAccountType.Adjust:
                        CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ConstValue.Inventory_AdjustRequestMaintainUrlFormat, data.OrderSysNo.ToString()), null, true);
                        break;
                }

            }
        }
    }

}
