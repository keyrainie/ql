using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades.RmaPolicy;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.UserControls;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class RmaPolicySettingQuery : PageBase
    {
        #region const
        private RmaPolicySettingQueryVM vm;
        private DefaultRMAPolicyFacade facade;
        private DefaultRMAPolicyFilter filter;
        private List<RmaPolicySettingQueryVM> vms;
        #endregion

        #region Method
        public RmaPolicySettingQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            //查询退换货
            ucRmaPolicyComboxList.IsEdit = false;
            //获取页面信息
            vm = new RmaPolicySettingQueryVM();
            QuerySection.DataContext = vm;
            facade = new DefaultRMAPolicyFacade(this);
            this.QueryResultGrid.LoadingDataSource += 
                new EventHandler<LoadingDataEventArgs>(QueryResultGrid_LoadingDataSource);
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            filter = vm.ConvertVM<RmaPolicySettingQueryVM, DefaultRMAPolicyFilter>();
            filter.RMAPolicySysNo = ucRmaPolicyComboxList.SelectValue;
            facade.GetDefaultRMAPolicy(filter, e.PageSize, e.PageIndex, e.SortField, (obj, args) => {
                vms = DynamicConverter<RmaPolicySettingQueryVM>
                        .ConvertToVMList<List<RmaPolicySettingQueryVM>>(args.Result.Rows);
                this.QueryResultGrid.ItemsSource = vms;
                this.QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DefaultRMAPolicyEditMaintain item = new DefaultRMAPolicyEditMaintain();

            item.Dialog = Window.ShowDialog("新建设定退换货政策", item, (s, args) =>
            {
                this.QueryResultGrid.Bind();
            }, new Size(650, 250));
            QueryResultGrid.Bind();
        }

        private void HyperlinkButton_EditClick(object sender, RoutedEventArgs e)
        {
            DefaultRMAPolicyEditMaintain item = new DefaultRMAPolicyEditMaintain();
            dynamic selectitem = this.QueryResultGrid.SelectedItem as dynamic;
            RmaPolicySettingQueryVM _vm = new RmaPolicySettingQueryVM();
            _vm.SysNo = selectitem.SysNo;
            _vm.RMAPolicySysNo = selectitem.RMAPolicySysNo;
            _vm.C1Name = selectitem.C1Name;
            _vm.C2Name = selectitem.C2Name;
            _vm.C3Name = selectitem.C3Name;
            _vm.C1SysNo = selectitem.C1SysNo;
            _vm.C2SysNo = selectitem.C2SysNo;
            _vm.C3SysNo = selectitem.C3SysNo;
            _vm.BrandSysNo = selectitem.BrandSysNo;
            _vm.BrandName = selectitem.BrandName;
            item.Data = _vm;
            item.Dialog = Window.ShowDialog("编辑设定退换货政策", item, (s, args) =>
            {
                this.QueryResultGrid.Bind();
            }, new Size(650, 250));
            QueryResultGrid.Bind();
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryResultGrid.Bind();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            List<int> delSysNo = new List<int>();
            CPApplication.Current.CurrentPage.Context.Window.Confirm("是否需要进行删除操作吗？", (o, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    vms.ForEach(item =>
                    {
                        if (item.IsChecked) delSysNo.Add(int.Parse(item.SysNo.ToString()));
                    });
                }
                if (delSysNo.Count > 0)
                {
                    facade.DelDefaultRMAPolicy(delSysNo, (obj, arg) =>
                    {
                        if (arg.FaultsHandle()) return;
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("删除成功!", MessageBoxType.Success);
                    });
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请输选择需要维护的品牌!");
                }
                QueryResultGrid.Bind();
            });
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (vm == null || checkBoxAll == null)
                return;
            vms.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }
        #endregion
    }
}
