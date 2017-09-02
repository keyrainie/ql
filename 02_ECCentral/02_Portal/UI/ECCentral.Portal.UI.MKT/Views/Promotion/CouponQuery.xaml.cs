using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.Service.Utility;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class CouponQuery : PageBase
    {
        private CouponsQueryFilterViewModel _filterVM = new CouponsQueryFilterViewModel();

        private CouponsFacade _facade;

        public CouponQuery()
        {
            InitializeComponent();

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            List<WebChannelVM> webChennelList = new List<WebChannelVM>();
            foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            {
                webChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            }
            webChennelList.Insert(0, new WebChannelVM() { ChannelName = ResCommonEnum.Enum_Select });
            lstChannel.ItemsSource = webChennelList;
            lstChannel.SelectedValue = 1;

            List<KeyValuePair<CouponsStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<CouponsStatus>();
            statusList.Insert(0, new KeyValuePair<CouponsStatus?, string>(null, ResCommonEnum.Enum_Select));
            lstStatus.ItemsSource = statusList;
            lstStatus.SelectedIndex = 0;

            GridQueryFilter.DataContext = _filterVM;
            btnStackPanel.DataContext = _filterVM;
            _facade = new CouponsFacade(this);

        }

        private void GridQueryFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonSearch_Click(ButtonSearch, new RoutedEventArgs());
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.GridQueryFilter))
            {
                this.dgResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<CouponsQueryFilterViewModel>(this._filterVM);

                dgResult.Bind();
            }
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_CouponMaintainUrlFormat, ""), null, true);
        }

        private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = dgResult.ItemsSource;
            foreach (dynamic row in rows)
            {
                CouponsStatus status = CouponsStatus.Finish;
                Enum.TryParse<CouponsStatus>(row.Status.ToString(), out status);

                if (status == CouponsStatus.Init || status == CouponsStatus.Ready
                    || status == CouponsStatus.Run || status == CouponsStatus.WaitingAudit)
                {
                    row.IsChecked = chk.IsChecked.Value ;
                }
                else
                {
                    row.IsChecked = false;
                }
            }        
        }

        private void dgResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            var vm = this.dgResult.QueryCriteria as CouponsQueryFilterViewModel;
            vm.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            _facade.Query(vm, (obj, args) =>
            {
                dgResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                dgResult.TotalCount = args.Result.TotalCount;

            });
        }

        private void hybtnCouponName_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            var sysno = row.SysNo;
            this.Window.Navigate(string.Format(ConstValue.MKT_CouponMaintainUrlFormat + "?sysno={0}", sysno), null, true);

        }

        private void hybtnEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            var sysno = row.SysNo;

            this.Window.Navigate(string.Format(ConstValue.MKT_CouponMaintainUrlFormat + "?sysno={0}&operation=edit", sysno), null, true);
        }

        private void hybtnMgt_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            var sysno = row.SysNo;
            this.Window.Navigate(string.Format(ConstValue.MKT_CouponMaintainUrlFormat + "?sysno={0}&operation=mgt", sysno), null, true);
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            BatchProcess(PSOperationType.Void, ResCouponQuery.Msg_Void);
        }

        private void btnBatchSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
            BatchProcess(PSOperationType.SubmitAudit, ResCouponQuery.Msg_SubAudit);
        }

        private void btnBatchAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            BatchProcess(PSOperationType.AuditRefuse, ResCouponQuery.Msg_RefuseAudit);
        }

        private void btnBatchAuditPass_Click(object sender, RoutedEventArgs e)
        {
            BatchProcess(PSOperationType.AuditApprove, ResCouponQuery.Msg_ApproveAudit);
        }

        private void btnBatchStop_Click(object sender, RoutedEventArgs e)
        {
            BatchProcess(PSOperationType.Stop, ResCouponQuery.Msg_StopAudit);
        }

        private void btnBatchCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            BatchProcess(PSOperationType.CancelAudit, ResCouponQuery.Msg_CancelSudit);
        }

        private void BatchProcess(PSOperationType opt, string optname)
        {
            dynamic rows = dgResult.ItemsSource;
            if (rows == null)
            {
                Window.Alert(ResCouponQuery.Msg_SelcOneMoreRecords);
                return;
            }
            List<int?> sysNoList = new List<int?>();
            foreach (dynamic row in rows)
            {
                if (row.IsChecked )
                {
                    sysNoList.Add(row.SysNo);
                }
            }
            if (sysNoList.Count == 0)
            {
                Window.Alert(ResCouponQuery.Msg_SelcOneMoreRecords);
                return;
            }
            _facade.BatchProcessCoupons(sysNoList, opt, (obj, args) =>
            {
                if (args.Result.FailureRecords.Count == 0)
                {
                    Window.Alert(string.Format(ResCouponQuery.Msg_BatchDealSuccess, optname));
                }
                else
                {
                    string msg = args.Result.FailureRecords.Join(Environment.NewLine) + Environment.NewLine;
                    if (args.Result.SuccessRecords.Count > 0)
                    {
                        msg += ResCouponQuery.Msg_DealSuccess + Environment.NewLine;
                        msg += args.Result.SuccessRecords.Join(Environment.NewLine);
                    }

                    Window.Alert(msg);
                }
                ButtonSearch_Click(this.ButtonSearch, new RoutedEventArgs());
            });
        }

        private void btnViewUsedLog_Click(object sender, RoutedEventArgs e)
        {
            //dynamic rows = dgResult.ItemsSource;
            //if (rows == null || dgResult.SelectedItem == null)
            //{
            //    Window.Alert(ResCouponQuery.Msg_SlecOneRecord);
            //    return;
            //}
            //dynamic row = dgResult.SelectedItem;
            //int sysno = row.SysNo;
            //this.Window.Navigate(string.Format(ConstValue.MKT_CouponCodeRedeemLogMaintainUrlFormat + "?sysno={0}", sysno), null, true);
            this.Window.Navigate(ConstValue.MKT_CouponCodeRedeemLogMaintainUrlFormat, null, true);
        }

        private void btnViewGetLog_Click(object sender, RoutedEventArgs e)
        {
            //dynamic rows = dgResult.ItemsSource;
            //if (rows == null || dgResult.SelectedItem==null)
            //{
            //    Window.Alert(ResCouponQuery.Msg_SlecOneRecord);
            //    return;
            //}
            //dynamic row = dgResult.SelectedItem;
            //int sysno = row.SysNo;
            //this.Window.Navigate(string.Format(ConstValue.MKT_CouponCodeCustomerLogMaintainUrlFormat + "?sysno={0}", sysno), null, true);
            this.Window.Navigate(ConstValue.MKT_CouponCodeCustomerLogMaintainUrlFormat, null, true);
        }
    }
}
