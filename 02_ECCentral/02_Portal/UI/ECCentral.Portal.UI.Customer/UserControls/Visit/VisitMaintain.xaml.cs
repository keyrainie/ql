using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using System;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Customer.Resources;
using System.Collections.Generic;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class VisitMaintain : UserControl
    {
        private CustomerVisitMaintainView PageView;
        public IDialog Dialog
        {
            private get;
            set;
        }
        public int CustomerSysNo
        {
            private get;
            set;
        }
        public bool IsOrderVisit
        {
            get;
            set;
        }
        CustomerVisitFacade facade = null;
        public VisitMaintain()
        {
            InitializeComponent();
            facade = new CustomerVisitFacade();
            PageView = new CustomerVisitMaintainView()
            {
                Log = new VisitLogVM()
                {
                    CallResult = VisitCallResult.Connected,
                    DealStatus = VisitDealStatus.FollowUp,
                    ConsumeDesire = YNStatusThree.Uncertain
                },
                Customer = new CustomerMaster()
            };
            Loaded += new RoutedEventHandler(VisitMaintain_Loaded);
            tbcVisit.SelectionChanged += new SelectionChangedEventHandler(tbcVisit_SelectionChanged);
        }
        IWindow Window;
        public VisitMaintain(IWindow window)
            : this()
        {
            Window = window;
        }
        void VisitMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            ucVisitLogList.IsOrderVisit = IsOrderVisit;
            cmbCallResult.ItemsSource = EnumConverter.GetKeyValuePairs<VisitCallResult>(EnumConverter.EnumAppendItemType.None);
            cmbDealStatus.ItemsSource = EnumConverter.GetKeyValuePairs<VisitDealStatus>(EnumConverter.EnumAppendItemType.None);
            cmbConsumeDesire.ItemsSource = EnumConverter.GetKeyValuePairs<YNStatusThree>(EnumConverter.EnumAppendItemType.None);

            PageView.Customer.SysNo = CustomerSysNo;
            PageView.Log.CustomerSysNo = CustomerSysNo;
            facade.GetCustomerInfo(CustomerSysNo, (customerMaster) =>
            {
                PageView.Customer = customerMaster;
                ucCustomerBaseInfo.CustomerInfo = PageView.Customer;
                PageView.Log.CustomerID = PageView.Customer.CustomerID;
            });
            spVisitLog.DataContext = PageView.Log;
            if (IsOrderVisit)
            {
                GetVistOrder(CustomerSysNo);
            }
            SetControlsShow();
            cbRemind_Click(cbRemind, new RoutedEventArgs());
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            PreCheck();
            if (PageView.Log.HasValidationErrors)
                return;
            if (cbRemind.IsChecked.HasValue && cbRemind.IsChecked.Value)
            {
                if (PageView.Log.RemindDate == null || PageView.Log.RemindDate < DateTime.Now)
                {
                    ShowMessage(ResCustomerVisit.RemindDate);
                    return;
                }
            }
            else
            {
                PageView.Log.RemindDate = null;
            }
            facade.AddLog(PageView, () =>
            {
                Window.Alert(ResVisitMaintain.msg_title_Info, ResVisitMaintain.msg_content_saveSucess, MessageType.Information, (o, s) =>
                    {
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        if (Dialog != null) Dialog.Close();
                    });

            });
        }

        private void PreCheck()
        {
            if (PageView.Log.Note == null)
                PageView.Log.Note = string.Empty;
            if (PageView.Log.QQ == null)
                PageView.Log.QQ = string.Empty;
            if (PageView.Log.MSN == null)
                PageView.Log.MSN = string.Empty;
        }

        private void tbcVisit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbcVisit.SelectedIndex == 1 && ucVisitLogList.Logs == null)
            {
                facade.QueryVisitLog(IsOrderVisit, CustomerSysNo, (logList) =>
                {
                    PageView.LogList = logList;
                    ucVisitLogList.Logs = PageView.LogList;
                });
            }
        }
        private void SetControlsShow()
        {
            txtMSN.Visibility = txtQQ.Visibility = tbkMSN.Visibility = tbkQQ.Visibility = IsOrderVisit ? Visibility.Collapsed : Visibility.Visible;
            tbkOrderSysNo.Visibility = hlbtnOrderSysNo.Visibility = IsOrderVisit ? Visibility.Visible : Visibility.Collapsed;
            this.tbiAddLog.Header = IsOrderVisit ? ResVisitMaintain.tab_AddMaintainLog : ResVisitMaintain.tab_AddVisitLog;
            this.tbiHistory.Header = IsOrderVisit ? ResVisitMaintain.tab_MaintainHistory : ResVisitMaintain.tab_VisitHistory;
        }

        private void hlbtnOrderSysNo_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            string url = String.Format(ConstValue.SOMaintainUrlFormat, PageView.Log.SoSysNo);
            this.Window.Navigate(url, null, true);
        }

        private void cbRemind_Click(object sender, RoutedEventArgs e)
        {
            ucDataTime.IsEnabled = cbRemind.IsChecked.HasValue && cbRemind.IsChecked.Value;
        }
        private void ShowMessage(string message)
        {
            Window.Alert(message);
        }

        //处理状态选择事件
        private void cmbDealStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageView.Log.DealStatus != null)
            {
                switch (PageView.Log.DealStatus)
                {
                    case VisitDealStatus.Complete:
                        this.txtbCallResult.Visibility = this.cmbCallResult.Visibility = System.Windows.Visibility.Collapsed;
                        this.cbRemind.Visibility = this.ucDataTime.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case VisitDealStatus.Failed:
                        this.cbRemind.Visibility = this.ucDataTime.Visibility = System.Windows.Visibility.Collapsed;
                        this.txtbCallResult.Visibility = this.cmbCallResult.Visibility = System.Windows.Visibility.Visible;
                        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<VisitCallResult?, string>> list = EnumConverter.GetKeyValuePairs<VisitCallResult>(EnumConverter.EnumAppendItemType.None);
                        list.Remove(new System.Collections.Generic.KeyValuePair<VisitCallResult?, string>(VisitCallResult.OtherTime, "另约时间"));
                        this.cmbCallResult.ItemsSource = list;
                        this.cmbCallResult.SelectedIndex = 0;
                        break;
                    case VisitDealStatus.FollowUp:
                        this.cbRemind.Visibility = this.ucDataTime.Visibility = System.Windows.Visibility.Visible;
                        this.txtbCallResult.Visibility = this.cmbCallResult.Visibility = System.Windows.Visibility.Visible;
                        this.cmbCallResult.ItemsSource = EnumConverter.GetKeyValuePairs<VisitCallResult>(EnumConverter.EnumAppendItemType.None);
                        this.cmbCallResult.SelectedIndex = 0;
                        break;
                }
            }
        }

        /// <summary>
        /// 加载维护订单
        /// </summary>
        /// <param name="customerSysNo"></param>
        private void GetVistOrder(int customerSysNo)
        {
            facade.GetVisitOrderByCustomerSysNo(customerSysNo, (VisitOrders) =>
                {
                    var result = new List<string>();

                    VisitOrders.ForEach(s =>
                        {
                            result.Add(s.SoSysNo.ToString());
                        });
                    if (result.Count > 0)
                    {
                        PageView.Log.SoSysNo = int.Parse(result[0]);
                        this.hlbtnOrderSysNo.Content = PageView.Log.SoSysNo;
                    }
                    else
                        PageView.Log.SoSysNo = null;
                });
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            if (Dialog != null) Dialog.Close();
        }
    }
}
