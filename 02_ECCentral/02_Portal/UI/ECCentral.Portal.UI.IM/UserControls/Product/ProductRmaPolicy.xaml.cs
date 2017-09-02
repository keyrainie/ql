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
using ECCentral.Portal.Basic.Interface;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductRmaPolicy : UserControl, ISave
    {
        private ProductRmaPolicyVM VM { get; set; }
        private ProductFacade facade { get; set; }
        private bool IsBrandWarranty = false;
        public int? ProductSysNo { get; set; }
        public ProductRmaPolicy()
        {
            InitializeComponent();
            this.Loaded += ProductRmaPolicy_Loaded;
           
        }

       
        void cbRmaPolicy_RmaSelectChange(object sender, EventArgs e)
        {
            RmaPolicyVM rma= cbRmaPolicy.SelectRmaPolicy;
            if (rma != null)
            {
                VM.ChangeDate = rma.ChangeDate;
                VM.ReturnDate = rma.ReturnDate;
                VM.IsRequest = rma.IsRequest;
                VM.RmaPolicySysNo = rma.SysNo;
            }
        }

        void ProductRmaPolicy_Loaded(object sender, RoutedEventArgs e)
        {
            this.cbRmaPolicy.RmaSelectChange += cbRmaPolicy_RmaSelectChange;
            VM = new ProductRmaPolicyVM();
            facade = new ProductFacade(CPApplication.Current.CurrentPage);
            facade.GetProductRMAPolicyByProductSysNo(ProductSysNo, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                if (arg.Result != null)
                {
                    VM.WarrantyDay = arg.Result.WarrantyDay==null?"":arg.Result.WarrantyDay.ToString();
                    VM.WarrantyDesc = arg.Result.WarrantyDesc;
                    VM.IsBrandWarranty = arg.Result.IsBrandWarranty == "Y";
                    VM.RmaPolicySysNo = arg.Result.RMAPolicyMasterSysNo;
                    cbRmaPolicy.BingSelectValue(VM.RmaPolicySysNo);
                   // cbRmaPolicy.VM.RmaPolicy = (from p in cbRmaPolicy.VM.Data where p.SysNo == VM.RmaPolicySysNo select p).First();
                    
                }
                this.DataContext = VM;
            });
          
        }

        #region ISave Members

        public void Save()
        {
            if (IsBrandWarranty)
            {
                if (!ValidationManager.Validate(this))
                {
                    return;
                }
            }
            facade.UpdateProductRMAPolicy(VM, ProductSysNo, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("更新商品退换货信息成功!", MessageBoxType.Success);
            });
        }

        #endregion

        private void linkEdit_Click_1(object sender, RoutedEventArgs e)
        {
            this.txtWarrantyDay.IsEnabled = true;
            this.txtWarrantyDesc.IsEnabled = true;
            IsBrandWarranty = true;
        }

        private void linkDelete_Click_1(object sender, RoutedEventArgs e)
        {
            VM.WarrantyDay = string.Empty;
            VM.WarrantyDesc = string.Empty;
            this.txtWarrantyDay.IsEnabled = false;
            this.txtWarrantyDesc.IsEnabled = false;
            IsBrandWarranty = false;
        }

        private void cbBrandWarranty_Click_1(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null)
            {
                if (cb.IsChecked==false)
                {
                    VM.WarrantyDay = string.Empty;
                    VM.WarrantyDesc = string.Empty;
                    this.txtWarrantyDay.IsEnabled = false;
                    this.txtWarrantyDesc.IsEnabled = false;
                }
            }
        }
    }
}
