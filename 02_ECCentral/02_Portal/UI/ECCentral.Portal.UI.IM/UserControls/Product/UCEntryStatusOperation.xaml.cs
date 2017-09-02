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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM.Product;

namespace ECCentral.Portal.UI.IM.UserControls.Product
{
    public partial class UCEntryStatusOperation : UserControl
    {
        public IDialog dialog;
        private EntryStatusOperation action;
        private int ProductSysNo;
        private ProductEntryStatus? entryStatus;
        private ProductEntryStatusEx? entryStatusEx;
        ProductEntryFacade facades;

        public UCEntryStatusOperation(EntryStatusOperation _action, int _ProductSysNo)
        {
            this.action = _action;
            this.ProductSysNo = _ProductSysNo;
            InitializeComponent();
            facades = new ProductEntryFacade(CPApplication.Current.CurrentPage);

            switch (action)
            {
                case EntryStatusOperation.Audit:
                    this.btnAuditSucess.Visibility = System.Windows.Visibility.Visible;
                    this.btnAuditFail.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EntryStatusOperation.Inspection:
                    this.btnInspectionSucess.Visibility = System.Windows.Visibility.Visible;
                    this.btnInspectionFail.Visibility = System.Windows.Visibility.Visible;
                    break;
                case EntryStatusOperation.Customs:
                    this.btnCustomsSucess.Visibility = System.Windows.Visibility.Visible;
                    this.btnCustomsFail.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void AuditFail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = this.ProductSysNo;
            info.AuditNote = this.AuditNote.Text;
            facades.AuditFail(info, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("审核不通过成功！");
                    this.btnAuditSucess.IsEnabled = false;
                    this.btnAuditFail.IsEnabled = false;
                    this.dialog.Close();
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("审核不通过失败！");
                }
            });
        }

        private void AuditSucess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = this.ProductSysNo;
            info.AuditNote = this.AuditNote.Text;
            facades.AuditSucess(info, (args) =>
                {
                    if (args)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("审核通过成功！");
                        this.btnAuditSucess.IsEnabled = false;
                        this.btnAuditFail.IsEnabled = false;
                        this.dialog.Close();
                    }
                    else
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("审核通过失败！");
                    }
                });
        }

        private void InspectionFail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = this.ProductSysNo;
            info.AuditNote = this.AuditNote.Text;
            facades.InspectionFail(info, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("商检不通过成功！");
                    this.btnAuditSucess.IsEnabled = false;
                    this.btnAuditFail.IsEnabled = false;
                    this.dialog.Close();
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("商检不通过失败！");
                }
            });
        }

        private void InspectionSucess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = this.ProductSysNo;
            info.AuditNote = this.AuditNote.Text;
            facades.InspectionSucess(info, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("商检通过成功！");
                    this.btnAuditSucess.IsEnabled = false;
                    this.btnAuditFail.IsEnabled = false;
                    this.dialog.Close();
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("商检通过失败！");
                }
            });
        }

        private void CustomsFail_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = this.ProductSysNo;
            info.AuditNote = this.AuditNote.Text;
            facades.CustomsFail(info, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("报关不通过成功！");
                    this.btnAuditSucess.IsEnabled = false;
                    this.btnAuditFail.IsEnabled = false;
                    this.dialog.Close();
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("报关不通过失败！");
                }
            });
        }

        private void CustomsSucess_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductEntryInfo info = new ProductEntryInfo();
            info.ProductSysNo = this.ProductSysNo;
            info.AuditNote = this.AuditNote.Text;
            facades.CustomsSuccess(info, (args) =>
            {
                if (args)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("报关通过成功！");
                    this.btnAuditSucess.IsEnabled = false;
                    this.btnAuditFail.IsEnabled = false;
                    this.dialog.Close();
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("报关通过失败！");
                }
            });
        }
    }
}
