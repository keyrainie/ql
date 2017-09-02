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
using ECCentral.QueryFilter.PO;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.PO.Views
{
    [View]
    public partial class CommissionQuery : PageBase
    {
        public CommissionQueryFilter queryRequest;
        public VendorCommissionFacade serviceFacade;
        public VendorCommissionQueryVM queryVM;
        public List<CommissionMasterVM> gridSource;

        public CommissionQuery()
        {
            InitializeComponent();
        }



        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            queryRequest = new CommissionQueryFilter();
            serviceFacade = new VendorCommissionFacade(this);
            queryVM = new VendorCommissionQueryVM();
            gridSource = new List<CommissionMasterVM>();

            this.DataContext = queryVM;
            SetAccessControl();
        }

        private void SetAccessControl()
        {
            //页面权限控制:

            //关闭PO单:
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.PO_Commission_Close))
            {
                this.btnClose.IsEnabled = false;
            }
        }
        #region [Methods]

        #endregion

        #region [Events]

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            if (null != gridSource && 0 < gridSource.Count)
            {
                gridSource.ForEach(w => w.IsChecked = ((CheckBox)sender).IsChecked ?? false);
            }
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            this.queryRequest = EntityConverter<VendorCommissionQueryVM, CommissionQueryFilter>.Convert(queryVM);
            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            queryRequest.PageInfo.PageSize = QueryResultGrid.PageSize;
            queryRequest.PageInfo.PageIndex = QueryResultGrid.PageIndex;
            queryRequest.PageInfo.SortBy = e.SortField;

            serviceFacade.QueryCommissions(queryRequest, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                var commissionList = args.Result.Rows;
                int totalCount = args.Result.TotalCount;

                QueryResultGrid.TotalCount = totalCount;

                gridSource = DynamicConverter<CommissionMasterVM>.ConvertToVMList(commissionList);
                QueryResultGrid.ItemsSource = gridSource;
                //计算总金额:
                serviceFacade.QueryCommissionsTotalAmt(queryRequest, (obj1, args1) =>
                  {
                      if (args1.FaultsHandle())
                      {
                          return;
                      }
                      decimal totalAmt = args1.Result;
                      if (0 < totalAmt)
                      {
                          txtTotalAmountAlertText.Visibility = Visibility.Visible;
                          txtTotalAmountAlertText.Text = string.Format(ResCommissionQuery.InfoMsg_TotalAmt, totalAmt.ToString());
                      }
                      else
                      {
                          txtTotalAmountAlertText.Visibility = Visibility.Collapsed;
                      }
                  });

            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            int totalOperateCount = 0;
            string closeCommissionSysNoList = string.Empty;

            if (QueryResultGrid.ItemsSource != null)
            {
                foreach (object ovj in QueryResultGrid.ItemsSource)
                {
                    CommissionMasterVM item = ovj as CommissionMasterVM;
                    if (item.IsChecked)
                    {
                        totalOperateCount++;
                        closeCommissionSysNoList += string.Format("{0};", item.SysNo.Value.ToString());
                    }
                }
            }
            if (string.IsNullOrEmpty(closeCommissionSysNoList))
            {
                Window.Alert(ResCommissionQuery.InfoMsg_Title, ResCommissionQuery.ErrorMsg_Close, MessageType.Error);
                return;
            }
            //关闭佣金操作:
            serviceFacade.BatchCloseCommissions(closeCommissionSysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    Window.Alert(args.Error.Faults[0].ErrorDescription);
                    return;
                }
                int totalSuccCount = args.Result;
                Window.Alert(string.Format(ResCommissionQuery.InfoMsg_Opertion, totalSuccCount, totalOperateCount - totalSuccCount));
                Window.Refresh();
            });
        }

        private void hpSysNo_Click(object sender, RoutedEventArgs e)
        {
            CommissionMasterVM vm = this.QueryResultGrid.SelectedItem as CommissionMasterVM;
            if (null != vm)
            {
                //结算编号链接:
                Window.Navigate(string.Format(ConstValue.PO_CommissionItemView, vm.SysNo.Value), null, true);
            }
        }

        private void btnNewSO_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(ConstValue.PO_CommissionNew, null, true);
        }

        #endregion
    }

}
