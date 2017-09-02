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

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url,NeedAccess=false)]
    public partial class GroupBuyingFeedbackQuery : PageBase
    {
        public FeedbackQueryVM VM
        {
            get
            {
                return this.GridCondition.DataContext as FeedbackQueryVM;
            }
            set
            {
                this.GridCondition.DataContext = value;
            }
        }        

        public GroupBuyingFeedbackQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            CodeNamePairHelper.GetList("MKT", "GroupBuyingFeedBackType", CodeNamePairAppendItemType.All, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                var vm = new FeedbackQueryVM();
                vm.FeedbackTypeList=a.Result;
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

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void ButtonBatchRead_Click(object sender, RoutedEventArgs e)
        {
            var source = this.DataGrid.ItemsSource as dynamic;
            if (source == null)
            {
                //this.Window.Alert("请选择要操作的数据！");
                this.Window.Alert(ResGroupBuyingFeedbackQuery.Info_SelectData);
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
                this.Window.Alert(ResGroupBuyingFeedbackQuery.Info_SelectData);
                return;
            }
            new GroupBuyingFacade(this).BatchReadGroupbuyingFeedback(list, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                Window.Alert(ResGroupBuyingFeedbackQuery.Info_Tips, a.Result, MessageType.Information, (se, arg) =>
                {
                    this.DataGrid.Bind();
                });
            });
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            new GroupBuyingFacade(this).QueryGroupBuyingFeedback(this.VM, new PagingInfo { PageIndex = e.PageIndex, PageSize = e.PageSize, SortBy = e.SortField }, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }

                var getList = a.Result.Rows;

                foreach (var x in getList)
                {
                    if(!string.IsNullOrEmpty(x["Content"]))
                    {
                        if (x["Content"].ToString().Length > 41)
                        {
                            x["ContentDisplay"] = x["Content"].ToString().Substring(0, 40) + "...";
                        }
                        else
                        { 
                            x["ContentDisplay"] = x["Content"].ToString();
                        }
                    }
                    else
                    {
                        x["ContentDisplay"] = string.Empty;
                    }
                }

                this.DataGrid.ItemsSource = a.Result.Rows.ToList("IsChecked",false);
                this.DataGrid.TotalCount = a.Result.TotalCount;
            });
        }       
    }
}
