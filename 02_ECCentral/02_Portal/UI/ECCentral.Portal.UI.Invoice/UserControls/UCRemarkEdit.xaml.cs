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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Resources;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    public partial class UCRemarkEdit : PopWindow
    {
        FinanceVM financeView;

        public FinanceVM FinanceView
        {
            get { return financeView; }
            set { financeView = value; }
        }

        PayableVM payableView;

        public PayableVM PayableView
        {
            get { return payableView; }
            set { payableView = value; }
        }

        FinancialFacade m_facade;

        public UCRemarkEdit()
        {
            InitializeComponent();
        }

        public UCRemarkEdit(FinanceVM view)
        {
            InitializeComponent();
            this.FinanceView = view;
            m_facade = new FinancialFacade();
            Loaded += new RoutedEventHandler(UCRemarkEdit_Loaded);
        }

        void UCRemarkEdit_Loaded(object sender, RoutedEventArgs e)
        {
            QueryHstoryMemo(FinanceView);
            this.InfoGrid.DataContext = FinanceView;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewMemo.Text))
            {
                FinanceView.NewMemo = txtNewMemo.Text;
                m_facade.AddMemo(new PayableVM() { SysNo = FinanceView.SysNo, Memo = FinanceView.Memo, NewMemo = FinanceView.NewMemo }, (obj, args) =>
                    {
                        if (args.FaultsHandle()) return;
                        FinanceView.Memo = args.Result.Memo;
                        this.CloseDialog(DialogResultType.OK);
                    });

            }
            else
            {
                AlertInformationDialog(ResCommon.Message_NotNullRemark);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseDialog(DialogResultType.Cancel);
        }

        private void QueryHstoryMemo(FinanceVM view)
        {
            m_facade.GetMemoBySysNo(view.SysNo, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                FinanceView = DynamicConverter<FinanceVM>.ConvertToVM(args.Result.Rows[0]);
            });
        }
    }
}
