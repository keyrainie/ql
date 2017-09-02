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
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class FPSet : PageBase
    {
        List<CodeNamePair> leftList;
        List<OrderCheckItemVM> rightList;
        public FPSet()
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
            OrderCheckItemQueryFilter queryFilter = new OrderCheckItemQueryFilter();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.ReferenceType = "FP";
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                rightList = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                ListBox_SelectedPayTypeList.ItemsSource = rightList;

                CodeNamePairHelper.GetList("Customer", "FPType", (obj2, args2) =>
                {
                    leftList = args2.Result;
                    if (rightList != null && leftList != null)
                    {
                        foreach (OrderCheckItemVM p in rightList)
                        {
                            leftList.Remove(leftList.Where(x => x.Code == p.ReferenceContent).FirstOrDefault());
                        }
                    }
                    ListBox_PayTypeList.ItemsSource = leftList;
                });
            });
        }
        private void Button_MoveToRight_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM itemVM = new OrderCheckItemVM();
            CodeNamePair item = (CodeNamePair)ListBox_PayTypeList.SelectedItem;
            if (item != null)
            {
                itemVM.ReferenceContent = item.Code;
                itemVM.Description = item.Name;
                itemVM.ReferenceType = "FP";
                itemVM.Status = 0;
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
            CodeNamePair item = new CodeNamePair();
            OrderCheckItemVM itemVM = (OrderCheckItemVM)ListBox_SelectedPayTypeList.SelectedItem;
            if (itemVM != null)
            {
                item.Code = itemVM.ReferenceContent;
                item.Name = itemVM.Description;
                if (leftList == null)
                {
                    leftList = new List<CodeNamePair>();
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
                Window.Alert("已选FP状态必须至少保留一项");
                return;
            }

            OrderCheckItemFacade facade = new OrderCheckItemFacade();
            BatchCreatOrderCheckItemReq req = new BatchCreatOrderCheckItemReq();
            req.orderCheckItemList = facade.ConvertToBatchOperation(rightList);
            req.ReferenceType = "FP";
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
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_FP_Save))
            {
                this.Button_MoveToLeft.IsEnabled = this.Button_MoveToRight.IsEnabled = this.Button_Save.IsEnabled = false;
            }
        }
        #endregion
    }
}
