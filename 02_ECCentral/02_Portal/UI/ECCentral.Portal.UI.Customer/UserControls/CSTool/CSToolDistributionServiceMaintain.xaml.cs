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
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Components.Facades;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.UI.Customer.Resources;

namespace ECCentral.Portal.UI.Customer.UserControls
{
    public partial class CSToolDistributionServiceMaintain : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }
        //public IPage Page
        //{
        //    get;
        //    set;
        //}
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

        List<ValidationEntity> ValidationListForServiceType;
        List<ValidationEntity> ValidationListForServiceObject;
        public CSToolDistributionServiceMaintain()
        {
            InitializeComponent();
            LoadComboBoxData();
            BuildValidateCondition();
        }

        private void BuildValidateCondition()
        {
            ValidationListForServiceType = new List<ValidationEntity>();
            ValidationListForServiceObject = new List<ValidationEntity>();
            ValidationListForServiceType.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, Combox_DTServiceType.SelectedValue, ResOrderCheck.msg_ValidateServiceType));
            ValidationListForServiceObject.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, Combox_ServiceObject.SelectedValue, ResOrderCheck.msg_ValidateServiceObject));
        }

        public bool Validate()
        {
            var r = ValidationHelper.Validation(this.Combox_DTServiceType, ValidationListForServiceType) && ValidationHelper.Validation(this.Combox_ServiceObject, ValidationListForServiceObject);
            if ((this.DataContext as OrderCheckItemVM).ReferenceType == "DT11")
            {
                if (this.tbServiceTime_First.Value == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("服务时间不能为空！");
                    return false;
                }
                return r;
            }
            else
            {
                if (this.tbServiceTime_First.Value == null || this.tbServiceTime_Second.Value == null)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("服务时间不能为空！");
                    return false;
                }
                return r;
            }
        }

        private void LoadComboBoxData()
        {
            CodeNamePairHelper.GetList("Customer", "DistributionServiceType", CodeNamePairAppendItemType.Select, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.Combox_DTServiceType.ItemsSource = args.Result;
            });
            CommonDataFacade commonDataFacade = new CommonDataFacade(CPApplication.Current.CurrentPage);
            commonDataFacade.GetShippingTypeList(true, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                this.Combox_ServiceObject.ItemsSource = args.Result;
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate())
                return;
            OrderCheckItemFacade facade = new OrderCheckItemFacade();

            OrderCheckItemVM model = this.DataContext as OrderCheckItemVM;
            if (model.ReferenceType == "DT11")
            {
                model.Description = model.ServiceTime_First.Value.ToString("HH:mm");
            }
            else
            {
                if (model.ServiceTime_Second.Value.TimeOfDay <= model.ServiceTime_First.Value.TimeOfDay)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请保证时间点一小于时间点二！");
                    return;
                }
                model.Description = model.ServiceTime_First.Value.ToString("HH:mm") + "," + model.ServiceTime_Second.Value.ToString("HH:mm");
            }
            if (model.SysNo.HasValue) //编辑
            {
                facade.UpdateOrderCheckItem(model, (obj, args) =>
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
            else
            {
                model.Status = OrderCheckStatus.Valid;
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

        private void Combox_DTServiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Combox_DTServiceType.SelectedValue == null)
            {
                lbServiceTime.Visibility = Visibility.Collapsed;
                StackPanel_Time.Visibility = Visibility.Collapsed;
                tbServiceTime_First.Visibility = Visibility.Collapsed;
                this.tbServiceTime_Second.Visibility = Visibility.Collapsed;
                return;
            }
            if (Combox_DTServiceType.SelectedValue.ToString() == "DT11")
            {
                lbServiceTime.Visibility = Visibility.Visible;
                StackPanel_Time.Visibility = Visibility.Visible;
                tbServiceTime_First.Visibility = Visibility.Visible;
                this.tbServiceTime_Second.Visibility = Visibility.Collapsed;
            }
            else if (Combox_DTServiceType.SelectedValue.ToString() == "DT12")
            {
                lbServiceTime.Visibility = Visibility.Visible;
                StackPanel_Time.Visibility = Visibility.Visible;
                tbServiceTime_First.Visibility = Visibility.Visible;
                this.tbServiceTime_Second.Visibility = Visibility.Visible;
            }
        }
    }
}