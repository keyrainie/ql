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
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.Portal.UI.MKT.Models.Floor;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Floor
{
    public partial class UCProductForSection : UserControl
    {
        public IDialog Dialog { get; set; }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public UCProductForSection()
        {
            InitializeComponent();
            LayoutRoot.DataContext = model;
            Loaded += new RoutedEventHandler(UCProductForSection_Loaded);
        }
        FloorSectionProductVM model = new FloorSectionProductVM();

        public UCProductForSection(FloorSectionProductVM vm)
        {
            InitializeComponent();
            model = vm;
            LayoutRoot.DataContext = model;
            Loaded += new RoutedEventHandler(UCProductForSection_Loaded);
        }

        private void UCProductForSection_Loaded(object sender, RoutedEventArgs e)
        {
            ucProductPicker.LoadProductBySysNo();
            this.Loaded -= UCProductForSection_Loaded;
        }

        private void ucProductPicker_ProductSelected(object sender, Basic.Components.UserControls.ProductPicker.ProductSelectedEventArgs e)
        {
            ProductVM vm = e.SelectedProduct as ProductVM;
            if (vm != null)
            {
                model.ProductTitle = vm.ProductName;
                model.ProductPrice = vm.CurrentPrice;
                model.ProductID = vm.ProductID;
                model.ProductSysNo = vm.SysNo.Value;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = model;
                Dialog.Close(true);
            }
        }
    }
}
