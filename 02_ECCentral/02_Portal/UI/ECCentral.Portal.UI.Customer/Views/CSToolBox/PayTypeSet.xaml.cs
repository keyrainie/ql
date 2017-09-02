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
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Customer.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Components.UserControls.PayTypePicker;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class PayTypeSet : PageBase
    {
        List<PayType> leftList;
        List<OrderCheckItemVM> rightList;

        public PayTypeSet()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            LoadListBoxData();
            CheckRights();
        }

        private void LoadListBoxData()
        {
            PayTypeFacade commonDataFacade = new PayTypeFacade(this);
            OrderCheckItemQueryFilter queryFilter = new OrderCheckItemQueryFilter();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.ReferenceType = "PT";

            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                rightList = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                ListBox_SelectedPayTypeList.ItemsSource = rightList;

                commonDataFacade.GetPayTypeList((obj2, args2) =>
                {
                    leftList = args2.Result;
                    if (rightList != null && leftList != null)
                    {
                        foreach (OrderCheckItemVM p in rightList)
                        {
                            leftList.Remove(leftList.Where(x => x.SysNo.ToString() == p.ReferenceContent).FirstOrDefault());
                        }
                    }
                    ListBox_PayTypeList.ItemsSource = leftList;
                });
            });
        }

        private void Button_MoveToRight_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM itemVM = new OrderCheckItemVM();
            PayType item = (PayType)ListBox_PayTypeList.SelectedItem;
            if (item != null)
            {
                itemVM.ReferenceContent = item.SysNo.ToString();
                itemVM.Description = item.PayTypeName;
                itemVM.ReferenceType = "PT";
                itemVM.Status =  OrderCheckStatus.Valid;
                leftList.Remove(item);
                if (rightList == null)
                {
                    rightList = new List<OrderCheckItemVM>();
                }
                rightList.Add(itemVM);
                ListBox_PayTypeList.ItemsSource = null;
                ListBox_PayTypeList.ItemsSource = leftList;
                ListBox_SelectedPayTypeList.ItemsSource = null;
                ListBox_SelectedPayTypeList.ItemsSource = rightList;
            }
            else
            {
                Window.Alert(ResOrderCheck.Msg_SelectMoveItem);
            }
        }

        private void Button_MoveToLeft_Click(object sender, RoutedEventArgs e)
        {
            PayType item = new PayType();
            OrderCheckItemVM itemVM = (OrderCheckItemVM)ListBox_SelectedPayTypeList.SelectedItem;
            if (itemVM != null)
            {
                item.SysNo = Convert.ToInt32(itemVM.ReferenceContent);
                item.PayTypeName = itemVM.Description;
                if (leftList == null)
                {
                    leftList = new List<PayType>();
                }
                leftList.Add(item);
                rightList.Remove(itemVM);
                ListBox_PayTypeList.ItemsSource = null;
                ListBox_PayTypeList.ItemsSource = leftList;
                ListBox_SelectedPayTypeList.ItemsSource = null;
                ListBox_SelectedPayTypeList.ItemsSource = rightList;
            }
            else
            {
                Window.Alert(ResOrderCheck.Msg_SelectMoveItem);
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (rightList == null || rightList.Count == 0)
            {
                Window.Alert("已选支付方式必须至少保留一项");
                return;
            }
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            BatchCreatOrderCheckItemReq req = new BatchCreatOrderCheckItemReq();
            req.orderCheckItemList = facade.ConvertToBatchOperation(rightList);
            req.ReferenceType = "PT";
            facade.BatchCreateOrderCheckItem(req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                else
                {
                    Window.Alert(ResOrderCheck.Msg_SaveOk);
                }
            });
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_PayType_Save))
            {
                this.Button_MoveToLeft.IsEnabled = this.Button_MoveToRight.IsEnabled = this.Button_Save.IsEnabled = false;
            }
        }
        #endregion
    }
}