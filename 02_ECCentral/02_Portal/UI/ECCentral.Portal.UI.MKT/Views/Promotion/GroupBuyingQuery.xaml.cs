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
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class GroupBuyingQuery : PageBase
    {
        private GroupBuyingFacade _Facade;
        private GroupBuyingQueryVM _VM;

        public GroupBuyingQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _Facade = new GroupBuyingFacade(this);
            _VM = new GroupBuyingQueryVM();
            _Facade.GetGroupBuyingTypes((s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    foreach (var gbt in args.Result)
                    {
                        _VM.GroupBuyingTypeList.Add(new KeyValuePair<int, string>(gbt.Key, gbt.Value));
                    }
                    _Facade.GetGroupBuyingAreas((s1, args1) =>
                    {
                        if (!args1.FaultsHandle())
                        {
                            foreach (var gba in args1.Result)
                            {
                                _VM.GroupBuyingAreaList.Add(new KeyValuePair<int, string>(gba.Key, gba.Value));
                            }

                            _VM.GroupBuyingTypeList.Insert(0, new KeyValuePair<int, string>(0, ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All));
                            _VM.GroupBuyingAreaList.Insert(0, new KeyValuePair<int, string>(0, ECCentral.BizEntity.Enum.Resources.ResCommonEnum.Enum_All));
                            this.DataContext = _VM;
                            this.cmbGroupBuyingCategoryType.SelectedIndex = 0;
                            this.cmbGroupBuyingArea.SelectedIndex = 0;

                        }
                    });
                }
            });
            this.DataContext = _VM;

        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DataGrid.Bind();
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var clonedVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<GroupBuyingQueryVM>(_VM);
            this.DataGrid.QueryCriteria = clonedVM;
            this.DataGrid.Bind();
        }

        private void DataGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            PagingInfo p = new PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField
            };

            _Facade.Query(this.DataGrid.QueryCriteria as GroupBuyingQueryVM, p, (s, args) =>
            {
                if (!args.FaultsHandle())
                {
                    this.DataGrid.TotalCount = args.Result.TotalCount;
                    this.DataGrid.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                }
            });
        }

        //private void DataGridCheckBoxAllCode_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox chk = (CheckBox)sender;
        //    dynamic rows = DataGrid.ItemsSource;
        //    foreach (dynamic row in rows)
        //    {
        //        GroupBuyingStatus status = GroupBuyingStatus.Finished;
        //        Enum.TryParse<GroupBuyingStatus>(row.Status.ToString(), out status);

        //        if (status == GroupBuyingStatus.Pending || status == GroupBuyingStatus.Active)
        //            row.IsChecked = chk.IsChecked.Value;
        //    }
        //}

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Window.Navigate(string.Format(ConstValue.MKT_GroupBuyingMaintainUrlFormat, "?op=new"), null, true);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            dynamic data = this.DataGrid.SelectedItem as dynamic;
            HyperlinkButton btn = sender as HyperlinkButton;
            if (data != null)
            {
                string op = btn.Name.Equals("hybtnEdit") ? "edt" : "mgt";
                string url = string.Format(ConstValue.MKT_GroupBuyingMaintainUrlFormat, "?op=" + op + "&sysNo=" + data.SysNo);
                Window.Navigate(url, null, true);
            }
            else
            {
                //Window.Alert("请选择团购后操作。", MessageType.Error);
                Window.Alert(ResGroupBuyingQuery.Msg_AfterSelGroupBuy, MessageType.Error);
            }
        }

        private void ButtonVoid_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNos = GetCheckedNo();

            if (sysNos.Count > 0)
            {
                _Facade.Void(sysNos, (result) =>
                {
                    if (result)
                    {
                        //Window.Alert("批量作废成功!");
                        Window.Alert(ResGroupBuyingQuery.Msg_BatchVoidSuccess);
                        DataGrid.Bind();
                    }
                    else
                    {
                        //Window.Alert("批量作废失败!");
                        Window.Alert(ResGroupBuyingQuery.Msg_BatchVoidFailed);
                    }
                });
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            List<int> sysNos = GetCheckedNo();

            if (sysNos.Count > 0)
            {
                _Facade.Stop(sysNos, (result) =>
                {
                    if (result)
                    {
                        //Window.Alert("中止处理成功!");
                        Window.Alert(ResGroupBuyingQuery.Msg_StopSuccess);
                        DataGrid.Bind();
                    }
                    else
                    {
                        //Window.Alert("中止处理失败!");
                        Window.Alert(ResGroupBuyingQuery.Msg_StopFailed);
                    }
                });
            }
        }

        /// <summary>
        /// 获取所有选中行的系统编号
        /// </summary>
        /// <returns></returns>
        private List<int> GetCheckedNo()
        {
            List<int> sysNoList = new List<int>();

            dynamic rows = DataGrid.ItemsSource;
            if (rows == null)
            {
                //Window.Alert("请至少勾选一条数据！");
                Window.Alert(ResGroupBuyingQuery.Msg_OneMoreData);
            }
            foreach (dynamic row in rows)
            {
                if (row.IsChecked)
                {
                    sysNoList.Add(row.SysNo);
                }
            }
            if (sysNoList.Count == 0)
            {
                //Window.Alert("请至少勾选一条数据！");
                Window.Alert(ResGroupBuyingQuery.Msg_OneMoreData);
            }
            return sysNoList;
        }

        private void cmbGroupBuyingCategoryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            new GroupBuyingFacade(this).GetAllGroupBuyingCategory((se, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }

                List<GroupBuyingCategoryInfo> list;

                if (this._VM.CategoryType.HasValue)
                {
                    list = a.Result.Where(p => p.CategoryType == this._VM.CategoryType.Value).ToList();
                }
                else
                {
                    list = a.Result.ToList();
                }

                _VM.GroupBuyingCategoryList.Clear();
                _VM.GroupBuyingCategoryList.Add(new GroupBuyingCategoryVM(){SysNo=0,Name="--所有--"});
                list.ForEach(p =>
                {
                    var v = EntityConverter<GroupBuyingCategoryInfo, GroupBuyingCategoryVM>.Convert(p);
                    _VM.GroupBuyingCategoryList.Add(v);
                });

                this.cmbGroupBuyingCategory.SelectedIndex = 0;
                //this._VM.GroupBuyingCategorySysNo = list.FirstOrDefault().SysNo;
                //if (this._VM.GroupBuyingCategorySysNo == null)
                //{
                //    this.cmbGroupBuyingCategory.SelectedIndex = 0;
                //}
            });
        }
    }
}
