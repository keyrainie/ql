using System;
using System.Collections.Generic;
using System.Windows;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.UI.Common.Models;
using ECCentral.Portal.UI.Common.UserControls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.Views
{
    [View(IsSingleton = true)]
    public partial class FreeShippingChargeRuleQuery : PageBase
    {
        public FreeShippingChargeRuleQueryVM _queryVM = new FreeShippingChargeRuleQueryVM();
        public FreeShippingChargeRuleFacade _facade;
        public AreaQueryFacade _areaQueryFacade;

        public FreeShippingChargeRuleQuery()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            _facade = new FreeShippingChargeRuleFacade(this);
            _areaQueryFacade = new AreaQueryFacade();

            _queryVM = new FreeShippingChargeRuleQueryVM();
            GridQueryFilter.DataContext = _queryVM;

            _areaQueryFacade.QueryProvinceAreaList((obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<AreaInfo> areaList = args.Result;
                if (areaList == null)
                {
                    areaList = new List<AreaInfo>();
                }
                areaList.Insert(0, new AreaInfo() { ProvinceName = ResCommonEnum.Enum_Select });
                comArea.ItemsSource = areaList;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.GridQueryFilter))
            {
                QueryResult.Bind();
            }
        }

        private void QueryResult_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            _facade.Query(_queryVM, (obj, args) =>
            {
                QueryResult.ItemsSource = args.Result.Rows;
                QueryResult.TotalCount = args.Result.TotalCount;
            });
        }

        private void btnNewArea_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_FreeShippingChargeRule_Edit))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            UCFreeShippingChargeRule dialog = new UCFreeShippingChargeRule(null);
            dialog.CurrentDialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("新建免运费规则", dialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    QueryResult.Bind();
                }
            });
        }

        private void Hyperlink_EditData_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_FreeShippingChargeRule_Edit))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            dynamic rows = QueryResult.SelectedItem;
            if (rows == null)
            {
                return;
            }
            var sysno = rows.SysNo;

            UCFreeShippingChargeRule dialog = new UCFreeShippingChargeRule(sysno);
            dialog.CurrentDialog = CPApplication.Current.CurrentPage.Context.Window.ShowDialog("编辑免运费规则", dialog, (obj, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    QueryResult.Bind();
                }
            });
        }

        private void Hyperlink_DeleteData_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Common_FreeShippingChargeRule_Delete))
            {
                Window.Alert("您没有此功能的操作权限！");
                return;
            }
            dynamic rows = QueryResult.SelectedItem;
            if (rows == null)
            {
                return;
            }
            int sysno = (int)rows.SysNo;

            Window.Confirm("你确定要删除该条免运费规则吗", (obj, arg) =>
            {
                if (arg.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    _facade.Delete(sysno, (x, y) =>
                    {
                        if (y.FaultsHandle())
                        {
                            return;
                        }
                        Window.Alert("删除成功！");
                        QueryResult.Bind();
                    });
                }
            });
        }
    }
}