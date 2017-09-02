using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades;
using ECCentral.Portal.UI.Invoice.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Invoice.Views
{
    /// <summary>
    /// 应付款单据查询
    /// </summary>
    [View(IsSingleton = true, SingletonType = SingletonTypes.Page, NeedAccess = true)]
    public partial class PayItemListOrderQuery : PageBase
    {
        #region Private Fields

        private PayItemListOrderQueryVM queryVM;
        private PayItemListOrderQueryVM lastQueryVM;
        private PayItemFacade payItemFacade;

        #endregion Private Fields

        #region Constructor

        public PayItemListOrderQuery()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PayItemListOrderQuery_Loaded);
        }

        #endregion Constructor

        #region Event Handlers

        private void PayItemListOrderQuery_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(PayItemListOrderQuery_Loaded);
            InitData();
        }

        private void dgPayItemOrderList_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            payItemFacade.QueryCanBePayOrder(lastQueryVM, e.PageSize, e.PageIndex, e.SortField, result =>
            {
                this.dgPayItemOrderList.ItemsSource = result.Rows;
                this.dgPayItemOrderList.TotalCount = result.TotalCount;
            });
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var flag = ValidationManager.Validate(this.SeachBuilder);
            if (flag)
            {
                this.lastQueryVM = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone<PayItemListOrderQueryVM>(queryVM);

                this.dgPayItemOrderList.Bind();
            }
        }

        /// <summary>
        /// 增加付款单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Add_Click(object sender, RoutedEventArgs e)
        {
            var getSelectedItem = this.dgPayItemOrderList.SelectedItem as dynamic;
            if (null != getSelectedItem)
            {
                string url = "";
                var orderType = (PayableOrderType?)getSelectedItem.OrderType;
                if (orderType == PayableOrderType.PO)
                {
                    var orderStatus = (PurchaseOrderStatus?)getSelectedItem.OrderStatus;
                    if (orderStatus == PurchaseOrderStatus.InStocked || orderStatus == PurchaseOrderStatus.PartlyInStocked)
                    {
                        url = string.Format(ConstValue.Invoice_PayItemMaintainUrl, "?PaySysNo=" + getSelectedItem.PaySysNo);
                    }
                    //else if (orderStatus == PurchaseOrderStatus.WaitingApportion || orderStatus == PurchaseOrderStatus.WaitingInStock)
                    else if (orderStatus == PurchaseOrderStatus.WaitingInStock)
                    {
                        var queryString = string.Format("?OrderSysNo={0}&OrderType={1}", getSelectedItem.OrderSysNo, (int)getSelectedItem.OrderType);
                        url = string.Format(ConstValue.Invoice_PayItemMaintainUrl, queryString);
                    }
                }
                else if (orderType == PayableOrderType.VendorSettleOrder)
                {
                    var orderStatus = (SettleStatus?)getSelectedItem.OrderStatus;
                    if (orderStatus == SettleStatus.SettlePassed)
                    {
                        var queryString = string.Format("?OrderSysNo={0}&OrderType={1}", getSelectedItem.OrderSysNo, (int)getSelectedItem.OrderType);
                        url = string.Format(ConstValue.Invoice_PayItemMaintainUrl, queryString);
                    }
                }
                Window.Navigate(url, null, true);
            }
        }

        private void cmbOrderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (queryVM.OrderType == PayableOrderType.PO)
            {
                tbPOETP.Visibility = Visibility.Visible;
                drPOETP.Visibility = Visibility.Visible;
                tbPOStatus.Visibility = Visibility.Visible;
                drPOStatus.Visibility = Visibility.Visible;

                tbVendorSettleStatus.Visibility = Visibility.Collapsed;
                cmbVendorSettleStatus.Visibility = Visibility.Collapsed;
            }
            else if (queryVM.OrderType == PayableOrderType.VendorSettleOrder)
            {
                tbPOETP.Visibility = Visibility.Collapsed;
                drPOETP.Visibility = Visibility.Collapsed;
                tbPOStatus.Visibility = Visibility.Collapsed;
                drPOStatus.Visibility = Visibility.Collapsed;

                tbVendorSettleStatus.Visibility = Visibility.Visible;
                cmbVendorSettleStatus.Visibility = Visibility.Visible;
            }
            else
            {
                tbPOETP.Visibility = Visibility.Collapsed;
                drPOETP.Visibility = Visibility.Collapsed;
                tbPOStatus.Visibility = Visibility.Collapsed;
                drPOStatus.Visibility = Visibility.Collapsed;

                tbVendorSettleStatus.Visibility = Visibility.Collapsed;
                cmbVendorSettleStatus.Visibility = Visibility.Collapsed;
            }
        }

        #endregion Event Handlers

        #region Private Methods

        private void InitData()
        {
            payItemFacade = new PayItemFacade(this);
            queryVM = new PayItemListOrderQueryVM();
            this.SeachBuilder.DataContext = lastQueryVM = queryVM;
        }

        #endregion Private Methods
    }
}