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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Common.Models;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Common.Resources;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Enum.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace ECCentral.Portal.UI.Common.Views
{
    [View]
    public partial class ShipTypeAreaUn : PageBase
    {
        ShipTypeAreaUnQueryFilterVM _viewModel;
        ShipTypeFacade _facade;
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _viewModel = new ShipTypeAreaUnQueryFilterVM();
            _facade = new ShipTypeFacade(this);
            QueryFilter.DataContext = _viewModel;
        }
        public ShipTypeAreaUn()
        {
            InitializeComponent();
        }

        private void QueryResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            this._viewModel.PageInfo.PageIndex = e.PageIndex;
            this._viewModel.PageInfo.PageSize = e.PageSize;
            this._viewModel.PageInfo.SortBy = e.SortField;

            _facade.QueryShipTypeAreaUnList(_viewModel, (obj, args) =>
            {
                QueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                QueryResult.TotalCount = args.Result.TotalCount;

            });
        }

        private void Serch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.QueryFilter))
            {
                // if (_filterVM.HasValidationErrors) return;
                QueryResult.Bind();
            }
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ck = (CheckBox)sender;
            dynamic rows = QueryResult.ItemsSource;
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    row.IsChecked = ck.IsChecked.Value;
                }
            }
        }

        private void btnBatchVoid_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaUn_BatchDelete))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            List<int> sysnoList = new List<int>();
            dynamic rows = QueryResult.ItemsSource;
            if (rows == null)
            {
                Window.Alert("请选择需要删除的数据！");
                return;
            }

            foreach (var row in rows)
            {
                if (row.IsChecked)
                {
                    sysnoList.Add(row.SysNo);
                }
            }
            if (sysnoList.Count <= 0)
            {
                Window.Alert("请选择需要删除的数据!");
                return;
            }
            Window.Confirm("确定删除所选数据?", (o, s) =>
            {
                if (s.DialogResult == DialogResultType.OK)
                {
                    _facade.VoidShipTypeAreaUn(sysnoList, (m, args) =>
                        {
                            Window.Alert("提示信息", "删除成功！", MessageType.Information, (g, ar) =>
                            {
                                QueryResult.Bind();
                            });
                        });
                }
            });
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_ShipTypeAreaUn_Add))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            Window.Navigate(ConstValue.Common_ShipTypeAreaUnAddnewUrlFormat, null, true);
        }
    }
}
