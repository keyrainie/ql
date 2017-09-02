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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Utilities;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CSToolKeywordMaintain : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        List<ValidationEntity> validationKeyword;
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
        private OrderCheckItemFacade facade;
        public OrderCheckItemFacade Facade
        {
            get
            {
                return facade;
            }
            set
            {
                facade = value;
            }
        }
        public CSToolKeywordMaintain()
        {
            InitializeComponent();
            BuildValidateCondition();
        }
        private void BuildValidateCondition()
        {
            validationKeyword = new List<ValidationEntity>();
            validationKeyword.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, this.tbKeyWord.Text, ResOrderCheck.msg_ValidateKeyWords));
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            OrderCheckItemVM model = this.DataContext as OrderCheckItemVM;
            if (model.IsCA)
            {
                model.ReferenceType = "CA";
            }
            else if (model.IsCN)
            {
                model.ReferenceType = "CN";
            }
            else
            {
                model.ReferenceType = "CP";
            }
            model.Description = string.Empty;
            model.Status = OrderCheckStatus.Valid;
            Facade.CreateOrderCheckItem(model, (obj, args) =>
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
            orderCheckItemVM.IsCA = true;
            this.DataContext = orderCheckItemVM;
            isLoaded = true;
        }

        private bool ValidateInput()
        {
            bool ret = true;

            if (!ValidationHelper.Validation(this.tbKeyWord, validationKeyword))
            {
                ret = false;
            }
            return ret;
        }
    }
}
