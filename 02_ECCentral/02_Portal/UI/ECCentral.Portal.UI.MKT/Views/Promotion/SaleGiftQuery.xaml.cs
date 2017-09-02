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
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SaleGiftQuery : PageBase
    {
        private SaleGiftQueryFilterViewModel _filterVM = new SaleGiftQueryFilterViewModel();
        private SaleGiftFacade _facade;

        public SaleGiftQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            //List<WebChannelVM> webChennelList = new List<WebChannelVM>();
            //foreach (UIWebChannel uiChannel in CPApplication.Current.CurrentWebChannelList)
            //{
            //    webChennelList.Add(new WebChannelVM() { ChannelID = uiChannel.ChannelID, ChannelName = uiChannel.ChannelName });
            //}

            //webChennelList.Insert(0, new WebChannelVM() { ChannelName = ResCommonEnum.Enum_Select });
            //lstChannel.ItemsSource = webChennelList;
            //lstChannel.SelectedIndex = 0;

            List<KeyValuePair<SaleGiftStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<SaleGiftStatus>();
            statusList.Insert(0, new KeyValuePair<SaleGiftStatus?, string>(null, ResCommonEnum.Enum_Select));
            cmbStatus.ItemsSource = statusList;
            cmbStatus.SelectedIndex = 0;

            List<KeyValuePair<SaleGiftType?, string>> typeList = EnumConverter.GetKeyValuePairs<SaleGiftType>();
            typeList.Insert(0, new KeyValuePair<SaleGiftType?, string>(null, ResCommonEnum.Enum_Select));
            cmbType.ItemsSource = typeList;
            cmbType.SelectedIndex = 0;

            _filterVM.ChannelID = "1";

            gridFilter.DataContext = _filterVM;
            btnStackPanel.DataContext = _filterVM;

            _facade = new SaleGiftFacade(this);
            
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.gridFilter) && !_filterVM.HasValidationErrors)
            {
                //if (ucPM.SelectedPMSysNo != null)
                //{
                //    _filterVM.PMUser = ucPM.SelectedPMName;
                //}
                //else
                //{
                //    _filterVM.PMUser = string.Empty;
                //}
                this.dgResult.QueryCriteria = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<SaleGiftQueryFilterViewModel>(_filterVM);

                dgResult.Bind();
            }
        }

        private void gridFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonSearch_Click(this.ButtonSearch, new RoutedEventArgs());
            }
        }
        
        private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            dynamic rows = dgResult.ItemsSource;
            foreach (dynamic row in rows)
            {
                SaleGiftStatus status = SaleGiftStatus.Finish;
                Enum.TryParse<SaleGiftStatus>(row.Status.ToString(), out status);

                if (status == SaleGiftStatus.Init || status == SaleGiftStatus.Ready
                    || status == SaleGiftStatus.Run || status == SaleGiftStatus.WaitingAudit)
                {
                    row.IsChecked = chk.IsChecked.Value;
                }
                else
                {
                    row.IsChecked = false;
                }
            }        
        }

        private void hybtnSysNo_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            var sysno = row.SysNo;
            string url = string.Format(ConstValue.MKT_SaleGiftMaintainUrlFormat + "?sysno={0}", sysno);
            this.Window.Navigate(url, null, true);
        }

        private void hybtnEdit_Click(object sender, RoutedEventArgs e)
        {
            
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            var sysno = row.SysNo;
            string url = string.Format(ConstValue.MKT_SaleGiftMaintainUrlFormat + "?sysno={0}&operation=edit", sysno);
            this.Window.Navigate(url, null, true);
        }

        private void hybtnMgt_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            var sysno = row.SysNo;
            string url = string.Format(ConstValue.MKT_SaleGiftMaintainUrlFormat + "?sysno={0}&operation=mgt", sysno);
            this.Window.Navigate(url, null, true);
        }

        private void dgResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {                        
            var vm = this.dgResult.QueryCriteria as SaleGiftQueryFilterViewModel;
            vm.CompanyCode = CPApplication.Current.CompanyCode;
            vm.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };           
            _facade.Query(vm, (obj, args) =>
            {
                dgResult.ItemsSource = args.Result.Rows.ToList("IsChecked",false);
                dgResult.TotalCount = args.Result.TotalCount;

            });             
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(ConstValue.MKT_SaleGiftMaintainUrlFormat, null, true);
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            // BatchProcess(PSOperationType.Void, "作废");
            BatchProcess(PSOperationType.Void,ResSaleGiftQuery.Msg_Void);
        }

        private void btnBatchSubmitAudit_Click(object sender, RoutedEventArgs e)
        {
           // BatchProcess(PSOperationType.SubmitAudit, "提交审核");
            BatchProcess(PSOperationType.SubmitAudit, ResSaleGiftQuery.Msg_SubAudit);
        }

        private void btnBatchAuditRefuse_Click(object sender, RoutedEventArgs e)
        {
            //BatchProcess(PSOperationType.AuditRefuse, "审核拒绝");
            BatchProcess(PSOperationType.AuditRefuse, ResSaleGiftQuery.Msg_RefuseAudit);

        }

        private void btnBatchAuditPass_Click(object sender, RoutedEventArgs e)
        {
            //BatchProcess(PSOperationType.AuditApprove, "审核通过");
            BatchProcess(PSOperationType.AuditApprove, ResSaleGiftQuery.Msg_ApproveAudit);

        }

        private void btnBatchStop_Click(object sender, RoutedEventArgs e)
        {
            //BatchProcess(PSOperationType.Stop, "中止");
            BatchProcess(PSOperationType.Stop, ResSaleGiftQuery.Msg_StopAudit);
        }

        private void btnBatchCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            //BatchProcess(PSOperationType.CancelAudit, "撤销审核");
            BatchProcess(PSOperationType.CancelAudit, ResSaleGiftQuery.Msg_CancelSudit);
        }
          

        private void BatchProcess(PSOperationType opt, string optname)
        {
            Window.MessageBox.Show("商家创建的商品为待审核时不能撤回,如有勾选,系统会自动忽略!", MessageBoxType.Information);
            dynamic rows = dgResult.ItemsSource;
            if (rows == null)
            {
                //Window.Alert("请至少勾选一条数据！");
                Window.Alert(ResSaleGiftQuery.Msg_SelcOneMoreRecords);
                return;
            }
            List<int?> sysNoList = new List<int?>();
            foreach (dynamic row in rows)
            {
                if (row.IsChecked && row.RequestSysNo== 0)
                {
                    sysNoList.Add(row.SysNo);
                }
            }
            if (sysNoList.Count == 0)
            {
               // Window.Alert("请至少勾选一条数据！");
                Window.Alert(ResSaleGiftQuery.Msg_SelcOneMoreRecords);
                return;
            }
            _facade.BatchProcessSaleGift(sysNoList, opt, (obj, args) =>
            {
                if (args.Result.FailureRecords.Count == 0)
                {
                    //Window.Alert(string.Format("批量{0}赠品成功！", optname));
                    Window.Alert(string.Format(ResSaleGiftQuery.Msg_BatchDealSuccess, optname));
                }
                else
                {
                    string msg = args.Result.FailureRecords.Join(Environment.NewLine) + Environment.NewLine;
                    if (args.Result.SuccessRecords.Count > 0)
                    {
                       // msg += "其余赠品活动处理成功:" + Environment.NewLine;
                        msg += ResSaleGiftQuery.Msg_DealSuccess + Environment.NewLine;
                        msg +=  args.Result.SuccessRecords.Join(Environment.NewLine);
                    }
                    Window.Alert(msg);
                }
                ButtonSearch_Click(this.ButtonSearch, new RoutedEventArgs());
            });
        }

        private void hybtnCopyNew_Click(object sender, RoutedEventArgs e)
        {
            dynamic row = dgResult.SelectedItem;
            if (row == null) return;
            int? sysno = row.SysNo;
            _facade.CopyCreateNew(sysno, (obj, args) =>
                {
                   // Window.Alert("复制创建赠品成功，新赠品活动的系统编号为：" +args.Result.ToString());
                    Window.Alert(ResSaleGiftQuery.Msg_CopySuccess + args.Result.ToString());
                });

        }

        /// <summary>
        /// 刷新。
        /// </summary>
        private void Refresh()
        {
            this.ButtonSearch_Click(null, null);
        }

        private void Button_BatchNew_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_BatchCreateSaleGiftUrlFormat, ""), null, true);
        }        
    }
}