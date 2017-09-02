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


namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton=true)]
    public partial class AreaQuery :PageBase
    {
        public AreaQueryFilterVM _filterVM=new AreaQueryFilterVM();
        public AreaFacade _facade;
        public AreaQuery()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            List<KeyValuePair<AreaStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<AreaStatus>();
            statusList.Insert(0, new KeyValuePair<AreaStatus?, string>(null, ResCommonEnum.Enum_All));
            lisStatus.ItemsSource = statusList;
            lisStatus.SelectedIndex = 0;
            GridQueryFilter.DataContext = _filterVM;
            _facade = new AreaFacade(this);

        }

        private void Serch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.GridQueryFilter))
            {
               // if (_filterVM.HasValidationErrors) return;
                QueryResult.Bind();
            }
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            this._filterVM.PagingInfo.PageIndex = e.PageIndex;
            this._filterVM.PagingInfo.PageSize = e.PageSize;
            this._filterVM.PagingInfo.SortBy = e.SortField;
            _facade.QueryAreaList(_filterVM, (obj, args) =>
            {
                QueryResult.ItemsSource = args.Result.Rows.ToList("IsChecked", false);
                QueryResult.TotalCount = args.Result.TotalCount;

            });
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_AreaQuery_Add))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            this.Window.Navigate(ConstValue.Common_AreaMaintainUrlFormat + "?operation=add", null, true);
        }

        private void Hyperlink_EditData_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_AreaQuery_Edit))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            dynamic rows = QueryResult.SelectedItem;
            if (rows == null)
            {
                return;
            }
            var sysno=rows.SysNo;
            string url = string.Format(ConstValue.Common_AreaMaintainUrlFormat+"?sysno={0}&operation=edit", sysno);
            this.Window.Navigate(url, null, true);
        }

    }
}
