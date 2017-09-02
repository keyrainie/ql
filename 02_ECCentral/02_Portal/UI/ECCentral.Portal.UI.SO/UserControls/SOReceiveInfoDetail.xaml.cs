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
using System.ComponentModel.DataAnnotations;

#region Newegg.Oversea.Oversea Libs

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.UI.SO.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

#endregion Newegg.Oversea.Oversea Libs

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class SOReceiveInfoDetail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        public SOReceiveInfoDetail(SOReceiverInfoVM soReceiverInfoVM)
        {
            InitializeComponent();

            soReceiverInfoVM.IsDefault = false;
            gbSOReceiveInfoDetail.DataContext = soReceiverInfoVM;
            #region 加载收货地址列表    
            new OtherDomainQueryFacade().QueryCustomerShippingAddress(soReceiverInfoVM.CustomerSysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result != null)
                {
                    var list = args.Result.Convert<ShippingAddressInfo, SOReceiverInfoVM>();
                    if (list.Count > 0)
                    {
                        cmbSelectReceiveAddressBreif.ItemsSource = list;
                        cmbSelectReceiveAddressBreif.SelectedIndex = 0;
                    }
                }
            });
            #endregion
        }

        #region Event Handler

        private void cmbSelectReceiveAddressBreif_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbSOReceiveInfoDetail.DataContext = ((ComboBox)sender).SelectedValue; 
        }

        private void btnSOReveice_Confirm_Click(object sender, RoutedEventArgs e)
        {
            SOReceiverInfoVM model = gbSOReceiveInfoDetail.DataContext as SOReceiverInfoVM;
            if (model != null)
            {
                //更新地址
                new OtherDomainQueryFacade().UpdateCustomerShippingAddress(model.ConvertVM<SOReceiverInfoVM, ShippingAddressInfo>()
                    , (obj, args) => {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        //由父窗口执行数据加载操作           
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK,
                            Data = model
                        });
                });
            }
        }

        private void btnSOReveice_Cancel_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(new ResultEventArgs
            {
                DialogResult = DialogResultType.Cancel
            });
        }

        #endregion Event Handler

        #region Helper Methods

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }

        #endregion Helper Methods
    }
}
