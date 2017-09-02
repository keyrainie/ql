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
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class PropertyMaintain : UserControl
    {

        private bool _isEditing = false;
        private int _editingSysNo;

        public IDialog Dialog { get; set; }

        //PropertyVM model;
        private PropertyFacade facade;
        //private string propertySysNo;

        public PropertyMaintain()
        {
            InitializeComponent();
            facade = new PropertyFacade();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                facade.GetPropertyBySysNo(_editingSysNo, (obj, args) =>
                {
                    PropertyVM vm = new PropertyVM();

                    vm.SysNo = args.Result.SysNo.Value;
                    vm.PropertyName = args.Result.PropertyName.Content;
                    vm.Status = Convert.ToInt32(args.Result.Status).ToString();                    
                    this.DataContext = vm;
                    if (args.Result.Status == PropertyStatus.Active)
                    {
                        dplistPropertyStatus.SelectedIndex = 0;
                    }
                    else
                    {
                        dplistPropertyStatus.SelectedIndex = 1;
                    }

                });
            }
            else
            {                
                this.DataContext = new PropertyVM();
                dplistPropertyStatus.SelectedIndex = 0;
            }
        }

        public void BeginEditing(int sysNo)
        {
            _isEditing = true;
            _editingSysNo = sysNo;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            PropertyVM vm = this.DataContext as PropertyVM;
            vm.SysNo = Convert.ToInt32(_editingSysNo);
            if (vm.SysNo > 0)
            {
                facade.UpdateProperty(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyMaintain.Info_SaveSuccessfully);
                    CloseDialog(DialogResultType.OK);
                });
            }
            else
            {
                facade.CreateProperty(vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResPropertyMaintain.Info_SaveSuccessfully);
                    CloseDialog(DialogResultType.OK);
                });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }
}
