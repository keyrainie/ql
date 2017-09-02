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
using ECCentral.Portal.UI.RMA.Models;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Facades;
using ECCentral.BizEntity.RMA;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.RMA.Resources;

namespace ECCentral.Portal.UI.RMA.UserControls
{
    public partial class UCCloseRMATracking : UserControl
    {
        private List<ValidationEntity> Validation;

        public IDialog Dialog
        {
            get;
            set;
        }

        public RMATrackingVM VM
        {
            get
            {
                return this.DataContext as RMATrackingVM;
            }
        }


        public UCCloseRMATracking()
        {
            InitializeComponent();

            BuildValidate();

            Loaded += new RoutedEventHandler(CloseRMATracking_Loaded);
        }
        private void BuildValidate()
        {
            Validation = new List<ValidationEntity>();
            Validation.Add(new ValidationEntity(ValidationEnum.IsNotEmpty, null, ResRMATracking.Msg_CheckNote));
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (this.Dialog.ResultArgs.Data != null)
            {
                this.Dialog.Close();
            }
            else
            {
                this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                this.Dialog.Close();
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationHelper.Validation(this.TextBox_Note, Validation))
            {
                return;
            }
            RMATrackingFacade facade = new RMATrackingFacade(CPApplication.Current.CurrentPage);
            facade.Close(VM, (obj, args) =>
            {
                if (Dialog != null)
                {
                    Dialog.ResultArgs.Data = args;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            });
        }

        private void CloseRMATracking_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(CloseRMATracking_Loaded);

            this.Button_Save.Visibility = VM.Status == InternalMemoStatus.Open ? Visibility.Visible : Visibility.Collapsed;
            this.TextBox_Note.IsReadOnly = VM.Status == InternalMemoStatus.Open ? false : true;
        }
    }
}
