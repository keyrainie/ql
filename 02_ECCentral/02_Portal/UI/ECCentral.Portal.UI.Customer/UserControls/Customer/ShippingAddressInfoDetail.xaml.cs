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

#region ECCentral Libs

using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.Resources;

#endregion ECCentral Libs

#region Newegg.Oversea.Oversea Libs

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

#endregion Newegg.Oversea.Oversea Libs

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class ShippingAddressInfoDetail : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private ShippingAddressVM m_shippingAddress;
        public ShippingAddressVM CurrentShippingAddrInfoVM
        {
            get
            {
                return m_shippingAddress;
            }
            private set
            {
                m_shippingAddress = value;
                LayoutRoot.DataContext = value;
            }
        }

        public CustomerFacade CustomerFacade
        {
            get;
            set;
        }

        public ShippingAddressInfoDetail()
        {
            InitializeComponent();
            NewShippingDataContext();
            Loaded += new RoutedEventHandler(ShippingAddressInfoDetail_Loaded);
        }

        public ShippingAddressInfoDetail(ShippingAddressVM shippingAddress)
            : this()
        {
            CurrentShippingAddrInfoVM = UtilityHelper.DeepClone(shippingAddress);
        }

        public void SetAllReadOnlyOrEnable()
        {
            foreach (var item in DetailInfo.Children)
            {
                if (item is CheckBox)
                {
                    (item as CheckBox).IsEnabled = false;
                }
                if (item is TextBox)
                {
                    (item as TextBox).IsReadOnly = true;
                }
                if (item is DatePicker)
                {
                    (item as DatePicker).IsEnabled = false;
                }
            }
            ucAreaPicker.IsEnabled = false;
            btnSave.Visibility = System.Windows.Visibility.Collapsed;
            btnCancel.Visibility = System.Windows.Visibility.Collapsed;
        }

        #region Event Handler

        private void ShippingAddressInfoDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(ShippingAddressInfoDetail_Loaded);
            CustomerFacade = new CustomerFacade(CPApplication.Current.CurrentPage);
        }

        private void NewShippingDataContext()
        {
            ShippingAddressVM model = new ShippingAddressVM()
            {
                IsDefault = false
            };
            CurrentShippingAddrInfoVM = model;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ShippingAddressVM model = LayoutRoot.DataContext as ShippingAddressVM;
            bool flag = ValidationManager.Validate(LayoutRoot);

            if (!model.HasValidationErrors && CustomerFacade != null)
            {
                if (!string.IsNullOrEmpty(model.CustomerID) && model.CustomerSysNo != null)
                {
                    CustomerFacade.UpdateShippingAddress(model, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CloseDialog(new ResultEventArgs
                        {
                            DialogResult = DialogResultType.OK,
                            Data = model
                        });
                    });
                }
                else
                {
                    var copy = UtilityHelper.DeepClone(model);
                    CurrentShippingAddrInfoVM = copy;
                    CloseDialog(new ResultEventArgs
                    {
                        DialogResult = DialogResultType.OK,
                        Data = copy
                    });
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
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