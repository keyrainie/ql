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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades.Promotion;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls.SaleGift
{
    public partial class UCSaleGiftProductRange : UserControl
    {
        bool isLoaded = false;
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCSaleGiftProductRange()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCSaleGiftProductRange_Loaded);
        }

        void UCSaleGiftProductRange_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel;
            if (vm.IsOnlyViewMode)
            { 
                OperationControlStatusHelper.SetControlsStatus(this.ucLimitScope, true);
                OperationControlStatusHelper.SetControlsStatus(this.ucLimitProduct, true); 
            }


            //if (vm.Type == SaleGiftType.Full)
            //{
            //    this.ucLimitProduct.dgProductOnly.Columns[8].Visibility = System.Windows.Visibility.Collapsed;
            //}


            isLoaded = true;
        }

        private void btnSaveProductRange_Click(object sender, RoutedEventArgs e)
        {

            SaleGiftInfoViewModel vm = this.DataContext as SaleGiftInfoViewModel; 

            ValidationManager.Validate(this.ucLimitProduct.dgProductOnly);

            foreach (SaleGift_RuleSettingViewModel rowVM in vm.ProductCondition)
            {
                if (rowVM.HasValidationErrors) return;

                if (rowVM.RelProduct != null && rowVM.RelProduct.HasValidationErrors) return;
            }

            foreach(SaleGift_RuleSettingViewModel rm in vm.ProductOnlyList)
            {
                if (rm.HasValidationErrors) return;

                if (rm.RelProduct != null && rm.RelProduct.HasValidationErrors) return;
            }           

            SaleGiftFacade facade = new SaleGiftFacade(CPApplication.Current.CurrentPage);
            facade.SetSaleGiftSaleRules(vm, (obj, args) =>
                {
                    CurrentWindow.Alert("主商品规则保存成功！");
                });
        }

         
    }
}
