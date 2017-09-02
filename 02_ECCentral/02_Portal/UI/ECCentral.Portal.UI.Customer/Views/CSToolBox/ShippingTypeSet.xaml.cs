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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Customer.Views
{
    [View]
    public partial class ShippingTypeSet : PageBase
    {
        List<ShippingType> leftList;
        List<OrderCheckItemVM> rightList;

        public ShippingTypeSet()
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
            CommonDataFacade commonDataFacade = new CommonDataFacade(this);
            OrderCheckItemQueryFilter queryFilter = new OrderCheckItemQueryFilter();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = int.MaxValue,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.ReferenceType = "ST";
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            OrderCheckItemFacade facade = new OrderCheckItemFacade(this);
            facade.QueryOrderCheckItem(queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                rightList = DynamicConverter<OrderCheckItemVM>.ConvertToVMList(args.Result.Rows);
                rightList = FindAll(rightList, x => x.SysNo.HasValue);

                ListBox_SelectedShippingTypeList.ItemsSource = rightList;

                commonDataFacade.GetShippingTypeList(true, (obj2, args2) =>
                {
                    leftList = args2.Result;
                    leftList = FindAll(leftList, x => x.SysNo.HasValue);

                    if (rightList != null && leftList != null)
                    {
                        foreach (OrderCheckItemVM p in rightList)
                        {
                            leftList.Remove(leftList.Where(x => x.SysNo.ToString() == p.ReferenceContent).FirstOrDefault());
                        }
                    }
                    ListBox_ShippingTypeList.ItemsSource = leftList;
                });
            });
        }

        private List<TSource> FindAll<TSource>(List<TSource> source, Func<TSource, bool> selector)
        {
            List<TSource> data = new List<TSource>();
            source.ForEach(x =>
            {
                if (selector(x))
                {
                    data.Add(x);
                }
            });

            return data;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (rightList == null || rightList.Count == 0)
            {
                Window.Alert("已选配送方式必须至少保留一项");
                return;
            }

            OrderCheckItemFacade facade = new OrderCheckItemFacade();
            BatchCreatOrderCheckItemReq req = new BatchCreatOrderCheckItemReq();
            req.orderCheckItemList = facade.ConvertToBatchOperation(rightList);
            req.ReferenceType = "ST";
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

        private void Button_MoveToRight_Click(object sender, RoutedEventArgs e)
        {
            OrderCheckItemVM itemVM = new OrderCheckItemVM();
            ShippingType item = (ShippingType)ListBox_ShippingTypeList.SelectedItem;
            if (item != null)
            {
                itemVM.ReferenceContent = item.SysNo.ToString();
                itemVM.Description = item.ShippingTypeName;
                itemVM.ReferenceType = "ST";
                itemVM.Status = 0;

                leftList.Remove(item);
                if (rightList == null)
                {
                    rightList = new List<OrderCheckItemVM>();
                }
                rightList.Add(itemVM);
                ListBox_ShippingTypeList.ItemsSource = null;
                ListBox_ShippingTypeList.ItemsSource = leftList;
                ListBox_SelectedShippingTypeList.ItemsSource = null;
                ListBox_SelectedShippingTypeList.ItemsSource = rightList;
            }
            else
            {
                Window.Alert(ResOrderCheck.Msg_SelectMoveItem);
            }
        }

        private void Button_MoveToLeft_Click(object sender, RoutedEventArgs e)
        {
            ShippingType item = new ShippingType();
            OrderCheckItemVM itemVM = (OrderCheckItemVM)ListBox_SelectedShippingTypeList.SelectedItem;
            if (itemVM != null)
            {
                item.SysNo = Convert.ToInt32(itemVM.ReferenceContent);
                item.ShippingTypeName = itemVM.Description;
                if (leftList == null)
                {
                    leftList = new List<ShippingType>();
                }
                leftList.Add(item);
                rightList.Remove(itemVM);
                ListBox_ShippingTypeList.ItemsSource = null;
                ListBox_ShippingTypeList.ItemsSource = leftList;
                ListBox_SelectedShippingTypeList.ItemsSource = null;
                ListBox_SelectedShippingTypeList.ItemsSource = rightList;
            }
            else
            {
                Window.Alert(ResOrderCheck.Msg_SelectMoveItem);
            }
        }

        #region 按钮权限控制
        private void CheckRights()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Customer_OrderCheck_ShipType_Save))
            {
                this.Button_MoveToLeft.IsEnabled = this.Button_MoveToRight.IsEnabled = this.Button_Save.IsEnabled = false;
            }
        }
        #endregion
    }
}