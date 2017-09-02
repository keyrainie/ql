using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.UserControls
{
    /// <summary>
    /// 应付款发票状态维护
    /// </summary>
    public partial class UCPayInvoiceMaintain : PopWindow
    {
        private PayInvoiceMaintainVM vm;

        public UCPayInvoiceMaintain()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCPayInvoiceMaintain_Loaded);
            this.vm = new PayInvoiceMaintainVM();
        }

        private void UCPayInvoiceMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPayInvoiceMaintain_Loaded);
            this.BaseInfo.DataContext = vm;
        }

        public UCPayInvoiceMaintain(PayInvoiceMaintainVM model)
            : this()
        {
            this.vm = model;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.BaseInfo);
            if (!flag)
            {
                return;
            }
            List<PayableInfo> entities = new List<PayableInfo>();
            List<string> sysNos = vm.SysNos.Split(',').ToList();
            foreach (string sysNo in sysNos)
            {
                entities.Add(new PayableInfo()
                {
                    SysNo = Convert.ToInt32(sysNo),
                    InvoiceStatus = vm.InvoiceStatus,
                    InvoiceFactStatus = vm.InvoiceFactStatus,
                    Note = vm.Note
                });
            }
            PayableFacade facade = new PayableFacade(CPApplication.Current.CurrentPage);
            facade.UpdateInvoiceStatus(entities, (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                CloseDialog(args, DialogResultType.OK);
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }
    }
}