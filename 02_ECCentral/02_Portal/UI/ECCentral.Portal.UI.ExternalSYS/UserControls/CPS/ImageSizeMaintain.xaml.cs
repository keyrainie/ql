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
using ECCentral.Portal.UI.ExternalSYS.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.ExternalSYS.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.ExternalSYS.UserControls
{
    public partial class ImageSizeMaintain : UserControl
    {
        private ImageSizeVM model;
        private ImageSizeFacade facade;
        public IDialog Dialog { get; set; }
        public ImageSizeMaintain()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                model = new ImageSizeVM();
                facade = new ImageSizeFacade();
                this.DataContext = model;
            };
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            
            ImageSizeVM vm = this.DataContext as ImageSizeVM;
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            facade.CreateImageSize(vm, (obj, arg) => 
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                CPApplication.Current.CurrentPage.Context.Window.Alert("添加成功!");
                CloseDialog(DialogResultType.OK);
            });
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
