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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models.Floor;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Floor
{
    public partial class UCBrandForSection : UserControl
    {
        public IDialog Dialog { get; set; }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        FloorSectionBrandVM vm = new FloorSectionBrandVM();


        public UCBrandForSection()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCBrandForSection_Loaded);
        }
        public UCBrandForSection(FloorSectionBrandVM inVM)
        {
            InitializeComponent();
            vm = inVM;
            Loaded += new RoutedEventHandler(UCBrandForSection_Loaded);
        }

        private void UCBrandForSection_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCBrandForSection_Loaded);
            InitBrands();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = vm;
                Dialog.Close(true);
            }
        }

        private void InitBrands()
        {
            List<KeyValuePair<int, string>> brandList = new List<KeyValuePair<int, string>>();
            var facade = new FloorFacade(CPApplication.Current.CurrentPage);
            facade.QueryBrand((s, args) =>
            {
                var data = args.Result as dynamic;
                foreach (var row in data.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(row.BrandName_Ch))
                        brandList.Add(new KeyValuePair<int, string>(row.SysNo, row.BrandName_Ch));
                }
                cmbBrands.ItemsSource = brandList;
                DataContext = vm;
            });
        }
    }
}
