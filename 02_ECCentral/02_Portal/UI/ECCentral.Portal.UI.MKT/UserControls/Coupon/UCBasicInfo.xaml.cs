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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;

namespace ECCentral.Portal.UI.MKT.UserControls.Coupon
{
    public partial class UCBasicInfo : UserControl
    {
        bool isLoaded = false;
        public UCBasicInfo()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBasicInfo_Loaded);   
        }

        
        void UCBasicInfo_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
           
            CouponsInfoViewModel vm = this.DataContext as CouponsInfoViewModel;
            isLoaded = true;
            if (vm != null && vm.MerchantSysNo.HasValue && vm.MerchantSysNo.Value > 0)
            {
                ucVendor.LoadVendorInfo(vm.MerchantSysNo.Value);
            }
        }

        

        //public event SelectionChangedEventHandler OnMKTChannelTypeChanged;
        private void cmbMKTChannelType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (this.OnMKTChannelTypeChanged != null)
            //{
            //    this.OnMKTChannelTypeChanged(sender, e);
            //}
            CouponsInfoViewModel vm = (CouponsInfoViewModel)this.DataContext;
            
            if (vm.CouponChannelType.HasValue && vm.CouponChannelType == CouponsMKTType.MKTPM)
            {                
                tbEIMSSysno.IsReadOnly = false;
                tbEIMSSysno.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                vm.EIMSSysNo = null;
                tbEIMSSysno.IsReadOnly = true;
                tbEIMSSysno.Background = new SolidColorBrush(Colors.Transparent);
            }

            vm.ValidationErrors.Clear();
        }

        public event SelectionChangedEventHandler OnProductRangeTypeChanged;
        private void cmbProductRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.OnProductRangeTypeChanged != null)
            {
                this.OnProductRangeTypeChanged(sender, e);
            }
        }



         

       

        
    }


    

}
