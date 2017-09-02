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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Service.IM.Restful;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCGiftCardProductMaintain : UserControl
    {
        public GiftCardProductVM giftCardProductVM;
        
        private GiftCardFacade facade;
        public IDialog Dialog { get; set; }

        //礼品卡商品类别

        public int? C3SysNo
        {
            get;
            set;
        }

        public UCGiftCardProductMaintain()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCGiftCardProductMaintain_Loaded);
        }

        void UCGiftCardProductMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCGiftCardProductMaintain_Loaded);
            facade = new GiftCardFacade(CPApplication.Current.CurrentPage);
            giftCardProductVM = giftCardProductVM ?? new GiftCardProductVM();
            this.LayoutRoot.DataContext = giftCardProductVM;
            
            voucherProduct.ProductSelected += voucherProduct_ProductSelected;

            facade.GetGiftCardC3SysNo((no) =>
            {
                C3SysNo = no;
            });
        }

        private void voucherProduct_ProductSelected(object sender, ProductSelectedEventArgs e)
        {
            ProductVM product = e.SelectedProduct;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            if (voucherProduct.SelectedProductInfo != null && voucherProduct.SelectedProductInfo.C3SysNo != C3SysNo)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("相关联的商品不是礼品卡商品!", MessageType.Warning);
                return;
            }
            if (giftCardProductVM.SysNo>0)
            {
                UpdateVoucherProduct();
            }
            else
            {
                SaveVoucherProduct();
            }
        }


        #region  辅助方法
       
        private void UpdateVoucherProduct()
        {
            facade.UpdateVoucherProductInfo(giftCardProductVM, (result) =>
            {
                if (result == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("更新失败!", MessageType.Warning);
                  
                }
                else
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("提示", "更新成功", MessageType.Information, (obj, args) =>
                    {
                        CloseDialog((new ResultEventArgs() { DialogResult = DialogResultType.OK }));
                    });
                }
            });
        }

        private void SaveVoucherProduct()
        {
            facade.AddGiftVoucherProductInfo(giftCardProductVM, (result) =>
            {
                int sysNo = result;
                if (sysNo > 0)
                {
                   CPApplication.Current.CurrentPage.Context.Window.Alert("提示", "创建成功!", MessageType.Information, (obj, args) =>
                    {
                        CloseDialog((new ResultEventArgs() { DialogResult = DialogResultType.OK }));
                    });
                }
            });
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
        #endregion
    }
}