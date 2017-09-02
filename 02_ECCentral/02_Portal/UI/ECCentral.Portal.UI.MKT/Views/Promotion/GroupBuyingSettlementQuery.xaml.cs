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
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url,NeedAccess=false)]
    public partial class GroupBuyingSettlementQuery : PageBase
    {
        public GroupBuyingSettlementQueryVM VM
        {
            get
            {
                return this.GridCondition.DataContext as GroupBuyingSettlementQueryVM;
            }
            set
            {
                this.GridCondition.DataContext = value;
            }
        }

        public GroupBuyingSettlementQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            this.VM = new GroupBuyingSettlementQueryVM();
            if (!string.IsNullOrEmpty(this.Request.Param))
            {
                this.VM.SysNo = this.Request.Param;
            }
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            var ckb = sender as CheckBox;
            if (ckb == null) return;
            var viewList = this.DataGrid.ItemsSource as dynamic;
            if (viewList == null) return;
            foreach (var view in viewList)
            {
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value && view.Status == 0;
            }
        }        

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.GridCondition))
            {
                this.DataGrid.Bind();
            }
        }

        private void hylView_Click(object sender, RoutedEventArgs e)
        {
            UCGroupBuyingSettlementDetail uc = new UCGroupBuyingSettlementDetail();
            var link = sender as HyperlinkButton;
            var info = link.DataContext as dynamic;
            uc.DataContext = info;
            //var dialog = this.Window.ShowDialog("详细信息", uc);
            var dialog = this.Window.ShowDialog(ResGroupBuyingSettlementQuery.Info_DetailInfo, uc);
            uc.Dialog = dialog;
        }

        private void ButtonBatchAudit_Click(object sender, RoutedEventArgs e)
        {
            var source = this.DataGrid.ItemsSource as dynamic;
            if (source == null)
            {
                //this.Window.Alert("请选择要操作的数据！");
                this.Window.Alert(ResGroupBuyingSettlementQuery.Info_SelectOperateData);
                return;
            }
            List<int> list = new List<int>();
            foreach (var item in source)
            {
                if (item.IsChecked)
                {
                    list.Add(item.SysNo);
                }
            }
            if (list.Count == 0)
            {
                //this.Window.Alert("请选择要操作的数据！");
                this.Window.Alert(ResGroupBuyingSettlementQuery.Info_SelectOperateData);
                return;
            }

            Window.Confirm(ResGroupBuyingSettlementQuery.Info_SureAudit, (ss, ee) =>
            {
                if (ee.DialogResult == DialogResultType.OK)
                {
                    new GroupBuyingFacade(this).BatchAuditGroupBuyingSettlement(list, (s, a) =>
                    {
                        if (a.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert(ResGroupBuyingSettlementQuery.Info_Tips, a.Result, MessageType.Information, (se, arg) =>
                        {
                            this.DataGrid.Bind();
                        });
                    });
                }
            });           
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new GroupBuyingFacade(this).QuerySettlement(this.VM, new PagingInfo { PageIndex = e.PageIndex, PageSize = e.PageSize, SortBy = e.SortField }, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                this.DataGrid.ItemsSource = a.Result.Rows.ToList("IsChecked",false);
                //this.DataGrid.ItemsSource = a.Result.Rows.ToList();
                this.DataGrid.TotalCount = a.Result.TotalCount;
            });
        }       
    }
}
