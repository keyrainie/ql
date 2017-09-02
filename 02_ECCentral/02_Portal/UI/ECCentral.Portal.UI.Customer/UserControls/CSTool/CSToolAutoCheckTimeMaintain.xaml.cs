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
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Resources;
using ECCentral.Portal.Basic.Converters;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CSToolAutoCheckTimeMaintain : UserControl
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


        public CSToolAutoCheckTimeMaintain()
        {
            InitializeComponent();
        }


        public bool Validate()
        {
            if (this.dpBegin.SelectedDateTime == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择开始时间！");
                return false;
            }
            if (this.dpEnd.SelectedDateTime == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择结束时间！");
                return false;
            }
            if (this.dpEnd.SelectedDateTime <= this.dpBegin.SelectedDateTime)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("结束时间应大于开始时间！");
                return false;
            }
            return true;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
                return;
            OrderCheckItemVM model = this.DataContext as OrderCheckItemVM;
            model.ReferenceType = "SA";
            model.Status = OrderCheckStatus.Valid;
            if (dpBegin.SelectedDateTime.HasValue)
                model.ReferenceContent = dpBegin.SelectedDateTime.Value.ToString(ResConverter.DateTime_LongFormat);
            if (dpEnd.SelectedDateTime.HasValue)
                model.Description = dpEnd.SelectedDateTime.Value.ToString(ResConverter.DateTime_LongFormat);
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
