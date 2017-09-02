using ECCentral.Portal.UI.MKT.Facades;
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

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCBusinessCooperationDetail : UserControl
    {
        public IDialog Dialog { get; set; }

        public IPage Page
        {
            get
            {
                return CPApplication.Current.CurrentPage;
            }
        }

        public UCBusinessCooperationDetail()
        {
            InitializeComponent();
        }

        private void btnHandle_Click(object sender, RoutedEventArgs e)
        {
            var d = this.DataContext as dynamic;
            new GroupBuyingFacade(this.Page).HandleGroupbuyingBusinessCooperation((int)d.SysNo, (s, a) =>
            {
                if (a.FaultsHandle())
                {
                    return;
                }
                Page.Context.Window.Alert("提示", "处理成功！", MessageType.Information, (se, arg) =>
                {
                    this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK };
                    this.Dialog.Close();
                });
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }
    }
}
