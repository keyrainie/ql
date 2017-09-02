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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.Basic.Components.UserControls.VendorPicker;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Models.Vendor;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Utility;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.PO.Views
{
    [View(IsSingleton = true)]
    public partial class SecondDomainQuery : PageBase
    {
        private SecondDomainQueryVM PageQueryView;
        public ECCentral.Portal.UI.PO.Facades.VendorFacade vendorFacade;

        public SecondDomainQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            PageQueryView = new SecondDomainQueryVM();

            vendorFacade = new ECCentral.Portal.UI.PO.Facades.VendorFacade(this);

            this.DataContext = PageQueryView;
        }

        private void btnChooseVendor_Click(object sender, MouseButtonEventArgs e)
        {
            UCVendorQuery selectDialog = new UCVendorQuery() { SelectionMode = SelectionMode.Multiple };
            selectDialog.Dialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("商家查询", selectDialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    List<string> vendorList = new List<string>();
                    List<DynamicXml> getSelectedVendors = args.Data as List<DynamicXml>;
                    if (null != getSelectedVendors)
                    {
                        foreach (DynamicXml getSelectedVendor in getSelectedVendors)
                        {
                            vendorList.Add(getSelectedVendor["VendorName"].ToString());
                            PageQueryView.VendorSysNoList.Add(Convert.ToInt32(getSelectedVendor["SysNo"]));
                        }
                        this.txtVendorName.Text = vendorList.Join(",");
                    }
                }
            }, new Size(750, 650));
        }

        private void btnClearVendor_Click(object sender, MouseButtonEventArgs e)
        {
            PageQueryView.VendorSysNoList.Clear();
            this.txtVendorName.Text = string.Empty;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void QueryResultGrid_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {

            vendorFacade.QuerySecondDomain(PageQueryView, e.PageIndex, e.PageSize, e.SortField, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                if (args.Result != null && args.Result.Rows != null)
                {
                    foreach (var row in args.Result.Rows)
                    {
                        if (row["SecondDomainStatus"] != null)
                        {
                            switch ((int)row["SecondDomainStatus"])
                            {
                                case -1:
                                    row["SecondDomainStatusDisplay"] = "未通过审核";
                                    break;
                                case 0:
                                    row["SecondDomainStatusDisplay"] = "待审核";
                                    row["AuditThrough"] = "审核通过";
                                    row["AuditThroughNot"] = "审核不通过";
                                    break;
                                case 1:
                                    row["SecondDomainStatusDisplay"] = "通过审核";
                                    break;
                                case 2:
                                    row["SecondDomainStatusDisplay"] = "已生效";
                                    break;
                            }
                        }
                    }
                    this.QueryResultGrid.ItemsSource = args.Result.Rows;
                    this.QueryResultGrid.TotalCount = args.Result.TotalCount;
                }
            });
        }

        private void HyperlinkAuditThrough_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确定要执行审核通过操作！", (obj1, args1) =>
            {
                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dynamic row = this.QueryResultGrid.SelectedItem as dynamic;
                    if (row != null)
                    {
                        vendorFacade.SecondDomainAuditThrough((int)row.SysNo, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("操作成功！");
                            QueryResultGrid.Bind();
                        });
                    }
                }
            });
        }

        private void HyperlinkAuditThroughNot_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确定要执行审核不通过操作", (obj1, args1) =>
            {
                if (args1.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    dynamic row = this.QueryResultGrid.SelectedItem as dynamic;
                    if (row != null)
                    {
                        vendorFacade.SecondDomainAuditThroughNot((int)row.SysNo, (obj, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Window.Alert("操作成功！");
                            QueryResultGrid.Bind();
                        });
                    }
                }
            });
        }

    }
}
