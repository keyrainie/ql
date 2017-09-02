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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.UI.Customer.Facades;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CSToolAmountMaintain : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        private OrderCheckItemVM orderCheckItemVM;
        public OrderCheckItemVM OrderCheckItemVM
        {
            get
            {
                return orderCheckItemVM;
            }
            set
            {
                orderCheckItemVM = value;
            }
        }
        List<ValidationEntity> ValidationForAmount;
        public CSToolAmountMaintain()
        {
            InitializeComponent();
            LoadReferenceTypeComboBoxData();
            BuildValidate();
        }

        private void BuildValidate()
        {
            ValidationForAmount = new List<ValidationEntity>();
            ValidationForAmount.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResOrderCheck.msg_ValidateAmount));
            ValidationForAmount.Add(new ValidationEntity(ValidationEnum.IsDecimal, null, ResOrderCheck.msg_ValidateAmountFormate));
        }

        private void LoadReferenceTypeComboBoxData()
        {
            CodeNamePairHelper.GetList("Customer", "AmountType", (obj, args) =>
            {
                this.Combo_ReferenceType.ItemsSource = args.Result;
                this.Combo_ReferenceType.SelectedIndex = 0;
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.tbKeyWord, ValidationForAmount))
                return;
            OrderCheckItemVM model = this.DataContext as OrderCheckItemVM;
            model.Description = string.Empty;
            model.Status = OrderCheckStatus.Valid;
            OrderCheckItemFacade facade = new OrderCheckItemFacade();
            facade.CreateOrderCheckItem(model, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                else
                {
                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.Data = args;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                }
            });
        }
        private bool isLoaded = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }
            this.DataContext = orderCheckItemVM;
            isLoaded = true;
        }

    }
}
