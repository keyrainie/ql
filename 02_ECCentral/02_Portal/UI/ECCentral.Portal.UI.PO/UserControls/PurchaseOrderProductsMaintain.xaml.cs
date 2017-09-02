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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.PO.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.PO.Models;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class PurchaseOrderProductsMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public PurchaseOrderFacade serviceFacade;
        public PurchaseOrderItemInfoVM itemVM;
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

        public PurchaseOrderProductsMaintain(string poStockName, PurchaseOrderItemInfoVM itemVM, PurchaseOrderType poType)
        {
            InitializeComponent();
            this.ucProduct.IsEnabled = false;
            this.itemVM = UtilityHelper.DeepClone<PurchaseOrderItemInfoVM>(itemVM);
            this.itemVM.StockName = poStockName;
            currentPOType = poType;
            this.Loaded += new RoutedEventHandler(PurchaseOrderProductsMaintain_Loaded);
        }

        void PurchaseOrderProductsMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= PurchaseOrderProductsMaintain_Loaded;
            this.DataContext = this.itemVM;
        }

        #region [Events]
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }

            #region 验证正常采购单的采购数量要大于0,负采购单的采购数量要小于0
            if (currentPOType != null)
            {
                if (currentPOType == PurchaseOrderType.Normal)
                {
                    if (Convert.ToInt32(itemVM.PurchaseQty) < 0)
                    {
                        this.lblError.Text = "正常采购单的采购数量要大于0,负采购单的采购数量要小于0 !";
                        return;
                    }
                }
                else if (currentPOType == PurchaseOrderType.Negative)
                {
                    if (Convert.ToInt32(itemVM.PurchaseQty) >= 0)
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
            //修改操作:
            this.Dialog.ResultArgs.Data = this.itemVM;
            this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
            Dialog.Close(true);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //取消操作:
            Dialog.Close(true);
        }
        #endregion
    }
}
