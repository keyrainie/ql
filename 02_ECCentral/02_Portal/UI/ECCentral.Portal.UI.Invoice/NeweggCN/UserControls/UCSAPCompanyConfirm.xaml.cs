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
using ECCentral.Portal.UI.Invoice.UserControls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.Invoice.NeweggCN.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.BizEntity.Invoice.SAP;
using ECCentral.Portal.UI.Invoice.NeweggCN.Resources;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.UserControls
{
    public partial class UCSAPCompanyConfirm : PopWindow
    {
        private SAPCompanyVM InputVM;
        private SAPCompanyInfo ExistSAPCompany;
        private int AlertFlag;
        public UCSAPCompanyConfirm()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCSAPCompanyConfirm_Loaded);
            this.InputVM = new SAPCompanyVM();
        }
        public UCSAPCompanyConfirm(SAPCompanyVM vm, SAPCompanyInfo entity, int alertFlag)
            : this()
        {
            this.InputVM = vm;
            this.ExistSAPCompany = entity;
            this.AlertFlag = alertFlag;
        }

        private void UCSAPCompanyConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCSAPCompanyConfirm_Loaded);
            this.Text_ExistingSAPCompany.Text = string.Format(ResSAP.Label_ExistingSAPCompany, ExistSAPCompany.StockID, ExistSAPCompany.SAPCompanyCode);
            this.Text_InputSAPCompany.Text = string.Format(ResSAP.Label_InputSAPCompany, InputVM.StockID, InputVM.SAPCompanyCode);

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            new SAPFacade(CPApplication.Current.CurrentPage).CreateSAPCompany(InputVM, AlertFlag, () =>
                CloseDialog(DialogResultType.OK));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

    }
}
