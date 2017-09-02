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
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCRefundPrepayCard : PopWindow
    {
        public UCRefundPrepayCard()
        {
            InitializeComponent();
        }

        public AuditRefundVM VM
        {
            get;
            set;
        }

        private RefundPrepayCardVM refundVM;

        private AuditRefundFacade facade;

        public UCRefundPrepayCard(AuditRefundVM vm)
        {
            InitializeComponent();
            this.VM = vm;
            Loaded += new RoutedEventHandler(UCRefundPrepayCard_Loaded);
        }

        void UCRefundPrepayCard_Loaded(object sender, RoutedEventArgs e)
        {
            BindComboxData();
            facade = new AuditRefundFacade();
            this.DataContext = VM;
            refundVM = new RefundPrepayCardVM()
            {
                SOIncomeBankInfoSysNo = VM.SysNo
            };
        }

        //绑定下拉框数据
        private void BindComboxData()
        {
            this.cmbAuditStatus.ItemsSource = EnumConverter.GetKeyValuePairs<RefundStatus>();
            this.cmbAuditStatus.SelectedIndex = 0;

            this.cmbOrderType.ItemsSource = EnumConverter.GetKeyValuePairs<RefundOrderType>();
            this.cmbOrderType.SelectedIndex = 0;

            new OtherDomainDataFacade(CurrentPage).GetRefundReaons(true, (obj, args) =>
                {
                    this.cmbRMAReason.ItemsSource = args.Result;
                    int i = 0;
                    for (; i < args.Result.Count; i++)
                    {
                        if (args.Result[i].Name == VM.RMAReason)
                            break;
                    }
                    this.cmbRMAReason.SelectedIndex = i;
                });
        }

        //退预付卡
        private void btnRefundPrepay_Click(object sender, RoutedEventArgs e)
        {
            facade.RefundPrepayCard(refundVM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        this.btnRefundPrepay.IsEnabled = false;
                    else
                        CloseDialog(DialogResultType.OK);
                });
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseDialog(DialogResultType.Cancel);
        }

        //订单号链接
        private void hlbtnSOSysNo_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentWindow.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, this.VM.SOSysNo), null, true);
        }

        //退款单号链接
        private void hlbtnRMANumber_Click(object sender, RoutedEventArgs e)
        {
            if (this.VM.OrderType == RefundOrderType.RO)
            {
                this.CurrentWindow.Navigate(string.Format(ConstValue.RMA_RefundMaintainUrl, this.VM.RMANumber), null, true);
            }
            else if (this.VM.OrderType == RefundOrderType.AO || this.VM.OrderType == RefundOrderType.OverPayment)
            {
                this.CurrentWindow.Navigate(string.Format(ConstValue.SOMaintainUrlFormat, this.VM.RMANumber), null, true);
            }
          
        }
    }
}
