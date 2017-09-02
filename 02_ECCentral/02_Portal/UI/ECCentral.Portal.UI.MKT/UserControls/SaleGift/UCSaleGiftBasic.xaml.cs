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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.UserControls.SaleGift
{
    public partial class UCSaleGiftBasic : UserControl
    {
        bool isLoaded = false;
        public UCSaleGiftBasic()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSaleGiftBasic_Loaded);
        }

        void UCSaleGiftBasic_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            isLoaded = true;
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            if (vm.Type.Value == SaleGiftType.Full )
                //vm.Type.Value == SaleGiftType.FirstOrder || vm.Type.Value == SaleGiftType.Additional)
            {
                tbMinAmount.SetReadOnly(false);
            }
            else
            {
                tbMinAmount.SetReadOnly(true);
            }

            if (vm.SysNo.HasValue)
            {
                cmbType.IsEnabled = false;
            }
            else
            {
                cmbType.IsEnabled = true;
                if (vm.Type == SaleGiftType.Full || vm.Type == SaleGiftType.Vendor)
                    ////|| vm.Type.Value == SaleGiftType.FirstOrder || vm.Type.Value == SaleGiftType.Additional)
                {
                    vendorPicker.IsEnabled = true;
                }
                else
                {
                    vendorPicker.IsEnabled = false;
                }
            }
        }


    }
}
