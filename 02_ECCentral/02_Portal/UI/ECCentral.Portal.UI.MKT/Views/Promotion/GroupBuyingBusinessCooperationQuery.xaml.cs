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
    public partial class GroupBuyingBusinessCooperationQuery : PageBase
    {
        public BusinessCooperationQueryVM VM
        {
            get
            {
                return this.GridCondition.DataContext as BusinessCooperationQueryVM;
            }
            set
            {
                this.GridCondition.DataContext = value;
            }
        }        

        public GroupBuyingBusinessCooperationQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            CodeNamePairHelper.GetList("MKT", "GroupBuyingTypeList",  CodeNamePairAppendItemType.All, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                var vm = new BusinessCooperationQueryVM();
                vm.GroupBuyingTypeList = a.Result;
                this.VM = vm;
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
                view.IsChecked = ckb.IsChecked != null && ckb.IsChecked.Value && view.Status == 0;
            }
        }               

        private void ButtonBatchHandle_Click(object sender, RoutedEventArgs e)
        {
            var source = this.DataGrid.ItemsSource as dynamic;
            if (source == null)
            {
                //this.Window.Alert("请选择要操作的数据！");
                this.Window.Alert(ResGroupBuyingBusinessCooperationQuery.Info_SelectOperateData);
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
                this.Window.Alert(ResGroupBuyingBusinessCooperationQuery.Info_SelectOperateData);
                return;
            }
            new GroupBuyingFacade(this).BatchHandleGroupbuyingBusinessCooperation(list, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResGroupBuyingBusinessCooperationQuery.Info_Tips, a.Result, MessageType.Information, (se, arg) =>
                {
                    this.DataGrid.Bind();
                });
            });
        }

        private void hylView_Click(object sender, RoutedEventArgs e)
        {
            UCBusinessCooperationDetail uc = new UCBusinessCooperationDetail();
            var link = sender as HyperlinkButton;
            var info = link.DataContext as dynamic;
            uc.DataContext = info;
            var dialog = this.Window.ShowDialog(ResGroupBuyingBusinessCooperationQuery.Info_DetaileInfo, uc, (s, a) =>
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
            new GroupBuyingFacade(this).QueryBusinessCooperation(this.VM, new PagingInfo { PageIndex = e.PageIndex, PageSize = e.PageSize, SortBy = e.SortField }, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                this.DataGrid.ItemsSource = a.Result.Rows.ToList("IsChecked", false);
                this.DataGrid.TotalCount = a.Result.TotalCount;
            });
        }       
    }
}
