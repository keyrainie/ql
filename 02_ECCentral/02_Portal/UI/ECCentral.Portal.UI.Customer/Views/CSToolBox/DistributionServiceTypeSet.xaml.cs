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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class DistributionServiceTypeSet : PageBase
    {
        public OrderCheckItemVM model;

        public DistributionServiceTypeSet()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            model = new OrderCheckItemVM();
            this.DataContext = model;
            LoadComboBoxData();
            base.OnPageLoad(sender, e);
            CheckRights();
        }

        private void LoadComboBoxData()
        {
            CodeNamePairHelper.GetList("Customer", "DistributionServiceType", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                this.Combox_DTServiceType.ItemsSource = args.Result;
                this.Combox_DTServiceType.SelectedIndex = 0;
            });
            CommonDataFacade commonDataFacade = new CommonDataFacade(this);
            commonDataFacade.GetShippingTypeList(true, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.Combox_ServiceObject.ItemsSource = args.Result;
                this.Combox_ServiceObject.SelectedIndex = 0;
            });
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.DataGrid_DTServiceList.Bind();
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
            queryFilter.ReferenceContent = (Combox_ServiceObject.SelectedIndex == 0 ? string.Empty : model.ReferenceContent);
            queryFilter.ReferenceType = (Combox_DTServiceType.SelectedIndex == 0 ? string.Empty : model.ReferenceType);
            queryFilter.ReferenceTypeIn = "'DT11','DT12'";
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                List<OrderCheckItemVM> list = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                List<CodeNamePair> OrderCheckItemType = new List<CodeNamePair>();
                CodeNamePairHelper.GetList("Customer", "DistributionServiceType", (obj2, args2) =>
                {
                    OrderCheckItemType = args2.Result;
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
                DataGrid_DTServiceList.ItemsSource = list;
                DataGrid_DTServiceList.TotalCount = args.Result.TotalCount;
            });
        }

        private void Hyperlink_ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_DistributionService_Active))
            {
                Window.Alert(ResOrderCheck.Msg_NoRigths_DT_ChangeStatus);
                return;
            }
            OrderCheckItemVM orderCheckItemVM = this.DataGrid_DTServiceList.SelectedItem as OrderCheckItemVM;
            if (null != orderCheckItemVM)
            {
                OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
                orderCheckItemVM.Status = orderCheckItemVM.Operator;
                facade.UpdateOrderCheckItem(orderCheckItemVM, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    this.DataGrid_DTServiceList.Bind();
                });
            }
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM orderCheckItemVM = new OrderCheckItemVM();
            orderCheckItemVM.ReferenceType = "";
            CSToolDistributionServiceMaintain uctlDTMaintain = new CSToolDistributionServiceMaintain();
            uctlDTMaintain.OrderCheckItemVM = orderCheckItemVM;
            //uctlDTMaintain.Page = this;
            uctlDTMaintain.Dialog = Window.ShowDialog(ResOrderCheck.DialogTitle_DTMaitain, uctlDTMaintain, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK && args.Data != null)
                {
                    this.DataGrid_DTServiceList.Bind();
                }
            });
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_DTServiceList.SelectedItem != null)
            {
                OrderCheckItemVM orderCheckItemVM = DataGrid_DTServiceList.SelectedItem as OrderCheckItemVM;
                if (orderCheckItemVM.Description.Split(',').Length > 1)
                {
                    orderCheckItemVM.ServiceTime_First = DateTime.Parse(orderCheckItemVM.Description.Split(',')[0]);
                    orderCheckItemVM.ServiceTime_Second = DateTime.Parse(orderCheckItemVM.Description.Split(',')[1]);
                }
                else
                {
                    orderCheckItemVM.ServiceTime_First = DateTime.Parse(orderCheckItemVM.Description);
                }
                CSToolDistributionServiceMaintain uctlDTMaintain = new CSToolDistributionServiceMaintain();
                uctlDTMaintain.OrderCheckItemVM = orderCheckItemVM;
                //uctlDTMaintain.Page = this;
                uctlDTMaintain.Dialog = Window.ShowDialog(ResOrderCheck.DialogTitle_DTMaitain, uctlDTMaintain, (obj, args) =>
                {
                    if (args.DialogResult == DialogResultType.OK)
                    {
                        this.DataGrid_DTServiceList.Bind();
                    }
                });
            }
            else
            {
                Window.Alert("请选择要编辑的项。");
            }
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_DistributionService_Add))
                this.Button_New.IsEnabled = false;
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_DistributionService_Edit))
                this.Button_Edit.IsEnabled = false;
        }
        #endregion
    }
}