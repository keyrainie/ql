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
using ECCentral.Portal.UI.PO.Models;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.PO.Resources;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderProductsNew : UserControl
    {
        public IDialog Dialog { get; set; }
        public PurchaseOrderFacade serviceFacade;
        public PurchaseOrderItemInfoVM newVM;
        PurchaseOrderType currentPOType;
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public IPage CurrentPage
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public PurchaseOrderProductsNew(string poStockName, PurchaseOrderType poType)
        {
            InitializeComponent();
            //默认为上海仓:
            this.newVM = new PurchaseOrderItemInfoVM() { StockName = poStockName };
            currentPOType = poType;
            this.DataContext = newVM;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (string.IsNullOrEmpty(newVM.ProductID))
            {
                CurrentWindow.Alert(ResPurchaseOrderNew.Msg_PurchaseProductNull);
                return;
            }

            #region 验证正常采购单的采购数量要大于0,负采购单的采购数量要小于0
            if (currentPOType != null)
            {
                if (currentPOType == PurchaseOrderType.Normal)
                {
                    if (Convert.ToInt32(newVM.PurchaseQty) < 0)
                    {
                        this.lblError.Text = "正常采购单的采购数量要大于0,负采购单的采购数量要小于0 !";
                        return;
                    }
                }
                else if (currentPOType == PurchaseOrderType.Negative)
                {
                    if (Convert.ToInt32(newVM.PurchaseQty) >= 0)
                    {
                        this.lblError.Text = "正常采购单的采购数量要大于0,负采购单的采购数量要小于0 !";
                        return;
                    }
                }
                else
                {
                    this.lblError.Text = string.Empty;
                }
            }

            #endregion

            //添加操作;
            this.Dialog.ResultArgs.Data = this.newVM;
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            Dialog.Close(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作:
            Dialog.Close(true);
        }

        private void ucProduct_ProductSelected(object sender, Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs e)
        {
            if (null != e.SelectedProduct)
            {
                int selectProductID = e.SelectedProduct.SysNo.Value;
                //获取默认的采购价格(商品的当前价格，当前成本)
                //TODO:这里是采购价格,不是正常价格
                if (e.SelectedProduct.VFItem == "Y" && (e.SelectedProduct.VFItem == "U" || e.SelectedProduct.VFItem == "L"))
                {
                    newVM.OrderPrice = e.SelectedProduct.PurchasePrice.HasValue ? e.SelectedProduct.PurchasePrice.Value.ToString("f2") : "0.00";
                }
                else
                {
                    newVM.OrderPrice = e.SelectedProduct.VirtualPrice.HasValue ? e.SelectedProduct.VirtualPrice.Value.ToString("f2") : "0.00";
                }
                newVM.BriefName = e.SelectedProduct.ProductName;
                //型号:
                newVM.ProductMode = e.SelectedProduct.ProductID;
                newVM.ProductName = e.SelectedProduct.ProductName;
                newVM.ReturnCost = 0m;
                //当前成本 ：
                newVM.CurrentUnitCost = e.SelectedProduct.UnitCost;
                newVM.AvailableQty = e.SelectedProduct.AvailableQty;
            }
        }
    }
}
