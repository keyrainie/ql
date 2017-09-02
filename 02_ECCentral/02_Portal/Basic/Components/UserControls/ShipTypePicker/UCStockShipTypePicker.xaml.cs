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
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.Portal.Basic.Components.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using System.Windows.Data;

namespace ECCentral.Portal.Basic.Components.UserControls.ShipTypePicker
{
    public partial class UCStockShipTypePicker : UserControl
    {
        public ShipTypeFacade _shipTypeFacade;
        public StockQueryFacade _stockFacade;
        public List<ShippingType> _listShipType;
        public List<StockInfo> _listStockInfo;

        public int? SelectedShipType
        {
            get
            {
                return (int?)GetValue(SelectedShipTypeProperty);
            }
            set
            {
                SetValue(SelectedShipTypeProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedShipTypeProperty =
            DependencyProperty.Register("SelectedShipType", typeof(int?), typeof(UCStockShipTypePicker), new PropertyMetadata(null));

        public UCStockShipTypePicker()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCStockShipTypePicker_Loaded);
        }

        private void UCStockShipTypePicker_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCStockShipTypePicker_Loaded);

            var exp = this.GetBindingExpression(UCStockShipTypePicker.SelectedShipTypeProperty);

            if (exp != null && exp.ParentBinding != null)
            {
                string path = exp.ParentBinding.Path.Path;

                Binding binding = new Binding();
                binding.Path = new PropertyPath(path);
                binding.Mode = BindingMode.TwoWay;
                binding.NotifyOnValidationError = true;
                cmbShipType.SetBinding(Newegg.Oversea.Silverlight.Controls.Combox.SelectedValueProperty, binding);
            }

            LoadList();
        }

       // 加载出所有的数据
        private void LoadList()
        {
            _shipTypeFacade = new ShipTypeFacade(CPApplication.Current.CurrentPage);
            _stockFacade = new StockQueryFacade(CPApplication.Current.CurrentPage);
            _shipTypeFacade.GetShipTypeList((s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                _listShipType = args.Result;
                _listShipType.Insert(0,new ShippingType() {  ShippingTypeName = ResCommonEnum.Enum_Select });

                
                _stockFacade.QueryStockAll((f, ar) =>
                    {
                        if (ar.FaultsHandle())
                        {
                            return;
                        }
                        _listStockInfo = ar.Result;
                        _listStockInfo.Insert(0,new StockInfo() {  StockName = ResCommonEnum.Enum_Select });

                                             
                        cmbStock.ItemsSource = _listStockInfo;
                        cmbShipType.ItemsSource = _listShipType;  
                        
                        cmbStock.SelectedIndex=0;

                        cmbShipType.SelectedIndex = 0;
                    });
            });
        }

        private void cmbStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int? stockSysNo = (int?)cmbStock.SelectedValue;
           // StockQueryVM queryVM = new StockQueryVM();
            //queryVM.StockSysNo = stockInfo.SysNo;
            if (stockSysNo.HasValue)
            {
                List<ShippingType> targetList = _listShipType.Where(f => f.OnlyForStockSysNo == stockSysNo).ToList();
                targetList.Insert(0, new ShippingType() { ShippingTypeName = ResCommonEnum.Enum_Select });
                
                cmbShipType.ItemsSource = targetList;
                cmbShipType.SelectedIndex = 0;
            }
            else
            {
                cmbShipType.ItemsSource = _listShipType;
                cmbShipType.SelectedIndex = 0;
            }
           
    
        }

        private void cmbShipType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            int? shipTypeSysNo = (int?)cmbShipType.SelectedValue;
            //int? shipTypeSysNo = shipTypeInfo.SysNo;
            if (shipTypeSysNo.HasValue)
            {
                int? stockSysNo = _listShipType.Where(s => s.SysNo == shipTypeSysNo).FirstOrDefault().OnlyForStockSysNo;
                if (stockSysNo.HasValue && stockSysNo != null)
                {
                    cmbStock.SelectionChanged -= cmbStock_SelectionChanged;
                    cmbStock.SelectedValue = stockSysNo;
                    cmbStock.SelectionChanged += cmbStock_SelectionChanged;
                }
            }
        }

    }
}
