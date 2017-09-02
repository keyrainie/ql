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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCAddNewsAdvReply : UserControl
    {
        public IDialog Dialog { get; set; }
        /// <summary>
        /// 评论内容的ID
        /// </summary>
        public int SysNo{get;set;}
        private NewsAdvReplyQueryVM vm;
        private NewsAdvReplyQueryFacade facade;

        public UCAddNewsAdvReply()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddNewsAdvReply_Loaded);
        }

        private void UCAddNewsAdvReply_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddNewsAdvReply_Loaded);
            vm = new NewsAdvReplyQueryVM();
            LayoutRoot.DataContext = vm;
            facade = new NewsAdvReplyQueryFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                facade.LoadNewsAdvReply(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<NewsAdvReply, NewsAdvReplyQueryVM>();
                    LayoutRoot.DataContext = vm;
                });
            }
            else
            {
                vm = new NewsAdvReplyQueryVM();
                LayoutRoot.DataContext = vm;
            }
        }

        /// <summary>
        /// 保存回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            vm = LayoutRoot.DataContext as NewsAdvReplyQueryVM;
            //if (invalidStatus.IsChecked==true)
            //    vm.Status = "D";
            //else
            //    vm.Status = "A";
            NewsAdvReply item = vm.ConvertVM<NewsAdvReplyQueryVM, NewsAdvReply>();
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;

            facade.CreateNewsAdvReply(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

    }
}
