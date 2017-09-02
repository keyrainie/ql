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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.MKT.UserControls.Floor;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models.Floor;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class Floor : PageBase
    {
        private FloorFacade facade;
        private List<FloorTemplate> Templates = new List<FloorTemplate>();
        public IPage CurrentPage
        {
            get { return CPApplication.Current.CurrentPage; }
        }
        public Floor()
        {
            InitializeComponent();

        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            facade = new FloorFacade(this);
            facade.GetTemplate((s, args) =>
            {
                Templates = args.Result;
                cbPageCodes.ItemsSource = EnumConverter.GetKeyValuePairs<PageCodeType>(EnumConverter.EnumAppendItemType.All);
                cbPageCodes.SelectedIndex = 0;
                DataGrid.Bind();
            });
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            FloorVM v = new FloorVM();
            v.Templates = Templates;
            v.TemplateSysNo = Templates[0].TemplateSysNo.Value;
            UCFloorMaintain uc = new UCFloorMaintain(v);
            uc.Dialog = CurrentPage.Context.Window.ShowDialog(MKT.Resources.ResFloor.Title_Floor_New,
                uc, (ss, argss) =>
                {
                    DataGrid.Bind();
                });
        }

        private string GetTemplateName(int sysNo)
        {
            foreach (var item in Templates)
            {
                if (item.TemplateSysNo == sysNo) return item.TemplateName;
            }
            return "";
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            FloorMasterQueryFilter filter = new FloorMasterQueryFilter();
            filter.PageType = (PageCodeType?)cbPageCodes.SelectedValue;

            if (filter.PageType != PageCodeType.C1 && filter.PageType != PageCodeType.Promotion)
            {
                filter.PageCode = cbPageID.SelectedItem == null || !filter.PageType.HasValue ? string.Empty : "1";
            }
            else
            {
                filter.PageCode =((KeyValuePair<string, string>)cbPageID.SelectedItem).Key;
            }

            filter.PageInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = DataGrid.PageIndex,
                PageSize = DataGrid.PageSize
            };

            facade.QueryFloor(filter, (s, args) =>
            {
                var model = new List<FloorVM>();

                foreach (var row in args.Result.Rows)
                {
                    var vm = new FloorVM();
                    vm.FloorLogoSrc = row.FloorLogoSrc;
                    vm.FloorName = row.FloorName;
                    //vm.FloorNo = "#" + i;
                    vm.Remark = row.Remark;
                    vm.Status = row.Status;
                    vm.SysNo = row.SysNo;
                    vm.TemplateName = GetTemplateName(row.TemplateSysNo);
                    vm.Templates = Templates;
                    vm.TemplateSysNo = row.TemplateSysNo;
                    vm.Priority = row.Priority.ToString();
                    vm.PageType = row.PageType;
                    vm.PageCode = row.PageCode;
                    vm.PageName = row.PageName;
                    model.Add(vm);
                }

                var query = from a in model
                            group a by a.PageCode into g
                            select g;
                foreach (var m in query)
                {
                    var ls = m.ToList<FloorVM>();
                    if (ls != null)
                    {
                        for (var i = 0; i < ls.Count(); i++)
                        {
                            ls[i].FloorNo = "#" + (i + 1);
                        }
                    }
                }

                DataGrid.ItemsSource = model;
                DataGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            FloorVM vm = DataGrid.SelectedItem as FloorVM;
            UCFloorMaintain uc = new UCFloorMaintain(vm);
            uc.Dialog = CurrentPage.Context.Window.ShowDialog(MKT.Resources.ResFloor.Title_Floor_Maintain,
                uc, (s, args) =>
                {
                    DataGrid.Bind();
                });
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResFloor.Info_Confirm_Delete, (s, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var sysNo = (DataGrid.SelectedItem as FloorVM).SysNo;
                    facade.DeleteFloor(sysNo, (s1, args1) =>
                    {
                        if (args1.FaultsHandle())
                        {
                            return;
                        }
                        else
                        {
                            DataGrid.Bind();
                        }
                    });
                }
            });
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            FloorVM vm = DataGrid.SelectedItem as FloorVM;
            if (vm != null)
            {
                var sysNo = vm.SysNo;
                Window.Navigate(string.Format(ConstValue.MKT_FloorSectionMaintainUrlFormat, sysNo), null, true);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataGrid.Bind();
        }

        private void cbPageCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pageType = (PageCodeType?)cbPageCodes.SelectedValue;
            spPageID.Visibility = Visibility.Collapsed;
            if (pageType == PageCodeType.C1 || pageType == PageCodeType.Promotion)
            {
                facade.QueryPageID(((int)pageType).ToString(), CPApplication.Current.CompanyCode, (s, args) =>
                {
                    args.Result.Insert(0, new KeyValuePair<string, string>(string.Empty, ResCommonEnum.Enum_All));
                    cbPageID.ItemsSource = args.Result;
                    cbPageID.SelectedIndex = 0;
                    spPageID.Visibility = Visibility.Visible;
                });
            }
        }
    }
}
