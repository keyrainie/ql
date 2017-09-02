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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using System.Collections.ObjectModel;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCMessageConfirm : UserControl
    {
        private IDialog _CurrentDialog;

        public IDialog CurrentDialog
        {
            get { return _CurrentDialog; }
            set { _CurrentDialog = value; }
        }

        public string MessageContent
        {
            set { txtMessageContent.Text = value; }
        }


        public UCMessageConfirm(string msgContent)
        {
            InitializeComponent();
            MessageContent = msgContent;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            CurrentDialog.ResultArgs.DialogResult = DialogResultType.OK;
            CurrentDialog.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            CurrentDialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            CurrentDialog.Close();
        }
    }

}

