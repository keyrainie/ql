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
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CSToolProductAnd3CMaintain : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        public OrderCheckItemVM orderCheckItemVM;
        List<ValidationEntity> ValidationForStatus;
        public CSToolProductAnd3CMaintain()
        {
            orderCheckItemVM = new OrderCheckItemVM();
            this.DataContext = orderCheckItemVM;
            InitializeComponent();
            LoadComboBoxData();
            BuildValidate();
        }

        private void BuildValidate()
        {
            ValidationForStatus = new List<ValidationEntity>();
            ValidationForStatus.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResOrderCheck.msg_ValidateStatus));
        }

        private void LoadComboBoxData()
        {
            List<KeyValuePair<OrderCheckStatus?, string>> statusList = EnumConverter.GetKeyValuePairs<OrderCheckStatus>(EnumConverter.EnumAppendItemType.All);
            this.Combox_Status.ItemsSource = statusList;
        }

        private bool isLoaded = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                return;
            }

            isLoaded = true;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.Combox_Status, ValidationForStatus) )
                return;
            if (rbtnProductType.IsChecked.Value)
            {
                if (orderCheckItemVM.Category3No == null){
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResOrderCheck.msg_ValidateCategory);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(orderCheckItemVM.ProductID))
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResOrderCheck.msg_ValidateProduct);
                    return;
                }
            }
            OrderCheckItemFacade facade = new OrderCheckItemFacade();
            OrderCheckItemVM model = this.DataContext as OrderCheckItemVM;
            if (model.IsProductType)
            {
                model.ReferenceType = "PC3";
            }
            else if (model.IsProductSysNo)
            {
                model.ReferenceType = "PID";
            }
            model.Description = model.Category3No.HasValue ? model.Category3No.ToString() : string.Empty;
            model.ReferenceContent = model.ProductID;
            facade.CreateOrderCheckItem(model, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            });
        }
    }
}
