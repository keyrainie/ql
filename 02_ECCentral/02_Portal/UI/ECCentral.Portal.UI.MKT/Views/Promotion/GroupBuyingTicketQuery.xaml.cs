using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.QueryFilter.Common;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.UserControls;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url, NeedAccess = false)]
    public partial class GroupBuyingTicketQuery : PageBase
    {
        public GroupBuyingTicketQueryVM VM
        {
            get
            {
                return this.GridCondition.DataContext as GroupBuyingTicketQueryVM;
            }
            set
            {
                this.GridCondition.DataContext = value;
            }
        }

        public GroupBuyingTicketQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.VM = new GroupBuyingTicketQueryVM();
        }

        private void hylView_Click(object sender, RoutedEventArgs e)
        {
            UCBusinessCooperationDetail uc = new UCBusinessCooperationDetail();
            var link = sender as HyperlinkButton;
            var info = link.DataContext as dynamic;
            uc.DataContext = info;
            var dialog = this.Window.ShowDialog("详细信息", uc, (s, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    this.DataGrid.Bind();
                }
            });
            uc.Dialog = dialog;
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new GroupBuyingFacade(this).QueryGroupBuyingTicket(this.VM, new PagingInfo { PageIndex = e.PageIndex, PageSize = e.PageSize, SortBy = e.SortField }, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                if (a.Result.Rows != null)
                {
                    foreach (var item in a.Result.Rows)
                    {
                        if (item.Status != GroupBuyingTicketStatus.Used)
                        {
                            item.TicketID = "******";
                        }
                        item.IsEnableVoid = (item.Status == GroupBuyingTicketStatus.UnUse ||
                                             item.Status == GroupBuyingTicketStatus.Expired ||
                                             item.Status == GroupBuyingTicketStatus.Created);
                    }
                }
                this.DataGrid.ItemsSource = a.Result.Rows.ToList("IsChecked", false);
                this.DataGrid.TotalCount = a.Result.TotalCount;
            });
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = this.DataGrid.ItemsSource as dynamic;
            if (viewList == null) return;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value && (view.Status == GroupBuyingTicketStatus.UnUse || view.Status == GroupBuyingTicketStatus.Expired || view.Status == GroupBuyingTicketStatus.Created);
            }
        }

        private void hylVoid_Click(object sender, RoutedEventArgs e)
        {
            var list = new List<int>();
            var vm = (sender as HyperlinkButton).DataContext as dynamic;
            list.Add(vm.SysNo);
            Window.Confirm("你确定要作废已选数据吗？", (ss, ee) =>
            {
                if (ee.DialogResult == DialogResultType.OK)
                {
                    new GroupBuyingFacade(this).BatchVoidGroupBuyingTicket(list, (s, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("提示", a.Result, MessageType.Information, (se, arg) => this.DataGrid.Bind());
                    });
                }
            });
        }

        private void ButtonBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            var source = this.DataGrid.ItemsSource as dynamic;
            if (source == null)
            {
                this.Window.Alert("请选择要操作的数据！");
                return;
            }
            var list = new List<int>();
            foreach (var item in source)
            {
                if (item.IsChecked)
                {
                    list.Add(item.SysNo);
                }
            }
            if (list.Count == 0)
            {
                this.Window.Alert("请选择要操作的数据！");
                return;
            }

            Window.Confirm("你确定要作废已选数据吗？", (ss, ee) =>
            {
                if (ee.DialogResult == DialogResultType.OK)
                {
                    new GroupBuyingFacade(this).BatchVoidGroupBuyingTicket(list, (s, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("提示", a.Result, MessageType.Information, (se, arg) => this.DataGrid.Bind());
                    });
                }
            });
        }

    }
}
