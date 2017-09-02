using ECCentral.Portal.UI.IM.Models.Product;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
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

namespace ECCentral.Portal.UI.IM.UserControls.Product
{
    public partial class ProductBatchEntryNote : UserControl
    {
        public IDialog Dialog
        {
            get;
            set;
        }

        private IWindow Window
        {
            get
            {
                return Page != null ? Page.Context.Window : CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        private IPage Page
        {
            get;
            set;
        }

        public ProductBatchEntryVM Model { get; set; }

        public ProductBatchEntryNote(IPage page, ProductBatchEntryVM vm)
        {
            Model = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void btnAuditPass_Click(object sender, RoutedEventArgs e)
        {
            Model.AuditPass = true;
            CloseDialog(null);
        }

        private void btnAuditReject_Click(object sender, RoutedEventArgs e)
        {
            Model.AuditPass = false;
            CloseDialog(null);
        }

        private void CloseDialog(ResultEventArgs args)
        {
            if (Dialog != null)
            {
                if (args == null)
                {
                    args = new ResultEventArgs();
                    args.Data = Model;
                }
                Dialog.ResultArgs = args;
                Dialog.Close();
            }
        }
    }
}
