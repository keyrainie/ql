using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Common.Facades;
using ECCentral.Portal.UI.Common.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.Common.UserControls
{
    public partial class ShipTypeAreaPriceMaintain : UserControl
    {
        public IDialog DiolgHander { get; set; }
        public IPage Page { get; set; } 
        public ShipTypeAreaPriceInfoVM _view;
        public ShipTypeFacade _facade;
        public ShipTypeAreaPriceInfo _entity;
        public ShipTypeAreaPriceMaintain(ShipTypeAreaPriceInfoVM entity)
        {
            InitializeComponent();
            if (entity == null)
            {
                _view = new ShipTypeAreaPriceInfoVM();
            }
            else
            {
                _view = entity;
            }
            Loaded += new RoutedEventHandler(ShipTypeAreaPriceMaintain_Loaded);

        }
        public void ShipTypeAreaPriceMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            _facade = new ShipTypeFacade(CPApplication.Current.CurrentPage);
            BindComboBoxData();
            LayoutRoot.DataContext = _view;
        }

        private void BindComboBoxData()
        {
            //商户:
            this.Merchant.ItemsSource = EnumConverter.GetKeyValuePairs<CompanyCustomer>(EnumConverter.EnumAppendItemType.All);
            this.Merchant.SelectedIndex = 0;
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {

            if (ValidationManager.Validate(LayoutRoot))
            {
                if (!_view.SysNo.HasValue)
                {
                    if(string.IsNullOrWhiteSpace(this.AreaPichker.SelectedCitySysNo))
                    {
                        Message("请选择送货市级区域");
                        return;
                    }
                    _view.AreaSysNo =int.Parse(this.AreaPichker.SelectedCitySysNo);
                    _facade.CreateShipTypeAreaPrice(_view, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Message("操作已成功");
                            CloseDialog(new ResultEventArgs() { DialogResult=DialogResultType.OK});
                        });

                }
                else
                {
                    _facade.UpdateShipTypeAreaPrice(_view, (s, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }
                            Message("操作已成功");
                            CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                        });
                }
            }
        }
        

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _view = new ShipTypeAreaPriceInfoVM();
            LayoutRoot.DataContext = _view;
        }
        private void Message(string msg)
        {
            Page.Context.Window.Alert(msg, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
        }
        private void CloseDialog(ResultEventArgs args)
        {
            if (DiolgHander != null)
            {
                DiolgHander.ResultArgs = args;
                DiolgHander.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if(DiolgHander!=null)
            {
                DiolgHander.ResultArgs.Data=null;
                DiolgHander.ResultArgs.DialogResult = DialogResultType.Cancel;
                DiolgHander.Close();
            }
        }
    }
}
