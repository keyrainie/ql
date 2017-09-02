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
using System.Windows.Navigation;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Service.MKT.Restful.RequestMsg;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SysConfigQuery : PageBase
    {
        private SysConfigQueryFilter filter;
        private SysConfigFacade facade;
        private SysConfigVM viewModel;
        public SysConfigQuery()
        {
            InitializeComponent();
            viewModel = new SysConfigVM();
            this.DataContext = viewModel;
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            facade = new SysConfigFacade(this);
            base.OnPageLoad(sender, e);
            CodeNamePairHelper.GetList("MKT", "SysConfigType", CodeNamePairAppendItemType.All, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                cmbType.ItemsSource = args.Result;
                cmbType.SelectedIndex = 0;
            });
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter = viewModel.ConvertVM<SysConfigVM, SysConfigQueryFilter>();

            filter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            facade.Query(filter, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                DataGrid.ItemsSource = DynamicConverter<SysConfigItemVM>.ConvertToVMList(args.Result.Rows);
                DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var list = GetSelected();
            if (list.Count > 0)
            {
                facade.Update(list, (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert("更新成功!");
                });
            }
            else
            {
               Window.Alert(ResComment.Information_MoreThanOneRecord, MessageType.Warning);
            }
        }


        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            if (ckb != null)
            {
                List<SysConfigItemVM> viewList = this.DataGrid.ItemsSource as List<SysConfigItemVM>;
                if (viewList != null)
                {
                    foreach (var view in viewList)
                    {
                        view.IsChecked = ckb.IsChecked != null ? ckb.IsChecked.Value : false;
                    }
                }
            }
        }

        private List<SysConfigReq> GetSelected()
        {
            List<SysConfigReq> list = new List<SysConfigReq>();
            if (this.DataGrid.ItemsSource != null)
            {
                List<SysConfigItemVM> viewList = this.DataGrid.ItemsSource as List<SysConfigItemVM>;
                foreach (var view in viewList)
                {
                    if (view.IsChecked)
                    {
                        list.Add(new SysConfigReq() { Key = view.Key, Value = view.Value, ChannlID = viewModel.ChannelID });
                    }
                }
            }
            return list;
        }
    }
}
