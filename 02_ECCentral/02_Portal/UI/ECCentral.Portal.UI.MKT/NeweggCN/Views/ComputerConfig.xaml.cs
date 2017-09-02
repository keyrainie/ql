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
using ECCentral.Portal.UI.MKT.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ComputerConfig : PageBase
    {
        private ComputerConfigMasterQueryVM _queryVM;

        public ComputerConfig()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            //绑定查询区域中的渠道列表
            var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.lstChannelList.ItemsSource = channelList;

            _queryVM = new ComputerConfigMasterQueryVM();
            this.GridFilter.DataContext = _queryVM;

            this.lstOwner.ItemsSource = EnumConverter.GetKeyValuePairs<ComputerConfigOwner>(EnumConverter.EnumAppendItemType.All);
            this.lstStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ComputerConfigStatus>(EnumConverter.EnumAppendItemType.All);
            this.lstOwner.SelectedIndex = 1;

            ComputerConfigFacade service = new ComputerConfigFacade(this);
            service.GetAllConfigTypes((s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    BindConfigTypes(args.Result);
                });

            service.GetEditUsers(CPApplication.Current.CompanyCode, "1", (s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                BindEditUsers(args.Result);
            });
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (_queryVM.MinPriceRange!=null && _queryVM.MaxPriceRange!=null
                && _queryVM.MinPriceRange >= _queryVM.MaxPriceRange)
            {
                this.Window.Alert("配置单金额最小值必须小于最大值!");
                return;
            }

            //1.初始化查询条件
            //2.请求服务查询
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };
            ComputerConfigFacade facade = new ComputerConfigFacade(this);
            facade.Query(_queryVM, p, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var rows = args.Result.Rows.ToList("IsChecked", false);
                this.DataGrid.ItemsSource = rows;
                this.DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_ComputerConfigMaintainUrlFormat, ""), null, true);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            this.Window.Navigate(string.Format(ConstValue.MKT_ComputerConfigMaintainUrlFormat, row.SysNo), null, true);
        }

        private void ButtonAudit_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            this.Window.Navigate(string.Format(ConstValue.MKT_ComputerConfigMaintainUrlFormat, row.SysNo), null, true);
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnEdit = sender as HyperlinkButton;
            var row = btnEdit.DataContext as dynamic;
            this.Window.Navigate(string.Format(ConstValue.MKT_ComputerConfigMaintainUrlFormat, row.SysNo), ComputerConfigMaintainMode.Copy, true);
        }

        private void ButtonVoid_Click(object sender, RoutedEventArgs e)
        {
            List<dynamic> selected = new List<dynamic>();
            dynamic rows = this.DataGrid.ItemsSource;
            if (rows != null)
            {
                foreach (var item in rows)
                {
                    if (item.IsChecked)
                    {
                        selected.Add(item);
                    }
                }
            }
            if (selected.Count == 0)
            {
                Window.Alert(ResComputerConfig.Info_PleaseSelect);
                return;
            }
            Window.Confirm(ResComputerConfig.Confirm_Void, (cs, cr) =>
            {
                if (cr.DialogResult == DialogResultType.OK)
                {

                    new ComputerConfigFacade(this).Void(selected.Select<dynamic,int>(item=>item.SysNo).ToList(), () =>
                    {
                        Window.Alert(ResComputerConfig.Info_VoidSuccess);
                        foreach (var row in selected)
                        {
                            row.Status = ComputerConfigStatus.Void;
                        }
                    });
                }
            });
        }

        private void DataGridCheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            if (_checkBoxAll != null)
            {
                dynamic rows = this.DataGrid.ItemsSource;
                if (rows != null)
                {
                    foreach (var item in rows)
                    {
                        item.IsChecked = _checkBoxAll.IsChecked ?? false;
                    }
                }
            }
        }

        private CheckBox _checkBoxAll;
        private void DataGridCheckBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            _checkBoxAll = sender as CheckBox;
        }

        private void lstChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lstChannelList.SelectedValue != null)
            {
                ComputerConfigFacade service = new ComputerConfigFacade(this);
                service.GetEditUsers(CPApplication.Current.CompanyCode, this.lstChannelList.SelectedValue.ToString(), (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    BindEditUsers(args.Result);
                });
            }
            else
            {
                BindEditUsers(null);
            }
        }

        private void BindEditUsers(List<UserInfo> users)
        {
            if (users == null)
            {
                users = new List<UserInfo>();
            }
            List<CodeNamePair> codeNames = new List<CodeNamePair>();
            users.ForEach((usr) =>
                {
                    codeNames.Add(new CodeNamePair { Code = usr.UserName, Name = usr.UserName });
                });
            codeNames.Insert(0, new CodeNamePair { Code = null, Name = ResCommonEnum.Enum_All });
            this.lstEditUser.ItemsSource = codeNames;
        }

        private void BindConfigTypes(List<ComputerConfigType> types)
        {
            if (types == null)
            {
                types = new List<ComputerConfigType>();
            }
            types.Insert(0, new ComputerConfigType { TypeName = ResCommonEnum.Enum_All, SysNo = null });
            this.lstConfigType.ItemsSource = types;
        }


    }

    internal enum ComputerConfigMaintainMode
    {
        Copy
    }

}
