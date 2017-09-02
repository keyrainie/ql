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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.UserControls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.Inventory.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class VirtualRequestMaintainBatch : PageBase
    {
        VirtualRequestBatchView PageView;
        VirtualRequestQueryFacade QueryFacade;
        VirtualRequestVM RequestVM;
        public VirtualRequestMaintainBatch()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            RequestVM = new VirtualRequestVM();
            PageView = new VirtualRequestBatchView();
            QueryFacade = new VirtualRequestQueryFacade(this);
            expanderCondition.DataContext = PageView.QueryInfo;
            dgProductInventoryInfo.DataContext = PageView;
            gridRequstInfo.DataContext = RequestVM;

            CodeNamePairHelper.GetList(ConstValue.DomainName_Inventory, "VirtualRequestType", CodeNamePairAppendItemType.None, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.cmbVirtualTypeList.ItemsSource = args.Result;
                this.cmbVirtualTypeList.SelectedIndex = 0;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            cbDataGridSelectAll.IsChecked = false;
            dgProductInventoryInfo.Bind();
        }

        private void btnSetRequestBatch_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.tbVirtualQtyPresetBatch))
            {
                return;
            }
            if (ucSelectStockComboBox.SelectedStockSysNo < 1)
            {
                Window.Alert("请选择虚库渠道仓库!");
                return;
            }
            int selectCount = 0;
            if (PageView.Result != null)
            {
                PageView.Result.ForEach(item =>
                {
                    if (item.IsChecked)
                    {
                        selectCount++;
                        item.RequestQuantity = string.IsNullOrEmpty(RequestVM.VirtualQuantity) ? null : RequestVM.VirtualQuantity;
                        item.Reason = RequestVM.RequestNote;
                        item.StockSysNo = ucSelectStockComboBox.SelectedStockSysNo;
                    }
                });
            }
            if (selectCount <= 0)
            {
                Window.Alert("请选择您要设置的行!");
                return;
            }
        }

        private void btnCreateRequestBatch_Click(object sender, RoutedEventArgs e)
        {
            if (ucSelectStockComboBox.SelectedStockSysNo == null || ucSelectStockComboBox.SelectedStockSysNo < 1)
            {
                Window.Alert("请选择虚库渠道仓库!");
                return;
            }
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                List<VirtualRequestVM> requestList = new List<VirtualRequestVM>();
                if (PageView.Result != null)
                {
                    PageView.Result.ForEach(item =>
                    {
                        if (item.IsChecked)
                        {
                            requestList.Add(new VirtualRequestVM
                            {
                                StartDate = RequestVM.StartDate,
                                EndDate = RequestVM.EndDate,
                                VirtualType = RequestVM.VirtualType,
                                VirtualQuantity = item.RequestQuantity,
                                RequestNote = item.Reason,
                                StockSysNo = ucSelectStockComboBox.SelectedStockSysNo,
                                ProductID = item.ItemCode,
                                ProductName = item.ItemName,
                                ProductSysNo = item.ItemNumber
                            });
                        }
                    });
                }

                if (requestList == null || requestList.Count == 0)
                {
                    Window.Alert("请提供创建虚库的商品信息!");
                    return;
                }

                new VirtualRequestMaintainFacade(this).ApplyRequest(requestList, (vmList) =>
                    {
                        if (vmList != null)
                        {
                            Window.Alert("提示", ResShiftRequestQuery.Msg_VirtualApplySuccess, MessageType.Information, (obj, args) =>
                            {
                                if (args.DialogResult == DialogResultType.Cancel)
                                {
                                    Window.Refresh();
                                }
                            });
                        }
                    });
            }
        }

        private void linkBtnExpiredDate3Day_Click(object sender, RoutedEventArgs e)
        {
            DateTime nowDate = DateTime.Now;
            RequestVM.StartDate = nowDate;
            RequestVM.EndDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day).AddDays(3 + 1).Subtract(new TimeSpan(0, 0, 1));
        }

        private void LinkBtnExpiredDate7Day_Click(object sender, RoutedEventArgs e)
        {
            DateTime nowDate = DateTime.Now;
            RequestVM.StartDate = nowDate;
            RequestVM.EndDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day).AddDays(7 + 1).Subtract(new TimeSpan(0, 0, 1));
        }

        private void linkBtnNoExpiredDate_Click(object sender, RoutedEventArgs e)
        {
            RequestVM.StartDate = DateTime.Now;
            RequestVM.EndDate = null;
        }

        private void chbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            if (PageView.Result != null)
            {
                PageView.Result.ForEach(item =>
                {
                    item.IsChecked = (sender as CheckBox).IsChecked.Value;
                    item.RequestQuantity = null;
                    item.Reason = null;
                });
            }
        }

        private void dgProductInventoryInfo_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            PageView.QueryInfo.PagingInfo = new QueryFilter.Common.PagingInfo
            {
                PageIndex = e.PageIndex,
                PageSize = e.PageSize,
                SortBy = e.SortField

            };
            QueryFacade.QueryProducts(PageView.QueryInfo, (totalCount, vmList) =>
            {
                PageView.TotalCount = totalCount;
                PageView.Result = vmList;
                dgProductInventoryInfo.ItemsSource = PageView.Result;
                dgProductInventoryInfo.TotalCount = PageView.TotalCount;
            });
        }

    }
}
