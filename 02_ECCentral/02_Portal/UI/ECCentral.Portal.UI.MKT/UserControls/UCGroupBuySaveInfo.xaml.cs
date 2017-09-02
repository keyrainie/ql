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
using ECCentral.Portal.UI.MKT.Models.GroupBuying;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class UCGroupBuySaveInfo : UserControl
    {
        public IDialog Dialog { get; set; }
        public string ProductID { get; set; }
        public List<GroupBuySaveInfoVM> MsgListVM { get; set; }
        public GroupBuyingMaintainVM vm { private get; set; }
        public UCGroupBuySaveInfo()
        {
            InitializeComponent();
            MsgListVM = new List<GroupBuySaveInfoVM>();
            this.Loaded += new RoutedEventHandler(UCGroupBuySaveInfo_Loaded);
        }

        void UCGroupBuySaveInfo_Loaded(object sender, RoutedEventArgs e)
        {
            int count = MsgListVM.Count;
            MsgListVM[count - 1].IsShowConfirmInfo =  System.Windows.Visibility.Visible;
            this.resultControl.ItemsSource = MsgListVM;
            new GroupBuyingFacade(CPApplication.Current.CurrentPage).GetProductPromotionMargionByGroupBuying(vm, (obj, arg) =>
            {
                if (!string.IsNullOrEmpty(arg.Result))
                {
                    txtMargin.Text = arg.Result;
                }
            });
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.Dialog != null)
            {
                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Dialog != null)
            {
                this.Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                this.Dialog.Close();
            }
        }       
    }
}
