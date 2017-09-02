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
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class KeywordSet : PageBase
    {
        OrderCheckItemFacade facade;
        public KeywordSet()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            this.dataGrid1.Bind();
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_Keywords_Add))
                this.Button_New.IsEnabled = false;
        }
        private void DataGrid_Result_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            OrderCheckItemQueryFilter queryFilter = new OrderCheckItemQueryFilter();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = e.PageSize,
                PageIndex = e.PageIndex,
                SortBy = e.SortField
            };
            queryFilter.ReferenceTypeIn = "'CA','CP','CN'";
            facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<OrderCheckItemVM> list = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                List<CodeNamePair> OrderCheckItemType = new List<CodeNamePair>();
                CodeNamePairHelper.GetList("Customer", "KeyWordType", (obj2, args2) =>
                {
                    OrderCheckItemType = args2.Result;  //在顾客端拿到的类型为: List<CodeNamePair> 
                    foreach (CodeNamePair pair in OrderCheckItemType)
                    {
                        foreach (OrderCheckItemVM itemVM in list)
                        {
                            if (itemVM.ReferenceType.ToString() == pair.Code)
                            {
                                itemVM.ReferenceTypeName = pair.Name;
                            }
                        }
                    }
                });
                dataGrid1.ItemsSource = list;
                dataGrid1.TotalCount = args.Result.TotalCount;
            });
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM orderCheckItemVM = new OrderCheckItemVM();
            CSToolKeywordMaintain uctlKeywordMaintain = new CSToolKeywordMaintain();
            uctlKeywordMaintain.OrderCheckItemVM = orderCheckItemVM;
            uctlKeywordMaintain.Facade = facade;
            uctlKeywordMaintain.Dialog = Window.ShowDialog(ResOrderCheck.DialogTitle_AddKeyword, uctlKeywordMaintain, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.dataGrid1.Bind();
                }
            });
        }

        private void Hyperlink_ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_Keywords_Active))
            {
                Window.Alert(ResOrderCheck.Msg_NoRights_Keywords_ChangeStatus);
                return;
            }
            OrderCheckItemVM orderCheckItemVM = dataGrid1.SelectedItem as OrderCheckItemVM;
            if (null != orderCheckItemVM)
            {
                OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
                orderCheckItemVM.Status = orderCheckItemVM.Operator;//状态交换
                facade.UpdateOrderCheckItem(orderCheckItemVM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.dataGrid1.Bind();
                });
            }
        }
    }
}
