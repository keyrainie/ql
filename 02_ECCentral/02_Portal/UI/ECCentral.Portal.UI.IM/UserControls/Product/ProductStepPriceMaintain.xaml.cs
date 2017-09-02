using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
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

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class ProductStepPriceMaintain : UserControl
    {
        public IDialog DiolgHander { get; set; }
        public IPage Page { get; set; }
        public ProductStepPriceInfoVM _view;
        public ProductPriceRequestFacade _facade;

        public ProductStepPriceMaintain(ProductStepPriceInfoVM entity)
        {
            InitializeComponent();
            if (entity == null)
            {
                _view = new ProductStepPriceInfoVM();
                ucProductPicker.IsEnabled = true;
            }
            else
            {
                _view = entity;
                ucProductPicker.IsEnabled = false;
            }
            _facade = new ProductPriceRequestFacade(CPApplication.Current.CurrentPage);
            LayoutRoot.DataContext = _view;
        }


        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            if (_view.ProductSysNo != null && _view.StepPrice != null)
            {
                _view.InUser = CPApplication.Current.LoginUser.LoginName;
                _facade.CreateProductStepPrice(_view, (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (args.Result == 1)
                    {
                        Message("保存成功！");
                        CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
                    }
                });
            }



            //if (ValidationManager.Validate(LayoutRoot))
            //{
            //    if (!_view.SysNo.HasValue)
            //    {
            //        if (string.IsNullOrWhiteSpace(this.AreaPichker.SelectedCitySysNo))
            //        {
            //            Message("请选择送货市级区域");
            //            return;
            //        }
            //        _view.AreaSysNo = int.Parse(this.AreaPichker.SelectedCitySysNo);
            //        _facade.CreateShipTypeAreaPrice(_view, (s, args) =>
            //        {
            //            if (args.FaultsHandle())
            //            {
            //                return;
            //            }
            //            Message("操作已成功");
            //            CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
            //        });

            //    }
            //    else
            //    {
            //        _facade.UpdateShipTypeAreaPrice(_view, (s, args) =>
            //        {
            //            if (args.FaultsHandle())
            //            {
            //                return;
            //            }
            //            Message("操作已成功");
            //            CloseDialog(new ResultEventArgs() { DialogResult = DialogResultType.OK });
            //        });
            //    }
            //}
        }


        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _view = new ProductStepPriceInfoVM();
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
            if (DiolgHander != null)
            {
                DiolgHander.ResultArgs.Data = null;
                DiolgHander.ResultArgs.DialogResult = DialogResultType.Cancel;
                DiolgHander.Close();
            }
        }
    }
}
