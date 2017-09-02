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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Comment
{
    public partial class UCEditLeaveWord : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo
        {
            get;
            set;
        }
        //private const string EmailFormat = "{0}{1}邮件回复：{2}";
        private LeaveWordQueryVM vm;
        private LeaveWordQueryFacade facade;

        public UCEditLeaveWord()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCEditLeaveWord_Loaded);
        }

        private void UCEditLeaveWord_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCEditLeaveWord_Loaded);
            facade = new LeaveWordQueryFacade(CPApplication.Current.CurrentPage);
            comProcessStatus.ItemsSource = EnumConverter.GetKeyValuePairs<CommentProcessStatus>(EnumConverter.EnumAppendItemType.All);

            if (SysNo > 0)
            {
                facade.LoadLeaveWord(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<LeaveWordsItem, LeaveWordQueryVM>();
                    LayoutRoot.DataContext = vm;
                });
            }
        }

        /// <summary>
        /// 保存回复 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            vm = LayoutRoot.DataContext as LeaveWordQueryVM;
            LeaveWordsItem item = vm.ConvertVM<LeaveWordQueryVM, LeaveWordsItem>();
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            item.SysNo = SysNo;
            facade.UpdateLeaveWord(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return; 
                
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
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

        /// <summary>
        /// 回复客户邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            vm = LayoutRoot.DataContext as LeaveWordQueryVM;
            LeaveWordsItem item = vm.ConvertVM<LeaveWordQueryVM, LeaveWordsItem>();
            if (string.IsNullOrEmpty(vm.MailReplyContent))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_MailContentIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                
            else
            {
                string str = string.IsNullOrEmpty(item.ReplyContent) ? "" : "\r\n";
                item.ReplyContent = string.Format(ResComment.Content_ReplyMailFormat, item.ReplyContent, str, vm.MailReplyContent); 
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                item.SysNo = SysNo;
                facade.SendCustomerEmailForLeaveWord(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
        }
    }
}
